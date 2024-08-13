using AutoMapper;
using NewsApplication.BLL.Dtos.Account;
using NewsApplication.BLL.Dtos.News;
using NewsApplication.DAL.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsApplication.BLL.AutoMapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<ApplicationUser, SignUpUserDto>().ReverseMap();
            CreateMap<ApplicationUser, UpdateUserDto>().ReverseMap();
            CreateMap<ApplicationUser, SignUpUserDto>().ReverseMap();

            CreateMap<News, AddNewsDto>().ReverseMap();
            CreateMap<News, ReadNewsDto>().ReverseMap();
            CreateMap<News, UpdateNewsDto>().ReverseMap();
        }
    }
}
