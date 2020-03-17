using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using payment_gateway_repository.Model;
using PayPal.v1.Payments;

namespace payment_gateway_core.Payment
{
    public class PaymentMapper
    {
        private readonly UserPayment _payment;

        public PaymentMapper(UserPayment payment)
        {
            _payment = payment;
        }

        public PayPal.v1.Payments.Payment GetPaymentDetails()
        {
            var fundingInstrumentList = new List<FundingInstrument>
            {
                new FundingInstrument
                {
                    CreditCard = GetCreditCard(_payment.CardDetails)
                }
            };

            return new PayPal.v1.Payments.Payment
            {
                Intent = "sale",
                Transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        Amount = new Amount
                        {
                            Total = _payment.Payment.Amount.ToString(CultureInfo.InvariantCulture),
                            Currency = _payment.Payment.Currency
                        }
                    }
                },
                Payer = new Payer
                {
                    PaymentMethod = "CREDIT_CARD",
                    FundingInstruments = fundingInstrumentList
                }
            };
        }

        private static CreditCard GetCreditCard(CardDetails cardDetails)
        {
            return new CreditCard
            {
                Cvv2 = cardDetails.Cvv,
                ExpireMonth = cardDetails.ExpiryDateMonth,
                ExpireYear = cardDetails.ExpiryDateYear,
                FirstName = cardDetails.CardFirstname,
                LastName = cardDetails.CardLastname,
                Number = cardDetails.CardNumber,
                Type = cardDetails.Type
            };
        }
    }
}
