using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.application.Models.Admin
{
    /// <summary>
    /// 權限
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 權限 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 權限代碼
        /// </summary>
        public string Code { get; set; } = default!;

        /// <summary>
        /// 建立時間
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        [Column("created_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
