namespace eshop.application.DTOs.Auth.Response
{
    public class LoginResponse
    {
        /// <summary>
        /// JWT 驗證 Token，用於後續 API 請求的身分驗證
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}
