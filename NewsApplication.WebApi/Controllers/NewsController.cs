using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApplication.BLL.Dtos.News;
using Shipping.BLL.Managers;
using System.Security.Claims;

namespace NewsApplication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[Authorize(Roles = "Normal")]
    //[Authorize(Roles = "Admin")]
    //[Authorize(Roles = "ContentAdmin")]
    public class NewsController : ControllerBase
    {
        private readonly INewsManager _newsManager;
        public NewsController(INewsManager newsManager)
        {
            _newsManager = newsManager;
        }
        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReadNewsDto>>> GetAll()
        {
            return Ok(await _newsManager.GetAllAsync());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadNewsDto>> GetByID(int id)
        {
            var news =await _newsManager.GetByIdAsync(id);
            if (news is null)
            {
                return NotFound("News Item Not Found");
            }
            return Ok(news);
        }
        [HttpPost]
        public async Task<ActionResult<AddNewsDto>> Insert([FromForm]AddNewsDto addNewsDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Return validation errors
                    return BadRequest(ModelState);
                }

                await CheckImageAndUpload(addNewsDto.Image);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    addNewsDto.ApplicationUserId = userId;
                }

                return Ok(await _newsManager.AddAsync(addNewsDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }

        private async Task CheckImageAndUpload(IFormFile Image)
        {
            if (Image != null)
            {
                // Define the path where the file will be saved
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                // Ensure the uploads directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                // Create the full path for the file
                var filePath = Path.Combine(uploadsFolder, Image.FileName);
                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateNewsDto>> Update(int id, [FromForm] UpdateNewsDto updateNewsDto)
        {
            try
            {
                if (id != updateNewsDto.Id)
                {
                    return NotFound("Not Matches Keys");
                }

                await CheckImageAndUpload(updateNewsDto.Image);

                var updatedNews = await _newsManager.UpdateAsync(updateNewsDto);
                if (updatedNews == null)
                {
                    return NotFound("News item not found.");
                }

                return Ok(updatedNews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _newsManager.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
