using System.ComponentModel.DataAnnotations;

namespace eshop.application.DTO.Requests.Role
{
    public class RoleAddRequest
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;

        [Required]
        public List<int> PermissionIds { get; set; } = [];
    }
}
