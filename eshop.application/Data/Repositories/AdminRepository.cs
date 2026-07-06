using Dapper;
using eshop.application.DTO.Responses.Admin;
using System.Data.Common;

namespace eshop.application.Data.Repositories
{
    public class AdminRepository : BaseRepository
    {
        private readonly ILogger<AdminRepository> _logger;

        public AdminRepository(DbConnection dbConnection, IUnitOfWork unitOfWork, ILogger<AdminRepository> logger)
            : base(dbConnection, unitOfWork)
        {
            _logger = logger;
        }

        /// <summary>
        /// 依帳號查詢登入所需資訊（密碼為雜湊值，驗證交給呼叫端）
        /// </summary>
        public async Task<(int? Id, string? Password, bool? IsEnable)> GetAdminByAccountAsync(string account)
        {
            const string sql = @"
                SELECT `id` AS Id, `password` AS Password, `is_enable` AS IsEnable
                FROM `admin`
                WHERE `account` = @Account
                LIMIT 1;
            ";

            return await _dbConnection.QueryFirstOrDefaultAsync<(int? Id, string? Password, bool? IsEnable)>(
                sql,
                new { Account = account }
            );
        }

        public async Task<int?> GetAdminIdAsync(string account)
        {
            const string sql = @"
                SELECT `id`
                FROM `admin`
                WHERE `account` = @Account
                LIMIT 1;
            ";

            return await _dbConnection.ExecuteScalarAsync<int?>(
                sql,
                new { Account = account }
            );
        }

        public async Task<int> GetAdminRoleIdAsync(int adminId)
        {
            const string sql = @"
                SELECT `role_id`
                FROM `admin`
                WHERE `id` = @AdminId
                LIMIT 1;
            ";
            return await _dbConnection.ExecuteScalarAsync<int>(
                sql,
                new { AdminId = adminId }
            );
        }

        public async Task<(List<AdminListResponse> list, int count)> GetAllAsync(string? keyword, int pageIndex, int pageSize)
        {
            pageIndex = Math.Max(pageIndex, 1);
            pageSize = Math.Max(pageSize, 1);

            var conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                conditions.Add("a.account LIKE CONCAT('%', @Keyword, '%')");
            }

            var whereSql = conditions.Count > 0
                ? " WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            var parameters = new
            {
                Keyword = keyword,
                Offset = (pageIndex - 1) * pageSize,
                PageSize = pageSize
            };

            var countSql = $@"
                SELECT COUNT(1)
                FROM `admin` a
                {whereSql};
            ";

            var count = await _dbConnection.ExecuteScalarAsync<int>(
                countSql,
                parameters,
                transaction: _dbTransaction
            );

            var listSql = $@"
                SELECT
                    a.id AS AdminId,
                    a.account AS Account,
                    a.role_id AS RoleId,
                    r.name AS RoleName,
                    a.is_enable AS IsEnabled,
                    a.modifier AS Modifier,
                    a.updated_at AS UpdatedAt
                FROM `admin` a
                LEFT JOIN `role` r ON a.role_id = r.id
                {whereSql}
                ORDER BY a.id DESC
                LIMIT @Offset, @PageSize;
            ";

            var list = await _dbConnection.QueryAsync<AdminListResponse>(
                listSql,
                parameters,
                transaction: _dbTransaction
            );

            return (list.ToList(), count);
        }

        public async Task<List<RoleIdNameResponse>> GetRoleListAsync()
        {
            var sql = @"
                SELECT
                    r.id AS RoleId,
                    r.name AS Name
                FROM `role` r
                ORDER BY r.id ASC;
            ";
            var list = await _dbConnection.QueryAsync<RoleIdNameResponse>(
                sql,
                transaction: _dbTransaction
            );
            return list.ToList();
        }

