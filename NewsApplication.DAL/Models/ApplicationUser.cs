using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace NewsApplication.DAL.Models
{
    public enum UserCategory
    {
        Normal,
        ContentAdmin,
        Admin
    }
    public class ApplicationUser : IdentityUser
    {
        [Required , DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public bool IsDeleted { get; set; } = false;
        public List<RefreshToken> RefreshTokens { get; set; }
        public ICollection<News> News { get; set; } = new HashSet<News>();
    }
}
