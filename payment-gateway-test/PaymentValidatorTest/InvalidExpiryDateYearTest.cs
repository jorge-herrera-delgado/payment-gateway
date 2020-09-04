using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.PaymentValidatorTest
{
    [TestClass]
    public class InvalidExpiryDateYearTest
    {
        [TestMethod]
        public void InvalidExpiryDateYear_Success()
        {
            var year = DateTime.Now.Year;
            //Arrange
            var validator = new InvalidExpiryDateYear(year);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void InvalidExpiryDateYear_Failed()
        {
            var year = DateTime.Now.Year - 1;
            //Arrange
            var validator = new InvalidExpiryDateYear(year);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }
    }
}
