using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Order.Request
{
    public class OrderSubmitItemRequest
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        [Required]
        public long ProductId { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
