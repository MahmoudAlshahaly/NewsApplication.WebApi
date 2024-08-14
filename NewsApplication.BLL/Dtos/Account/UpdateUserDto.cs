using System.ComponentModel.DataAnnotations;

namespace NewsApplication.BLL.Dtos.Account
{
    public class UpdateUserDto
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Full Name is required.")]
        [Display(Name = "Full Name")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Of Birth")]
        [Required(ErrorMessage = "Date Of Birth is required.")]
        public DateTime? DateOfBirth { get; set; }
    }
}
