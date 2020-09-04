using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using payment_gateway.Helper;
using payment_gateway.Model;
using payment_gateway.Services.Engine;
using payment_gateway_repository.Repository.Contract;
using Repo = payment_gateway_repository.Model;

namespace payment_gateway.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly AppSettings _appSettings;
        
        public UserService(IUserRepository repository, IOptions<AppSettings> appSettings)
        {
            _repository = repository;
            _appSettings = appSettings.Value;
        }

        //It is a simple authentication security, we can be more strict regarding this.
        //It can be used for external and internal users
        public Repo.User Authenticate(string username, string password, bool isRegistered)
        {
            var user = new Repo.User();
            if (isRegistered)
            {
                user = _repository.GetItem(x => x.UserLogin.Username == username && x.UserLogin.Password == password);

                return user;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.UserLogin = new Repo.UserLogin {Token = tokenHandler.WriteToken(token)};

            return user.WithoutPassword();
        }
    }
}
