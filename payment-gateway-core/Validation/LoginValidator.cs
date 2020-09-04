using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Model;

namespace payment_gateway_core.Validation
{
    public class LoginValidator : IValidatorManager<User>
    {
        private readonly ILogger<LoginValidator> _log;
        public object ReturnObject { get; private set; }

        public LoginValidator(ILogger<LoginValidator> log)
        {
            _log = log;
        }

        //if it needs to add more security and/or validation, we can add them based on requirements.
        public Task<IEnumerable<Func<Result>>> GetValidatorsResult(User model)
        {
            _log.LogInformation($"[Adding] Login Validator results for: UserId: {model?.UserId}");
            var listFunc = new List<Func<Result>>
            {
                () => new ObjectNull(model, "Username or password is incorrect", ErrorCode.NoAuthorized).Process()
            };
            ReturnObject = model;
            _log.LogInformation($"[Finished] Login Validator results for: UserId: {model?.UserId}");
            return Task.FromResult<IEnumerable<Func<Result>>>(listFunc);
        }
    }
}
