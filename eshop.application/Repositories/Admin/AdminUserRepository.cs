using eshop.application.Data;
using eshop.application.Repositories.Admin.Interfaces;
using Dapper;
using System.Data.Common;
using eshop.application.Models;

namespace eshop.application.Repositories.Admin
{
    public class AdminUserRepository : BaseRepository, IAdminUserRepository
    {
        public AdminUserRepository(DbConnection dbConnection, IUnitOfWork unitOfWork)
            : base(dbConnection, unitOfWork)
        {
        }

        /// <inheritdoc />
        public async Task<AdminUser?> GetAsync(string account)
        {
            const string sql = "SELECT * FROM `admin` WHERE `account` = @account";

            return await _dbConnection.QuerySingleOrDefaultAsync<AdminUser>(sql, new { account }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<AdminUser?> GetAsync(int id)
        {
            const string sql = "SELECT * FROM `admin` WHERE `id` = @id";

            return await _dbConnection.QuerySingleOrDefaultAsync<AdminUser>(sql, new { id }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> GetCountAsync(string? keyword)
        {
            var whereSql = string.IsNullOrWhiteSpace(keyword)
                ? string.Empty
                : " WHERE `account` LIKE CONCAT('%', @Keyword, '%')";

            var sql = $"SELECT COUNT(1) FROM `admin`{whereSql};";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { Keyword = keyword }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AdminUser>> GetPagedListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var whereSql = string.IsNullOrWhiteSpace(keyword)
                ? string.Empty
                : " WHERE `account` LIKE CONCAT('%', @Keyword, '%')";

            var sql = $@"
                SELECT *
                FROM `admin`
                {whereSql}
                ORDER BY `id` DESC
                LIMIT @Offset, @PageSize;
            ";

            var parameters = new
            {
                Keyword = keyword,
                Offset = (pageIndex - 1) * pageSize,
                PageSize = pageSize
            };

            return await _dbConnection.QueryAsync<AdminUser>(sql, parameters, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsByAccountAsync(string account)
        {
            const string sql = "SELECT EXISTS(SELECT 1 FROM `admin` WHERE `account` = @account);";

            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { account }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> AddAsync(AdminUser adminUser)
        {
            const string sql = "INSERT INTO `admin` (account, password, role_id, is_enable, modifier) VALUES (@Account, @Password, @RoleId, @IsEnabled, @Modifier);";

            return await _dbConnection.ExecuteAsync(sql, adminUser, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> UpdateAsync(AdminUser adminUser)
        {
            const string sql = "UPDATE `admin` SET account = @Account, password = @Password, role_id = @RoleId, is_enable = @IsEnabled, modifier = @Modifier WHERE id = @Id;";

            return await _dbConnection.ExecuteAsync(sql, adminUser, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> RemoveRangeAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return 0;
            }

            const string sql = "DELETE FROM `admin` WHERE id IN @ids;";

            return await _dbConnection.ExecuteAsync(sql, new { ids }, _dbTransaction);
        }
    }
}
