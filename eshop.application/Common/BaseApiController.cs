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
        /// Get 管理員 ID
        /// </summary>
        protected int GetAdminId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == AuthConstants.Claim.AdminId);
            return claim == null ? throw new UnauthorizedAccessException("AdminId claim is missing.") : int.Parse(claim.Value);
        }

        /// <summary>
        /// Get 管理員名稱
        /// </summary>
        protected string GetAdminName()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == AuthConstants.Claim.AdminName);
            return claim == null ? throw new UnauthorizedAccessException("AdminId claim is missing.") : claim.Value;
        }
    }
}
