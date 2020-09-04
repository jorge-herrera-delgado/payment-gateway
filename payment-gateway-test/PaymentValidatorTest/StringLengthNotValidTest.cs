using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.PaymentValidatorTest
{
    [TestClass]
    public class StringLengthNotValidTest
    {
        [TestMethod]
        public void StringLengthNotValid_Success()
        {
            //Arrange
            var validator = new StringLengthNotValid("TEST", 4, "PropertyTest");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
        }

        [TestMethod]
        public void StringLengthNotValid_Failed()
        {
            //Arrange
            var validator = new StringLengthNotValid("TEST", 5, "PropertyTest");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
        }

        [TestMethod]
        public void StringLengthNotValid_NullValue_Failed()
        {
            //Arrange
            var validator = new StringLengthNotValid(null, 0, "PropertyTest");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
        }
    }
}
