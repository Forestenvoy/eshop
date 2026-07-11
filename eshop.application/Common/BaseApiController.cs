using Microsoft.AspNetCore.Mvc;

namespace eshop.application.Common
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected ILogger Logger { get; }

        protected BaseApiController(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// 取得目前登入的後台帳號
        /// </summary>
        protected string GetAdminAccount()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == AuthConstants.Claim.Admin);
            return claim == null ? throw new UnauthorizedAccessException("AdminAccount claim is missing.") : claim.Value;
        }
    }
}
