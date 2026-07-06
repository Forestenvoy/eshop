namespace eshop.application.DTO.Responses.Role
{
    public class PermissionListResponse
    {
        public int PermissionId { get; set; }

        /// <summary>
        /// 權限名稱
        /// </summary>
        public string PermissionName { get; set; } = string.Empty;
    }
}
