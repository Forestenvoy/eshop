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
    }
}
