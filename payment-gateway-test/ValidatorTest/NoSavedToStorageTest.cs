using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.ValidatorTest
{
    [TestClass]
    public class NoSavedToStorageTest
    {
        [TestMethod]
        public void NoSavedToStorage_Success()
        {
            //Arrange
            var validator = new NoSavedToStorage(true, "Test");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
        }

        [TestMethod]
        public void NoSavedToStorage_Failed()
        {
            //Arrange
            var validator = new NoSavedToStorage(false, "Test");
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.AreEqual(result.ErrorCode, ErrorCode.DataBaseError);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
        }
    }
}
