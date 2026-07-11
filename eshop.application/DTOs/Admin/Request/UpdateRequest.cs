using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Admin.Request
{
    public class UpdateRequest
    {
        /// <summary>
        /// 管理員 ID
        /// </summary>
        [Required]
        public int AdminId { get; set; }

        /// <summary>
        /// 密碼（留空表示不更新）
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 密碼確認
        /// </summary>
        public string PasswordConfirm { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        public int RoleId { get; set; }

        /// <summary>
        /// 啟用狀態
        /// </summary>
        [Required]
        public bool IsEnabled { get; set; }
    }
}
