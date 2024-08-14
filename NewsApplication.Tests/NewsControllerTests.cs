using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewsApplication.Api.Controllers;
using NewsApplication.BLL.Dtos.News;
using Shipping.BLL.Managers;
using System.Net;
using System.Security.Claims;

namespace NewsApplication.Tests
{
    [TestClass]
    public class NewsControllerTests
    {
        private readonly Mock<INewsManager> _newsManagerMock;
        private readonly NewsController _newsController;
        private readonly Fixture _fixture;
        private Mock<IFormFile> _mockFormFile;
        public NewsControllerTests()
        {
            _newsManagerMock = new Mock<INewsManager>();
            _newsController = new NewsController(_newsManagerMock.Object);
            _fixture = new Fixture();
            _mockFormFile = new Mock<IFormFile>();

            var fakeToken = "Bearer your-jwt-token"; 
            _newsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                         new Claim(ClaimTypes.NameIdentifier, "1") //User ID
                    })),
                    Request =
                    {
                        //Headers =
                        //{
                        //    { "Authorization", fakeToken }
                        //}
                    }
                }
            };
        }
        [TestMethod]
        public async Task GetAll_ShouldReturnOkResult_WithNewsList()
        {
            // Arrange
            //var newsList = new List<ReadNewsDto>
            //{
            //     new ReadNewsDto { Title = "Test News 1" },
            //     new ReadNewsDto { Title = "Test News 2" }
            //};

            //use AutoFixure Package to populate Fake Data for testing
             var newsList = _fixture.Create<List<ReadNewsDto>>();
            _newsManagerMock.Setup(n => n.GetAllAsync()).ReturnsAsync(newsList);

            // Act
            var result = await _newsController.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as List<ReadNewsDto>;

            okResult.Should().NotBeNull();
            returnValue.Should().NotBeNull();
            returnValue.Should().HaveCount(newsList.Count);
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task GetByID_ShouldReturnOkResult_WithNewsItem()
        {
            // Arrange
            var newsItem = new ReadNewsDto { Title = "Test News" };
            _newsManagerMock.Setup(n => n.GetByIdAsync(1)).ReturnsAsync(newsItem);

            // Act
            var result = await _newsController.GetByID(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as ReadNewsDto;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            returnValue.Title.Should().Be(newsItem.Title);
        }
        [TestMethod]
        public async Task GetByID_ShouldReturnNotFoundResult_WithNull()
        {
            // Arrange
            ReadNewsDto newsItem = null;
            _newsManagerMock.Setup(n => n.GetByIdAsync(1)).ReturnsAsync(newsItem);

            // Act
            var result = await _newsController.GetByID(1);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            notFoundResult.Value.Should().Be("News Item Not Found");
        }

        [TestMethod]
        public async Task Insert_ShouldReturnOkResult_WithAddedNews()
        {
            // Arrange
            var addNewsDto = new AddNewsDto
            {
                Title = "New News",
                TitleAr = "أخبار جديدة",
                Description = "Description",
                DescriptionAr = "وصف",
                PublishDate = DateTime.UtcNow,
                Image = _mockFormFile.Object, 
                //ImageURL = "imageurl",
                ApplicationUserId = "1"
            };
            _mockFormFile.Setup(f => f.FileName).Returns("test_image.jpg");
            _newsManagerMock.Setup(n => n.AddAsync(It.IsAny<AddNewsDto>())).ReturnsAsync(addNewsDto);

            // Act
            var result = await _newsController.Insert(addNewsDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var returnValue = okResult.Value as AddNewsDto;
            returnValue.Should().BeEquivalentTo(addNewsDto);

            // Ensure the file saving logic was called
            _newsManagerMock.Verify(n => n.AddAsync(It.IsAny<AddNewsDto>()), Times.Once);
        }
        [TestMethod]
        public async Task Insert_ShouldReturnOkRequest_WhenImageIsMissing()
        {
            // Arrange
            var addNewsDto = new AddNewsDto
            {
                Title = "New News",
                TitleAr = "أخبار جديدة",
                Description = "Description",
                DescriptionAr = "وصف",
                PublishDate = DateTime.UtcNow,
                Image = null,
                //ImageURL = "imageurl",
                ApplicationUserId = "1"
            };
            _newsManagerMock.Setup(n => n.AddAsync(It.IsAny<AddNewsDto>())).ReturnsAsync(addNewsDto);

            // Act
            var result = await _newsController.Insert(addNewsDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task Insert_ShouldReturnBadRequest_WhenRequiredFieldNotPopulate()
        {
            // Arrange
            _newsController.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await _newsController.Insert(new AddNewsDto());

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task Insert_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var addNewsDto = new AddNewsDto
            {
                Title = "New News",
                TitleAr = "أخبار جديدة",
                Description = "Description",
                DescriptionAr = "وصف",
                PublishDate = DateTime.UtcNow,
                Image = null,
                //ImageURL = "imageurl",
                ApplicationUserId = "1"
            };
            
            _newsManagerMock.Setup(f => f.AddAsync(It.IsAny<AddNewsDto>())).Throws(new Exception("Exception"));

            // Act
            var result = await _newsController.Insert(addNewsDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().Be("Exception");
        }
        [TestMethod]
        public async Task Update_ShouldReturnNotFoundResult_WithDifferentKeys()
        {
            // Arrange
            var updateNewsDto = new UpdateNewsDto
            {
                Id = 2,
                Title = "Updated News"
            };

            // Act
            var result = await _newsController.Update(1, updateNewsDto);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            notFoundResult.Value.Should().Be("Not Matches Keys");
        }
        [TestMethod]
        public async Task Update_ShouldReturnNotFoundResult_WithKeyNotExist()
        {
            // Arrange
            var updateNewsDto = new UpdateNewsDto
            {
                Id = 1,
                Title = "Updated News"
            };
            _newsManagerMock.Setup(n => n.UpdateAsync(updateNewsDto)).ReturnsAsync((UpdateNewsDto)null);

            // Act
            var result = await _newsController.Update(1, updateNewsDto);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            notFoundResult.Value.Should().Be("News item not found.");
        }
        [TestMethod]
        public async Task Update_ShouldReturnOkResult_WithUpdatedNews()
        {
            // Arrange
            var updateNewsDto = new UpdateNewsDto
            {
                Id = 1,
                Title = "Updated News"
            };
            _newsManagerMock.Setup(n => n.UpdateAsync(updateNewsDto)).ReturnsAsync(updateNewsDto);

            // Act
            var result = await _newsController.Update(1, updateNewsDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var returnValue = okResult.Value as UpdateNewsDto;
            returnValue.Should().BeEquivalentTo(updateNewsDto);
            returnValue.Title.Should().Be(updateNewsDto.Title);
        }
        [TestMethod]
        public async Task Update_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var updateNewsDto = new UpdateNewsDto
            {
                Id = 1,
                Title = "Updated News"
            };

            _newsManagerMock.Setup(f => f.UpdateAsync(It.IsAny<UpdateNewsDto>())).Throws(new Exception("Exception"));

            // Act
            var result = await _newsController.Update(1,updateNewsDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().Be("Exception");
        }

        [TestMethod]
        public async Task Delete_ShouldReturnOkResult_WhenItemExist()
        {
            // Arrange
            _newsManagerMock.Setup(n => n.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _newsController.Delete(1);

            // Assert
            var okResult = result as OkResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task Delete_ShouldReturnBadRequestResult_WhenThrowException()
        {
            // Arrange
            _newsManagerMock.Setup(n => n.DeleteAsync(1)).Throws(new Exception("Item not exist"));

            // Act
            var result = await _newsController.Delete(1);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().Be("Item not exist");
        }
    }
}