using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway.Mapper.Engine;
using payment_gateway.Services.Action.Engine;
using payment_gateway.Services.Engine;
using payment_gateway_core.Helper;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway.Services.Action.User
{
    public class RegistrationAction : IAction
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IValidationService _validationService;
        private readonly IValidatorManager<RegistrationValidator, payment_gateway_repository.Model.User> _validator;
        private readonly ILogger<RegistrationAction> _log;

        public RegistrationAction(IUserRepository userRepository,
            IUserService userService,
            IValidationService validationService,
            IValidatorManager<RegistrationValidator, payment_gateway_repository.Model.User> validator,
            ILogger<RegistrationAction> log)
        {
            _userRepository = userRepository;
            _userService = userService;
            _validationService = validationService;
            _validator = validator;
            _log = log;
        }

        public async Task<object> ProcessAction(object value)
        {
            var model = value as Model.User ?? throw new ArgumentNullException(value.GetType().ToString(), "The value is not implemented.");
            _log.LogInformation($"[Started] User Registration for: UserId: {model.UserId} - Username: {model.UserLogin.Username}");
            
            var mapper = MapperModelFactory<Model.User, payment_gateway_repository.Model.User>.GetMapper();
            var modelMap = mapper.MapToDestination(model);

            //For a User and in order to make a payment has to be registered in the system so we can process the data
            var token = _userService.Authenticate(modelMap, false);
            modelMap.UserLogin.Token = token;

            var validation = await _validationService.ProcessValidation(_validator, modelMap);
            var result = validation as Result;
            if (result?.StatusCode == StatusCode.Failed)
            {
                _log.LogError(result.ErrorCode.ConvertToInt(), GetMessage(result, modelMap));
                return result;
            }

            var saved = await _userRepository.AddItemAsync(modelMap);
            result = new NoSavedToStorage(saved, "User").Process();
            if (result?.StatusCode == StatusCode.Failed)
            {
                _log.LogCritical(result.ErrorCode.ConvertToInt(), GetMessage(result, modelMap));
                return result;
            }

            _log.LogInformation($"[Finished] User Registration for: UserId: {model.UserId} - Username: {model.UserLogin.Username}");

            return modelMap;
        }

        private static string GetMessage(Result result, payment_gateway_repository.Model.User user) 
            => $"Payment Error Details: {result.StatusDetail} - UserId: {user.UserId} - Username: {user.UserLogin.Username}";
    }
}
