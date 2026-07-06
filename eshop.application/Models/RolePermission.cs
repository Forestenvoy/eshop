namespace eshop.application.Models
{
    /// <summary>
    /// 角色與權限的對應
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// 對應 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 角色 ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 權限 ID
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
