using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway_core.Model;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway_core.Validation
{
    public class AllPaymentsValidator : IValidatorManager<AllPaymentsValidator, PaymentsFilter>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AllPaymentsValidator> _log;
        public object ReturnObject { get; private set; }

        public AllPaymentsValidator(IUserRepository userRepository, ILogger<AllPaymentsValidator> log)
        {
            _userRepository = userRepository;
            _log = log;
        }

        public async Task<IEnumerable<Func<Result>>> GetValidatorsResult(PaymentsFilter model)
        {
            _log.LogInformation($"[Adding] All Payments Validator results for: UserId: {model?.UserId}");
            var user = await _userRepository.GetItemAsync(u => u.UserId == new Guid(model.UserId));

            var listFunc = new List<Func<Result>>
            {
                () => new ObjectNull(user, $"This user ({model?.UserId}) is not authorized to get these details.", ErrorCode.NoAuthorized).Process()
            };

            ReturnObject = model;

            _log.LogInformation($"[Finished] All Payments Validator results for: UserId: {model?.UserId}");

            return listFunc;
        }
    }
}