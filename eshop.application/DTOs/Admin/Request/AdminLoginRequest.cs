using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Admin.Request
{
    public class AdminLoginRequest
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required(ErrorMessage = "帳號為必填")]
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 密碼
        /// </summary>
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; } = string.Empty;
    }
}
