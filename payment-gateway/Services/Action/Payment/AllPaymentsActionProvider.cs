using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway.Mapper.Engine;
using payment_gateway.Services.Action.Engine;
using payment_gateway.Services.Engine;
using payment_gateway_core.Helper;
using payment_gateway.Model;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Repository.Contract;
using CoreModel = payment_gateway_core.Model;

namespace payment_gateway.Services.Action.Payment
{
    public class AllPaymentsActionProvider : IAction
    {
        private readonly IValidationService _validationService;
        private readonly IValidatorManager<AllPaymentsValidator, CoreModel.PaymentsFilter> _validator;
        private readonly IPaymentRepository _repository;
        private readonly ILogger<AllPaymentsActionProvider> _log;

        public AllPaymentsActionProvider(
            IValidationService validationService,
            IValidatorManager<AllPaymentsValidator, CoreModel.PaymentsFilter> validator,
            IPaymentRepository repository,
            ILogger<AllPaymentsActionProvider> log)
        {
            _validationService = validationService;
            _validator = validator;
            _repository = repository;
            _log = log;
        }

        public async Task<object> ProcessAction(object value)
        {
            var model = value as PaymentsFilter ?? throw new ArgumentNullException(value.GetType().ToString(), "The value is not implemented.");
            _log.LogInformation($"[Started] Get Payments for: UserId: {model.UserId}");

            var mapper = MapperModelFactory<PaymentsFilter, CoreModel.PaymentsFilter>.GetMapper();
            var modelMap = mapper.MapToDestination(model);

            var validation = await _validationService.ProcessValidation(_validator, modelMap);
            var result = validation as Result;
            if (result?.StatusCode == StatusCode.Failed)
            {
                _log.LogError(result.ErrorCode.ConvertToInt(), GetMessage(result, modelMap));
                return result;
            }

            var resultModel = _repository.GetMongoQueryable().GetPaymentsQueryable(modelMap).ToList();

            _log.LogInformation($"[Finished] Get Payments for: UserId: {modelMap.UserId}");

            return resultModel;
        }

        private static string GetMessage(Result result, CoreModel.PaymentsFilter filter)
            => $"Payment Error Details: {result.StatusDetail} - User: {filter.UserId}";
    }
}