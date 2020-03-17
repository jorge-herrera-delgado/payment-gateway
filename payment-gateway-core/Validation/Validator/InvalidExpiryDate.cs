using System;
using System.Collections.Generic;
using System.Text;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class InvalidExpiryDate : IPaymentValidator
    {
        private readonly int _month;
        private readonly int _year;

        public InvalidExpiryDate(int month, int year)
        {
            _month = month;
            _year = year;
        }

        public Result Process()
        {
            var result = new Result();
            var resultDate = new DateTime(_year, _month, 1);

            if (resultDate > DateTime.Now.Date) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.InvalidExpiryDate;
            result.StatusDetail = "The Card Expiry Date is not valid.";

            return result;
        }
    }
}
