using Dapper;
using eshop.application.Common;
using eshop.application.Models.Admin;
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
            await SeedProductAsync(connection, logger, ct);
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

            var admin = new AdminUser { Account = "admin" };
            var hashedPassword = new PasswordHasher<AdminUser>().HashPassword(admin, "admin");

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

        private static async Task SeedProductAsync(IDbConnection connection, ILogger? logger, CancellationToken ct)
        {
            if (await HasDataAsync(connection, "product", ct))
            {
                logger?.LogInformation("✅ 'product' already has data, no insert needed.");
                return;
            }

            // id 明確指定,對應 wwwroot/images/product/{id}.webp 這 10 張已經放好的圖檔,不依賴 AUTO_INCREMENT 的隱含順序
            var products = new[]
            {
                new { Id = 1, Name = "AirPods Pro 3", Price = 7490m, Stock = 50 },
                new { Id = 2, Name = "機械式鍵盤-紅軸", Price = 2890m, Stock = 30 },
                new { Id = 3, Name = "羅技 MX Master 4", Price = 4190m, Stock = 80 },
                new { Id = 4, Name = "27型 雙模4K 電競螢幕", Price = 8990m, Stock = 20 },
                new { Id = 5, Name = "USB-C 11合1雙4K擴充基座", Price = 2990m, Stock = 60 },
                new { Id = 6, Name = "雙軸鋁合金筆電散熱支架", Price = 599m, Stock = 100 },
                new { Id = 7, Name = "電競滑鼠墊 XXL", Price = 990m, Stock = 200 },
                new { Id = 8, Name = "USB-C 快速充電器", Price = 699m, Stock = 70 },
                new { Id = 9, Name = "NAS 網路儲存伺服器", Price = 19900m, Stock = 10 },
                new { Id = 10, Name = "外接 SSD 1TB", Price = 3978m, Stock = 35 },
            }.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Stock,
                ImageUrl = $"/images/product/{p.Id}.webp",
                Sort = p.Id,
                Modifier = "System",
            });

            await connection.ExecuteAsync(new CommandDefinition(@"
                INSERT INTO `product` (`id`, `name`, `price`, `stock`, `image_url`, `is_enabled`, `sort`, `modifier`)
                VALUES (@Id, @Name, @Price, @Stock, @ImageUrl, TRUE, @Sort, @Modifier);",
                products,
                cancellationToken: ct));

            logger?.LogInformation("✅ Inserted 10 default records into 'product'.");
        }
    }
}
