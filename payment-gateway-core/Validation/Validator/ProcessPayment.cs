using System;
using payment_gateway_core.Helper;
using payment_gateway_core.Payment.Engine;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class ProcessPayment : IPaymentValidator
    {
        private readonly IBankProcessor _bankProcessor;
        private readonly string _card;

        public ProcessPayment(IBankProcessor bankProcessor, string card)
        {
            _bankProcessor = bankProcessor;
            _card = card;
        }

        public Result Process()
        {
            var result = new Result();
            var process = _bankProcessor.Process().Result;
            if (process != null && process.GetType() != typeof(BraintreeHttp.HttpException))
                return result;

            if (process?.GetType() == typeof(BraintreeHttp.HttpException))
            {
                result.ErrorCode = ErrorCode.PaymentFailed;
                result.StatusCode = StatusCode.Failed;
                result.StatusDetail = ((BraintreeHttp.HttpException)process).Message;
            }
            else
            {
                result.ErrorCode = ErrorCode.PaymentFailed;
                result.StatusCode = StatusCode.Failed;
                result.StatusDetail = $"The payment process has failed for the Card Number {_card.MaskStringValue(4)}";
            }

            return result;
        }
    }
}
