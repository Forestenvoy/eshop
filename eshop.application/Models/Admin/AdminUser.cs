using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.application.Models.Admin
{
    /// <summary>
    /// 後台管理員
    /// </summary>
    public class AdminUser
    {
        /// <summary>
        /// 管理員 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; } = default!;

        /// <summary>
        /// 密碼（雜湊值）
        /// </summary>
        public string Password { get; set; } = default!;

        /// <summary>
        /// 角色 ID
        /// </summary>
        [Column("role_id")]
        public int? RoleId { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [Column("is_enable")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 最後異動人
        /// </summary>
        public string? Modifier { get; set; }

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
