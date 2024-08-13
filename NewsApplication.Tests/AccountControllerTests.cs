using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewsApplication.Api.Controllers;
using NewsApplication.BLL.Managers.AccountManager;
using Shipping.BLL.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
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
                         new Claim(ClaimTypes.NameIdentifier, "1") // Example User ID
                    }))
                }
            };
        }
    }
}
