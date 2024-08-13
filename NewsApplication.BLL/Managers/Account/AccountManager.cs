
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NewsApplication.BLL.Dtos.Account;
using NewsApplication.DAL.Models;
using NewsApplication.DAL.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NewsApplication.BLL.Managers.AccountManager
{
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountManager(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _roleManager = roleManager;
        }
        public async Task<TokenDataDto> LoginUser(LoginUserDto loginDTO)
        {
            TokenDataDto tokenData = new TokenDataDto();

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result)
            {
                return new TokenDataDto { Message = "Invalid credentials" };
            }
            string token = await GenerateToken(user);

            tokenData.UserName = user.UserName;
            tokenData.UserId=user.Id;
            tokenData.Token = token;
            tokenData.UserCategory = user.UserCategory;

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                tokenData.RefreshToken = activeRefreshToken.Token;
                tokenData.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                tokenData.RefreshToken = refreshToken.Token;
                tokenData.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return tokenData;
        }
        public async Task<TokenDataDto> RegisterUser(SignUpUserDto entity)
        {
            var user = _mapper.Map<SignUpUserDto, ApplicationUser>(entity);
            var result = await _userManager.CreateAsync(user, entity.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new TokenDataDto { Message = errors };
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role,user.UserCategory.ToString())
            };
            await _userManager.AddClaimsAsync(user, claims);

            string token = await GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            return new TokenDataDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserCategory = user.UserCategory,
                Token = token,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };
        }
        private async Task<string> GenerateToken(ApplicationUser user)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("SecretKey").Value ?? string.Empty));
            SigningCredentials SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claimsList = await _userManager.GetClaimsAsync(user);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claimsList,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: SigningCredentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
        public async Task LogoutUser()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<UpdateUserDto> UpdateUser(UpdateUserDto entity)
        {
            var user = await _userManager.FindByIdAsync(entity.Id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            _mapper.Map(entity, user);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
            }
            return entity;
        }
        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<string> AddRoleAsync(AddRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }

        public async Task<TokenDataDto> RefreshTokenAsync(string token)
        {
            var tokenData = new TokenDataDto();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                tokenData.Message = "Invalid token";
                return tokenData;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                tokenData.Message = "Inactive token";
                return tokenData;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            tokenData.Token = await GenerateToken(user);
            tokenData.UserName = user.UserName;
            tokenData.UserCategory = user.UserCategory;
            tokenData.RefreshToken = newRefreshToken.Token;
            tokenData.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return tokenData;
        }
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }
        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}
