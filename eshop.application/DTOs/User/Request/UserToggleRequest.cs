using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.User.Request
{
    public class UserToggleRequest
    {
        /// <summary>
        /// 會員 ID
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// 是否啟用(false = 停權)
        /// </summary>
        [Required]
        public bool IsEnabled { get; set; }
    }
}
