using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.PaymentValidatorTest
{
    [TestClass]
    public class InvalidExpiryDateTest
    {
        [TestMethod]
        public void InvalidExpiryDate_Success()
        {
            var month = DateTime.Now.Month + 1;
            var year = DateTime.Now.Year + 1;
            //Arrange
            var validator = new InvalidExpiryDate(month, year);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void InvalidExpiryDate_Failed()
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            //Arrange
            var validator = new InvalidExpiryDate(month, year);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void InvalidExpiryDate_ArgumentOutOfRangeException()
        {
            //Arrange
            var validator = new InvalidExpiryDate(0, 0);
            //Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => validator.Process(), "Invalid month and year to validate.");
        }
    }
}
