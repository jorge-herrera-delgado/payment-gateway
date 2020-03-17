using System;
using System.Collections.Generic;
using System.Text;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class StringValueIsNumeric : IPaymentValidator
    {
        private readonly string _value;
        private readonly string _property;

        public StringValueIsNumeric(string value, string property)
        {
            _value = value;
            _property = property;
        }

        public Result Process()
        {
            var result = new Result();
            if (int.TryParse(_value, out _)) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.InvalidValue;
            result.StatusDetail = $"The value in {_property} has to be numeric.";

            return result;
        }
    }
}
