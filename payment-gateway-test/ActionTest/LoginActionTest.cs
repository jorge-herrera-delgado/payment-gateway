using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public class LoginActionTest
    {
        private RepoModel.User _userRepo;
        private ApiModel.User _userApi;
        private ILogger<LoginAction> _log;
        private Mock<IUserRepository> _mockRepositoryTrue;
        private Mock<IUserRepository> _mockRepositoryFalse;
        private Mock<IUserService> _mockUsService;
        private Mock<IValidatorManager<LoginValidator, RepoModel.User>> _mockValManager;
        private Mock<IValidationService> _mockValService;

        private const string TokenResultTest = "Test123456789.1234564asd4987das65d4as65d4sa6d5132s1gfd5g1";

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
            _log = logFactory.CreateLogger<LoginAction>();

            //Mock
            _mockRepositoryTrue = new Mock<IUserRepository>();
            _mockRepositoryTrue.Setup(x => x.GetItemAsync(It.IsAny<Expression<Func<RepoModel.User, bool>>>())).Returns(Task.FromResult(_userRepo));
            _mockRepositoryTrue.Setup(x => x.UpdateItemAsync(It.IsAny<RepoModel.User>(), It.IsAny<Expression<Func<RepoModel.User, string>>>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            _mockRepositoryFalse = new Mock<IUserRepository>();
            _mockRepositoryFalse.Setup(x => x.GetItemAsync(It.IsAny<Expression<Func<RepoModel.User, bool>>>())).Returns(Task.FromResult(_userRepo));
            _mockRepositoryFalse.Setup(x => x.UpdateItemAsync(It.IsAny<RepoModel.User>(), It.IsAny<Expression<Func<RepoModel.User, string>>>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            _mockUsService = new Mock<IUserService>();
            _mockUsService.Setup(s => s.Authenticate(It.IsAny<RepoModel.User>(), true))
                .Returns((RepoModel.User user, bool isRegistered) => 
                    user.UserLogin.Username != "InvalidToken" 
                        ? _userRepo.UserLogin.Token 
                        : TokenResultTest);

            _mockValManager = new Mock<IValidatorManager<LoginValidator, RepoModel.User>>();
            _mockValManager.Setup(m => m.GetValidatorsResult(It.IsAny<RepoModel.User>()))
                .Returns(Task.FromResult(It.IsAny<IEnumerable<Func<Result>>>()));

            _mockValService = new Mock<IValidationService>();
            _mockValService.Setup(v => v.ProcessValidation(_mockValManager.Object, It.IsAny<RepoModel.User>()))
                .Returns(Task.FromResult((object)_userRepo));
        }

        [TestMethod]
        public void LoginAction_Successful()
        {
            //Arrange
            var action = new LoginAction(_mockRepositoryTrue.Object, _mockUsService.Object, _mockValService.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_userApi.UserLogin).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RepoModel.User));
        }

        [TestMethod]
        public void LoginAction_InvalidToken_StorageUpdated_Successful()
        {
            //Mock
            //Simulates an invalid token and a valid updating in storage
            var temp = _userRepo;
            temp.UserLogin.Username = "InvalidToken";

            _mockRepositoryTrue.Setup(x => x.GetItemAsync(It.IsAny<Expression<Func<RepoModel.User, bool>>>())).Returns(Task.FromResult(temp));
            //Arrange
            var action = new LoginAction(_mockRepositoryTrue.Object, _mockUsService.Object, _mockValService.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_userApi.UserLogin).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RepoModel.User));
            Assert.AreEqual(((RepoModel.User)result).UserLogin.Token, TokenResultTest);
        }

        [TestMethod]
        public void LoginAction_InvalidToken_StorageNoUpdated_Successful()
        {
            //Mock
            //Simulates an invalid token and an invalid updating in storage
            var temp = _userRepo;
            temp.UserLogin.Username = "InvalidToken";

            _mockRepositoryFalse.Setup(x => x.GetItemAsync(It.IsAny<Expression<Func<RepoModel.User, bool>>>())).Returns(Task.FromResult(temp));
            //Arrange
            var action = new LoginAction(_mockRepositoryFalse.Object, _mockUsService.Object, _mockValService.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_userApi.UserLogin).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RepoModel.User));
            Assert.AreEqual(((RepoModel.User)result).UserLogin.Token, TokenResultTest);
        }

        [TestMethod]
        public void LoginAction_AggregateException()
        {
            //Arrange
            var action = new LoginAction(_mockRepositoryTrue.Object, _mockUsService.Object, _mockValService.Object, _mockValManager.Object, _log);
            //Act and Assert
            Assert.ThrowsException<AggregateException>(() => action.ProcessAction(null).Result, "Parameter is null. Invalid value.");
        }

        [TestMethod]
        public void LoginAction_ValidationService_Failed()
        {
            //Mock
            //Returns an error result in validation
            var mockValService = new Mock<IValidationService>();
            mockValService.Setup(v => v.ProcessValidation(_mockValManager.Object, It.IsAny<RepoModel.User>())).Returns(
                Task.FromResult(new Result
                {
                    ErrorCode = ErrorCode.NoAuthorized,
                    StatusCode = StatusCode.Failed,
                    StatusDetail = "Test User No Authorized"
                } as object));
            //Arrange
            var action = new LoginAction(_mockRepositoryTrue.Object, _mockUsService.Object, mockValService.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_userApi.UserLogin).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Result));
            Assert.IsTrue(((Result)result).StatusCode == StatusCode.Failed);
            Assert.IsTrue(((Result)result).ErrorCode == ErrorCode.NoAuthorized);
        }
    }
}
