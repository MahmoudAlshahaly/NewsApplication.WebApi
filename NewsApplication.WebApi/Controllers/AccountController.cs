using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewsApplication.BLL.Dtos.Account;
using NewsApplication.BLL.Managers.AccountManager;

namespace NewsApplication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountManager _accountManager;
        public AccountController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }
        [HttpPost("RegisterAsync")]
        public async Task<ActionResult<TokenDataDto>> RegisterAsync([FromBody] SignUpUserDto model)
        {
            var result = await _accountManager.RegisterUser(model);

            if (result is null)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("LoginAsync")]
        public async Task<ActionResult<TokenDataDto>> LoginAsync([FromBody] LoginUserDto model)
        {
            var result = await _accountManager.LoginUser(model);

            if (result is null)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("AddRoleAsync")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountManager.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _accountManager.RefreshTokenAsync(refreshToken);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await _accountManager.RevokeTokenAsync(token);

            if (!result)
                return BadRequest("Token is invalid!");

            return Ok();
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
