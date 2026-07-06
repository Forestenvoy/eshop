namespace eshop.application.Models
{
    /// <summary>
    /// 權限
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 權限 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 權限代碼
        /// </summary>
        public string Name { get; set; } = string.Empty;

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
