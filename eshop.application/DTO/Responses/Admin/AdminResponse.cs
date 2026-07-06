namespace eshop.application.DTO.Responses.Admin
{
    public class AdminResponse
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        public bool IsEnabled { get; set; }
    }
}
