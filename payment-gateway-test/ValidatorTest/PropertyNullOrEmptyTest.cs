using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.ValidatorTest
{
    [TestClass]
    public class PropertyNullOrEmptyTest
    {
        [TestMethod]
        public void PropertyNullOrEmpty_Success()
        {
            //Arrange
            var validator = new PropertyNullOrEmpty(new KeyValuePair<string, string>("PropertyKey", "PropertyValue"));
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
        }

        [TestMethod]
        public void PropertyNullOrEmpty_Failed()
        {
            //Arrange
            var validator = new PropertyNullOrEmpty(new KeyValuePair<string, string>("PropertyKey", null));
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.AreEqual(result.ErrorCode, ErrorCode.ValueIsNullOrEmpty);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
        }
    }
}
