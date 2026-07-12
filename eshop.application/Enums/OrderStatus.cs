namespace eshop.application.Enums
{
    /// <summary>
    /// 訂單狀態
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 建立中
        /// </summary>
        Created = 0,

        /// <summary>
        /// 待付款
        /// </summary>
        PendingPayment = 1,

        /// <summary>
        /// 已付款
        /// </summary>
        Paid = 2,

        /// <summary>
        /// 出貨中
        /// </summary>
        Shipping = 3,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 4,

        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled = 5
    }
}
