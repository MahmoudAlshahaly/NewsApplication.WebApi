using System.ComponentModel.DataAnnotations;

namespace NewsApplication.BLL.Dtos.Account
{
    public class UpdateUserDto
    {
        public string Id { get; set; }
        [Display(Name = "Full Name")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
    }
}
