using System;
using System.Threading.Tasks;
using payment_gateway_core.Payment.Engine;
using payment_gateway_repository.Model;
using PayPal.Core;
using PayPal.v1.Payments;

namespace payment_gateway_core.Payment
{
    public class PaypalManager : IBankProcessor
    {
        private readonly UserPayment _payment;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public PaypalManager(UserPayment payment, string clientId, string clientSecret)
        {
            _payment = payment;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<object> Process()
        {
            var env = new SandboxEnvironment(_clientId, _clientSecret);
            var client = new PayPalHttpClient(env);
            var paymentDetails = new PaymentMapper(_payment).GetPaymentDetails();
            var request = new PaymentCreateRequest();
            request.RequestBody(paymentDetails);

            try
            {
                var response = await client.Execute(request);
                //var statusCode = response.StatusCode;
                return response.Result<PayPal.v1.Payments.Payment>();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
