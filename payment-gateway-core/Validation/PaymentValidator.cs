using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_gateway_core.Helper;
using payment_gateway_core.Payment.Engine;
using payment_gateway_core.Resolver.Engine;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;

namespace payment_gateway_core.Validation
{
    public class PaymentValidator : IValidatorManager
    {
        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<UserPayment> _repository;
        private readonly IBankProcessor _bankProcessor;
        private readonly UserPayment _userPayment;
        public object ReturnObject { get; private set; }

        public PaymentValidator(IRepository<User> repositoryUser, IRepository<UserPayment> repository, IBankProcessor bankProcessor, UserPayment userPayment)
        {
            _repositoryUser = repositoryUser;
            _repository = repository;
            _bankProcessor = bankProcessor;
            _userPayment = userPayment;
        }

        //all the validators can be added in IOC container
        public async Task<IEnumerable<Func<Result>>> GetValidatorsResult()
        {
            //All properties cannot be null or empty
            var properties = _userPayment.FilteredProperties().ToList();
            properties.AddRange(_userPayment.CardDetails.FilteredProperties());
            properties.AddRange(_userPayment.Payment.FilteredProperties());

            var listFunc = properties.Select(property => (Func<Result>)(() => new PropertyNullOrEmpty(property).Process())).ToList();

            var user = await _repositoryUser.GetItemAsync(u => u.UserId == _userPayment.UserId);

            //There might be more validators we can probably add in here if needed
            var list = new List<IPaymentValidator>
            {
                new InvalidUser(user), //user not valid
                new StringValueIsNumeric(_userPayment.CardDetails.Cvv, nameof(_userPayment.CardDetails.Cvv)), //CVV has to be numeric
                new StringLengthNotValid(_userPayment.CardDetails.CardNumber, 16, nameof(_userPayment.CardDetails.CardNumber)), //card number is 16 digits
                new StringLengthNotValid(_userPayment.CardDetails.Cvv, 3, nameof(_userPayment.CardDetails.Cvv)), //CVV is 3 digits
                new InvalidCardNumber(_userPayment.CardDetails.CardNumber), //card number is valid
                new InvalidExpiryDateMonth(_userPayment.CardDetails.ExpiryDateMonth), //expiry month valid
                new InvalidExpiryDateYear(_userPayment.CardDetails.ExpiryDateYear), //expiry year valid
                new InvalidExpiryDate(_userPayment.CardDetails.ExpiryDateMonth, _userPayment.CardDetails.ExpiryDateYear), //expiry date is valid
                new InvalidAmount(_userPayment.Payment.Amount), //amount cannot be zero or negative
                new InvalidCurrency(_userPayment.Payment.Currency),//currency should be valid
                new ProcessPayment(_bankProcessor, _userPayment.CardDetails.CardNumber) //process the bank payment
            };

            listFunc.AddRange(list.Select(validator => (Func<Result>)validator.Process));

            listFunc.Add(() => new NoSavedToStorage(_repository.AddItemAsync(_userPayment).Result, "User Payment").Process());
            ReturnObject = _userPayment;
            return listFunc;
        }
    }
}
