using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using payment_gateway.Mapper;
using payment_gateway.Model;
using payment_gateway.Services.Action.Payment;
using payment_gateway.Services.Engine;
using RepoModel = payment_gateway_repository.Model;

namespace payment_gateway.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IActionService _actionService;
        private readonly ILogger<PaymentController> _log;

        public PaymentController(IActionService actionService, ILogger<PaymentController> log)
        {
            _actionService = actionService;
            _log = log;
        }

        /// <summary>
        /// Process payments.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /do-payment
        ///     {
        ///         "user-id": "97190759-1638-42fa-abb7-1ac97111978d",
        ///         "product-id": "123456987",
        ///         "product-name": "Product Name",
        ///         "details": "Some details regarding the product",
        ///         "card-details": {
        ///             "card-firstname": "Firstname",
        ///             "card-lastname": "Lastname",
        ///             "card-number": "1234567890123456",
        ///             "expiry-date-month": 4,
        ///             "expiry-date-year": 2024,
        ///             "cvv": "123",
        ///             "type": "mastercard"
        ///         },
        ///         "price": {
        ///             "amount": 100,
        ///             "currency": "EUR"
        ///         }
        ///     }
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>A newly created user</returns>
        /// <response code="202">Returns the newly processed payment.</response>
        /// <response code="400">If the payment cannot be processed.</response> 
        [Produces("application/json"), Route("do-payment")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseModel> DoPayment([FromBody] Payment model)
        {
            //I used Paypal to simulate a bank for simplicity. Paypal is not a bank but it helps for this exercise to simulate a real one.
            //It can be extended/changed to any bank if requirements are met and/or extending the functionality
            try
            {
                var process = await _actionService.ProcessAction<ProcessPaymentAction>(model);
                return new GlobalResultToResponse<RepoModel.Payment>().MapToDestination(process);
            }
            catch (Exception ex)
            {
                _log.LogError(500, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Process payments.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /get-last-payment/3fa85f64-5717-4562-b3fc-2c963f66afa6
        /// </remarks>
        /// <param name="userId"></param>
        /// <returns>The last payment processed by the logged user.</returns>
        /// <response code="200">Returns last payment processed by the logged user.</response>
        /// <response code="400">If it is a bad request.</response> 
        [Produces("application/json"), Route("get-last-payment/{userId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseModel> GetLastPayment(string userId)
        {
            try
            {
                //given the user id, the user gets the latest processed payment.
                var process = await _actionService.ProcessAction<LastPaymentsActionProvider>(userId);
                return new GlobalResultToResponse<RepoModel.Payment>().MapToDestination(process);
            }
            catch (Exception ex)
            {
                _log.LogError(500, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get a list of payments by filter.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /get-payments
        ///     {
        ///         "user-id": "97190759-1638-42fa-abb7-1ac97111978d",
        ///         "start-date": "2020-01-31T00:00:00.000Z",
        ///         "end-date": "2020-12-31T23:59:59.000Z",
        ///         "product-id": "123456789987-123",
        ///         "product-name": "Product Name",
        ///         "card-number": "1234567890123456"
        ///     }
        /// </remarks>
        /// <param name="filter"></param>
        /// <returns>A list of payments.</returns>
        /// <response code="200">Returns list of payments.</response>
        /// <response code="400">If it is a bad request.</response> 
        [Produces("application/json"), Route("get-payments")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseModel> GetPayments([FromBody] PaymentsFilter filter)
        {
            try
            {
                //given the data in the filter, the user gets a collection of processed payments.
                var process = await _actionService.ProcessAction<AllPaymentsActionProvider>(filter);
                return new GlobalResultToResponse<IEnumerable<RepoModel.Payment>>().MapToDestination<Payment>(process);
            }
            catch (Exception ex)
            {
                _log.LogError(500, ex.Message);
                throw;
            }
        }
    }
}