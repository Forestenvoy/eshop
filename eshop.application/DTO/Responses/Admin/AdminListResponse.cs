namespace eshop.application.DTO.Responses.Admin
{
    public class AdminListResponse
    {
        public int AdminId { get; set; }

        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 角色 ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名稱
        /// </summary>
        public string? RoleName { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string? Modifier { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
