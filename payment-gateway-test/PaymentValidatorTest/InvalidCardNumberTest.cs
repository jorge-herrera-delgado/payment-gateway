using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.PaymentValidatorTest
{
    [TestClass]
    public class InvalidCardNumberTest
    {
        private const string ValidCardNumber = "5232100934299477";
        private const string InvalidCardNumber = "2232300934259471";

        [TestMethod]
        public void InvalidCardNumber_Success()
        {
            //Arrange
            var validator = new InvalidCardNumber(ValidCardNumber);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void InvalidCardNumber_Failed()
        {
            //Arrange
            var validator = new InvalidCardNumber(InvalidCardNumber);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
            Assert.IsTrue(!string.IsNullOrEmpty(result.Status));
        }

        [TestMethod]
        public void InvalidCardNumber_NullValue_Failed()
        {
            //Arrange
            var validator = new InvalidCardNumber(null);
            //Act and Assert
            Assert.ThrowsException<ArgumentNullException>(() => validator.Process(), "Invalid value in Card Number parameter. The value cannot be null.");
        }
    }
}
