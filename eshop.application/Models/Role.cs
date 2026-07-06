namespace eshop.application.Models
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role
    {
        /// <summary>
        /// 角色 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 角色名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

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