        public async Task<AdminResponse?> GetAsync(int adminId)
        {
            var sql = @"
                SELECT
                    a.account AS Account,
                    a.role_id AS RoleId,
                    r.name AS RoleName,
                    a.is_enable AS IsEnabled
                FROM `admin` a
                LEFT JOIN `role` r ON a.role_id = r.id
                WHERE a.id = @AdminId;
            ";

            return await _dbConnection.QuerySingleOrDefaultAsync<AdminResponse>(
                sql,
                new { AdminId = adminId },
                transaction: _dbTransaction
            );
        }

        /// <summary>
        /// 新增帳號
        /// </summary>
        /// <param name="account"></param>
        /// <param name="roleId"></param>
        /// <param name="hashedPassword">已雜湊過的密碼</param>
        /// <param name="adminAccount"></param>
        public async Task AddAsync(string account, int roleId, string hashedPassword, string adminAccount)
        {
            var sql = @"
                INSERT INTO `admin` (account, role_id, password, is_enable, modifier, created_at, updated_at)
                VALUES (@Account, @RoleId, @Password, TRUE, @AdminAccount, NOW(), NOW());
            ";

            await _dbConnection.ExecuteAsync(
                sql,
                new
                {
                    Account = account,
                    RoleId = roleId,
                    Password = hashedPassword,
                    AdminAccount = adminAccount
                },
                transaction: _dbTransaction
            );
        }

        public async Task<bool> CheckAdminAccountExistsAsync(string account)
        {
            var sql = @"
                SELECT EXISTS(
                    SELECT 1
                    FROM `admin`
                    WHERE `account` = @Account
                );
            ";
            return await _dbConnection.ExecuteScalarAsync<bool>(
                sql,
                new { Account = account },
                transaction: _dbTransaction
            );
        }

        public async Task<bool> CheckAdminIdExistsAsync(int adminId)
        {
            var sql = @"
                SELECT EXISTS(
                    SELECT 1
                    FROM `admin`
                    WHERE `id` = @AdminId
                );
            ";
            return await _dbConnection.ExecuteScalarAsync<bool>(
                sql,
                new { AdminId = adminId },
                transaction: _dbTransaction
            );
        }

        /// <summary>
        /// 更新帳號
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <param name="roleId"></param>
        /// <param name="hashedPassword">已雜湊過的密碼，null/空白表示不更新密碼</param>
        /// <param name="isEnabled"></param>
        /// <param name="adminAccount"></param>
        public async Task UpdateAsync(
            int id,
            string account,
            int? roleId,
            string? hashedPassword,
            bool? isEnabled,
            string adminAccount)
        {
            var setClauses = new List<string>();
            var parameters = new DynamicParameters();

            parameters.Add("Id", id);
            parameters.Add("AdminAccount", adminAccount);

            if (!string.IsNullOrWhiteSpace(account))
            {
                setClauses.Add("account = @Account");
                parameters.Add("Account", account);
            }

            if (roleId.HasValue && roleId > 0)
            {
                setClauses.Add("role_id = @RoleId");
                parameters.Add("RoleId", roleId.Value);
            }

            if (!string.IsNullOrWhiteSpace(hashedPassword))
            {
                setClauses.Add("password = @Password");
                parameters.Add("Password", hashedPassword);
            }

            if (isEnabled.HasValue)
            {
                setClauses.Add("is_enable = @IsEnabled");
                parameters.Add("IsEnabled", isEnabled.Value);
            }

            if (setClauses.Count == 0)
            {
                return;
            }

            var sql = $@"
                UPDATE `admin`
                SET
                    {string.Join(",\n            ", setClauses)},
                    modifier = @AdminAccount,
                    updated_at = NOW()
                WHERE id = @Id;
            ";

            await _dbConnection.ExecuteAsync(
                sql,
                parameters,
                transaction: _dbTransaction
            );
        }

        public async Task DeleteAsync(int adminId)
        {
            var sql = @"
                DELETE FROM `admin`
                WHERE id = @AdminId;
            ";
            await _dbConnection.ExecuteAsync(
                sql,
                new { AdminId = adminId },
                transaction: _dbTransaction
            );
        }
    }
}
