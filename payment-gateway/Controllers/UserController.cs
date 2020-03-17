using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using payment_gateway.Mapper;
using payment_gateway.Mapper.Engine;
using payment_gateway_repository.Model;
using payment_gateway.Services.Engine;
using payment_gateway_core.Resolver.ResolverManager;
using payment_gateway_core.Validation;
using payment_gateway_repository.Engine.Repository;

namespace payment_gateway.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRepository<User> _repository;

        public UserController(IUserService userService, IRepository<User> repository)
        {
            _userService = userService;
            _repository = repository;
            _userService.Repository = _repository;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> UserLogin([FromBody] Model.UserLogin model)
        {
            //The User/Merchant will log in in order to process a payment and/or make requests for other actions
            var user = _userService.Authenticate(model.Username, model.Password, true);
            var validation = await new ValidatorManager(new LoginValidator(user)).Run();
            var response = new GlobalResultToResponse<User>().MapToDestination(validation);
            return new OkObjectResult(response);
        }

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]

        public async Task<IActionResult> UserRegister([FromBody] Model.User model)
        {
            //For a User and in order to make a payment has to be registered in the system so we can process the data
            var user = _userService.Authenticate(model.UserLogin.Username, model.UserLogin.Password, false);
            var mapper = MapperModelFactory<Model.User, User>.GetMapper();
            var modelMap = mapper.MapToDestination(model);
            modelMap.UserLogin.Token = user.UserLogin.Token;
            var validation = await new ValidatorManager(new RegistrationValidator(modelMap, _repository)).Run();
            var response = new GlobalResultToResponse<User>().MapToDestination(validation);
            return new OkObjectResult(response);
        }

    }
}