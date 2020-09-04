using System;
using System.Collections.Generic;
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
    public class GetLastPaymentTest
    {
        private ApiModel.Payment _apiPayment;
        private RepoModel.Payment _repoPayment;
        private Guid _userId;
        private List<ApiModel.Payment> _payments;

        private ILogger<PaymentController> _log;
        private Mock<IActionService> _mockActService;

        [TestInitialize]
        public void Init()
        {
            _userId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
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
                UserId = _userId
            };
            _payments = new List<ApiModel.Payment> { _apiPayment };
            _repoPayment = new PaymentApiToRepo().MapToDestination(_apiPayment);

            var logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _log = logFactory.CreateLogger<PaymentController>();

            //Mock
            _mockActService = new Mock<IActionService>();
            _mockActService.Setup(s => s.ProcessAction<LastPaymentsActionProvider>(It.IsAny<string>()))
                .Returns((string user) =>
                {
                    if (string.IsNullOrEmpty(user))
                        return Task.FromResult<object>(null);

                    //Simulates an exception
                    if (user == "Exception")
                        throw new NullReferenceException("Test Throw Null Exception from LastPaymentsActionProvide. Parameter cannot be null.");

                    //Simulates an invalid user
                    if (user.Length < 36 || user.Length > 36)
                        return Task.FromResult<object>(new Result
                        {
                            ErrorCode = ErrorCode.InvalidValue,
                            StatusCode = StatusCode.Failed,
                            StatusDetail = "Test Invalid User ID"
                        });

                    return Task.FromResult<object>(_repoPayment);
                });
        }

        [TestMethod]
        public void PaymentController_GetLastPayment_Successful()
        {
            //Arrange
            var controller = new PaymentController(_mockActService.Object, _log);
            //Act
            var result = controller.GetLastPayment(_userId.ToString()).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.TotalRecords == 1);
            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOfType(result.Data, typeof(ApiModel.Payment));
            Assert.AreEqual(((ApiModel.Payment)result.Data).PaymentId, _repoPayment.PaymentId);
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorCode));
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorMessage));
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorStackTrace));
        }

        [TestMethod]
        public void PaymentController_GetLastPayment_Null_Failed()
        {
            //Arrange
            var controller = new PaymentController(_mockActService.Object, _log);
            //Act
            var result = controller.GetLastPayment(null).Result;
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
        public void PaymentController_GetLastPayment_Error_Failed()
        {
            //Arrange
            var controller = new PaymentController(_mockActService.Object, _log);
            //Act
            var result = controller.GetLastPayment("TEST_1234567890").Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.TotalRecords == 1);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(((Result)result.Data).ErrorCode == ErrorCode.InvalidValue);
            Assert.IsTrue(((Result)result.Data).StatusCode == StatusCode.Failed);
            Assert.IsTrue(result.ErrorCode == "301");
            Assert.IsTrue(!string.IsNullOrEmpty(result.ErrorMessage));

        }

        [TestMethod]
        public void PaymentController_GetLastPayment_NullReferenceException_Failed()
        {
            //Arrange
            var controller = new PaymentController(_mockActService.Object, _log);
            //Act and Assert
            Assert.ThrowsExceptionAsync<NullReferenceException>(() => controller.GetLastPayment("Exception"), "Test Null Exception.");
        }
    }
}
