using eshop.application.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace eshop.application.Configurations
{
    /// <summary>
    /// 身份驗證與授權設定
    /// </summary>
    public static class ConfigureAuthentication
    {
        /// <summary>
        /// 註冊系統的 JWT 身份驗證與授權機制（Authentication + Authorization）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IServiceCollection AddJwtAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            string jwtSecret = configuration.GetValue<string>("Token:Secret")
                ?? throw new InvalidOperationException("JWT Secret is not configured.");

            string jwtIssuer = configuration.GetValue<string>("Token:Issuer")
                ?? throw new InvalidOperationException("JWT Issuer is not configured.");

            string jwtAudience = configuration.GetValue<string>("Token:Audience")
                ?? throw new InvalidOperationException("JWT Audience is not configured.");

            services.AddAuthentication(options =>
            {
                // 預設使用 JWT Bearer 驗證
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ===== Token 基本驗證 =====
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,

                    // ===== Issuer 驗證 =====
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,

                    // ===== Audience 驗證 =====
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,

                    // ===== Signature 驗證 =====
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret)
                    ),

                    // Role 對應 claim
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    // 401：未登入 / Token 無效 (過期、簽名驗證失敗、格式錯誤)
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = MediaTypeNames.Application.Json;

                        var responseModel =
                            context.AuthenticateFailure is SecurityTokenExpiredException
                                ? new ResponseModel(ResponseCode.TOKEN_EXPIRED)
                                : context.Error == "invalid_token"
                                    ? new ResponseModel(ResponseCode.TOKEN_INVALID)
                                    : new ResponseModel(ResponseCode.NO_LOGIN);

                        return context.Response.WriteAsync(
                            JsonConvert.SerializeObject(responseModel)
                        );
                    },
                    // 403：已登入但無權限
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = MediaTypeNames.Application.Json;

                        var responseModel = new ResponseModel(ResponseCode.FORBID);

                        return context.Response.WriteAsync(
                            JsonConvert.SerializeObject(responseModel)
                        );
                    }
                };

                // production 關閉（避免洩漏驗證細節）
                options.IncludeErrorDetails = environment.IsDevelopment();
            });

            // 權限授權（Permission-based policies）
            services.AddAuthorization(options =>
            {
                //// 由 Permission constants 自動註冊 policy，每個 permission 對應一個 policy
                //var permissions = typeof(AuthConstants.PermissionClaim)
                //    .GetFields(BindingFlags.Public | BindingFlags.Static)
                //    .Select(f => f.GetValue(null)?.ToString())
                //    .Where(p => !string.IsNullOrEmpty(p));

                //foreach (var permission in permissions)
                //{
                //    options.AddPolicy(permission!, policy =>
                //    {
                //        // 檢查 JWT 是否包含對應 Permission claim
                //        policy.RequireClaim(AuthConstants.Claim.Permission, permission!);
                //    });
                //}
            });

            return services;
        }
    }
}
