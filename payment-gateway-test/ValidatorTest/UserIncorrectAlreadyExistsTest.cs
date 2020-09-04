using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.ValidatorTest
{
    [TestClass]
    public class UserIncorrectAlreadyExistsTest
    {
        [TestMethod]
        public void UserIncorrectAlreadyExists_Success()
        {
            //Arrange
            var validator = new UserIncorrectAlreadyExists(false);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
        }

        [TestMethod]
        public void UserIncorrectAlreadyExists_Failed()
        {
            //Arrange
            var validator = new UserIncorrectAlreadyExists(true);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.AreEqual(result.ErrorCode, ErrorCode.UserAlreadyExists);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
        }
    }
}
