using System;
using System.Collections.Generic;
using System.Text;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class StringLengthNotValid : IPaymentValidator
    {
        private readonly string _value;
        private readonly int _length;
        private readonly string _property;

        public StringLengthNotValid(string value, int length, string property)
        {
            _value = value;
            _length = length;
            _property = property;
        }

        public Result Process()
        {
            var result = new Result();

            if (_value.Length == _length) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.InvalidValue;
            result.StatusDetail = $"The value in {_property} is not valid. It is {_length - _value.Length} digit missed.";

            return result;
        }
    }
}
