using System.Threading.Tasks;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway.Services.Engine
{
    public interface IValidationService
    {
        Task<object> ProcessValidation<TModel>(IValidatorManager<TModel> validator, TModel model) where TModel : class;
    }
}
