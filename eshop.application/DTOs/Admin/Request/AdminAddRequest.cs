using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Admin.Request
{
    public class AdminAddRequest
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        public string Account { get; set; } = string.Empty;

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
        /// 角色ID
        /// </summary>
        [Required]
        public int RoleId { get; set; }
    }
}
