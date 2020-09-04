using System;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway_core.Model;
using payment_gateway_core.Payment;
using payment_gateway_repository.Model;

namespace payment_gateway_test.PaymentTest
{
    [TestClass]
    public class PaypalManagerTest
    {
        private Payment _payment;
        private PaypalSettings _settings;
        private const string ValidCardNumber = "5232100934299477";

        [TestInitialize]
        public void Init()
        {
            //ToDo: Please add here your own Sandbox credentials
            _settings = new PaypalSettings
            {
                ClientId = "Your ClientId",
                ClientSecret = "Your Client Secret"
            };

            _payment = new Payment
            {
                CardDetails = new CardDetails
                {
                    CardFirstname = "Firstname",
                    CardLastname = "Lastname",
                    CardNumber = "1234567894567896",
                    Cvv = "123",
                    ExpiryDateMonth = 10,
                    ExpiryDateYear = 2025,
                    Type = "mastercard"
                },
                Created = DateTime.Now,
                Details = "Payment Test",
                PaymentId = Guid.NewGuid(),
                Price = new Price
                {
                    Amount = 100,
                    Currency = "EUR"
                },
                ProductId = "123ABC_TEST1",
                ProductName = "Product Test",
                UserId = Guid.NewGuid()
            };
        }

        [TestMethod]
        public void PaypalManager_PaypalSettings_NullReferenceException()
        {
            //Mock
            var mock = new Mock<IOptions<PaypalSettings>>();
            mock.Setup(s => s.Value).Returns<PaypalSettings>(null);
            //Arrange
            var manager = new PaypalManager(mock.Object);
            //Act and Assert
            Assert.ThrowsException<AggregateException>(() => manager.Process(_payment).Result);
        }

        [TestMethod]
        public void PaypalManager_PaypalSettings_NotValid_ReturnException()
        {
            //Mock
            var mock = new Mock<IOptions<PaypalSettings>>();
            mock.Setup(s => s.Value).Returns(new PaypalSettings());
            //Arrange
            var manager = new PaypalManager(mock.Object);
            //Act
            var result = manager.Process(_payment).Result;
            //Assert
            Assert.IsTrue(result.GetType() == typeof(AggregateException));
        }

        [TestMethod]
        public void PaypalManager_Success()
        {
            //Mock
            var mock = new Mock<IOptions<PaypalSettings>>();
            mock.Setup(s => s.Value).Returns(_settings);
            //Arrange
            var manager = new PaypalManager(mock.Object);
            _payment.CardDetails.CardNumber = ValidCardNumber;
            //Act
            var result = (PayPal.v1.Payments.Payment)manager.Process(_payment).Result;
            //Assert
            Assert.IsTrue(result != null && result.State == "approved");
        }
    }
}
