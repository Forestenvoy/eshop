using eshop.application.Enums;

namespace eshop.application.DTOs.Auth.Response
{
    public class UserProfileResponse
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
        /// 大頭貼
        /// </summary>
        public string? Avatar { get; set; }

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
    }
}
