using System;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class InvalidExpiryDateYear : IPaymentValidator
    {
        private readonly int _year;

        public InvalidExpiryDateYear(int year)
        {
            _year = year;
        }

        public Result Process()
        {
            var result = new Result();

            if (_year >= DateTime.Now.Year) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.InvalidExpiryDate;
            result.StatusDetail = "The Card Year is not valid.";

            return result;
        }
    }
}