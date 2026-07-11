namespace eshop.application.Common.Models
{
    public static class ResponseMessage
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const string SUCCESS = "Success";

        /// <summary>
        /// 失敗
        /// </summary>
        public const string FAIL = "Failed";

        /// <summary>
        /// 系統錯誤
        /// </summary>
        public const string ERROR = "System error. Please try again later.";

        /// <summary>
        /// 非法參數
        /// </summary>
        public const string INVALID_PARAMETERS = "Invalid parameters.";
    }
}
