using NewsApplication.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewsApplication.BLL.Dtos.Account
{
    public class TokenDataDto
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; } = "Success";
        public List<string>? Roles { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
