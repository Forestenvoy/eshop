using Dapper;
using eshop.application.Data;
using eshop.application.Models;
using eshop.application.Repositories.Admin.Interfaces;
using System.Data.Common;

namespace eshop.application.Repositories.Admin
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(DbConnection dbConnection, IUnitOfWork unitOfWork)
            : base(dbConnection, unitOfWork)
        {
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            const string sql = "SELECT * FROM `role` ORDER BY `id` ASC;";

            return await _dbConnection.QueryAsync<Role>(sql, transaction: _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            const string sql = "SELECT * FROM `permission` ORDER BY `id` ASC;";

            return await _dbConnection.QueryAsync<Permission>(sql, transaction: _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<Role?> GetAsync(int id)
        {
            const string sql = "SELECT * FROM `role` WHERE `id` = @id";

            return await _dbConnection.QuerySingleOrDefaultAsync<Role>(sql, new { id }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<List<int>> GetPermissionIdsAsync(int roleId)
        {
            const string sql = "SELECT `permission_id` FROM `role_permission` WHERE `role_id` = @roleId;";

            var result = await _dbConnection.QueryAsync<int>(sql, new { roleId }, _dbTransaction);
            return result.ToList();
        }

        /// <inheritdoc />
        public async Task<int> GetCountAsync(string? keyword)
        {
            var whereSql = string.IsNullOrWhiteSpace(keyword)
                ? string.Empty
                : " WHERE `name` LIKE CONCAT('%', @Keyword, '%')";

            var sql = $"SELECT COUNT(1) FROM `role`{whereSql};";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { Keyword = keyword }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Role>> GetPagedListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var whereSql = string.IsNullOrWhiteSpace(keyword)
                ? string.Empty
                : " WHERE `name` LIKE CONCAT('%', @Keyword, '%')";

            var sql = $@"
                SELECT *
                FROM `role`
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

            return await _dbConnection.QueryAsync<Role>(sql, parameters, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int id)
        {
            const string sql = @"SELECT EXISTS(SELECT 1 FROM `role` WHERE id = @id);";

            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { id }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsByNameAsync(string roleName)
        {
            const string sql = "SELECT EXISTS(SELECT 1 FROM `role` WHERE `name` = @roleName);";

            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { roleName }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<int> AddAsync(Role role, List<int> permissionIds)
        {
            const string sqlInsertRole = @"
                INSERT INTO `role` (name, modifier)
                VALUES (@Name, @Modifier);
                SELECT LAST_INSERT_ID();
            ";

            var roleId = await _dbConnection.ExecuteScalarAsync<int>(
                sqlInsertRole,
                new { role.Name, role.Modifier },
                _dbTransaction);

            if (permissionIds.Count > 0)
            {
                const string sqlInsertRolePermission = @"
                    INSERT INTO `role_permission` (role_id, permission_id)
                    VALUES (@RoleId, @PermissionId);
                ";

                var rolePermissionParams = permissionIds.Select(permissionId => new
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });

                await _dbConnection.ExecuteAsync(sqlInsertRolePermission, rolePermissionParams, _dbTransaction);
            }

            return roleId;
        }

        /// <inheritdoc />
        public async Task<int> UpdateAsync(Role role, List<int> permissionIds)
        {
            const string sqlUpdateRole = @"
                UPDATE `role`
                SET name = @Name, modifier = @Modifier
                WHERE id = @Id;
            ";

            var affected = await _dbConnection.ExecuteAsync(
                sqlUpdateRole,
                new { role.Id, role.Name, role.Modifier },
                _dbTransaction);

            const string sqlDeleteRolePermissions = "DELETE FROM `role_permission` WHERE `role_id` = @roleId;";
            await _dbConnection.ExecuteAsync(sqlDeleteRolePermissions, new { roleId = role.Id }, _dbTransaction);

            if (permissionIds.Count > 0)
            {
                const string sqlInsertRolePermission = @"
                    INSERT INTO `role_permission` (role_id, permission_id)
                    VALUES (@RoleId, @PermissionId);
                ";

                var rolePermissionParams = permissionIds.Select(permissionId => new
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });

                await _dbConnection.ExecuteAsync(sqlInsertRolePermission, rolePermissionParams, _dbTransaction);
            }

            return affected;
        }

        /// <inheritdoc />
        public async Task<int> RemoveRangeAsync(List<int> ids)
        {
            if (ids.Count == 0)
            {
                return 0;
            }

            const string sql = "DELETE FROM `role` WHERE id IN @ids;";

            return await _dbConnection.ExecuteAsync(sql, new { ids }, _dbTransaction);
        }

        /// <inheritdoc />
        public async Task<List<string>> GetPermissionCodesByAdminAsync(int adminId)
        {
            const string sql = @"
                SELECT p.code
                FROM `admin` a
                JOIN `role_permission` rp ON a.role_id = rp.role_id
                JOIN `permission` p ON rp.permission_id = p.id
                WHERE a.id = @adminId;
            ";

            var result = await _dbConnection.QueryAsync<string>(sql, new { adminId }, _dbTransaction);
            return result.ToList();
        }
    }
}
