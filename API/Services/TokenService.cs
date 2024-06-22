using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interface;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey= configuration["TokenKey"]??throw new Exception("Token key is null in appsettings");
         if(tokenKey.Length<64) throw new Exception("Key size is less than 64");

        var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));       
        var credentials= new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var claims= new List<Claim>{
            new Claim(ClaimTypes.NameIdentifier, user.UserName)            
        };
        var tokendescriptor= new SecurityTokenDescriptor{
            Subject= new ClaimsIdentity(claims),
            Expires= DateTime.Now.AddDays(7),
            SigningCredentials= credentials
        };
        var tokenHandler= new JwtSecurityTokenHandler();
        var token= tokenHandler.CreateToken(tokendescriptor);
        return tokenHandler.WriteToken(token);
    }
}
