namespace eshop.application.DTOs.Role.Response
{
    public class SimpleResponse
    {
        /// <summary>
        /// 角色 ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
