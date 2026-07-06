using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTO.Requests.Admin
{
    public class AdminUpdateRequest
    {
        /// <summary>
        /// 管理員 ID
        /// </summary>
        [Required]
        public int AdminId { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; } = string.Empty;

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
        public int RoleId { get; set; }

        /// <summary>
        /// 啟用狀態
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
