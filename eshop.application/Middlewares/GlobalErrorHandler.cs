using eshop.application.Common;
using eshop.application.Common.Models;
using Newtonsoft.Json;
using System.Net.Mime;

namespace eshop.application.Middlewares
{
    /// <summary>
    /// 全域例外處理 Middleware
    /// </summary>
    public class GlobalErrorHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandler> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public GlobalErrorHandler(RequestDelegate next, ILogger<GlobalErrorHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Middleware 執行入口，攔截 pipeline 中的未處理例外
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // 記錄未處理例外（系統層級錯誤）
                _logger.LogError(ex, "Request unhandled exception");

                // 回傳統一錯誤格式給前端
                await HandleExceptionAsync(context);
            }
        }

        /// <summary>
        /// 統一錯誤回應處理
        /// </summary>
        /// <remarks>
        /// 回傳固定錯誤格式，不暴露內部 exception 細節給 client
        /// </remarks>
        private static async Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var responseModel = ApiResponse.Fail(ResponseCode.ERROR, ResponseMessage.ERROR);

            await context.Response.WriteAsync(JsonConvert.SerializeObject(responseModel, JsonSetting.CamelCase));
        }
    }
}
