namespace eshop.application.Models
{
    /// <summary>
    /// 後台管理員
    /// </summary>
    public class Admin
    {
        /// <summary>
        /// 管理員 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 密碼（雜湊值）
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 角色 ID
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 最後異動人
        /// </summary>
        public string? Modifier { get; set; }

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
