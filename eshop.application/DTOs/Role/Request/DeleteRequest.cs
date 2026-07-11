namespace eshop.application.DTOs.Role.Request
{
    /// <summary>
    /// 刪除請求
    /// </summary>
    public class DeleteRequest
    {
        /// <summary>
        /// 角色 ID 清單
        /// </summary>
        public List<int> Ids { get; set; } = [];
    }
}
