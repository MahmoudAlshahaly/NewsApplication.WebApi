using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsApplication.DAL.Models
{
    public class News
    {
        public int Id { get; set; }
        [Required ,StringLength(200)]
        public string Title { get; set; }
        [Required, StringLength(200)]
        public string TitleAr { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string DescriptionAr { get; set; }
        [Required , DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }
        [Required]
        public string ImageURL { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
