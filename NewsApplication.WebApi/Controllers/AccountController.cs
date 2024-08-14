using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewsApplication.BLL.Dtos.Account;
using NewsApplication.BLL.Dtos.News;
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
        public async Task<ActionResult<TokenDataDto>> RegisterAsync([FromForm] SignUpUserDto model)
        {
            var result = await _accountManager.RegisterUser(model);

            if (result is null)
                return BadRequest(result);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        [HttpPost("LoginAsync")]
        public async Task<ActionResult<TokenDataDto>> LoginAsync([FromForm] LoginUserDto model)
        {
            var result = await _accountManager.LoginUser(model);

            if (result is null)
                return BadRequest(result);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateUserDto>> Update(string id, [FromForm] UpdateUserDto model)
        {
            try
            {
                if (id != model.Id)
                {
                    return NotFound("Not Matches Keys");
                }

                var updateUser = await _accountManager.UpdateUser(model);
                if (updateUser == null)
                {
                    return NotFound("user not found.");
                }

                return Ok(updateUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _accountManager.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddRoleAsync")]
        public async Task<IActionResult> AddRoleAsync([FromForm] AddRoleDto model)
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

            if(result.Token == null)
                return BadRequest(result);
            
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken([FromForm] RevokeTokenDto model)
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
