using System.Collections.Generic;
using System.Globalization;
using payment_gateway_repository.Model;
using PayPal.v1.Payments;

namespace payment_gateway_core.Payment
{
    public class PaymentMapper
    {
        private readonly payment_gateway_repository.Model.Payment _payment;

        public PaymentMapper(payment_gateway_repository.Model.Payment payment)
        {
            _payment = payment;
        }

        public PayPal.v1.Payments.Payment GetPaymentDetails()
        {
            var itmlist = new ItemList
            {
                Items = new List<Item>
                {
                    new Item
                    {
                        Name = _payment.ProductName,
                        Description = _payment.Details,
                        Currency = _payment.Price.Currency,
                        Price = _payment.Price.Amount.ToString(CultureInfo.InvariantCulture),
                        Quantity = "1",
                        Sku = _payment.ProductId
                    }
                }
            };

            var fundingInstrumentList = new List<FundingInstrument>
            {
                new FundingInstrument
                {
                    CreditCard = GetCreditCard(_payment.CardDetails)
                }
            };

            var transaction = new Transaction
            {
                Amount = new Amount
                {
                    Total = _payment.Price.Amount.ToString(CultureInfo.InvariantCulture),
                    Currency = _payment.Price.Currency,
                    Details = new AmountDetails
                        {Subtotal = _payment.Price.Amount.ToString(CultureInfo.InvariantCulture)}
                },
                Description = "Description on transaction",
                ItemList = itmlist
            };

            return new PayPal.v1.Payments.Payment
            {
                Intent = "sale",
                Transactions = new List<Transaction>
                {
                    transaction
                },
                Payer = new Payer
                {
                    //PayerInfo = payerInfo,
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
