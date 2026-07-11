using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Role.Request
{
    public class RoleAddRequest
    {
        /// <summary>
        /// 角色名稱
        /// </summary>
        [Required]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 權限列表
        /// </summary>
        [Required]
        public List<int> PermissionIds { get; set; } = [];
    }
}
