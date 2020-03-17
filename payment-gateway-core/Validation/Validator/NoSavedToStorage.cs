using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class NoSavedToStorage : IValidator
    {
        private readonly bool _saved;
        private readonly string _source;

        public NoSavedToStorage(bool saved, string source)
        {
            _saved = saved;
            _source = source;
        }

        public Result Process()
        {
            var result = new Result();

            if (_saved) return result;

            result.ErrorCode = ErrorCode.DataBaseError;
            result.StatusCode = StatusCode.Failed;
            result.StatusDetail = $"We could not save this {_source} due issues in the system, please try again in few minutes. If the issue persists, please contact us.";

            return result;
        }
    }
}
