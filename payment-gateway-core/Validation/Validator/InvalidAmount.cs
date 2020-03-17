using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class InvalidAmount : IPaymentValidator
    {
        private readonly decimal _amount;

        public InvalidAmount(decimal amount)
        {
            _amount = amount;
        }

        public Result Process()
        {
            var result = new Result();

            if (_amount > 0) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.InvalidAmount;
            result.StatusDetail = "The Amount is not valid.";

            return result;
        }
    }
}
