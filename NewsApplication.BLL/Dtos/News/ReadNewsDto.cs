using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsApplication.BLL.Dtos.News
{
    public class ReadNewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleAr { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        public DateTime PublishDate { get; set; }
        public string ImageURL { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
