namespace eshop.application.DTO.Responses.Admin
{
    public class AdminInfoResponse
    {
        public string Account { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public List<string> Permissions { get; set; } = [];
    }
}
