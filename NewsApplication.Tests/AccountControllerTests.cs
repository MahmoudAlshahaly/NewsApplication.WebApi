using AutoFixture;
using Azure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewsApplication.Api.Controllers;
using NewsApplication.BLL.Dtos.Account;
using NewsApplication.BLL.Dtos.News;
using NewsApplication.BLL.Managers.AccountManager;
using NewsApplication.DAL.Models;
using Shipping.BLL.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewsApplication.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private readonly Mock<IAccountManager> _accountManagerMock;
        private readonly AccountController _accountController;
        private readonly Fixture _fixture;
        public AccountControllerTests()
        {
            _accountManagerMock = new Mock<IAccountManager>();
            _accountController = new AccountController(_accountManagerMock.Object);
            _fixture = new Fixture();
            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                   {
                         new Claim(ClaimTypes.NameIdentifier, "1") //User ID
                   })),
                }
            };
        }
        [TestMethod]
        public async Task RegisterAsync_ShouldReturnOkResult_WithSignUpDto()
        {
            // Arrange
            //use AutoFixure Package to populate Fake Data for testing
            var signUpUser = _fixture.Create<SignUpUserDto>();
            var tokenData = _fixture.Create<TokenDataDto>();
            _accountManagerMock.Setup(n => n.RegisterUser(signUpUser)).ReturnsAsync(tokenData);

            // Act
            var result = await _accountController.RegisterAsync(signUpUser);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as TokenDataDto;

            okResult.Should().NotBeNull();
            returnValue.Should().NotBeNull();
            returnValue.Should().Be(tokenData);
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldReturnBadRequestResult_WhenTokenDataDtoNull()
        {
            // Arrange
            var signUpUser = _fixture.Create<SignUpUserDto>();
            TokenDataDto tokenData = null;
            _accountManagerMock.Setup(n => n.RegisterUser(signUpUser)).ReturnsAsync(tokenData);

            // Act
            var result = await _accountController.RegisterAsync(signUpUser);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.Value.Should().BeNull();
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task LoginAsync_ShouldReturnOkResult_WithLoginUserDto()
        {
            // Arrange
            var loginUser = _fixture.Create<LoginUserDto>();
            var tokenData = _fixture.Create<TokenDataDto>();
            _accountManagerMock.Setup(n => n.LoginUser(loginUser)).ReturnsAsync(tokenData);

            // Act
            var result = await _accountController.LoginAsync(loginUser);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as TokenDataDto;
            okResult.Should().NotBeNull();
            returnValue.Should().NotBeNull();
            returnValue.Should().Be(tokenData);
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task LoginAsync_ShouldReturnOkResult_WithRefreshTokenNull()
        {
            // Arrange
            var loginUser = _fixture.Create<LoginUserDto>();
            var tokenData = _fixture.Create<TokenDataDto>();
            tokenData.RefreshToken = null;
            _accountManagerMock.Setup(n => n.LoginUser(loginUser)).ReturnsAsync(tokenData);

            // Act
            var result = await _accountController.LoginAsync(loginUser);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as TokenDataDto;
            okResult.Should().NotBeNull();
            returnValue.Should().NotBeNull();
            returnValue.Should().Be(tokenData);
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task LoginAsync_ShouldReturnOkResult_WhenTokenDataDtoNull()
        {
            // Arrange
            var loginUser = _fixture.Create<LoginUserDto>();
            TokenDataDto tokenData = null;
            _accountManagerMock.Setup(n => n.LoginUser(loginUser)).ReturnsAsync(tokenData);

            // Act
            var result = await _accountController.LoginAsync(loginUser);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.Value.Should().BeNull();
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task Update_ShouldReturnNotFoundResult_WithNotMatchesKeys()
        {
            // Arrange
            var updateUser = _fixture.Create<UpdateUserDto>();
            _accountManagerMock.Setup(n => n.UpdateUser(updateUser)).ReturnsAsync(updateUser);

            // Act
            var result = await _accountController.Update("4546", updateUser);

            // Assert
            var okResult = result.Result as NotFoundObjectResult;
            okResult.Value.Should().Be("Not Matches Keys");
            okResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
        [TestMethod]
        public async Task Update_ShouldReturnOkResult_WithUpdateUserDto()
        {
            // Arrange
            var updateUser = _fixture.Create<UpdateUserDto>();
            _accountManagerMock.Setup(n => n.UpdateUser(updateUser)).ReturnsAsync(updateUser);

            // Act
            var result = await _accountController.Update(updateUser.Id,updateUser);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as UpdateUserDto;
            okResult.Should().NotBeNull();
            returnValue.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task Update_ShouldReturnNotFoundResult_WhenUpdateUserNull()
        {
            // Arrange
            var updateUser = _fixture.Create<UpdateUserDto>();
            _accountManagerMock.Setup(n => n.UpdateUser(updateUser)).ReturnsAsync((UpdateUserDto)null);

            // Act
            var result = await _accountController.Update(updateUser.Id, updateUser);

            // Assert
            var okResult = result.Result as NotFoundObjectResult;
            okResult.Value.Should().Be("user not found.");
            okResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Delete_ShouldReturnOkResult_WhenUserExist()
        {
            // Arrange
            _accountManagerMock.Setup(n => n.DeleteAsync("1")).Returns(Task.CompletedTask);

            // Act
            var result = await _accountController.Delete("1");

            // Assert
            var okResult = result as OkResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task Delete_ShouldReturnBadRequestResult_WhenThrowException()
        {
            // Arrange
            _accountManagerMock.Setup(n => n.DeleteAsync("1")).Throws(new Exception("user not exist"));

            // Act
            var result = await _accountController.Delete("1");

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().Be("user not exist");
        }
        [TestMethod]
        public async Task AddRoleAsync_ShouldReturnOkResult_WithAddRoleDto()
        {
            // Arrange
            var addrole = _fixture.Create<AddRoleDto>();
            
            _accountManagerMock.Setup(n => n.AddRoleAsync(addrole)).ReturnsAsync(string.Empty);

            // Act
            var result = await _accountController.AddRoleAsync(addrole);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task AddRoleAsync_ShouldReturnBadRequestResult_WithAddRoleDtoModelState()
        {
            // Arrange
            _accountController.ModelState.AddModelError("UserId", "Required");
            _accountController.ModelState.AddModelError("Role", "Required");
            var addrole = _fixture.Create<AddRoleDto>();

            // Act
            var result = await _accountController.AddRoleAsync(addrole);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task RefreshToken_ShouldReturnOkResult_WithValidToken()
        {
            // Arrange
            string refreshtoken = "mahmoudahmed";
            var tokenuser = _fixture.Create<TokenDataDto>();
            _accountManagerMock.Setup(n => n.RefreshTokenAsync(refreshtoken)).ReturnsAsync(tokenuser);
            _accountController.HttpContext.Request.Headers["Cookie"] = "refreshToken=mahmoudahmed";

            // Act
            var result = await _accountController.RefreshToken();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task RefreshToken_ShouldReturnBadRequestResult_WhenTokenNull()
        {
            // Arrange
            string refreshtoken = "mahmoudahmed";
            var tokenuser = _fixture.Create<TokenDataDto>();
            tokenuser.Token = null;
            _accountManagerMock.Setup(n => n.RefreshTokenAsync(refreshtoken)).ReturnsAsync(tokenuser);
            _accountController.HttpContext.Request.Headers["Cookie"] = "refreshToken=mahmoudahmed";

            // Act
            var result = await _accountController.RefreshToken();

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task RevokeToken_ShouldReturnOkResult_WithValidToken()
        {
            // Arrange
            bool response = true;
            var revoketokenuser = _fixture.Create<RevokeTokenDto>();
            _accountManagerMock.Setup(n => n.RevokeTokenAsync(revoketokenuser.Token)).ReturnsAsync(response);

            // Act
            var result = await _accountController.RevokeToken(revoketokenuser);

            // Assert
            var okResult = result as OkResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        [TestMethod]
        public async Task RevokeToken_ShouldReturnBadRequestResult_WhenResponseFalse()
        {
            // Arrange
            bool response = false;
            var revoketokenuser = _fixture.Create<RevokeTokenDto>();
            _accountManagerMock.Setup(n => n.RevokeTokenAsync(revoketokenuser.Token)).ReturnsAsync(response);

            // Act
            var result = await _accountController.RevokeToken(revoketokenuser);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Token is invalid!");
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task RevokeToken_ShouldReturnBadRequestResult_WhenTokenNull()
        {
            // Arrange
            string token = "mahmoudahmed";
            var revoketokenuser = _fixture.Create<RevokeTokenDto>();
            revoketokenuser.Token = null;
            bool response = true;
            _accountManagerMock.Setup(n => n.RevokeTokenAsync(token)).ReturnsAsync(response);
           // _accountController.HttpContext.Request.Headers["Cookie"] = "refreshToken=mahmoudahmed";

            // Act
            var result = await _accountController.RevokeToken(revoketokenuser);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Token is required!");
            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
