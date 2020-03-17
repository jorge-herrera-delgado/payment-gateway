using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Resolver.Engine
{
    public interface IValidatorManager
    {
        object ReturnObject { get; }
        Task<IEnumerable<Func<Result>>> GetValidatorsResult();
    }
}
