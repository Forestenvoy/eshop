namespace eshop.application.DTOs.Role.Response
{
    public class DetailResponse
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
