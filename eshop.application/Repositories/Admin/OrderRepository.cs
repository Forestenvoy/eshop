using Dapper;
using eshop.application.Data;
using eshop.application.Enums;
using eshop.application.Models;
using eshop.application.Repositories.Admin.Interfaces;
using System.Data.Common;

namespace eshop.application.Repositories.Admin
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(DbConnection dbConnection, IUnitOfWork unitOfWork)
            : base(dbConnection, unitOfWork)
        {
        }

        /// <inheritdoc />
        public async Task<Order?> GetAsync(long id)
        {
            const string sql = "SELECT * FROM `order` WHERE `id` = @id";

            return await _dbConnection.QuerySingleOrDefaultAsync<Order>(sql, new { id }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> GetCountAsync(string? keyword, OrderStatus? status)
        {
            var whereSql = BuildWhereSql(keyword, status);

            var sql = $"SELECT COUNT(1) FROM `order`{whereSql};";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { Keyword = keyword, Status = status }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Order>> GetPagedListAsync(string? keyword, OrderStatus? status, int pageIndex, int pageSize)
        {
            var whereSql = BuildWhereSql(keyword, status);

            var sql = $@"
                SELECT *
                FROM `order`
                {whereSql}
                ORDER BY `id` DESC
                LIMIT @Offset, @PageSize;
            ";

            var parameters = new
            {
                Keyword = keyword,
                Status = status,
                Offset = (pageIndex - 1) * pageSize,
                PageSize = pageSize
            };

            return await _dbConnection.QueryAsync<Order>(sql, parameters, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<OrderItem>> GetItemsAsync(long orderId)
        {
            const string sql = "SELECT * FROM `order_item` WHERE `order_id` = @orderId";

            return await _dbConnection.QueryAsync<OrderItem>(sql, new { orderId }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> UpdateStatusAsync(long id, OrderStatus status, PaymentStatus? paymentStatus)
        {
            var sql = paymentStatus.HasValue
                ? "UPDATE `order` SET `status` = @status, `payment_status` = @paymentStatus WHERE `id` = @id;"
                : "UPDATE `order` SET `status` = @status WHERE `id` = @id;";

            return await _dbConnection.ExecuteAsync(sql, new { id, status, paymentStatus }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<long> AddAsync(Order order)
        {
            const string sql = @"
                INSERT INTO `order` (order_no, user_id, total_amount, status, payment_status, receiver_name, receiver_phone, receiver_address, remark)
                VALUES (@OrderNo, @UserId, @TotalAmount, @Status, @PaymentStatus, @ReceiverName, @ReceiverPhone, @ReceiverAddress, @Remark);
                SELECT LAST_INSERT_ID();";

            return await _dbConnection.ExecuteScalarAsync<long>(sql, order, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> AddItemsAsync(IEnumerable<OrderItem> items)
        {
            const string sql = @"
                INSERT INTO `order_item` (order_id, product_id, product_name, price, quantity, subtotal)
                VALUES (@OrderId, @ProductId, @ProductName, @Price, @Quantity, @Subtotal);";

            return await _dbConnection.ExecuteAsync(sql, items, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> GetMemberCountAsync(long userId, string? keyword, OrderStatus? status)
        {
            var whereSql = BuildWhereSql(keyword, status, userId);

            var sql = $"SELECT COUNT(1) FROM `order`{whereSql};";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { Keyword = keyword, Status = status, UserId = userId }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Order>> GetMemberPagedListAsync(long userId, string? keyword, OrderStatus? status, int pageIndex, int pageSize)
        {
            var whereSql = BuildWhereSql(keyword, status, userId);

            var sql = $@"
                SELECT *
                FROM `order`
                {whereSql}
                ORDER BY `id` DESC
                LIMIT @Offset, @PageSize;
            ";

            var parameters = new
            {
                Keyword = keyword,
                Status = status,
                UserId = userId,
                Offset = (pageIndex - 1) * pageSize,
                PageSize = pageSize
            };

            return await _dbConnection.QueryAsync<Order>(sql, parameters, _dbTransaction);
        }

        private static string BuildWhereSql(string? keyword, OrderStatus? status, long? userId = null)
        {
            var conditions = new List<string>();

            if (userId.HasValue)
            {
                conditions.Add("`user_id` = @UserId");
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                conditions.Add("(`order_no` LIKE CONCAT('%', @Keyword, '%') OR `receiver_name` LIKE CONCAT('%', @Keyword, '%'))");
            }

            if (status.HasValue)
            {
                conditions.Add("`status` = @Status");
            }

            return conditions.Count == 0 ? string.Empty : $" WHERE {string.Join(" AND ", conditions)}";
        }
    }
}
