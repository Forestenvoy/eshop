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
            await CreateProductTableAsync(connection, logger, ct);
            await CreateUserTableAsync(connection, logger, ct);
            await CreateBalanceTableAsync(connection, logger, ct);
            await CreateOrderTableAsync(connection, logger, ct);
            await CreateOrderItemTableAsync(connection, logger, ct);
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
                    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
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
                    `code` VARCHAR(100) NOT NULL,
                    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `uk_permission_code` (`code`)
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
                    `role_id` INT NOT NULL,
                    `permission_id` INT NOT NULL,
                    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`role_id`, `permission_id`),
                    CONSTRAINT `fk_role_permission_role` FOREIGN KEY (`role_id`) REFERENCES `role`(`id`) ON DELETE CASCADE,
                    CONSTRAINT `fk_role_permission_permission` FOREIGN KEY (`permission_id`) REFERENCES `permission`(`id`) ON DELETE CASCADE
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
                    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `uk_admin` (`account`),
                    CONSTRAINT `fk_admin_role` FOREIGN KEY (`role_id`) REFERENCES `role`(`id`) ON DELETE SET NULL
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'admin' created.");
        }

        private static async Task CreateProductTableAsync(MySqlConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await TableExistsAsync(connection, "product", ct))
            {
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(@"
                CREATE TABLE `product` (
                    `id` BIGINT NOT NULL AUTO_INCREMENT,
                    `name` VARCHAR(255) NOT NULL,
                    `description` TEXT NULL,
                    `price` DECIMAL(10,2) NOT NULL,
                    `stock` INT NOT NULL DEFAULT 0,
                    `image_url` VARCHAR(500) NULL,
                    `is_enabled` BOOLEAN NOT NULL DEFAULT TRUE,
                    `sort` INT NOT NULL DEFAULT 0,
                    `modifier` VARCHAR(50) NULL,
                    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`id`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'product' created.");
        }

        private static async Task CreateUserTableAsync(MySqlConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await TableExistsAsync(connection, "user", ct))
            {
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(@"
                CREATE TABLE `user` (
                    `id` BIGINT NOT NULL AUTO_INCREMENT,
                    `name` VARCHAR(100) NULL,
                    `email` VARCHAR(255) NOT NULL,
                    `password` VARCHAR(255) NOT NULL,
                    `gender` TINYINT NOT NULL DEFAULT 0,
                    `avatar` VARCHAR(500) NULL,
                    `birthday` DATE NULL,
                    `phone` VARCHAR(20) NULL,
                    `address` VARCHAR(255) NULL,
                    `status` TINYINT NOT NULL DEFAULT 1,
                    `last_login_at` DATETIME NULL,
                    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `uk_user_email` (`email`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'user' created.");
        }

        private static async Task CreateBalanceTableAsync(MySqlConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await TableExistsAsync(connection, "balance", ct))
            {
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(@"
                CREATE TABLE `balance` (
                    `user_id` BIGINT NOT NULL,
                    `amount` DECIMAL(12,2) NOT NULL DEFAULT 0,
                    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`user_id`),
                    CONSTRAINT `fk_balance_user` FOREIGN KEY (`user_id`) REFERENCES `user`(`id`) ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'balance' created.");
        }

        private static async Task CreateOrderTableAsync(MySqlConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await TableExistsAsync(connection, "order", ct))
            {
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(@"
                CREATE TABLE `order` (
                    `id` BIGINT NOT NULL AUTO_INCREMENT,
                    `order_no` VARCHAR(50) NOT NULL,
                    `user_id` BIGINT NOT NULL,
                    `total_amount` DECIMAL(12,2) NOT NULL,
                    `status` TINYINT NOT NULL DEFAULT 0,
                    `payment_status` TINYINT NOT NULL DEFAULT 0,
                    `receiver_name` VARCHAR(100) NOT NULL,
                    `receiver_phone` VARCHAR(20) NOT NULL,
                    `receiver_address` VARCHAR(255) NOT NULL,
                    `remark` VARCHAR(500) NULL,
                    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
                    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `uk_order_order_no` (`order_no`),
                    CONSTRAINT `fk_order_user` FOREIGN KEY (`user_id`) REFERENCES `user`(`id`) ON DELETE RESTRICT
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'order' created.");
        }

        private static async Task CreateOrderItemTableAsync(MySqlConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await TableExistsAsync(connection, "order_item", ct))
            {
                return;
            }

            await connection.ExecuteAsync(new CommandDefinition(@"
                CREATE TABLE `order_item` (
                    `id` BIGINT NOT NULL AUTO_INCREMENT,
                    `order_id` BIGINT NOT NULL,
                    `product_id` BIGINT NOT NULL,
                    `product_name` VARCHAR(255) NOT NULL,
                    `price` DECIMAL(10,2) NOT NULL,
                    `quantity` INT NOT NULL,
                    `subtotal` DECIMAL(10,2) NOT NULL,
                    PRIMARY KEY (`id`),
                    CONSTRAINT `fk_order_item_order` FOREIGN KEY (`order_id`) REFERENCES `order`(`id`) ON DELETE CASCADE,
                    CONSTRAINT `fk_order_item_product` FOREIGN KEY (`product_id`) REFERENCES `product`(`id`) ON DELETE RESTRICT
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                cancellationToken: ct));

            logger?.LogInformation("✅ Table 'order_item' created.");
        }
    }
}
