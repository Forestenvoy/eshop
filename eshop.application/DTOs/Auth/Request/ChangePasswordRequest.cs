using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Auth.Request
{
    public class ChangePasswordRequest
    {
        /// <summary>
        /// 舊密碼
        /// </summary>
        [Required(ErrorMessage = "舊密碼為必填")]
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// 新密碼
        /// </summary>
        [Required(ErrorMessage = "新密碼為必填")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// 新密碼確認
        /// </summary>
        [Required(ErrorMessage = "新密碼確認為必填")]
        public string NewPasswordConfirm { get; set; } = string.Empty;
    }
}
