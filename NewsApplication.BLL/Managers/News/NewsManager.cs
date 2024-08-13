
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NewsApplication.BLL.Dtos.News;
using NewsApplication.DAL.Models;
using NewsApplication.DAL.Repositories;
using System.Drawing.Drawing2D;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Shipping.BLL.Managers
{
    public class NewsManager : INewsManager
    {
        private readonly IRepository<News> _repositoryNews;
        private readonly IMapper _mapper;

        public NewsManager(IRepository<News> repositoryNews, IMapper mapper)
        {
            _repositoryNews = repositoryNews;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ReadNewsDto>> GetAllAsync()
        {
            var newsList = await _repositoryNews.GetAllAsync();
            return _mapper.Map<List<News>, List<ReadNewsDto>>(newsList.AsNoTracking().ToList());
        }
        public async Task<ReadNewsDto> GetByIdAsync(int id)
        {
            News news = await _repositoryNews.GetByIdAsync(id);
            if (news == null) throw new KeyNotFoundException("News not found");
            return _mapper.Map<News, ReadNewsDto>(news);
        }
        public async Task<AddNewsDto> AddAsync(AddNewsDto entity)
        {
            News news = _mapper.Map<AddNewsDto, News>(entity);

            if(entity.Image != null)
                 news.ImageURL = $"/Images/{entity.Image.FileName}";

            await _repositoryNews.AddAsync(news);
            return entity;
        }
        public async Task<UpdateNewsDto> UpdateAsync(UpdateNewsDto entity)
        {
            News news = _mapper.Map<UpdateNewsDto, News>(entity);

            if (entity.Image != null)
                news.ImageURL = $"/Images/{entity.Image.FileName}";

            await _repositoryNews.UpdateAsync(news);
            return entity;
        }
        public async Task DeleteAsync(int id)
        {
            News news = await _repositoryNews.GetByIdAsync(id);
            news.IsDeleted = true;

            await _repositoryNews.UpdateAsync(news);
        }
        public async Task SoftDeleteExpiredNewsAsync(DateTime now)
        {
            var newsList = await _repositoryNews.GetAllAsync();
            var filteredNewsList = newsList.Where(news => news.PublishDate < now && !news.IsDeleted).ToList();
            foreach (var news in filteredNewsList)
            {
                news.IsDeleted = true;
                await _repositoryNews.UpdateAsync(news);
            }
        }
    }
}
