namespace eshop.application.Common
{
    /// <summary>
    /// 身份驗證相關常數
    /// </summary>
    public static class AuthConstants
    {
        /// <summary>
        /// JWT Claim 類型
        /// </summary>
        public static class Claim
        {
            /// <summary>
            /// TOKEN 類型
            /// </summary>
            public const string TokenType = "TT";

            /// <summary>
            /// 管理員ID
            /// </summary>
            public const string AdminId = "AID";

            /// <summary>
            /// 管理員名稱
            /// </summary>
            public const string AdminName = "ANE";

            /// <summary>
            /// 權限代碼
            /// </summary>
            public const string Permission = "P";
        }

        /// <summary>
        /// 授權策略
        /// </summary>
        public static class Policy
        {
            /// <summary>
            /// 後台專用
            /// </summary>
            public const string AdminOnly = "AdminOnly";

            /// <summary>
            /// 前台專用
            /// </summary>
            public const string WebOnly = "WebOnly";
        }

        /// <summary>
        /// 權限代碼
        /// </summary>
        public static class Permission
        {
            /// <summary>
            /// 角色-檢視內容
            /// </summary>
            public const string RoleView = "ROLE_VIEW";

            /// <summary>
            /// 角色-新增、編輯、刪除
            /// </summary>
            public const string RoleEdit = "ROLE_EDIT";

            /// <summary>
            /// 管理員-檢視內容
            /// </summary>
            public const string AdminView = "ADMIN_VIEW";

            /// <summary>
            /// 管理員-新增、編輯、刪除
            /// </summary>
            public const string AdminEdit = "ADMIN_EDIT";

            /// <summary>
            /// 紀錄-檢視內容
            /// </summary>
            public const string RecordView = "RECORD_VIEW";

            /// <summary>
            /// 紀錄-導出
            /// </summary>
            public const string RecordExport = "RECORD_EXPORT";

            /// <summary>
            /// 所有權限
            /// </summary>
            public static readonly string[] All =
            [
                RoleView,
                RoleEdit,
                AdminView,
                AdminEdit,
                RecordView,
                RecordExport
            ];
        }
    }
}
