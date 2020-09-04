using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Model;

namespace payment_gateway_test.ValidatorManagerTest
{
    [TestClass]
    public class LoginValidatorTest
    {
        private ILogger<LoginValidator> _log;

        [TestInitialize]
        public void Init()
        {
            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<LoginValidator>();
        }

        [TestMethod]
        public void LoginValidator_HasItems_All_Success()
        {
            //Arrange
            var manager = new LoginValidator(_log);
            //Act
            var list = manager.GetValidatorsResult(new User()).Result.ToList();
            //Assert
            Assert.IsTrue(manager.ReturnObject != null);
            Assert.IsTrue(list.Any());
            foreach (var result in list.Select(item => item.Invoke()))
            {
                Assert.IsTrue(result.StatusCode == StatusCode.Success);
            }
        }

        [TestMethod]
        public void LoginValidator_HasItems_Failed()
        {
            //Arrange
            var manager = new LoginValidator(_log);
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
