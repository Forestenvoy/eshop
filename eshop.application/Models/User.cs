using eshop.application.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.application.Models
{
    /// <summary>
    /// 用戶資料
    /// </summary>
    public class User
    {
        /// <summary>
        /// 使用者 Id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Column("name")]
        public string? Name { get; set; }

        /// <summary>
        /// 電子信箱
        /// </summary>
        [Column("email")]
        public string Email { get; set; } = default!;

        /// <summary>
        /// 密碼
        /// </summary>
        [Column("password")]
        public string Password { get; set; } = default!;

        /// <summary>
        /// 性別
        /// </summary>
        [Column("gender")]
        public UserGender Gender { get; set; }

        /// <summary>
        /// 頭像圖片 URL
        /// </summary>
        [Column("avatar")]
        public string? Avatar { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Column("birthday")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手機號碼
        /// </summary>
        [Column("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Column("address")]
        public string? Address {  get; set; }

        /// <summary>
        /// 使用者狀態
        /// </summary>
        [Column("status")]
        public UserStatus Status { get; set; }

        /// <summary>
        /// 最後登入時間
        /// </summary>
        [Column("last_login_at")]
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
