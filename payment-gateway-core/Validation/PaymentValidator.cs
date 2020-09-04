using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway_core.Helper;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway_core.Validation
{
    public class PaymentValidator : IValidatorManager<payment_gateway_repository.Model.Payment>
    {
        private readonly IUserRepository _repositoryUser;
        private readonly ILogger<PaymentValidator> _log;

        public object ReturnObject { get; private set; }

        public PaymentValidator(IUserRepository repositoryUser, ILogger<PaymentValidator> log)
        {
            _repositoryUser = repositoryUser;
            _log = log;
        }

        //all the validators can be added in IOC container
        public async Task<IEnumerable<Func<Result>>> GetValidatorsResult(payment_gateway_repository.Model.Payment userPayment)
        {
            _log.LogInformation($"[Adding] Payment Validator results for: UserId: {userPayment.UserId}");
            //All properties cannot be null or empty
            var properties = userPayment.FilteredProperties().ToList();
            properties.AddRange(userPayment.CardDetails.FilteredProperties());
            properties.AddRange(userPayment.Price.FilteredProperties());

            var listFunc = properties.Select(property => (Func<Result>)(() => new PropertyNullOrEmpty(property).Process())).ToList();

            var user = await _repositoryUser.GetItemAsync(u => u.UserId == userPayment.UserId);

            //There might be more validators we can probably add in here if needed
            var list = new List<IPaymentValidator>
            {
                new InvalidUser(user), //user not valid
                new StringValueIsNumeric(userPayment.CardDetails.Cvv, nameof(userPayment.CardDetails.Cvv)), //CVV has to be numeric
                new StringLengthNotValid(userPayment.CardDetails.CardNumber, 16, nameof(userPayment.CardDetails.CardNumber)), //card number is 16 digits
                new StringLengthNotValid(userPayment.CardDetails.Cvv, 3, nameof(userPayment.CardDetails.Cvv)), //CVV is 3 digits
                new InvalidCardNumber(userPayment.CardDetails.CardNumber), //card number is valid
                new InvalidExpiryDateMonth(userPayment.CardDetails.ExpiryDateMonth), //expiry month valid
                new InvalidExpiryDateYear(userPayment.CardDetails.ExpiryDateYear), //expiry year valid
                new InvalidExpiryDate(userPayment.CardDetails.ExpiryDateMonth, userPayment.CardDetails.ExpiryDateYear), //expiry date is valid
                new InvalidAmount(userPayment.Price.Amount), //amount cannot be zero or negative
                new InvalidCurrency(userPayment.Price.Currency),//currency should be valid
            };

            listFunc.AddRange(list.Select(validator => (Func<Result>)validator.Process));

            ReturnObject = userPayment;

            _log.LogInformation($"[Finished] Payment Validator results for: UserId: {userPayment.UserId}");
            return listFunc;
        }
    }
}
