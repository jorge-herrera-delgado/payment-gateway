using System;
using payment_gateway_core.Helper;
using payment_gateway_core.Payment.Engine;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway_core.Validation.Validator
{
    public class ProcessPayment : IPaymentValidator
    {
        private readonly IBankProcessor _bankProcessor;
        private readonly payment_gateway_repository.Model.Payment _payment;

        public ProcessPayment(IBankProcessor bankProcessor, payment_gateway_repository.Model.Payment payment)
        {
            _bankProcessor = bankProcessor;
            _payment = payment;
        }

        public Result Process()
        {
            var result = new Result();
            var process = _bankProcessor.Process(_payment).Result;
            if (process != null && process.GetType() != typeof(BraintreeHttp.HttpException) && process.GetType() != typeof(Exception))
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
                result.StatusDetail = $"The payment process has failed for the Card Number {_payment.CardDetails.CardNumber.MaskStringValue(4)}";
            }

            return result;
        }
    }
}
