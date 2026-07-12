using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Auth.Request
{
    public class LoginRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "Email為必填")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 密碼
        /// </summary>
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; } = string.Empty;
    }
}
