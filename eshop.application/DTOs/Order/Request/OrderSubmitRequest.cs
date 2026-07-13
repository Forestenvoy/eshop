using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Order.Request
{
    public class OrderSubmitRequest
    {
        /// <summary>
        /// 收件人姓名
        /// </summary>
        [Required]
        public string ReceiverName { get; set; } = default!;

        /// <summary>
        /// 收件人電話
        /// </summary>
        [Required]
        public string ReceiverPhone { get; set; } = default!;

        /// <summary>
        /// 收件地址
        /// </summary>
        [Required]
        public string ReceiverAddress { get; set; } = default!;

        /// <summary>
        /// 訂單備註
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 購買商品明細
        /// </summary>
        [Required]
        [MinLength(1)]
        public List<OrderSubmitItemRequest> Items { get; set; } = [];
    }
}
