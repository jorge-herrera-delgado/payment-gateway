using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway.Mapper;
using payment_gateway.Services;
using payment_gateway_core.Validation.Engine;
using RepoModel = payment_gateway_repository.Model;
using ApiModel = payment_gateway.Model;

namespace payment_gateway_test.ServiceTest
{
    [TestClass]
    public class ValidationServiceTest
    {
        private RepoModel.User _userRepo;
        private ApiModel.User _userApi;
        private Mock<IValidatorManager<object, RepoModel.User>> _mockValManager;

        [TestInitialize]
        public void Init()
        {
            _userApi = new ApiModel.User
            {
                FirstName = "Firstname",
                LastName = "Lastname",
                UserId = Guid.NewGuid(),
                UserLogin = new ApiModel.UserLogin
                {
                    Username = "TestUser",
                    Password = "123456",
                    Token = "1234564asd4987das65d4as65d4sa6d5132s1gfd5g1"
                }
            };
            _userRepo = new UserApiToRepo().MapToDestination(_userApi);

            _mockValManager = new Mock<IValidatorManager<object, RepoModel.User>>();
            
        }

        [TestMethod]
        public void ValidationService_Successful()
        {
            //Mock
            _mockValManager.Setup(m => m.GetValidatorsResult(It.IsAny<RepoModel.User>()))
                .Returns(Task.FromResult(new List<Func<Result>> { () => new Result(), () => new Result() }.AsEnumerable()));
            _mockValManager.SetupGet(p => p.ReturnObject).Returns(_userRepo);
            //Arrange
            var service = new ValidationService();
            //Act
            var result = service.ProcessValidation(_mockValManager.Object, _userRepo).Result;
            //Assert
            Assert.IsInstanceOfType(result, typeof(RepoModel.User));
            Assert.IsTrue(((RepoModel.User)result).UserId == _userRepo.UserId);
        }

        [TestMethod]
        public void ValidationService_NullReferenceException()
        {
            //Mock
            _mockValManager.Setup(m => m.GetValidatorsResult(It.IsAny<RepoModel.User>()))
                .Returns(Task.FromResult<IEnumerable<Func<Result>>>(null));
            //Arrange
            var service = new ValidationService();
            //Act and Assert
            Assert.ThrowsException<NullReferenceException>(
                () => service.ProcessValidation(_mockValManager.Object, null).Result,
                "Parameter cannot be null. Invalid value.");
        }

        [TestMethod]
        public void ValidationService_Failed()
        {
            //Mock
            _mockValManager.Setup(m => m.GetValidatorsResult(It.IsAny<RepoModel.User>()))
                .Returns(Task.FromResult(new List<Func<Result>>
                {
                    () => new Result(),
                    () => new Result
                    {
                        ErrorCode = ErrorCode.NoAuthorized, 
                        StatusCode = StatusCode.Failed,
                        StatusDetail = "Test Validation Service"
                    }
                }.AsEnumerable()));
            //Arrange
            var service = new ValidationService();
            //Act
            var result = service.ProcessValidation(_mockValManager.Object, _userRepo).Result;
            //Assert
            Assert.IsInstanceOfType(result, typeof(Result));
            Assert.IsTrue(((Result)result).StatusCode == StatusCode.Failed);
            Assert.IsTrue(((Result)result).ErrorCode == ErrorCode.NoAuthorized);
        }
    }
}
