using eshop.application.Data;
using eshop.application.Data.IRepositories;
using eshop.application.Data.Repositories;
using MySqlConnector;
using Serilog;
using System.Data.Common;

namespace eshop.application.Configurations
{
    /// <summary>
    /// 資料庫 相關配置
    /// </summary>
    public static class ConfigureDatabase
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString("DB")
                     ?? configuration.GetValue<string>("DB");

            // 註冊 MySQL 連線
            services.AddScoped<DbConnection>(sp =>
            {
                var conn = new MySqlConnection(connStr);
                return conn;
            });

            Log.Information("MySqlConnection has been configured successfully.");

            // 註冊 UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 自動註冊 Repository
            services.AddRepositories();

            // 啟動時自動建表／灌預設資料
            services.AddHostedService<DatabaseBootstrapper>();

            return services;
        }

        /// <summary>
        /// 註冊 Repositories
        /// </summary>
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            return services;
        }

    }
}
