using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;

namespace payment_gateway.Services.Engine
{
    public interface IUserService
    {
        string Authenticate(User user, bool isRegistered);
        string GenerateToken(string userId);
    }
}
