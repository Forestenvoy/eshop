using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.application.Models
{
    /// <summary>
    /// 商品
    /// </summary>
    public class Product
    {
        /// <summary>
        /// ID
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Column("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// 描述
        /// </summary>
        [Column("description")]
        public string? Description { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        [Column("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        [Column("stock")]
        public int Stock { get; set; }

        /// <summary>
        /// 圖片 URL
        /// </summary>
        [Column("image_url")]
        public string? ImageUrl { get; set; }


        /// <summary>
        /// 是否上架
        /// </summary>
        [Column("is_enabled")]
        public bool IsEnabled { get; set; }


        /// <summary>
        /// 排序權重
        /// </summary>
        [Column("sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [Column("modifier")]
        public string? Modifier { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
