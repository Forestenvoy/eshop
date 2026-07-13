namespace eshop.application.DTOs.Order.Response
{
    public class OrderDetailResponse : OrderResponse
    {
        /// <summary>
        /// 收件地址
        /// </summary>
        public string ReceiverAddress { get; set; } = string.Empty;

        /// <summary>
        /// 訂單備註
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 訂單商品明細
        /// </summary>
        public List<OrderItemResponse> Items { get; set; } = [];
    }
}
