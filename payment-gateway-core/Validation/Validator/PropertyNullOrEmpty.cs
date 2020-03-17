using System.Collections.Generic;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class PropertyNullOrEmpty : IValidator
    {
        private readonly KeyValuePair<string, string> _property;

        public PropertyNullOrEmpty(KeyValuePair<string, string> property)
        {
            _property = property;
        }
        public Result Process()
        {
            var result = new Result();

            var (key, value) = _property;
            if (!string.IsNullOrEmpty(value)) return result;

            result.ErrorCode = ErrorCode.ValueIsNullOrEmpty;
            result.StatusCode = StatusCode.Failed;
            result.StatusDetail = $"The value in {key} is required.";

            return result;
        }
    }
}
