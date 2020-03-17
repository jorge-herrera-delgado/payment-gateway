using payment_gateway_repository.Model;

namespace payment_gateway.Helper
{
    public static class ExtensionMethods
    {
        public static User WithoutPassword(this User user)
        {
            user.UserLogin.Password = null;
            return user;
        }
    }
}
