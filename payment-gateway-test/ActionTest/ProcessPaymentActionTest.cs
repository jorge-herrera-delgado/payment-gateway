using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway.Mapper;
using payment_gateway.Services.Action.Payment;
using payment_gateway.Services.Engine;
using payment_gateway_core.Payment.Engine;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Repository.Contract;
using RepoModel = payment_gateway_repository.Model;
using ApiModel = payment_gateway.Model;

namespace payment_gateway_test.ActionTest
{
    [TestClass]
    public class ProcessPaymentActionTest
    {
        private ApiModel.Payment _apiPayment;
        private RepoModel.Payment _repoPayment;
        private ApiModel.User _apiUser;
        private ILogger<ProcessPaymentAction> _log;
        private Mock<IPaymentRepository> _mockRepoTrue;
        private Mock<IPaymentRepository> _mockRepoFalse;
        private Mock<IValidationService> _mockValService;
        private Mock<IValidatorManager<RepoModel.Payment>> _mockValManager;
        private Mock<IBankProcessor> _mockBankProcessorObj;
        private Mock<IBankProcessor> _mockBankProcessorEx;

        [TestInitialize]
        public void Init()
        {
            _apiUser = new ApiModel.User
            {
                FirstName = "Firstname",
                LastName = "Lastname",
                UserId = Guid.NewGuid(),
                UserLogin = new ApiModel.UserLogin
                {
                    Username = "TestUser",
                    Password = "123456"
                }
            };
            new UserApiToRepo().MapToDestination(_apiUser);

            _apiPayment = new ApiModel.Payment
            {
                CardDetails = new ApiModel.CardDetails
                {
                    CardFirstname = "Firstname",
                    CardLastname = "Lastname",
                    CardNumber = "1234567894567896",
                    Cvv = "123",
                    ExpiryDateMonth = 10,
                    ExpiryDateYear = 2025,
                    Type = "mastercard"
                },
                Created = DateTime.Now,
                Details = "Payment Test",
                PaymentId = Guid.NewGuid(),
                Price = new ApiModel.Price
                {
                    Amount = 100,
                    Currency = "EUR"
                },
                ProductId = "123ABC_TEST1",
                ProductName = "Product Test",
                UserId = Guid.NewGuid()
            };
            _repoPayment = new PaymentApiToRepo().MapToDestination(_apiPayment);

            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<ProcessPaymentAction>();

            //Mock
            _mockRepoTrue = new Mock<IPaymentRepository>();
            _mockRepoTrue.Setup(r => r.AddItemAsync(It.IsAny<RepoModel.Payment>())).Returns(Task.FromResult(true));
            _mockRepoFalse = new Mock<IPaymentRepository>();
            _mockRepoFalse.Setup(r => r.AddItemAsync(It.IsAny<RepoModel.Payment>())).Returns(Task.FromResult(false));
            _mockValManager = new Mock<IValidatorManager<RepoModel.Payment>>();
            _mockValManager.Setup(m => m.GetValidatorsResult(It.IsAny<RepoModel.Payment>()))
                .Returns(Task.FromResult(It.IsAny<IEnumerable<Func<Result>>>()));
            _mockValService = new Mock<IValidationService>();
            _mockValService.Setup(v => v.ProcessValidation(_mockValManager.Object, It.IsAny<RepoModel.Payment>()))
                .Returns(Task.FromResult((object)_repoPayment));
            _mockBankProcessorObj = new Mock<IBankProcessor>();
            _mockBankProcessorObj.Setup(b => b.Process(It.IsAny<RepoModel.Payment>()))
                .Returns(Task.FromResult((object) _repoPayment));
            _mockBankProcessorEx = new Mock<IBankProcessor>();
            _mockBankProcessorEx.Setup(b => b.Process(It.IsAny<RepoModel.Payment>()))
                .Returns(Task.FromResult((object) new Exception("Test return exception.")));
        }

        [TestMethod]
        public void ProcessPaymentAction_Successful()
        {
            //Arrange
            var action = new ProcessPaymentAction(_mockRepoTrue.Object, _mockValService.Object, _mockBankProcessorObj.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_apiPayment).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RepoModel.Payment));
        }

        [TestMethod]
        public void ProcessPaymentAction_AggregateException()
        {
            //Arrange
            var action = new ProcessPaymentAction(_mockRepoTrue.Object, _mockValService.Object, _mockBankProcessorObj.Object, _mockValManager.Object, _log);
            //Act and Assert
            Assert.ThrowsException<AggregateException>(() => action.ProcessAction(null).Result, "Parameter is null. Invalid value.");
        }

        [TestMethod]
        public void ProcessPaymentAction_ValidationService_Failed()
        {
            //Mock
            //Returns an error result in validation
            var mockValService = new Mock<IValidationService>();
            mockValService.Setup(v =>
                    v.ProcessValidation(It.IsAny<IValidatorManager<RepoModel.Payment>>(),
                        It.IsAny<RepoModel.Payment>()))
                .Returns(
                    Task.FromResult(new Result
                    {
                        ErrorCode = ErrorCode.InvalidCardNumber,
                        StatusCode = StatusCode.Failed,
                        StatusDetail = "Test Invalid Card Number."
                    } as object));
            //Arrange
            var action = new ProcessPaymentAction(_mockRepoTrue.Object, mockValService.Object, _mockBankProcessorObj.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_apiPayment).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Result));
            Assert.IsTrue(((Result)result).StatusCode == StatusCode.Failed);
            Assert.IsTrue(((Result)result).ErrorCode == ErrorCode.InvalidCardNumber);
        }

        [TestMethod]
        public void ProcessPaymentAction_NoSavedToStorage_Failed()
        {
            //Arrange
            var action = new ProcessPaymentAction(_mockRepoFalse.Object, _mockValService.Object, _mockBankProcessorObj.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_apiPayment).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Result));
            Assert.IsTrue(((Result)result).StatusCode == StatusCode.Failed);
            Assert.IsTrue(((Result)result).ErrorCode == ErrorCode.DataBaseError);
        }

        [TestMethod]
        public void ProcessPaymentAction_BankProcessor_Failed()
        {
            //Arrange
            var action = new ProcessPaymentAction(_mockRepoFalse.Object, _mockValService.Object, _mockBankProcessorEx.Object, _mockValManager.Object, _log);
            //Act
            var result = action.ProcessAction(_apiPayment).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Result));
            Assert.IsTrue(((Result)result).StatusCode == StatusCode.Failed);
            Assert.IsTrue(((Result)result).ErrorCode == ErrorCode.PaymentFailed);
        }
    }
}
