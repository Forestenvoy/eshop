using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Order.Request
{
    public class OrderStatusChangeRequest
    {
        /// <summary>
        /// 訂單 ID
        /// </summary>
        [Required]
        public long OrderId { get; set; }
    }
}
