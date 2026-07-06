namespace eshop.application.DTO.Responses.Role
{
    public class RoleListResponse
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
