using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway_core.Model;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Model;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway_test.ValidatorManagerTest
{
    [TestClass]
    public class AllPaymentsValidatorTest
    {
        private PaymentsFilter _filter;
        private ILogger<AllPaymentsValidator> _log;

        [TestInitialize]
        public void Init()
        {
            _filter = new PaymentsFilter
            {
                UserId = "97190759-1638-42fa-abb7-1ac97111978d"
            };

            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<AllPaymentsValidator>();
        }

        [TestMethod]
        public void AllPaymentsValidator_HasItems_All_Success()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(new User()));
            //Arrange
            var manager = new AllPaymentsValidator(mock.Object, _log);
            //Act
            var list = manager.GetValidatorsResult(_filter).Result.ToList();
            //Assert
            Assert.IsTrue(manager.ReturnObject != null);
            Assert.IsTrue(list.Any());
            foreach (var result in list.Select(item => item.Invoke()))
            {
                Assert.IsTrue(result.StatusCode == StatusCode.Success);
            }
        }

        [TestMethod]
        public void AllPaymentsValidator_HasItems_Failed()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(null)).Returns(Task.FromResult<User>(null));
            //Arrange
            var manager = new AllPaymentsValidator(mock.Object, _log);
            //Act
            var list = manager.GetValidatorsResult(new PaymentsFilter()).Result.ToList();
            var results = list.Select(item => item.Invoke()).ToList();
            //Assert
            Assert.IsTrue(manager.ReturnObject != null);
            Assert.IsTrue(results.Any());
            Assert.IsTrue(results.Any(x => x.StatusCode == StatusCode.Failed));

            foreach (var result in results)
            {
                Assert.IsTrue(result.StatusCode == StatusCode.Failed);
                Assert.AreEqual(result.ErrorCode, ErrorCode.NoAuthorized);
            }
        }
        
        [TestMethod]
        public void AllPaymentsValidator_PaymentsFilter_NullValue_Failed()
        {
            //Mock
            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetItemAsync(null)).Returns(Task.FromResult<User>(null));
            //Arrange
            var manager = new AllPaymentsValidator(mock.Object, _log);
            //Act
            var list = manager.GetValidatorsResult(null).Result.ToList();
            var results = list.Select(item => item.Invoke()).ToList();
            //Assert
            Assert.IsTrue(manager.ReturnObject == null);
            Assert.IsTrue(results.Any());
            Assert.IsTrue(results.Any(x => x.StatusCode == StatusCode.Failed));

            foreach (var result in results)
            {
                Assert.IsTrue(result.StatusCode == StatusCode.Failed);
                Assert.AreEqual(result.ErrorCode, ErrorCode.NoAuthorized);
            }
        }
    }
}