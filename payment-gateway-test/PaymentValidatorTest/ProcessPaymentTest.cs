using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway_core.Payment.Engine;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Model;

namespace payment_gateway_test.PaymentValidatorTest
{
    [TestClass]
    public class ProcessPaymentTest
    {
        private Payment _payment;

        [TestInitialize]
        public void Init()
        {
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
        public void ProcessPayment_Success()
        {
            //Mock
            var mock = new Mock<IBankProcessor>();
            mock.Setup(x => x.Process(_payment)).Returns(Task.FromResult(new object()));
            //Arrange
            var validator = new ProcessPayment(mock.Object, _payment);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void ProcessPayment_HttpException_Failed()
        {
            //Mock
            var mock = new Mock<IBankProcessor>();
            mock.Setup(x => x.Process(_payment))
                .Returns(Task.FromResult(
                    new BraintreeHttp.HttpException(HttpStatusCode.BadRequest, null, "Test Failed") as object));
            //Arrange
            var validator = new ProcessPayment(mock.Object, _payment);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.AreEqual(result.ErrorCode, ErrorCode.PaymentFailed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void ProcessPayment_Exception_Failed()
        {
            //Mock
            var mock = new Mock<IBankProcessor>();
            mock.Setup(x => x.Process(_payment))
                .Returns(Task.FromResult(
                    new Exception("Test Failed") as object));
            //Arrange
            var validator = new ProcessPayment(mock.Object, _payment);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.AreEqual(result.ErrorCode, ErrorCode.PaymentFailed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }
    }
}
