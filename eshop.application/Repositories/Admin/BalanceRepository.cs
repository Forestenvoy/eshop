using eshop.application.Data;
using eshop.application.Models;
using eshop.application.Repositories.Admin.Interfaces;
using Dapper;
using System.Data.Common;

namespace eshop.application.Repositories.Admin
{
    public class BalanceRepository : BaseRepository, IBalanceRepository
    {
        public BalanceRepository(DbConnection dbConnection, IUnitOfWork unitOfWork)
            : base(dbConnection, unitOfWork)
        {
        }

        /// <inheritdoc />
        public async Task<int> AddAsync(Balance balance)
        {
            const string sql = "INSERT INTO `balance` (`user_id`, `amount`) VALUES (@UserId, @Amount);";

            return await _dbConnection.ExecuteAsync(sql, balance, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<Balance?> GetAsync(long userId)
        {
            const string sql = "SELECT * FROM `balance` WHERE `user_id` = @userId;";

            return await _dbConnection.QuerySingleOrDefaultAsync<Balance>(sql, new { userId }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> IncreaseAsync(long userId, decimal amount)
        {
            const string sql = "UPDATE `balance` SET `amount` = `amount` + @amount WHERE `user_id` = @userId;";

            return await _dbConnection.ExecuteAsync(sql, new { userId, amount }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> DecreaseIfSufficientAsync(long userId, decimal amount)
        {
            const string sql = "UPDATE `balance` SET `amount` = `amount` - @amount WHERE `user_id` = @userId AND `amount` >= @amount;";

            return await _dbConnection.ExecuteAsync(sql, new { userId, amount }, _dbTransaction);
        }
    }
}
