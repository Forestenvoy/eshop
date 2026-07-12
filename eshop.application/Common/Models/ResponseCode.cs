namespace eshop.application.Common.Models
{
    public enum ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS = 1,

        /// <summary>
        /// 失敗
        /// </summary>
        FAIL = -1,

        /// <summary>
        /// 系統錯誤
        /// </summary>
        ERROR = -2,

        /// <summary>
        /// 非法參數
        /// </summary>
        INVALID_PARAMS = -3,

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
        ACCOUNT_NOT_EXIST = -8,

        /// <summary>
        /// 密碼錯誤
        /// </summary>
        PASSWORD_ERROR = -9,

        /// <summary>
        /// 管理員不存在
        /// </summary>
        ADMIN_NOT_EXISTS = -100,

        /// <summary>
        /// 管理員已停用
        /// </summary>
        ADMIN_DISABLED = -101,

        /// <summary>
        /// 管理員已存在
        /// </summary>
        ADMIN_EXISTS = -102,

        /// <summary>
        /// 角色不存在
        /// </summary>
        ROLE_NOT_EXISTS = -103,

        /// <summary>
        /// 系統總管不可刪除
        /// </summary>
        ADMINISTRATOR_CAN_NOT_DELETE = -104,

        /// <summary>
        /// 角色已存在
        /// </summary>
        ROLE_EXISTS = -105,

        /// <summary>
        /// 商品不存在
        /// </summary>
        PRODUCT_NOT_EXISTS = -106,
    }
}
