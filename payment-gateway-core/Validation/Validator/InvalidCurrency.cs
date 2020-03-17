using System.Collections.Generic;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class InvalidCurrency : IPaymentValidator
    {
        //It can be stored in DB or get it from other sources like an external API and
        //being injected from outside for validation or any other process.
        //I put it here for simplicity.
        private static readonly List<string> Currencies = new List<string>{"EUR", "USD"};

        private readonly string _currency;

        public InvalidCurrency(string currency)
        {
            _currency = currency;
        }

        public Result Process()
        {
            var result = new Result();

            if (Currencies.Contains(_currency)) return result;

            result.StatusCode = StatusCode.Failed;
            result.ErrorCode = ErrorCode.InvalidCurrency;
            result.StatusDetail = "The Amount is not valid.";

            return result;
        }
    }
}