namespace eshop.application.DTO.Requests.Admin
{
    public class AdminLoginRequest
    {
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
