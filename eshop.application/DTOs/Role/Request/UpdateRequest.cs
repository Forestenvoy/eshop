using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Role.Request
{
    public class UpdateRequest
    {
        /// <summary>
        /// 角色 ID
        /// </summary>
        [Required]
        public int RoleId { get; set; }

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
