using System;
using System.Collections.Generic;
using System.Text;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class UserIncorrectAlreadyExists : IValidator
    {
        private readonly bool _existsUsername;

        public UserIncorrectAlreadyExists(bool existsUsername)
        {
            _existsUsername = existsUsername;
        }

        public Result Process()
        {
            var result = new Result();

            if (!_existsUsername) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.UserAlreadyExists;
            result.StatusDetail = "User is already registered in our system.";

            return result;
        }
    }
}
