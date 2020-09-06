using System.Threading.Tasks;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway.Services.Engine
{
    public interface IValidationService
    {
        Task<object> ProcessValidation<TClass, TModel>(IValidatorManager<TClass, TModel> validator, TModel model)
            where TClass : class
            where TModel : class;
    }
}
