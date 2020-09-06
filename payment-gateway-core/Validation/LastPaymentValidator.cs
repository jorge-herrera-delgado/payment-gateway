using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway_core.Validation
{
    public class LastPaymentValidator : IValidatorManager<LastPaymentValidator, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LastPaymentValidator> _log;
        public object ReturnObject { get; private set; }

        public LastPaymentValidator(IUserRepository userRepository, ILogger<LastPaymentValidator> log)
        {
            _userRepository = userRepository;
            _log = log;
        }

        public async Task<IEnumerable<Func<Result>>> GetValidatorsResult(string model)
        {
            _log.LogInformation($"[Adding] Last Payment Validator results for: UserId: {model}");
            var user = await _userRepository.GetItemAsync(u => u.UserId == new Guid(model));

            var listFunc = new List<Func<Result>>
            {
                () => new ObjectNull(user, $"This user ({model}) is not authorized to get these details.", ErrorCode.NoAuthorized).Process()
            };

            ReturnObject = model;

            _log.LogInformation($"[Finished] Last Payment Validator results for: UserId: {model}");

            return listFunc;
        }
    }
}
