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
    public class RegistrationValidatorTest
    {
        private User _user;
        private ILogger<RegistrationValidator> _log;

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

            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<RegistrationValidator>();
        }

        [TestMethod]
        public void RegistrationValidator_HasItems_Success()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(null)).Returns(Task.FromResult<User>(null));
            //Arrange
            var manager = new RegistrationValidator(mock.Object, _log);
            //Act
            var list = manager.GetValidatorsResult(_user).Result.ToList();
            //Assert
            Assert.IsTrue(manager.ReturnObject != null);
            Assert.IsTrue(list.Any());
            Assert.AreEqual(list.FirstOrDefault()?.Invoke().StatusCode, StatusCode.Success);
        }

        [TestMethod]
        public void RegistrationValidator_AggregateException()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(null)).Returns(Task.FromResult<User>(null));
            //Arrange
            var manager = new RegistrationValidator(mock.Object, _log);
            //Act and Assert
            Assert.ThrowsException<AggregateException>(() => manager.GetValidatorsResult(new User()).Result, "UserLogin property is null. Invalid value.");
        }

        [TestMethod]
        public void RegistrationValidator_HasItems_Failed()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(_user));
            //Arrange
            var manager = new RegistrationValidator(mock.Object, _log);
            //Act
            var list = manager.GetValidatorsResult(_user).Result.ToList();
            //Assert
            Assert.IsTrue(manager.ReturnObject != null);
            Assert.IsTrue(list.Any());
        }
    }
}
