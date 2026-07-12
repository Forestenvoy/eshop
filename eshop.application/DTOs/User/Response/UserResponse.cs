using eshop.application.Enums;

namespace eshop.application.DTOs.User.Response
{
    public class UserResponse
    {
        /// <summary>
        /// 會員 ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 性別
        /// </summary>
        public UserGender Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// 最後登入時間
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
