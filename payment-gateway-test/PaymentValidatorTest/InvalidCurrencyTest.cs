using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.PaymentValidatorTest
{
    [TestClass]
    public class InvalidCurrencyTest
    {
        [TestMethod]
        public void InvalidCurrency_Success()
        {
            //Arrange
            var validator = new InvalidCurrency("EUR");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void InvalidCurrency_Failed()
        {
            //Arrange
            var validator = new InvalidCurrency("TEST");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void InvalidCurrency_NullValue_Failed()
        {
            //Arrange
            var validator = new InvalidCurrency(null);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }
    }
}
