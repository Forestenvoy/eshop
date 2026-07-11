namespace eshop.application.DTOs.Admin.Response
{
    public class InfoResponse
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// 權限代碼清單
        /// </summary>
        public IEnumerable<string> Permissions { get; set; } = [];
    }
}
