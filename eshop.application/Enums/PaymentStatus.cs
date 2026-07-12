namespace eshop.application.Enums
{
    /// <summary>
    /// 付款狀態
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// 未付款
        /// </summary>
        Unpaid = 0,

        /// <summary>
        /// 已付款
        /// </summary>
        Paid = 1,

        /// <summary>
        /// 退款中
        /// </summary>
        Refunding = 2,

        /// <summary>
        /// 已退款
        /// </summary>
        Refunded = 3
    }
}
