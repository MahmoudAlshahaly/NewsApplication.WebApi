using NewsApplication.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace NewsApplication.BLL.Dtos.Account
{
    public class AddRoleDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Role is required.")]
        public UserCategory Role { get; set; }
    }
}