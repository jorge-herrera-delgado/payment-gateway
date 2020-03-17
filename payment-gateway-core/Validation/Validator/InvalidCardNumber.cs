using payment_gateway_core.Helper;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class InvalidCardNumber : IPaymentValidator
    {
        private readonly string _cardNumber;

        public InvalidCardNumber(string cardNumber)
        {
            _cardNumber = cardNumber;
        }

        public Result Process()
        {
            var result = new Result();

            if (Utilities.LuhnCheck(_cardNumber)) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.InvalidCardNumber;
            result.StatusDetail = "The Card Number is not valid.";

            return result;
        }
    }
}
