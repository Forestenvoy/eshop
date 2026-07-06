using eshop.application.Common;
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
        public string GenerateAdminToken(string account, IEnumerable<string> permissions, TimeSpan ttl)
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
                new(AuthConstants.Claim.TokenType, TokenTypeEnum.Admin.ToString()),
                new(AuthConstants.Claim.AdminAccount, account),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            foreach (var permission in permissions)
            {
                claims.Add(new Claim(AuthConstants.Claim.Permission, permission));
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
    }
}
