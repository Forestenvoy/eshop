namespace eshop.application.DTOs.Order.Response
{
    public class OrderSubmitResponse
    {
        /// <summary>
        /// 訂單 ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { get; set; } = string.Empty;
    }
}
