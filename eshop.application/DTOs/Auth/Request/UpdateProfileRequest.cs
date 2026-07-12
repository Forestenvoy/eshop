using eshop.application.Enums;

namespace eshop.application.DTOs.Auth.Request
{
    public class UpdateProfileRequest
    {
        /// <summary>
        /// 名稱
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        public UserGender? Gender { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 大頭貼(相對路徑)
        /// </summary>
        public string? Avatar { get; set; }
    }
}
