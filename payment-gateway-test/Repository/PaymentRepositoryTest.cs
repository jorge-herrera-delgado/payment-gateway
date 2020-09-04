using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway_repository.Engine.Contract;
using payment_gateway_repository.Engine.Model;
using payment_gateway_repository.Model;
using payment_gateway_repository.Repository;

namespace payment_gateway_test.Repository
{
    [TestClass]
    public class PaymentRepositoryTest
    {
        private Payment _payment;
        private Guid _userId;
        private List<Payment> _payments;

        private Mock<INonSqlDataSource> _mockDataSource;

        [TestInitialize]
        public void Init()
        {
            _userId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
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
                UserId = _userId
            };

            _payments = new List<Payment> {_payment};

            //Mock
            _mockDataSource = new Mock<INonSqlDataSource>();
            _mockDataSource.Setup(d => d.GetItem(It.IsAny<NonSqlSchema>(), It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns((NonSqlSchema s, Expression<Func<Payment, bool>> p) => p != null ? _payment : null);
            _mockDataSource.Setup(d => d.GetItemAsync(It.IsAny<NonSqlSchema>(), It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns((NonSqlSchema s, Expression<Func<Payment, bool>> p) => Task.FromResult<Payment>(p != null ? _payment : null));
            _mockDataSource.Setup(d => d.InsertAsync(It.IsAny<NonSqlSchema>(), It.IsAny<Payment>()))
                .Returns((NonSqlSchema s, Payment p) =>
                {
                    if(p == null)
                        throw new NullReferenceException("Test Null Exception for Payment");

                    return Task.FromResult(p.PaymentId != _payment.PaymentId);
                });
        }

        [TestMethod]
        public void PaymentRepository_GetItem_Successful()
        {
            //Arrange
            var repo = new PaymentRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetItem(x => x.PaymentId == _payment.PaymentId);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.PaymentId == _payment.PaymentId);
        }

        [TestMethod]
        public void PaymentRepository_GetItem_Failed()
        {
            //Arrange
            var repo = new PaymentRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetItem(null);
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void PaymentRepository_GetItemAsync_Successful()
        {
            //Arrange
            var repo = new PaymentRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetItemAsync(x => x.PaymentId == _payment.PaymentId).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.PaymentId == _payment.PaymentId);
        }

        [TestMethod]
        public void PaymentRepository_GetItemAsync_Failed()
        {
            //Arrange
            var repo = new PaymentRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetItemAsync(null).Result;
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void PaymentRepository_GetMongoQueryable_Successful()
        {
            //Mock
            _mockDataSource.Setup(d => d.GetMongoQueryable<Payment>(It.IsAny<NonSqlSchema>())).Returns(_payments.AsQueryable());
            //Arrange
            var repo = new PaymentRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetMongoQueryable();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.Any(x => x.PaymentId == _payment.PaymentId));
        }

        [TestMethod]
        public void PaymentRepository_GetMongoQueryable_Failed()
        {
            //Mock
            _mockDataSource.Setup(d => d.GetMongoQueryable<Payment>(It.IsAny<NonSqlSchema>())).Returns<IQueryable<Payment>>(null);
            //Arrange
            var repo = new PaymentRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetMongoQueryable();
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void PaymentRepository_AddItemAsync_Payment_No_Exists_Successful()
        {
            //Arrange
            var repo = new PaymentRepository(_mockDataSource.Object);
            //Act
            //Simulates a Non Existing payment in Storage
            var temp = new Payment();
            var result = repo.AddItemAsync(temp).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PaymentRepository_AddItemAsync_Payment_Already_Exists_Failed()
        {
            //Arrange
            var repo = new PaymentRepository(_mockDataSource.Object);
            //Act
            var result = repo.AddItemAsync(_payment).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PaymentRepository_NullReferenceException_Failed()
        {
            //Arrange
            var repo = new PaymentRepository(_mockDataSource.Object);
            //Act and Assert
            Assert.ThrowsExceptionAsync<NullReferenceException>(() => repo.AddItemAsync(null), "Null Reference Exception. Parameter cannot be null.");
        }
    }
}
