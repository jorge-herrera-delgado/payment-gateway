using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Model;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway_test.ValidatorManagerTest
{
    [TestClass]
    public class PaymentValidatorTest
    {
        private Payment _payment;
        private User _user;
        private ILogger<PaymentValidator> _log;
        private const string ValidCardNumber = "5232100934299477";

        [TestInitialize]
        public void Init()
        {
            _user = new User
            {
                FirstName = "Firstname",
                LastName = "Lastname",
                UserId = Guid.NewGuid(),
                UserLogin = new UserLogin
                {
                    Username = "TestUser",
                    Password = "123456"
                }
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

            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<PaymentValidator>();
        }

        [TestMethod]
        public void PaymentValidator_Success()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(_user));
            //Arrange
            var manager = new PaymentValidator(mock.Object, _log);
            _payment.CardDetails.CardNumber = ValidCardNumber;
            //Act
            var list = manager.GetValidatorsResult(_payment).Result.ToList();
            //Assert
            Assert.IsTrue(manager.ReturnObject != null);
            Assert.IsTrue(list.Any());
            foreach (var item in list)
            {
                Assert.IsTrue(item.Invoke().StatusCode == StatusCode.Success);
            }
        }

        [TestMethod]
        public void PaymentValidator_InvalidCardNumber_Success()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(_user));
            //Arrange
            var manager = new PaymentValidator(mock.Object, _log);
            //Act
            var list = manager.GetValidatorsResult(_payment).Result.ToList();
            //Assert
            Assert.IsTrue(manager.ReturnObject != null);
            Assert.IsTrue(list.Any());
            foreach (var result in list.Select(item => item.Invoke()).Where(x => x.StatusCode == StatusCode.Failed))
            {
                Assert.IsTrue(result.StatusCode == StatusCode.Failed);
                Assert.AreEqual(result.ErrorCode, ErrorCode.InvalidCardNumber);
            }
        }

        [TestMethod]
        public void PaymentValidator_AggregateException()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(null)).Returns(Task.FromResult<User>(null));
            //Arrange
            var manager = new PaymentValidator(mock.Object, _log);
            //Act and Assert
            Assert.ThrowsException<AggregateException>(() => manager.GetValidatorsResult(new Payment()).Result, "CardDetails property is null. Invalid value.");
        }

        [TestMethod]
        public void PaymentValidator_InvalidUser_Failed()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult<User>(null));
            //Arrange
            var manager = new PaymentValidator(mock.Object, _log);
            _payment.CardDetails.CardNumber = ValidCardNumber;
            //Act
            var list = manager.GetValidatorsResult(_payment).Result.ToList();
            //Assert
            Assert.IsTrue(manager.ReturnObject != null);
            Assert.IsTrue(list.Any());
            foreach (var result in list.Select(item => item.Invoke()).Where(x => x.StatusCode == StatusCode.Failed))
            {
                Assert.IsTrue(result.StatusCode == StatusCode.Failed);
                Assert.AreEqual(result.ErrorCode, ErrorCode.IncorrectPassword);
            }
        }
    }
}
