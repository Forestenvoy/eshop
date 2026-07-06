using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Serilog;
using StackExchange.Redis;

namespace eshop.application.Configurations
{
    /// <summary>
    /// Redis 快取相關配置
    /// </summary>
    public static class ConfigureRedis
    {
        /// <summary>
        /// 註冊 Redis
        /// </summary>
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["Redis:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Redis connection string is not configured.");
            }

            var multiplexer = ConnectionMultiplexer.Connect(connectionString);

            var db = multiplexer.GetDatabase();
            var ping = db.Ping();

            Log.Information($"Redis connected. Ping: {ping.TotalMilliseconds} ms");

            services.AddSingleton<IConnectionMultiplexer>(_ => multiplexer);

            services.AddScoped(sp =>
            {
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                return redis.GetDatabase();
            });

            Log.Information("Redis has been configured successfully.");

            return services;
        }

        /// <summary>
        /// 註冊分散式鎖
        /// </summary>
        /// <remarks>
        /// 使用 RedLock.net 實現，依賴已註冊的 <see cref="IConnectionMultiplexer"/>。
        /// 呼叫前請確認已先呼叫 <see cref="AddRedis"/>。
        /// </remarks>
        public static IServiceCollection AddDistributedLock(this IServiceCollection services)
        {
            services.AddSingleton<IDistributedLockFactory>(sp =>
            {
                var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();

                var redLockMultiplexers = new List<RedLockMultiplexer>
                {
                    new(multiplexer)
                };

                return RedLockFactory.Create(redLockMultiplexers);
            });

            Log.Information("DistributedLock (RedLock.net) has been configured successfully.");

            return services;
        }
    }
}
