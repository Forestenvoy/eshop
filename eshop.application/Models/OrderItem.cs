using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.application.Models
{
    /// <summary>
    /// 訂單商品明細
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// 訂單商品明細 Id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 訂單 Id
        /// </summary>
        [Column("order_id")]
        public long OrderId { get; set; }

        /// <summary>
        /// 商品 Id
        /// </summary>
        [Column("product_id")]
        public long ProductId { get; set; }

        /// <summary>
        /// 商品名稱（下單快照）
        /// </summary>
        [Column("product_name")]
        public string ProductName { get; set; } = default!;

        /// <summary>
        /// 商品價格（下單當下價格）
        /// </summary>
        [Column("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        [Column("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// 小計金額
        /// </summary>
        [Column("subtotal")]
        public decimal Subtotal { get; set; }
    }
}
