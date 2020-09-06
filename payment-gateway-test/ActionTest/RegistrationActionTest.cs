using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway.Mapper;
using payment_gateway.Services.Action.User;
using payment_gateway.Services.Engine;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Repository.Contract;
using RepoModel = payment_gateway_repository.Model;
using ApiModel = payment_gateway.Model;


namespace payment_gateway_test.ActionTest
{
    [TestClass]
    public class RegistrationActionTest
    {
        private RepoModel.User _userRepo;
        private ApiModel.User _userApi;
        private ILogger<RegistrationAction> _log;
        private Mock<IUserRepository> _mockRepoTrue;
        private Mock<IUserRepository> _mockRepoFalse;
        private Mock<IUserService> _mockUsService;
        private Mock<IValidatorManager<RegistrationValidator, RepoModel.User>> _mockValManager;
        private Mock<IValidationService> _mockValService;

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

            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<RegistrationAction>();

            //Mock
            _mockRepoTrue = new Mock<IUserRepository>();
            _mockRepoTrue.Setup(r => r.AddItemAsync(It.IsAny<RepoModel.User>())).Returns(Task.FromResult(true));

            _mockRepoFalse = new Mock<IUserRepository>();
            _mockRepoFalse.Setup(r => r.AddItemAsync(It.IsAny<RepoModel.User>())).Returns(Task.FromResult(false));

            _mockUsService = new Mock<IUserService>();
            _mockUsService.Setup(s => s.Authenticate(It.IsAny<RepoModel.User>(), false)).Returns(_userRepo.UserLogin.Token);

            _mockValManager = new Mock<IValidatorManager<RegistrationValidator, RepoModel.User>>();
            _mockValManager.Setup(m => m.GetValidatorsResult(It.IsAny<RepoModel.User>()))
                .Returns(Task.FromResult(It.IsAny<IEnumerable<Func<Result>>>()));

            _mockValService = new Mock<IValidationService>();
            _mockValService.Setup(v => v.ProcessValidation(_mockValManager.Object, It.IsAny<RepoModel.User>()))
                .Returns(Task.FromResult((object) _userRepo));
        }

        [TestMethod]
        public void RegistrationAction_Successful()
        {
            //Arrange
            var action = new RegistrationAction(_mockRepoTrue.Object, _mockUsService.Object, _mockValService.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_userApi).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RepoModel.User));
        }

        [TestMethod]
        public void RegistrationAction_AggregateException()
        {
            //Arrange
            var action = new RegistrationAction(_mockRepoTrue.Object, _mockUsService.Object, _mockValService.Object, _mockValManager.Object, _log);
            //Act and Assert
            Assert.ThrowsException<AggregateException>(() => action.ProcessAction(new ApiModel.User()).Result, "UserLogin property is null. Invalid value.");
            Assert.ThrowsException<AggregateException>(() => action.ProcessAction(null).Result, "Parameter is null. Invalid value.");
        }

        [TestMethod]
        public void RegistrationAction_ValidationService_Failed()
        {
            //Mock
            //Returns an error result in validation
            var mockValService = new Mock<IValidationService>();
            mockValService.Setup(v => v.ProcessValidation(_mockValManager.Object, It.IsAny<RepoModel.User>())).Returns(
                Task.FromResult(new Result
                {
                    ErrorCode = ErrorCode.UserAlreadyExists, StatusCode = StatusCode.Failed,
                    StatusDetail = "Test User Already Exists"
                } as object));
            //Arrange
            var action = new RegistrationAction(_mockRepoTrue.Object, _mockUsService.Object, mockValService.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_userApi).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Result));
            Assert.IsTrue(((Result)result).StatusCode == StatusCode.Failed);
            Assert.IsTrue(((Result)result).ErrorCode == ErrorCode.UserAlreadyExists);
        }

        [TestMethod]
        public void RegistrationAction_NoSavedToStorage_Failed()
        {
            //Arrange
            var action = new RegistrationAction(_mockRepoFalse.Object, _mockUsService.Object, _mockValService.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_userApi).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Result));
            Assert.IsTrue(((Result)result).StatusCode == StatusCode.Failed);
            Assert.IsTrue(((Result)result).ErrorCode == ErrorCode.DataBaseError);
        }
    }
}
