using NewsApplication.BLL.Dtos.News;

namespace Shipping.BLL.Managers
{
    public interface INewsManager
    {
        Task<IEnumerable<ReadNewsDto>> GetAllAsync();
        Task SoftDeleteExpiredNewsAsync(DateTime now);
        Task<ReadNewsDto> GetByIdAsync(int id);
        Task<AddNewsDto> AddAsync(AddNewsDto cityDto);
        Task<UpdateNewsDto> UpdateAsync(UpdateNewsDto cityDto);
        Task DeleteAsync(int id);
    }
}
