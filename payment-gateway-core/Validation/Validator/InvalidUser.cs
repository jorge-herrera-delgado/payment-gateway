using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Model;

namespace payment_gateway_core.Validation.Validator
{
    public class InvalidUser : IPaymentValidator
    {
        private readonly User _user;

        public InvalidUser(User user)
        {
            _user = user;
        }

        public Result Process()
        {
            var result = new Result();

            if (_user != null) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.IncorrectPassword;
            result.StatusDetail = "User is not valid. Please, login again to process the payment.";

            return result;
        }
    }
}
