using eshop.application.Data;
using eshop.application.Enums;
using eshop.application.Models;
using eshop.application.Repositories.Admin.Interfaces;
using Dapper;
using System.Data.Common;

namespace eshop.application.Repositories.Admin
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(DbConnection dbConnection, IUnitOfWork unitOfWork)
            : base(dbConnection, unitOfWork)
        {
        }

        /// <inheritdoc />
        public async Task<User?> GetAsync(long id)
        {
            const string sql = "SELECT * FROM `user` WHERE `id` = @id AND `status` <> @deletedStatus";

            return await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { id, deletedStatus = UserStatus.Deleted }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<User?> GetAsync(string email)
        {
            const string sql = "SELECT * FROM `user` WHERE `email` = @email";

            return await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { email }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> GetCountAsync(string? keyword)
        {
            var whereSql = string.IsNullOrWhiteSpace(keyword)
                ? " WHERE `status` <> @DeletedStatus"
                : " WHERE `status` <> @DeletedStatus AND (`name` LIKE CONCAT('%', @Keyword, '%') OR `email` LIKE CONCAT('%', @Keyword, '%'))";

            var sql = $"SELECT COUNT(1) FROM `user`{whereSql};";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { Keyword = keyword, DeletedStatus = UserStatus.Deleted }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetPagedListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var whereSql = string.IsNullOrWhiteSpace(keyword)
                ? " WHERE `status` <> @DeletedStatus"
                : " WHERE `status` <> @DeletedStatus AND (`name` LIKE CONCAT('%', @Keyword, '%') OR `email` LIKE CONCAT('%', @Keyword, '%'))";

            var sql = $@"
                SELECT *
                FROM `user`
                {whereSql}
                ORDER BY `id` DESC
                LIMIT @Offset, @PageSize;
            ";

            var parameters = new
            {
                Keyword = keyword,
                DeletedStatus = UserStatus.Deleted,
                Offset = (pageIndex - 1) * pageSize,
                PageSize = pageSize
            };

            return await _dbConnection.QueryAsync<User>(sql, parameters, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            const string sql = "SELECT EXISTS(SELECT 1 FROM `user` WHERE `email` = @email);";

            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { email }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<long> AddAsync(User user)
        {
            const string sql = @"
                INSERT INTO `user` (`name`, `email`, `password`, `gender`, `birthday`, `phone`, `address`, `status`)
                VALUES (@Name, @Email, @Password, @Gender, @Birthday, @Phone, @Address, @Status);
                SELECT LAST_INSERT_ID();";

            return await _dbConnection.ExecuteScalarAsync<long>(sql, user, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> UpdateProfileAsync(User user)
        {
            const string sql = @"
                UPDATE `user`
                SET `name` = @Name, `gender` = @Gender, `phone` = @Phone, `birthday` = @Birthday, `address` = @Address, `avatar` = @Avatar
                WHERE `id` = @Id;";

            return await _dbConnection.ExecuteAsync(sql, user, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> UpdatePasswordAsync(long id, string hashedPassword)
        {
            const string sql = "UPDATE `user` SET `password` = @hashedPassword WHERE `id` = @id;";

            return await _dbConnection.ExecuteAsync(sql, new { id, hashedPassword }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> ToggleStatusAsync(long id, UserStatus status)
        {
            const string sql = "UPDATE `user` SET `status` = @status WHERE `id` = @id;";

            return await _dbConnection.ExecuteAsync(sql, new { id, status }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> SoftDeleteRangeAsync(List<long> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return 0;
            }

            const string sql = "UPDATE `user` SET `status` = @status WHERE `id` IN @ids;";

            return await _dbConnection.ExecuteAsync(sql, new { ids, status = UserStatus.Deleted }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> UpdateLastLoginAsync(long id)
        {
            const string sql = "UPDATE `user` SET `last_login_at` = CURRENT_TIMESTAMP WHERE `id` = @id;";

            return await _dbConnection.ExecuteAsync(sql, new { id }, _dbTransaction);
        }
    }
}
