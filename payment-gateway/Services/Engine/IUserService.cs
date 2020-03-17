using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;

namespace payment_gateway.Services.Engine
{
    public interface IUserService
    {
        IRepository<User> Repository { get; set; }
        User Authenticate(string username, string password, bool isRegistered);
    }
}
