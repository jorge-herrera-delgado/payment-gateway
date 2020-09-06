using System;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway.Model;
using payment_gateway.Services;
using RepoModel = payment_gateway_repository.Model;

namespace payment_gateway_test.ServiceTest
{
    [TestClass]
    public class UserServiceTest
    {
        private RepoModel.User _userRepo;
        private Mock<IOptions<AppSettings>> _mockSettings;

        [TestInitialize]
        public void Init()
        {
            var appSettings = new AppSettings
            {
                Secret = "CC95A6964450EF5EC10FDA1963C0047CF0C39C4D870339D2312D3C6399A6318A"
            };

            _userRepo = new RepoModel.User
            {
                FirstName = "Firstname",
                LastName = "Lastname",
                UserId = Guid.NewGuid(),
                UserLogin = new RepoModel.UserLogin
                {
                    Username = "TestUser",
                    Password = "123456"
                }
            };

            //Mock
            _mockSettings = new Mock<IOptions<AppSettings>>();
            _mockSettings.Setup(s => s.Value).Returns(appSettings);
        }

        [TestMethod]
        public void UserService_Authenticate_Registered_User_ValidToken_Successful()
        {
            //Arrange
            var service = new UserService(_mockSettings.Object);
            //Act
            //Simulates a Valid Token
            _userRepo.UserLogin.Token = service.GenerateToken(_userRepo.UserId.ToString());
            var result = service.Authenticate(_userRepo, true);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrEmpty(result));
            Assert.IsTrue(result == _userRepo.UserLogin.Token);
        }

        [TestMethod]
        public void UserService_Authenticate_Registered_User_InvalidToken_Successful()
        {
            //Arrange
            var service = new UserService(_mockSettings.Object);
            //Act
            //Simulates an Invalid Token
            _userRepo.UserLogin.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjA1NjE4MGQzLTM2MjItNDc4OC05MmNmLWY3OGEzOWFiMTI2MSIsIm5iZiI6MTU4NzU1Nzk5NCwiZXhwIjoxNTg4MTYyNzk0LCJpYXQiOjE1ODc1NTc5OTR9.bVf1zOIW2f4OsYxMXTiVLMh0QdOc8uVPG7MeitRbNUw";
            var result = service.Authenticate(_userRepo, true);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrEmpty(result));
            Assert.IsTrue(result != _userRepo.UserLogin.Token);
        }

        [TestMethod]
        public void UserService_Authenticate_New_User_Successful()
        {
            //Arrange
            var service = new UserService(_mockSettings.Object);
            //Act
            var result = service.Authenticate(_userRepo, false);
            //Assert
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void UserService_GenerateToken_Successful()
        {
            //Arrange
            var service = new UserService(_mockSettings.Object);
            //Act
            var result = service.GenerateToken(_userRepo.UserId.ToString());
            //Assert
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }
    }
}
