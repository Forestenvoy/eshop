using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Product.Request
{
    public class ProductToggleRequest
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        [Required]
        public long ProductId { get; set; }

        /// <summary>
        /// 是否上架
        /// </summary>
        [Required]
        public bool IsEnabled { get; set; }
    }
}
