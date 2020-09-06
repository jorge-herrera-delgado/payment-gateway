using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway.Mapper;
using payment_gateway.Services.Action.Payment;
using payment_gateway.Services.Engine;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Repository.Contract;
using RepoModel = payment_gateway_repository.Model;
using ApiModel = payment_gateway.Model;
using CoreModel = payment_gateway_core.Model;

namespace payment_gateway_test.ActionTest
{
    [TestClass]
    public class AllPaymentsActionProviderTest
    {
        private ApiModel.Payment _apiPayment;
        private RepoModel.Payment _repoPayment;
        private ApiModel.PaymentsFilter _filter;
        private ILogger<AllPaymentsActionProvider> _log;
        private Mock<IPaymentRepository> _mockRepo;
        private Mock<IValidationService> _mockVaService;
        private Mock<IValidatorManager<AllPaymentsValidator, CoreModel.PaymentsFilter>> _mockValManager;

        [TestInitialize]
        public void Init()
        {
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

            _filter = new ApiModel.PaymentsFilter
            {
                UserId = _apiPayment.UserId.ToString(),
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            };

            _repoPayment = new PaymentApiToRepo().MapToDestination(_apiPayment);
            var payments = new List<RepoModel.Payment> { _repoPayment };

            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<AllPaymentsActionProvider>();

            //Mock
            _mockRepo = new Mock<IPaymentRepository>();
            _mockRepo.Setup(r => r.GetMongoQueryable()).Returns(payments.AsQueryable());
            _mockValManager = new Mock<IValidatorManager<AllPaymentsValidator, CoreModel.PaymentsFilter>>();
            _mockValManager.Setup(m => m.GetValidatorsResult(It.IsAny<CoreModel.PaymentsFilter>()))
                .Returns(Task.FromResult(It.IsAny<IEnumerable<Func<Result>>>()));
            _mockVaService = new Mock<IValidationService>();
            _mockVaService.Setup(v => v.ProcessValidation(_mockValManager.Object, It.IsAny<CoreModel.PaymentsFilter>()))
                .Returns(Task.FromResult((object)payments));
        }

        [TestMethod]
        public void AllPaymentsActionProvider_Successful()
        {
            //Arrange
            var action = new AllPaymentsActionProvider(_mockVaService.Object, _mockValManager.Object, _mockRepo.Object, _log);
            //Act
            var result = action.ProcessAction(_filter).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<RepoModel.Payment>));
            Assert.IsTrue(((IEnumerable<RepoModel.Payment>)result).Any());
            Assert.IsTrue(((IEnumerable<RepoModel.Payment>)result).FirstOrDefault(p => p.UserId == _repoPayment.UserId)?.PaymentId == _repoPayment.PaymentId);
        }

        [TestMethod]
        public void AllPaymentsActionProvider_AggregateException()
        {
            //Arrange
            var action = new AllPaymentsActionProvider(_mockVaService.Object, _mockValManager.Object, _mockRepo.Object, _log);
            //Act and Assert
            Assert.ThrowsException<AggregateException>(() => action.ProcessAction(null).Result, "Parameter is null. Invalid value.");
        }

        [TestMethod]
        public void AllPaymentsActionProvider_ValidationService_Failed()
        {
            //Mock
            //Returns an error result in validation
            var mockValService = new Mock<IValidationService>();
            mockValService.Setup(v => v.ProcessValidation(_mockValManager.Object, It.IsAny<CoreModel.PaymentsFilter>())).Returns(
                Task.FromResult(new Result
                {
                    ErrorCode = ErrorCode.NoAuthorized,
                    StatusCode = StatusCode.Failed,
                    StatusDetail = "Test User No Authorized"
                } as object));
            //Arrange
            var action = new AllPaymentsActionProvider(mockValService.Object, _mockValManager.Object, _mockRepo.Object, _log);
            //Act
            var result = action.ProcessAction(_filter).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Result));
            Assert.IsTrue(((Result)result).StatusCode == StatusCode.Failed);
            Assert.IsTrue(((Result)result).ErrorCode == ErrorCode.NoAuthorized);
        }
    }
}
