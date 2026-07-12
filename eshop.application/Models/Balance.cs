using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.application.Models
{
    /// <summary>
    /// 用戶餘額
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// 使用者 Id
        /// </summary>
        [Column("user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// 餘額
        /// </summary>
        [Column("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
