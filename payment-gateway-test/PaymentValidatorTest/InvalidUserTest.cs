using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Model;

namespace payment_gateway_test.PaymentValidatorTest
{
    [TestClass]
    public class InvalidUserTest
    {
        [TestMethod]
        public void InvalidUser_Success()
        {
            //Arrange
            var validator = new InvalidUser(new User());
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void InvalidUser_Failed()
        {
            //Arrange
            var validator = new InvalidUser(null);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }
    }
}
