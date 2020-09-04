using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway_core.Helper;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Model;

namespace payment_gateway_test.HelperTest
{
    [TestClass]
    public class UtilitiesTest
    {
        [TestMethod]
        public void ConvertToInt_ArgumentException()
        {
            //Arrange
            var test = "3";
            //Act and Assert
            Assert.ThrowsException<ArgumentException>(() => test.ConvertToInt(), "Source type is not enum");
        }

        [TestMethod]
        public void ConvertToInt_Successful()
        {
            //Arrange
            const StatusCode test = StatusCode.Success;
            //Act
            var result = test.ConvertToInt();
            //Assert
            Assert.AreEqual(result, 80);
        }

        [TestMethod]
        public void MaskStringValue_LengthLessThan_Value()
        {
            //Arrange
            var test = "12345";
            //Act
            var result = test.MaskStringValue(6);
            //Assert
            Assert.AreEqual(test.Length, result.Length);
            Assert.IsTrue(result.Equals("*****"));
        }

        [TestMethod]
        public void MaskStringValue_LengthEqualsThan_Value()
        {
            //Arrange
            var test = "12345";
            //Act
            var result = test.MaskStringValue(5);
            //Assert
            Assert.AreEqual(test.Length, result.Length);
            Assert.IsTrue(result.Equals(test));
        }

        [TestMethod]
        public void MaskStringValue_LengthGreaterThan_Value()
        {
            //Arrange
            var test = "12345";
            //Act
            var result = test.MaskStringValue(3);
            //Assert
            Assert.AreEqual(test.Length, result.Length);
            Assert.IsTrue(result.Equals("**345"));
        }

        [TestMethod]
        public void FilteredProperties_PropertiesNoExcluded()
        {
            //Arrange
            var login = new UserLogin
            {
                Username = "Test",
                Password = "123456",
                Token = "123456qwerty"
            };
            //Act
            var result = login.FilteredProperties().ToList();
            //Assert
            Assert.IsTrue(result != null && result.Any());
            Assert.AreEqual(result.FirstOrDefault(p => p.Key == nameof(login.Username)).Value, login.Username);
        }

        [TestMethod]
        public void FilteredProperties_PropertiesExcluded()
        {
            //Arrange
            var login = new UserLogin
            {
                Username = "Test",
                Password = "123456"
            };
            //Act
            var result = login.FilteredProperties(new[]{nameof(login.Token)}).ToList();
            //Assert
            Assert.IsTrue(result != null && result.Any());
            Assert.IsTrue(string.IsNullOrEmpty(result.FirstOrDefault(p => p.Key == nameof(login.Token)).Value));
        }
    }
}
