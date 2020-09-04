using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway_core.Helper;
using payment_gateway_core.Model;
using payment_gateway_repository.Model;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway_test.HelperTest
{
    [TestClass]
    public class PaymentsFilterHelperTest
    {
        private IQueryable<Payment> _payments;
        private Guid _userId;

        [TestInitialize]
        public void Init()
        {
            _userId = Guid.NewGuid();
            var payment = new Payment
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
                UserId = _userId
            };
            var list = new List<Payment> {payment};

            _payments = new EnumerableQuery<Payment>(list);
        }

        [TestMethod]
        public void PaymentsFilterHelper_GetPayments_Payments_ArgumentOutOfRangeException()
        {
            //Mock
            var mockRepo = new Mock<IPaymentRepository>();
            mockRepo.Setup(r => r.GetMongoQueryable()).Returns(It.IsAny<IQueryable<Payment>>());
            //Arrange
            var queryable = mockRepo.Object.GetMongoQueryable();
            //Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => queryable.GetPaymentsQueryable(new PaymentsFilter {UserId = _userId.ToString()}),
                "There are not payments in the storage.");
        }

        [TestMethod]
        public void PaymentsFilterHelper_GetPayments_Filter_ArgumentOutOfRangeException()
        {
            //Mock
            var mockRepo = new Mock<IPaymentRepository>();
            mockRepo.Setup(r => r.GetMongoQueryable()).Returns(_payments);
            //Arrange
            var queryable = mockRepo.Object.GetMongoQueryable();
            //Act and Assert
            Assert.ThrowsException<NullReferenceException>(() => queryable.GetPaymentsQueryable(null), "Filter cannot be null. Invalid parameter.");
        }

        [TestMethod]
        public void PaymentsFilterHelper_GetPayments_UserId_Successful()
        {
            //Mock
            var mockRepo = new Mock<IPaymentRepository>();
            mockRepo.Setup(r => r.GetMongoQueryable()).Returns(_payments);
            //Arrange and Act
            var result = mockRepo.Object.GetMongoQueryable().GetPaymentsQueryable(new PaymentsFilter {UserId = _userId.ToString()});
            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.ToList().Any());
            Assert.AreEqual(result.FirstOrDefault()?.PaymentId, _payments.FirstOrDefault()?.PaymentId);
        }

        [TestMethod]
        public void PaymentsFilterHelper_GetPayments_StartEndDate_Successful()
        {
            //Mock
            var mockRepo = new Mock<IPaymentRepository>();
            mockRepo.Setup(r => r.GetMongoQueryable()).Returns(_payments);
            //Arrange and Act
            var filter = new PaymentsFilter
            {
                UserId = _userId.ToString(),
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            };

            var result = mockRepo.Object.GetMongoQueryable().GetPaymentsQueryable(filter);
            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.ToList().Any());
            Assert.AreEqual(result.FirstOrDefault()?.PaymentId, _payments.FirstOrDefault()?.PaymentId);
        }

        [TestMethod]
        public void PaymentsFilterHelper_GetPayments_CardNumber_Successful()
        {
            //Mock
            var mockRepo = new Mock<IPaymentRepository>();
            mockRepo.Setup(r => r.GetMongoQueryable()).Returns(_payments);
            //Arrange and Act
            var filter = new PaymentsFilter
            {
                UserId = _userId.ToString(),
                CardNumber = "1234567894567896"
            };

            var result = mockRepo.Object.GetMongoQueryable().GetPaymentsQueryable(filter);
            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.ToList().Any());
            Assert.AreEqual(result.FirstOrDefault()?.PaymentId, _payments.FirstOrDefault()?.PaymentId);
        }

        [TestMethod]
        public void PaymentsFilterHelper_GetPayments_ProductId_Successful()
        {
            //Mock
            var mockRepo = new Mock<IPaymentRepository>();
            mockRepo.Setup(r => r.GetMongoQueryable()).Returns(_payments);
            //Arrange and Act
            var filter = new PaymentsFilter
            {
                UserId = _userId.ToString(),
                ProductId = "123ABC_TEST1"
            };

            var result = mockRepo.Object.GetMongoQueryable().GetPaymentsQueryable(filter);
            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.ToList().Any());
            Assert.AreEqual(result.FirstOrDefault()?.PaymentId, _payments.FirstOrDefault()?.PaymentId);
        }

        [TestMethod]
        public void PaymentsFilterHelper_GetPayments_ProductName_Successful()
        {
            //Mock
            var mockRepo = new Mock<IPaymentRepository>();
            mockRepo.Setup(r => r.GetMongoQueryable()).Returns(_payments);
            //Arrange and Act
            var filter = new PaymentsFilter
            {
                UserId = _userId.ToString(),
                ProductName = "Product Test"
            };

            var result = mockRepo.Object.GetMongoQueryable().GetPaymentsQueryable(filter);
            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.ToList().Any());
            Assert.AreEqual(result.FirstOrDefault()?.PaymentId, _payments.FirstOrDefault()?.PaymentId);
        }
    }
}
