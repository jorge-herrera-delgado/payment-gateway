using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway.Mapper.Engine;
using payment_gateway.Services.Action.Engine;
using payment_gateway.Services.Engine;
using payment_gateway_core.Helper;
using payment_gateway_core.Payment.Engine;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway.Services.Action.Payment
{
    public class ProcessPaymentAction : IAction
    {
        private readonly IPaymentRepository _repository;
        private readonly IValidationService _validationService;
        private readonly IBankProcessor _bankProcessor;
        private readonly IValidatorManager<payment_gateway_repository.Model.Payment> _validator;
        private readonly ILogger<ProcessPaymentAction> _log;

        public ProcessPaymentAction(IPaymentRepository repository,
            IValidationService validationService,
            IBankProcessor bankProcessor,
            IValidatorManager<payment_gateway_repository.Model.Payment> validator,
            ILogger<ProcessPaymentAction> log)
        {
            _repository = repository;
            _validationService = validationService;
            _bankProcessor = bankProcessor;
            _validator = validator;
            _log = log;
        }

        public async Task<object> ProcessAction(object value)
        {
            var payment = value as Model.Payment ?? throw new ArgumentNullException(value.GetType().ToString(), "The value is not implemented.");
            _log.LogInformation($"[Started] Payment process for Payment: {payment.PaymentId} - User: {payment.UserId}");
            var mapper = MapperModelFactory<Model.Payment, payment_gateway_repository.Model.Payment>.GetMapper();
            var model = mapper.MapToDestination(payment);

            var validator = await _validationService.ProcessValidation(_validator, model);
            var result = validator as Result;
            if (result?.StatusCode == StatusCode.Failed)
            {
                _log.LogError(result.ErrorCode.ConvertToInt(), GetMessage(result, model));
                return result;
            }

            //Process the bank payment
            result = new ProcessPayment(_bankProcessor, model).Process();
            if (result?.StatusCode == StatusCode.Failed)
            {
                _log.LogError(result.ErrorCode.ConvertToInt(), GetMessage(result, model));
                return result;
            }

            //Save the transaction in DB
            var saved = await _repository.AddItemAsync(model);
            result = new NoSavedToStorage(saved, "Payment").Process();
            if (result?.StatusCode == StatusCode.Failed)
            {
                _log.LogCritical(result.ErrorCode.ConvertToInt(), GetMessage(result, model));
                return result;
            }

            _log.LogInformation($"[Finished] Payment process for Payment: {payment.PaymentId} - User: {payment.UserId}");

            return model;
        }

        private static string GetMessage(Result result, payment_gateway_repository.Model.Payment payment) 
            => $"Payment Error Details: {result.StatusDetail} - Payment: {payment.PaymentId} - User: {payment.UserId}";
    }
}
