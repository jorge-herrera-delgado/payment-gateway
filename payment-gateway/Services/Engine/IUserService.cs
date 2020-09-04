using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;

namespace payment_gateway.Services.Engine
{
    public interface IUserService
    {
        User Authenticate(string username, string password, bool isRegistered);
    }
}
