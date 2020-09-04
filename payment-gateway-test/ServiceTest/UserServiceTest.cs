using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway.Model;
using payment_gateway.Services;
using payment_gateway_repository.Repository.Contract;
using RepoModel = payment_gateway_repository.Model;

namespace payment_gateway_test.ServiceTest
{
    [TestClass]
    public class UserServiceTest
    {
        private RepoModel.User _userRepo;
        private Mock<IUserRepository> _mockRepoTrue;
        private Mock<IUserRepository> _mockRepoFalse;
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
                    Password = "123456",
                    Token = "1234564asd4987das65d4as65d4sa6d5132s1gfd5g1"
                }
            };

            //Mock
            _mockRepoTrue = new Mock<IUserRepository>();
            _mockRepoTrue.Setup(r => r.GetItem(It.IsAny<Expression<Func<RepoModel.User, bool>>>())).Returns(_userRepo);

            _mockRepoFalse = new Mock<IUserRepository>();
            _mockRepoFalse.Setup(r => r.GetItem(It.IsAny<Expression<Func<RepoModel.User, bool>>>())).Returns<RepoModel.User>(null);

            _mockSettings = new Mock<IOptions<AppSettings>>();
            _mockSettings.Setup(s => s.Value).Returns(appSettings);
        }

        [TestMethod]
        public void UserService_Authenticate_Registered_User_Successful()
        {
            //Arrange
            var service = new UserService(_mockRepoTrue.Object, _mockSettings.Object);
            //Act
            var result = service.Authenticate(_userRepo.UserLogin.Username, _userRepo.UserLogin.Password, true);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrEmpty(result.UserLogin.Password));
        }

        [TestMethod]
        public void UserService_Authenticate_New_User_Successful()
        {
            //Arrange
            var service = new UserService(_mockRepoFalse.Object, _mockSettings.Object);
            //Act
            var result = service.Authenticate(_userRepo.UserLogin.Username, _userRepo.UserLogin.Password, false);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.UserLogin.Password));
            Assert.IsTrue(!string.IsNullOrEmpty(result.UserLogin.Token));
        }
    }
}
