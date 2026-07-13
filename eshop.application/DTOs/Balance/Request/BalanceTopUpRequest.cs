using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTOs.Balance.Request
{
    public class BalanceTopUpRequest
    {
        /// <summary>
        /// 充值金額
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
