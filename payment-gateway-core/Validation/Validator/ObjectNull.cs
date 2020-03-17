using System;
using System.Collections.Generic;
using System.Text;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class ObjectNull : IValidator
    {
        private readonly object _obj;
        private readonly string _message;
        private readonly ErrorCode _errorCode;

        public ObjectNull(object obj, string message, ErrorCode errorCode)
        {
            _obj = obj;
            _message = message;
            _errorCode = errorCode;
        }

        public Result Process()
        {
            var result = new Result();

            if (_obj != null) return result;

            result.ErrorCode = _errorCode;
            result.StatusCode = StatusCode.Failed;
            result.StatusDetail = _message;

            return result;
        }
    }
}
