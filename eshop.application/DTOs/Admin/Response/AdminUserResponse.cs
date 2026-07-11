namespace eshop.application.DTOs.Admin.Response
{
    public class AdminUserResponse
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string? Modifier { get; set; }

        /// <summary>
        /// 操作時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
