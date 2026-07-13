using eshop.application.Data;
using eshop.application.Repositories.Admin.Interfaces;
using Dapper;
using System.Data.Common;
using eshop.application.Models;

namespace eshop.application.Repositories.Admin
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(DbConnection dbConnection, IUnitOfWork unitOfWork)
            : base(dbConnection, unitOfWork)
        {
        }

        /// <inheritdoc />
        public async Task<Product?> GetAsync(long id)
        {
            const string sql = "SELECT * FROM `product` WHERE `id` = @id";

            return await _dbConnection.QuerySingleOrDefaultAsync<Product>(sql, new { id }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> GetCountAsync(string? keyword)
        {
            var whereSql = string.IsNullOrWhiteSpace(keyword)
                ? string.Empty
                : " WHERE `name` LIKE CONCAT('%', @Keyword, '%')";

            var sql = $"SELECT COUNT(1) FROM `product`{whereSql};";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { Keyword = keyword }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> GetPagedListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var whereSql = string.IsNullOrWhiteSpace(keyword)
                ? string.Empty
                : " WHERE `name` LIKE CONCAT('%', @Keyword, '%')";

            var sql = $@"
                SELECT *
                FROM `product`
                {whereSql}
                ORDER BY `sort` DESC, `id` ASC
                LIMIT @Offset, @PageSize;
            ";

            var parameters = new
            {
                Keyword = keyword,
                Offset = (pageIndex - 1) * pageSize,
                PageSize = pageSize
            };

            return await _dbConnection.QueryAsync<Product>(sql, parameters, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> AddAsync(Product product)
        {
            const string sql = @"
                INSERT INTO `product` (name, description, price, stock, image_url, is_enabled, sort, modifier)
                VALUES (@Name, @Description, @Price, @Stock, @ImageUrl, @IsEnabled, @Sort, @Modifier);";

            return await _dbConnection.ExecuteAsync(sql, product, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> UpdateAsync(Product product)
        {
            const string sql = @"
                UPDATE `product`
                SET name = @Name, description = @Description, price = @Price, stock = @Stock,
                    image_url = @ImageUrl, sort = @Sort, modifier = @Modifier
                WHERE id = @Id;";

            return await _dbConnection.ExecuteAsync(sql, product, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> ToggleAsync(long id, bool isEnabled, string modifier)
        {
            const string sql = "UPDATE `product` SET is_enabled = @isEnabled, modifier = @modifier WHERE id = @id;";

            return await _dbConnection.ExecuteAsync(sql, new { id, isEnabled, modifier }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> GetAllEnabledAsync()
        {
            const string sql = "SELECT * FROM `product` WHERE `is_enabled` = TRUE ORDER BY `sort` DESC, `id` ASC;";

            return await _dbConnection.QueryAsync<Product>(sql, transaction: _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> DecrementStockAsync(long productId, int quantity)
        {
            const string sql = @"
                UPDATE `product`
                SET `stock` = `stock` - @quantity
                WHERE `id` = @productId AND `stock` >= @quantity AND `is_enabled` = TRUE;";

            return await _dbConnection.ExecuteAsync(sql, new { productId, quantity }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> GetMaxSortAsync()
        {
            const string sql = "SELECT COALESCE(MAX(`sort`), 0) FROM `product`;";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, transaction: _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> SaveSortAsync(Dictionary<long, int> sortMap)
        {
            const string sql = "UPDATE `product` SET `sort` = @Sort WHERE `id` = @Id;";

            var parameters = sortMap.Select(kv => new { Id = kv.Key, Sort = kv.Value });

            return await _dbConnection.ExecuteAsync(sql, parameters, _dbTransaction);
        }
    }
}
