using System.Threading.Tasks;
using payment_gateway.Services.Engine;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway.Services
{
    public class ValidationService : IValidationService
    {
        public Task<object> ProcessValidation<TClass, TModel>(IValidatorManager<TClass, TModel> validator, TModel model) 
            where TClass : class 
            where TModel : class
        {
            foreach (var itemFunc in validator.GetValidatorsResult(model).Result)
            {
                var result = itemFunc.Invoke();
                if (result.StatusCode != StatusCode.Success)
                    return Task.FromResult<object>(result);
            }

            return Task.FromResult(validator.ReturnObject);
        }
    }
}
