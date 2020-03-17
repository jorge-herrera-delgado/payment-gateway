using System.Threading.Tasks;
using payment_gateway_core.Resolver.Engine;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Resolver.ResolverManager
{
    public class ValidatorManager
    {
        private readonly IValidatorManager _validator;

        public ValidatorManager(IValidatorManager validator)
        {
            _validator = validator;
        }

        public Task<object> Run()
        {
            foreach (var itemFunc in _validator.GetValidatorsResult().Result)
            {
                var result = itemFunc.Invoke();
                if (result.StatusCode != StatusCode.Success)
                    return Task.FromResult<object>(result);
            }

            return Task.FromResult(_validator.ReturnObject);
        }
    }
}
