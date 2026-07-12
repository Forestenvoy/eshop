namespace eshop.application.DTOs.Product.Response
{
    public class ProductResponse
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 圖片 URL
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 是否上架
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 排序權重
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string? Modifier { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
