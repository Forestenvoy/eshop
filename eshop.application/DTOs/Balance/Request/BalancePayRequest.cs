using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Balance.Request
{
    public class BalancePayRequest
    {
        /// <summary>
        /// 訂單 ID
        /// </summary>
        [Required]
        public long OrderId { get; set; }
    }
}
