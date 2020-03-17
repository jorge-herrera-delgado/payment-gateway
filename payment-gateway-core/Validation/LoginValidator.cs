using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using payment_gateway_core.Resolver.Engine;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Model;

namespace payment_gateway_core.Validation
{
    public class LoginValidator : IValidatorManager
    {
        private readonly User _user;
        public object ReturnObject { get; private set; }

        public LoginValidator(User user)
        {
            _user = user;
        }

        //if it needs to add more security and/or validation, we can add them based on requirements.
        //Another way we can achieve this functionality could be by IOC container.
        //All the assemblies under IValidator can be registered and then executed to get results.
        public Task<IEnumerable<Func<Result>>> GetValidatorsResult()
        {
            var listFunc = new List<Func<Result>>
            {
                () => new ObjectNull(_user, "Username or password is incorrect", ErrorCode.NoAuthorized).Process()
            };
            ReturnObject = _user;
            return Task.FromResult<IEnumerable<Func<Result>>>(listFunc);
        }
    }
}
