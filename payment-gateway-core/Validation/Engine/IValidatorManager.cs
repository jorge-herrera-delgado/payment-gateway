using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace payment_gateway_core.Validation.Engine
{
    public interface IValidatorManager<in TModel>
    {
        object ReturnObject { get; }
        Task<IEnumerable<Func<Result>>> GetValidatorsResult(TModel model);
    }
}
