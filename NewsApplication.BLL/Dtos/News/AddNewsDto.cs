using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewsApplication.BLL.Dtos.News
{
    public class AddNewsDto
    {
        [Required(ErrorMessage = "Title is required."), StringLength(200)]
        public string Title { get; set; }
        [Required(ErrorMessage = "TitleAr is required."), StringLength(200)]
        public string TitleAr { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "DescriptionAr is required.")]
        public string DescriptionAr { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public DateTime PublishDate { get; set; } = DateTime.Now.Date;
        public IFormFile Image { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
