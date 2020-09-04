using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using payment_gateway.Mapper;
using payment_gateway.Services.Action.User;
using payment_gateway.Services.Engine;
using payment_gateway.Model;
using RepoModel = payment_gateway_repository.Model;

namespace payment_gateway.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IActionService _actionService;
        private readonly ILogger<UserController> _log;

        public UserController(IActionService actionService, ILogger<UserController> log)
        {
            _actionService = actionService;
            _log = log;
        }

        /// <summary>
        /// Login for Registered Users.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /login
        ///     {
        ///         "username": "YourUserName",
        ///         "password": "YourPassword"
        ///     }
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>A logged user</returns>
        /// <response code="200">Returns the registered user.</response>
        /// <response code="400">If the user does not exists.</response> 
        [AllowAnonymous]
        [Produces("application/json"), Route("login")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseModel> UserLogin([FromBody] UserLogin model)
        {
            try
            {
                var process = await _actionService.ProcessAction<LoginAction>(model);
                return new GlobalResultToResponse<RepoModel.User>().MapToDestination(process);
            }
            catch (Exception ex)
            {
                _log.LogError(500, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Register New Users.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /register
        ///     {
        ///         "firstname": "test-2",
        ///         "lastname": "t-2",
        ///         "login": {
        ///             "username": "test2",
        ///             "password": "123456",
        ///             "token": ""
        ///         }
        ///     }
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>A newly created user</returns>
        /// <response code="201">Returns the newly created user.</response>
        /// <response code="400">If the user cannot be created.</response> 
        [AllowAnonymous]
        [Produces("application/json"), Route("register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseModel> UserRegister([FromBody] User model)
        {
            try
            {
                var process = await _actionService.ProcessAction<RegistrationAction>(model);
                return new GlobalResultToResponse<RepoModel.User>().MapToDestination(process);
            }
            catch (Exception ex)
            {
                _log.LogError(500, ex.Message);
                throw;
            }
        }

    }
}