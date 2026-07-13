namespace eshop.application.DTOs.Order.Response
{
    public class OrderItemResponse
    {
        /// <summary>
        /// 訂單商品明細 ID
        /// </summary>
        public long OrderItemId { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 商品名稱(下單快照)
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// 商品價格(下單當下價格)
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 小計金額
        /// </summary>
        public decimal Subtotal { get; set; }
    }
}
