using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.PaymentValidatorTest
{
    [TestClass]
    public class InvalidExpiryDateMonthTest
    {
        [TestMethod]
        public void InvalidExpiryDateMonth_Success()
        {
            var month = DateTime.Now.Month;
            //Arrange
            var validator = new InvalidExpiryDateMonth(month);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void InvalidExpiryDateMonth_Failed()
        {
            var month = DateTime.Now.Month + 12;
            //Arrange
            var validator = new InvalidExpiryDateMonth(month);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }
    }
}
