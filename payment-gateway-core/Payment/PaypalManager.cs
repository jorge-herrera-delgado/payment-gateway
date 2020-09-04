using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using payment_gateway_core.Model;
using payment_gateway_core.Payment.Engine;
using PaymentModel = payment_gateway_repository.Model;
using PayPal.Core;
using PayPal.v1.Payments;

namespace payment_gateway_core.Payment
{
    public class PaypalManager : IBankProcessor
    {
        private readonly PaypalSettings _paypalSettings;

        public PaypalManager(IOptions<PaypalSettings> paypalSettings)
        {
            _paypalSettings = paypalSettings.Value;
        }

        public async Task<object> Process(PaymentModel.Payment payment)
        {
            if (_paypalSettings == null) 
                throw new NullReferenceException("Paypal settings cannot be null.");

            var env = new SandboxEnvironment(_paypalSettings.ClientId, _paypalSettings.ClientSecret);
            var client = new PayPalHttpClient(env);
            var paymentDetails = new PaymentMapper(payment).GetPaymentDetails();
            var request = new PaymentCreateRequest();
            request.RequestBody(paymentDetails);

            try
            {
                var response = await client.Execute(request);
                var result = response.Result<PayPal.v1.Payments.Payment>();
                var json = JsonConvert.SerializeObject(result);
                return result;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
