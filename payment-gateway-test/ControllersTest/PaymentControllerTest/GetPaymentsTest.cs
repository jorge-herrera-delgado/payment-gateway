using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway.Controllers;
using payment_gateway.Mapper;
using payment_gateway.Services.Action.Payment;
using payment_gateway.Services.Engine;
using payment_gateway_core.Validation.Engine;
using ApiModel = payment_gateway.Model;
using RepoModel = payment_gateway_repository.Model;

namespace payment_gateway_test.ControllersTest.PaymentControllerTest
{
    [TestClass]
    public class GetPaymentsTest
    {

        private ApiModel.Payment _apiPayment;
        private RepoModel.Payment _repoPayment;
        private ApiModel.PaymentsFilter _filter;
        private Guid _userId;
        private List<RepoModel.Payment> _repoPayments;
        private List<ApiModel.Payment> _apiPayments;

        private ILogger<PaymentController> _log;
        private Mock<IActionService> _mockActService;

        [TestInitialize]
        public void Init()
        {
            _filter = new ApiModel.PaymentsFilter
            {
                UserId = "97190759-1638-42fa-abb7-1ac97111978d"
            };

            _userId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            _repoPayment = new RepoModel.Payment
            {
                CardDetails = new RepoModel.CardDetails
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
                Price = new RepoModel.Price
                {
                    Amount = 100,
                    Currency = "EUR"
                },
                ProductId = "123ABC_TEST1",
                ProductName = "Product Test",
                UserId = _userId
            };
            _apiPayment = new PaymentRepoToApi().MapToDestination(_repoPayment);
            _repoPayments = new List<RepoModel.Payment> { _repoPayment };
            _apiPayments = new PaymentsRepoToApi().MapToDestination(_repoPayments).ToList();

            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<PaymentController>();

            //Mock
            _mockActService = new Mock<IActionService>();
            _mockActService.Setup(s => s.ProcessAction<AllPaymentsActionProvider>(It.IsAny<ApiModel.PaymentsFilter>()))
                .Returns((ApiModel.PaymentsFilter filter) =>
                {
                    if (filter == null)
                        return Task.FromResult<object>(null);

                    //Simulates an exception
                    if (filter.ProductName == "Exception")
                        throw new NullReferenceException("Test Throw Null Exception from AllPaymentsActionProvider. Parameter cannot be null.");

                    //Simulates an invalid user
                    if (filter.UserId.Length < 36 || filter.UserId.Length > 36)
                        return Task.FromResult<object>(new Result
                        {
                            ErrorCode = ErrorCode.NoAuthorized,
                            StatusCode = StatusCode.Failed,
                            StatusDetail = "Test Invalid User ID"
                        });

                    return Task.FromResult<object>(_repoPayments);
                });
        }

        [TestMethod]
        public void PaymentController_GetPayments_Successful()
        {
            //Arrange
            var controller = new PaymentController(_mockActService.Object, _log);
            //Act
            var result = controller.GetPayments(_filter).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOfType(result.Data, typeof(IEnumerable<ApiModel.Payment>));
            Assert.AreEqual(((IEnumerable<ApiModel.Payment>)result.Data).Count(), result.TotalRecords);
            Assert.IsTrue(((IEnumerable<ApiModel.Payment>)result.Data).Any());
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorCode));
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorMessage));
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorStackTrace));
        }

        [TestMethod]
        public void PaymentController_GetPayments_Null_Failed()
        {
            //Arrange
            var controller = new PaymentController(_mockActService.Object, _log);
            //Act
            var result = controller.GetPayments(null).Result;
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
        public void PaymentController_GetPayments_Error_Failed()
        {
            //Arrange
            var controller = new PaymentController(_mockActService.Object, _log);
            //Act
            var result = controller.GetPayments(new ApiModel.PaymentsFilter {UserId = "TEST_1234567890"}).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.TotalRecords == 1);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(((Result)result.Data).ErrorCode == ErrorCode.NoAuthorized);
            Assert.IsTrue(((Result)result.Data).StatusCode == StatusCode.Failed);
            Assert.IsTrue(result.ErrorCode == "301");
            Assert.IsTrue(!string.IsNullOrEmpty(result.ErrorMessage));

        }

        [TestMethod]
        public void PaymentController_GetPayments_NullReferenceException_Failed()
        {
            //Arrange
            var controller = new PaymentController(_mockActService.Object, _log);
            //Act and Assert
            Assert.ThrowsExceptionAsync<NullReferenceException>(
                () => controller.GetPayments(new ApiModel.PaymentsFilter {ProductName = "Exception"}),
                "Test Null Exception.");
        }
    }
}

