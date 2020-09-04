using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway.Controllers;
using payment_gateway.Mapper;
using payment_gateway.Services.Action.User;
using payment_gateway.Services.Engine;
using payment_gateway_core.Validation.Engine;
using ApiModel = payment_gateway.Model;
using RepoModel = payment_gateway_repository.Model;

namespace payment_gateway_test.ControllersTest.UserControllerTest
{
    [TestClass]
    public class UserRegisterTest
    {
        private ApiModel.User _apiUser;
        private RepoModel.User _repoUser;

        private ILogger<UserController> _log;
        private Mock<IActionService> _mockActService;

        [TestInitialize]
        public void Init()
        {
            _repoUser = new RepoModel.User
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
            _apiUser = new UserRepoToApi().MapToDestination(_repoUser);
            _apiUser.UserLogin.Password = "123456";

            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<UserController>();

            //Mock
            _mockActService = new Mock<IActionService>();
            _mockActService.Setup(s => s.ProcessAction<RegistrationAction>(It.IsAny<ApiModel.User>()))
                .Returns((ApiModel.User user) =>
                {
                    if (user == null)
                        return Task.FromResult<object>(null);

                    //Simulates an exception
                    if (user.UserLogin == null)
                        throw new NullReferenceException("Test Throw Null Exception from RegistrationAction. Parameter cannot be null.");

                    //Simulates an invalid user
                    if (user.UserLogin.Username == "UserExists")
                        return Task.FromResult<object>(new Result
                        {
                            ErrorCode = ErrorCode.UserAlreadyExists,
                            StatusCode = StatusCode.Failed,
                            StatusDetail = "Test Invalid Username"
                        });

                    return Task.FromResult<object>(_repoUser);
                });
        }

        [TestMethod]
        public void UserController_RegistrationAction_Successful()
        {
            //Arrange
            var controller = new UserController(_mockActService.Object, _log);
            //Act
            var result = controller.UserRegister(_apiUser).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.TotalRecords == 1);
            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOfType(result.Data, typeof(ApiModel.User));
            Assert.AreEqual(((ApiModel.User)result.Data).UserId, _repoUser.UserId);
            Assert.IsTrue(string.IsNullOrEmpty(((ApiModel.User)result.Data).UserLogin.Password));
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorCode));
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorMessage));
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorStackTrace));
        }

        [TestMethod]
        public void UserController_RegistrationAction_Null_Failed()
        {
            //Arrange
            var controller = new UserController(_mockActService.Object, _log);
            //Act
            var result = controller.UserLogin(null).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.TotalRecords == 0);
            Assert.IsNull(result.Data);
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorCode));
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorMessage));
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorStackTrace));
        }

        [TestMethod]
        public void UserController_RegistrationAction_Error_Failed()
        {
            //Arrange
            var controller = new UserController(_mockActService.Object, _log);
            //Act
            var result = controller.UserRegister(new ApiModel.User{UserLogin = new ApiModel.UserLogin { Username = "UserExists" } }).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.TotalRecords == 1);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(((Result)result.Data).ErrorCode == ErrorCode.UserAlreadyExists);
            Assert.IsTrue(((Result)result.Data).StatusCode == StatusCode.Failed);
            Assert.IsTrue(result.ErrorCode == "301");
            Assert.IsTrue(!string.IsNullOrEmpty(result.ErrorMessage));

        }

        [TestMethod]
        public void UserController_RegistrationAction_NullReferenceException_Failed()
        {
            //Arrange
            var controller = new UserController(_mockActService.Object, _log);
            //Act and Assert
            Assert.ThrowsExceptionAsync<NullReferenceException>(() => controller.UserRegister(new ApiModel.User()), "Test Null Exception.");
        }
    }
}
