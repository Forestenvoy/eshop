using Dapper;
using eshop.application.Data.IRepositories;
using eshop.application.DTO.Responses.Admin;
using eshop.application.DTO.Responses.Role;
using System.Data.Common;
using System.Text;

namespace eshop.application.Data.Repositories
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(DbConnection dbConnection, IUnitOfWork unitOfWork, ILogger<RoleRepository> logger)
            : base(dbConnection, unitOfWork)
        {
            _logger = logger;
        }

        public async Task<(List<RoleListResponse> list, int count)> GetRoleListAsync(
            string? keyword,
            int pageIndex,
            int pageSize)
        {
            pageIndex = Math.Max(pageIndex, 1);
            pageSize = Math.Max(pageSize, 1);

            var conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                conditions.Add("r.name LIKE CONCAT('%', @Keyword, '%')");
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
                FROM `role` r
                {whereSql};
            ";

            var count = await _dbConnection.ExecuteScalarAsync<int>(
                countSql,
                parameters,
                transaction: _dbTransaction
            );

            var listSql = $@"
                SELECT
                    r.id AS RoleId,
                    r.name AS Name,
                    r.modifier AS Modifier,
                    r.updated_at AS UpdatedAt
                FROM `role` r
                {whereSql}
                ORDER BY r.id ASC
                LIMIT @Offset, @PageSize;
            ";

            var result = await _dbConnection.QueryAsync<RoleListResponse>(
                listSql,
                parameters,
                transaction: _dbTransaction
            );

            return (result.ToList(), count);
        }

        public async Task<RoleResponse?> GetRoleDetailAsync(int roleId)
        {
            var roleName = await _dbConnection.ExecuteScalarAsync<string?>(
                @"SELECT name
                  FROM `role`
                  WHERE id = @RoleId;",
                new { RoleId = roleId },
                transaction: _dbTransaction
            );

            if (roleName == null)
            {
                return null;
            }

            var permissionIds = (await _dbConnection.QueryAsync<int>(
                @"SELECT permission_id
                  FROM `role_permission`
                  WHERE role_id = @RoleId;",
                new { RoleId = roleId },
                transaction: _dbTransaction
            )).ToList();

            return new RoleResponse
            {
                RoleName = roleName,
                PermissionIds = permissionIds
            };
        }

        public async Task AddAsync(string? account, string roleName, List<int> permissionIds)
        {
            var sqlInsertRole = @"
                INSERT INTO `role` (name, modifier, created_at, updated_at)
                VALUES (@RoleName, @Account, NOW(), NOW());
                SELECT LAST_INSERT_ID();
            ";

            var roleId = await _dbConnection.ExecuteScalarAsync<int>(
                sqlInsertRole,
                new { RoleName = roleName, Account = account },
                transaction: _dbTransaction
            );

            var sqlInsertRolePermission = @"
                INSERT INTO `role_permission` (role_id, permission_id)
                VALUES (@RoleId, @PermissionId);
            ";
            var rolePermissionParams = permissionIds.Select(pid => new
            {
                RoleId = roleId,
                PermissionId = pid
            });

            await _dbConnection.ExecuteAsync(
                sqlInsertRolePermission,
                rolePermissionParams,
                transaction: _dbTransaction
            );
        }

        public async Task<List<PermissionListResponse>> GetPermissionListAsync()
        {
            var sql = @"
                SELECT
                    id AS PermissionId,
                    code AS PermissionCode
                FROM `permission`
                ORDER BY id ASC;
            ";

            var result = await _dbConnection.QueryAsync<PermissionListResponse>(
                sql,
                transaction: _dbTransaction
            );

            return result.ToList();
        }

        public async Task<List<string>> GetAdminIdRolePermissionListAsync(int adminId)
        {
            var sql = @"
                SELECT p.code
                FROM `admin` a
                JOIN `role_permission` rp ON a.role_id = rp.role_id
                JOIN `permission` p ON rp.permission_id = p.id
                WHERE a.id = @AdminId;
            ";

            var result = await _dbConnection.QueryAsync<string>(
                sql,
                new { AdminId = adminId },
                transaction: _dbTransaction
            );
            return result.ToList();
        }

        public async Task UpdateAsync(string account, int roleId, string roleName, List<int> permissionIds)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("UPDATE `role`");
            sqlBuilder.AppendLine("SET");

            var parameters = new DynamicParameters();
            var setList = new List<string>();

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                setList.Add("name = @RoleName");
                parameters.Add("RoleName", roleName);
            }

            setList.Add("modifier = @Account");
            setList.Add("updated_at = NOW()");

            parameters.Add("Account", account);
            parameters.Add("RoleId", roleId);

            sqlBuilder.AppendLine(string.Join(", ", setList));
            sqlBuilder.AppendLine("WHERE id = @RoleId;");

            var affected = await _dbConnection.ExecuteAsync(
                sqlBuilder.ToString(),
                parameters,
                transaction: _dbTransaction
            );

            if (affected == 0)
            {
                throw new InvalidOperationException("Role 不存在");
            }

            var sqlDeleteRolePermissions = @"
                DELETE FROM `role_permission`
                WHERE role_id = @RoleId;
            ";
            await _dbConnection.ExecuteAsync(
                sqlDeleteRolePermissions,
                new { RoleId = roleId },
                transaction: _dbTransaction
            );

            var sqlInsertRolePermission = @"
                INSERT INTO `role_permission` (role_id, permission_id)
                VALUES (@RoleId, @PermissionId);
            ";

            permissionIds = permissionIds
                .Distinct()
                .ToList();

            var rolePermissionParams = permissionIds.Select(pid => new
            {
                RoleId = roleId,
                PermissionId = pid
            });

            if (permissionIds.Count > 0)
            {
                await _dbConnection.ExecuteAsync(
                    sqlInsertRolePermission,
                    rolePermissionParams,
                    transaction: _dbTransaction
                );
            }
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            var sqlDeleteRolePermissions = @"
                DELETE FROM `role_permission`
                WHERE role_id = @RoleId;
            ";
            await _dbConnection.ExecuteAsync(
                sqlDeleteRolePermissions,
                new { RoleId = roleId },
                transaction: _dbTransaction
            );

            var sqlDeleteRole = @"
                DELETE FROM `role`
                WHERE id = @RoleId;
            ";
            await _dbConnection.ExecuteAsync(
                sqlDeleteRole,
                new { RoleId = roleId },
                transaction: _dbTransaction
            );
        }

        public async Task<bool> CheckRoleExistsAsync(int roleId)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM `role`
                WHERE id = @RoleId;
            ";
            var count = await _dbConnection.ExecuteScalarAsync<int>(
                sql,
                new { RoleId = roleId },
                transaction: _dbTransaction
            );
            return count > 0;
        }
    }
}
