using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.PaymentValidatorTest
{
    [TestClass]
    public class StringValueIsNumericTest
    {
        [TestMethod]
        public void StringValueIsNumeric_Success()
        {
            //Arrange
            var validator = new StringValueIsNumeric("1", "PropertyTest");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
        }

        [TestMethod]
        public void StringValueIsNumeric_Failed()
        {
            //Arrange
            var validator = new StringValueIsNumeric("TEST", "PropertyTest");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
        }

        [TestMethod]
        public void StringValueIsNumeric_ArgumentOutOfRangeException()
        {
            //Arrange
            var validator = new StringValueIsNumeric(null, "PropertyTest");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
        }
    }
}
