using Dapper;
using eshop.application.Common;
using eshop.application.Models;
using Microsoft.AspNetCore.Identity;
using MySqlConnector;
using System.Data;

namespace eshop.application.Data
{
    /// <summary>
    /// 資料庫初始資料建立器
    /// </summary>
    /// <remarks>
    /// 負責填入系統必要的預設資料（Seed Data），每張表都會先檢查是否已有資料，避免重複灌入。
    /// </remarks>
    public static class DataSeeder
    {
        public static async Task SeedAsync(string connectionString, ILogger? logger = null, CancellationToken ct = default)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync(ct);

            await SeedRoleAsync(connection, logger, ct);
            await SeedPermissionAsync(connection, logger, ct);
            await SeedRolePermissionAsync(connection, logger, ct);
            await SeedAdminAsync(connection, logger, ct);
        }

        private static async Task<bool> HasDataAsync(IDbConnection connection, string tableName, CancellationToken ct)
        {
            var count = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition($"SELECT COUNT(*) FROM `{tableName}`;", cancellationToken: ct));

            return count > 0;
        }

        private static async Task SeedRoleAsync(IDbConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await HasDataAsync(connection, "role", ct))
            {
                logger?.LogInformation("✅ 'role' already has data, no insert needed.");
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(
                "INSERT INTO `role` (`name`, `modifier`) VALUES (@Name, @Modifier);",
                new { Name = "Administrator", Modifier = "System" },
                cancellationToken: ct));

            logger?.LogInformation("✅ Inserted default role into 'role'.");
        }

        private static async Task SeedPermissionAsync(IDbConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await HasDataAsync(connection, "permission", ct))
            {
                logger?.LogInformation("✅ 'permission' already has data, no insert needed.");
                return;
            }

            var permissions = AuthConstants.Permission.All
                .Select(code => new { code });

            await connection.ExecuteAsync(new CommandDefinition(
                "INSERT IGNORE INTO permission(code) VALUES (@code);",
                permissions,
                cancellationToken: ct));

            logger?.LogInformation("✅ Inserted default records into 'permission'.");
        }

        private static async Task SeedRolePermissionAsync(IDbConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await HasDataAsync(connection, "role_permission", ct))
            {
                logger?.LogInformation("✅ 'role_permission' already has data, no insert needed.");
                return;
            }

            await connection.ExecuteAsync(
                    new CommandDefinition(
                        @"INSERT IGNORE INTO `role_permission`(`role_id`, `permission_id`)
                            SELECT
                                r.id,
                                p.id
                            FROM `role` r
                            CROSS JOIN `permission` p
                            WHERE r.name = @roleName;",
                        new
                        {
                            roleName = "Administrator"
                        },
                        cancellationToken: ct));

            logger?.LogInformation("✅ Inserted default records into 'role_permission'.");
        }

        private static async Task SeedAdminAsync(IDbConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await HasDataAsync(connection, "admin", ct))
            {
                logger?.LogInformation("✅ 'admin' already has data, no insert needed.");
                return;
            }

            var admin = new Admin { Account = "admin" };
            var hashedPassword = new PasswordHasher<Admin>().HashPassword(admin, "admin");

            await connection.ExecuteAsync(new CommandDefinition(@"
                INSERT INTO `admin` (`account`, `password`, `role_id`, `is_enable`)
                VALUES (@Account, @Password, @RoleId, @IsEnabled);",
                new
                {
                    admin.Account,
                    Password = hashedPassword,
                    RoleId = 1,
                    IsEnabled = true,
                },
                cancellationToken: ct));

            logger?.LogInformation("✅ Inserted default record into 'admin'.");
        }
    }
}
