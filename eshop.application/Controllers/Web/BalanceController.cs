using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.Balance.Request;
using eshop.application.DTOs.Balance.Response;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace eshop.application.Controllers.Web
{
    /// <summary>
    /// 會員餘額(前台)
    /// </summary>
    [Route("web/balance")]
    [Authorize(Policy = AuthConstants.Policy.WebOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Front)]
    public class BalanceController : BaseApiController
    {
        private readonly BalanceService _balanceService;

        public BalanceController(ILogger<BalanceController> logger, BalanceService balanceService)
            : base(logger)
        {
            _balanceService = balanceService;
        }

        /// <summary>
        /// 查詢我的餘額
        /// </summary>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<BalanceResponse>))]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _balanceService.GetAsync(GetUserId()));
        }

        /// <summary>
        /// 充值(未串接金流,直接加值)
        /// </summary>
        [HttpPost("topup")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> TopUpAsync([FromBody] BalanceTopUpRequest request)
        {
            return Ok(await _balanceService.TopUpAsync(GetUserId(), request));
        }

        /// <summary>
        /// 用餘額付款結清一筆待付款訂單
        /// </summary>
        [HttpPost("pay")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> PayAsync([FromBody] BalancePayRequest request)
        {
            return Ok(await _balanceService.PayAsync(GetUserId(), request));
        }
    }
}
