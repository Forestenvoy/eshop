namespace eshop.application.Common
{
    public enum ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS = 1,

        /// <summary>
        /// 非法參數
        /// </summary>
        BAD_PARAMS = -1,

        /// <summary>
        /// 系統錯誤
        /// </summary>
        ERROR = -2,

        /// <summary>
        /// 失敗
        /// </summary>
        FAIL = -3,

        /// <summary>
        /// 未登入
        /// </summary>
        NO_LOGIN = -4,

        /// <summary>
        /// 無權限
        /// </summary>
        FORBID = -5,

        /// <summary>
        /// Token 過期
        /// </summary>
        TOKEN_EXPIRED = -6,

        /// <summary>
        /// 無效 Token
        /// </summary>
        TOKEN_INVALID = -7,

        /// <summary>
        /// 帳號不存在
        /// </summary>
        ADMIN_NOT_EXISTS = -8,

        /// <summary>
        /// 帳號已停用
        /// </summary>
        ADMIN_DISABLED = -9,

        /// <summary>
        /// 帳號已存在
        /// </summary>
        ADMIN_EXISTS = -10,

        /// <summary>
        /// 角色不存在
        /// </summary>
        ROLE_NOT_EXISTS = -11,
    }
}
