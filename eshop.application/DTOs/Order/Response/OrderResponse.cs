using eshop.application.Enums;

namespace eshop.application.DTOs.Order.Response
{
    public class OrderResponse
    {
        /// <summary>
        /// 訂單 ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { get; set; } = string.Empty;

        /// <summary>
        /// 使用者 ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 訂單總金額
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// 付款狀態
        /// </summary>
        public PaymentStatus PaymentStatus { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string ReceiverPhone { get; set; } = string.Empty;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
