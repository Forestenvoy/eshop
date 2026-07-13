using eshop.application.Common;
using eshop.application.Enums;
using eshop.application.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eshop.application.Services
{
    /// <summary>
    /// JWT 簽發服務
    /// </summary>
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 產生後台管理員 Token
        /// </summary>
        public string GenerateAdminToken(AdminUser adminUser, IEnumerable<string> permissionCodes, TimeSpan ttl)
        {
            string jwtSecret = _configuration.GetValue<string>("Token:Secret")
                ?? throw new InvalidOperationException("JWT Secret is not configured.");

            string jwtIssuer = _configuration.GetValue<string>("Token:Issuer")
                ?? throw new InvalidOperationException("JWT Issuer is not configured.");

            string jwtAudience = _configuration.GetValue<string>("Token:Audience")
                ?? throw new InvalidOperationException("JWT Audience is not configured.");

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expireTimeUtc = now.Add(ttl);

            var claims = new List<Claim>
            {
                new(AuthConstants.Claim.TokenType, TokenType.Admin.ToString()),
                new(AuthConstants.Claim.AdminId, adminUser.Id.ToString()),
                new(AuthConstants.Claim.AdminName, adminUser.Account),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            foreach (var permissionCode in permissionCodes)
            {
                claims.Add(new Claim(AuthConstants.Claim.Permission, permissionCode));
            }

            var jwt = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                notBefore: now,
                expires: expireTimeUtc,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        /// <summary>
        /// 產生前台會員 Token
        /// </summary>
        public string GenerateUserToken(User user, TimeSpan ttl)
        {
            string jwtSecret = _configuration.GetValue<string>("Token:Secret")
                ?? throw new InvalidOperationException("JWT Secret is not configured.");

            string jwtIssuer = _configuration.GetValue<string>("Token:Issuer")
                ?? throw new InvalidOperationException("JWT Issuer is not configured.");

            string jwtAudience = _configuration.GetValue<string>("Token:Audience")
                ?? throw new InvalidOperationException("JWT Audience is not configured.");

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expireTimeUtc = now.Add(ttl);

            var claims = new List<Claim>
            {
                new(AuthConstants.Claim.TokenType, TokenType.Web.ToString()),
                new(AuthConstants.Claim.UserId, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            var jwt = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                notBefore: now,
                expires: expireTimeUtc,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
