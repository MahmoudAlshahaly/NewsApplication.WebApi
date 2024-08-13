using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsApplication.BLL.Dtos.News
{
    public class UpdateNewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleAr { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }
        public IFormFile Image { get; set; }
        public bool IsDeleted { get; set; }
    }
}
