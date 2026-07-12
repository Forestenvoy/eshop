using eshop.application.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.application.Models
{
    /// <summary>
    /// 訂單資料
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 訂單 Id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        [Column("order_no")]
        public string OrderNo { get; set; } = default!;

        /// <summary>
        /// 使用者 Id
        /// </summary>
        [Column("user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// 訂單總金額
        /// </summary>
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        [Column("status")]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// 付款狀態
        /// </summary>
        [Column("payment_status")]
        public PaymentStatus PaymentStatus { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        [Column("receiver_name")]
        public string ReceiverName { get; set; } = default!;

        /// <summary>
        /// 收件人電話
        /// </summary>
        [Column("receiver_phone")]
        public string ReceiverPhone { get; set; } = default!;

        /// <summary>
        /// 收件地址
        /// </summary>
        [Column("receiver_address")]
        public string ReceiverAddress { get; set; } = default!;

        /// <summary>
        /// 訂單備註
        /// </summary>
        [Column("remark")]
        public string? Remark { get; set; }

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
