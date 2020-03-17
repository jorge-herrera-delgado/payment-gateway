using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using payment_gateway.Mapper;
using payment_gateway.Mapper.Engine;
using payment_gateway.Model;
using payment_gateway_core.Payment;
using payment_gateway_core.Resolver.ResolverManager;
using payment_gateway_core.Validation;
using payment_gateway_repository.Engine.Repository;
using Repo = payment_gateway_repository.Model;

namespace payment_gateway.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserPaymentController : Controller
    {
        private readonly IRepository<Repo.UserPayment> _repository;
        private readonly IRepository<Repo.User> _repositoryUser;
        private readonly PaypalSettings _paypalSettings;

        public UserPaymentController(IRepository<Repo.UserPayment> repository, IRepository<Repo.User> repositoryUser, IOptions<AppSettings> appSettings)
        {
            _repository = repository;
            _repositoryUser = repositoryUser;
            _paypalSettings = appSettings.Value.PaypalSettings;
        }

        [Route("do-payment")]
        [HttpPost]
        public async Task<IActionResult> DoPayment([FromBody] UserPayment userPayment)
        {
            var mapper = MapperModelFactory<UserPayment, Repo.UserPayment>.GetMapper();
            var model = mapper.MapToDestination(userPayment);
            
            //I use Paypal to simulate a bank for simplicity. Paypal is not a bank but it helps for this exercise to simulate a real one.
            //It can be extended/changed to any bank if requirements are met and/or extending the functionality
            var validation = await new ValidatorManager(new PaymentValidator(_repositoryUser,_repository,
                new PaypalManager(model, _paypalSettings.ClientId, _paypalSettings.ClientSecret), model)).Run();
            var response = new GlobalResultToResponse<Repo.UserPayment>().MapToDestination(validation);
            return new OkObjectResult(response);
        }
    }
}