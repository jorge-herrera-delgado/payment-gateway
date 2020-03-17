using System;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class InvalidExpiryDateMonth : IPaymentValidator
    {
        private readonly int _month;

        public InvalidExpiryDateMonth(int month)
        {
            _month = month;
        }

        public Result Process()
        {
            var result = new Result();

            if (_month >= 1 && _month <= 12) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.InvalidExpiryDate;
            result.StatusDetail = "The Card Month is not valid.";

            return result;
        }
    }
}
