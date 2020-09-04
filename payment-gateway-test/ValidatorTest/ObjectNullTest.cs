using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;

namespace payment_gateway_test.ValidatorTest
{
    [TestClass]
    public class ObjectNullTest
    {
        [TestMethod]
        public void ObjectNull_Success()
        {
            //Arrange
            var validator = new ObjectNull(new object(), "Test", ErrorCode.ValueIsNullOrEmpty);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Success);
            Assert.IsTrue(string.IsNullOrEmpty(result.StatusDetail));
        }

        [TestMethod]
        public void ObjectNull_Failed()
        {
            //Arrange
            var validator = new ObjectNull(null, "Test", ErrorCode.ValueIsNullOrEmpty);
            //Act
            var result = validator.Process();
            //Assert
            Assert.AreEqual(result.StatusCode, StatusCode.Failed);
            Assert.AreEqual(result.ErrorCode, ErrorCode.ValueIsNullOrEmpty);
            Assert.IsTrue(!string.IsNullOrEmpty(result.StatusDetail));
        }
    }
}
