using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using payment_gateway.Model;
using payment_gateway.Services.Engine;
using Repo = payment_gateway_repository.Model;

namespace payment_gateway.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        //It is a simple authentication security, we can be more strict regarding this.
        //It can be used for external and internal users
        public string Authenticate(Repo.User user, bool isRegistered)
        {
            if (!isRegistered) 
                return GenerateToken(user.UserId.ToString());

            //Validate Token, if it is not valid => creates a new one
            return ValidateCurrentToken(user.UserLogin.Token) 
                ? user.UserLogin.Token 
                : GenerateToken(user.UserId.ToString());
        }

        public string GenerateToken(string userId)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userId)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool ValidateCurrentToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret)),
                }, out _);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
