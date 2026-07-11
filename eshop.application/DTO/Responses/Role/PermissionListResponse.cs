namespace eshop.application.DTO.Responses.Role
{
    public class PermissionListResponse
    {
        public int PermissionId { get; set; }

        /// <summary>
        /// 權限代碼
        /// </summary>
        public string PermissionCode { get; set; } = string.Empty;
    }
}
