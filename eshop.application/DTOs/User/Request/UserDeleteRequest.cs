namespace eshop.application.DTOs.User.Request
{
    public class UserDeleteRequest
    {
        /// <summary>
        /// 會員 ID 清單
        /// </summary>
        public List<long> Ids { get; set; } = [];
    }
}
