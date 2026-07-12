using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Auth.Request
{
    public class RegisterRequest
    {
        /// <summary>
        /// 名稱
        /// </summary>
        [Required(ErrorMessage = "名稱為必填")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "Email為必填")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 密碼
        /// </summary>
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 密碼確認
        /// </summary>
        [Required(ErrorMessage = "密碼確認為必填")]
        public string PasswordConfirm { get; set; } = string.Empty;
    }
}
