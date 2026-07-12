using eshop.application.Enums;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.User.Request
{
    public class UserAddRequest
    {
        /// <summary>
        /// 名稱
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 密碼確認
        /// </summary>
        [Required]
        public string PasswordConfirm { get; set; } = string.Empty;

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
    }
}
