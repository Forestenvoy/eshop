using Dapper;
using eshop.application.Data;
using MySqlConnector;

namespace eshop.application.Configurations
{
    /// <summary>
    /// 應用程式啟動時的資料庫初始化背景服務。
    /// <para>
    /// 僅在 Development 環境且設定檔中 <c>InitializeDatabase = true</c> 時執行，
    /// 依序完成以下兩個階段：
    /// <list type="number">
    ///   <item>確保資料庫存在（若不存在則建立）</item>
    ///   <item>透過 <see cref="SchemaMigrator"/> 建立或遷移資料表結構</item>
    ///   <item>透過 <see cref="DataSeeder"/> 填入必要的預設資料</item>
    /// </list>
    /// </para>
    /// </summary>
    public class DatabaseBootstrapper : IHostedService
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;
        private readonly ILogger<DatabaseBootstrapper> _logger;

        public DatabaseBootstrapper(
            IConfiguration config,
            IHostEnvironment env,
            ILogger<DatabaseBootstrapper> logger)
        {
            _config = config;
            _env = env;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // 僅在本地開發環境且設定檔顯式啟用時才執行
            var enable = _config.GetValue<bool>("InitializeDatabase");

            if (!_env.IsDevelopment() || !enable)
            {
                _logger.LogInformation("DatabaseBootstrap skipped. Env={Env}, Enable={Enable}", _env.EnvironmentName, enable);
                return;
            }

            var cs = _config.GetConnectionString("DB") ?? _config.GetValue<string>("DB");
            if (string.IsNullOrWhiteSpace(cs))
            {
                _logger.LogWarning("DatabaseBootstrap connection string 'DB' not found. Skip.");
                return;
            }

            _logger.LogInformation("DatabaseBootstrap starting...");

            // 對 MySQL 尚未就緒的情況進行重試，最多等待 30 秒（10 次 × 3 秒）
            await RetryAsync(
                async () => await EnsureDatabaseAndTablesAsync(cs, cancellationToken, _logger),
                retries: 10,
                delay: TimeSpan.FromSeconds(3),
                cancellationToken,
                _logger
                );

            _logger.LogInformation("DatabaseBootstrap done.");
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// 對非預期失敗（例如 MySQL 容器尚未啟動）進行有限次數的重試。
        /// </summary>
        private static async Task RetryAsync(Func<Task> action, int retries, TimeSpan delay, CancellationToken ct, ILogger? logger = null)
        {
            for (int i = 0; ; i++)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    await action();
                    return;
                }
                catch (Exception ex) when (i < retries && !ct.IsCancellationRequested)
                {
                    logger?.LogWarning(ex, "DatabaseBootstrap attempt {Attempt} failed. Retrying in {Delay}...", i + 1, delay);

                    await Task.Delay(delay, ct);
                }
            }
        }

        /// <summary>
        /// 確保資料庫存在，並依序執行結構遷移與資料填充。
        /// </summary>
        /// <param name="connectionString">目標資料庫的連線字串。</param>
        /// <param name="ct">取消權杖。</param>
        /// <param name="logger">Log 輸出。</param>
        public static async Task EnsureDatabaseAndTablesAsync(
            string connectionString,
            CancellationToken ct,
            ILogger? logger = null)
        {
            // 步驟一：確保資料庫存在（先連到 MySQL server，再建立目標 DB）
            var builder = new MySqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;
            builder.Database = "";

            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                await connection.OpenAsync(ct);

                var dbExists = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @db;",
                        new { db = databaseName },
                        cancellationToken: ct));

                if (dbExists == 0)
                {
                    await connection.ExecuteAsync(new CommandDefinition(
                        $"CREATE DATABASE `{databaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;",
                        cancellationToken: ct));

                    logger?.LogInformation("✅ Database '{Database}' created.", databaseName);
                }
            }

            // 步驟二：建立或遷移資料表結構
            await SchemaMigrator.MigrateAsync(connectionString, logger, ct);

            // 步驟三：填入必要的預設資料
            await DataSeeder.SeedAsync(connectionString, logger, ct);
        }
    }
}
