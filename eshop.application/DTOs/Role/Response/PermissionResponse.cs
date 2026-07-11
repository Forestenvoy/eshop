namespace eshop.application.DTOs.Role.Response
{
    public class PermissionResponse
    {
        /// <summary>
        /// 權限 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 權限代碼
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }
}
