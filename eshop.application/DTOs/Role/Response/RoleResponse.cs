namespace eshop.application.DTOs.Role.Response
{
    public class RoleResponse
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Modifier { get; set; }
        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
