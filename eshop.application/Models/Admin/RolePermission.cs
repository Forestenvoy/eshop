using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.application.Models.Admin
{
    /// <summary>
    /// 角色與權限的對應
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// 角色 ID
        /// </summary>
        [Column("role_id")]
        public int RoleId { get; set; }

        /// <summary>
        /// 權限 ID
        /// </summary>
        [Column("permissiom_id")]
        public int PermissionId { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
