namespace eshop.application.DTO.Responses.Role
{
    public class RoleResponse
    {
        /// <summary>
        /// 角色名稱
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 權限列表
        /// </summary>
        public List<int> PermissionIds { get; set; } = [];
    }
}
