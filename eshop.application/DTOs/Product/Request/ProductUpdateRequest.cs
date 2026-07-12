using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Product.Request
{
    public class ProductUpdateRequest
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        [Required]
        public long ProductId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        /// <summary>
        /// 圖片 URL
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}
