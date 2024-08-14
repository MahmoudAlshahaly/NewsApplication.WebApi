using NewsApplication.BLL.Dtos.Account;

namespace NewsApplication.BLL.Managers.AccountManager
{
    public interface IAccountManager
    {
        Task<TokenDataDto> LoginUser(LoginUserDto loginDTO);
        Task<TokenDataDto> RegisterUser(SignUpUserDto loginDTO);
        Task<string> AddRoleAsync(AddRoleDto model);
        Task<TokenDataDto> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<UpdateUserDto> UpdateUser(UpdateUserDto updateDTO);
        //Task LogoutUser();
        Task DeleteAsync(string id);
    }
}
