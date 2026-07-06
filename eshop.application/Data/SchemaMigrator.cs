using Dapper;
using MySqlConnector;

namespace eshop.application.Data
{
    /// <summary>
    /// 資料庫結構遷移器
    /// </summary>
    /// <remarks>
    /// 針對每張表檢查是否已存在（<c>INFORMATION_SCHEMA.TABLES</c>），不存在才建立。
    /// 屬於簡易、冪等（idempotent）的 code-first 建表機制，沒有版本表也沒有 up/down。
    /// </remarks>
    public static class SchemaMigrator
    {
        public static async Task MigrateAsync(string connectionString, ILogger? logger = null, CancellationToken ct = default)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync(ct);

            await CreateRoleTableAsync(connection, logger, ct);
            await CreatePermissionTableAsync(connection, logger, ct);
            await CreateRolePermissionTableAsync(connection, logger, ct);
            await CreateAdminTableAsync(connection, logger, ct);
        }

        private static async Task<bool> TableExistsAsync(MySqlConnection connection, string tableName, CancellationToken ct)
        {
            var count = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @tableName;",
                    new { tableName },
                    cancellationToken: ct));

            return count > 0;
        }

        private static async Task CreateRoleTableAsync(MySqlConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await TableExistsAsync(connection, "role", ct))
            {
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(@"
                CREATE TABLE `role` (
                    `id` INT NOT NULL AUTO_INCREMENT,
                    `name` VARCHAR(50) NOT NULL,
                    `modifier` VARCHAR(50) NULL,
                    `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `uk_role_name` (`name`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'role' created.");
        }

        private static async Task CreatePermissionTableAsync(MySqlConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await TableExistsAsync(connection, "permission", ct))
            {
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(@"
                CREATE TABLE `permission` (
                    `id` INT NOT NULL AUTO_INCREMENT,
                    `name` VARCHAR(100) NOT NULL,
                    `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `uk_permission_name` (`name`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'permission' created.");
        }

        private static async Task CreateRolePermissionTableAsync(MySqlConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await TableExistsAsync(connection, "role_permission", ct))
            {
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(@"
                CREATE TABLE `role_permission` (
                    `id` INT NOT NULL AUTO_INCREMENT,
                    `role_id` INT NOT NULL,
                    `permission_id` INT NOT NULL,
                    `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `uk_role_permission` (`role_id`, `permission_id`),
                    CONSTRAINT `fk_role_permission_role` FOREIGN KEY (`role_id`) REFERENCES `role`(`id`) ON DELETE CASCADE ON UPDATE CASCADE,
                    CONSTRAINT `fk_role_permission_permission` FOREIGN KEY (`permission_id`) REFERENCES `permission`(`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'role_permission' created.");
        }

        private static async Task CreateAdminTableAsync(MySqlConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await TableExistsAsync(connection, "admin", ct))
            {
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(@"
                CREATE TABLE `admin` (
                    `id` INT NOT NULL AUTO_INCREMENT,
                    `account` VARCHAR(255) NOT NULL,
                    `password` VARCHAR(255) NOT NULL,
                    `role_id` INT NULL,
                    `is_enable` BOOLEAN DEFAULT TRUE,
                    `modifier` VARCHAR(50) NULL,
                    `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `uk_admin_account` (`account`),
                    CONSTRAINT `fk_admin_role` FOREIGN KEY (`role_id`) REFERENCES `role`(`id`) ON DELETE SET NULL ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'admin' created.");
        }
    }
}
