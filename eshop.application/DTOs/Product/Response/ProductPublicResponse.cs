namespace eshop.application.DTOs.Product.Response
{
    public class ProductPublicResponse
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 圖片 URL
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        public int Stock { get; set; }
    }
}
