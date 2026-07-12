using eshop.application.Enums;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.User.Request
{
    public class UserUpdateRequest
    {
        /// <summary>
        /// 會員 ID
        /// </summary>
        [Required]
        public long UserId { get; set; }

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
    }
}
