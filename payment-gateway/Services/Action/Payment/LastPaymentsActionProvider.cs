using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway.Services.Action.Engine;
using payment_gateway.Services.Engine;
using payment_gateway_core.Helper;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway.Services.Action.Payment
{
    public class LastPaymentsActionProvider : IAction
    {
        private readonly IValidationService _validationService;
        private readonly IValidatorManager<LastPaymentValidator, string> _validator;
        private readonly IPaymentRepository _repository;
        private readonly ILogger<LastPaymentsActionProvider> _log;

        public LastPaymentsActionProvider(
            IValidationService validationService,
            IValidatorManager<LastPaymentValidator, string> validator,
            IPaymentRepository repository,
            ILogger<LastPaymentsActionProvider> log)
        {
            _validationService = validationService;
            _validator = validator;
            _repository = repository;
            _log = log;
        }

        public async Task<object> ProcessAction(object value)
        {
            var model = value as string ?? throw new ArgumentNullException(value.GetType().ToString(), "The value is not implemented.");
            _log.LogInformation($"[Started] Get Last Payment for: UserId: {model}");

            var validation = await _validationService.ProcessValidation(_validator, model);
            var result = validation as Result;
            if (result?.StatusCode == StatusCode.Failed)
            {
                _log.LogError(result.ErrorCode.ConvertToInt(), GetMessage(result, model));
                return result;
            }

            var resultModel = _repository.GetMongoQueryable().Where(u => u.UserId == new Guid(model)).OrderByDescending(x => x.Created).FirstOrDefault();

            _log.LogInformation($"[Finished] Get Last Payment for: UserId: {model}");

            return resultModel;
        }

        private static string GetMessage(Result result, string userId)
            => $"Payment Error Details: {result.StatusDetail} - User: {userId}";
    }
}
