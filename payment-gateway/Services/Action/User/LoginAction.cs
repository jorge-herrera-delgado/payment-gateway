using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway.Services.Action.Engine;
using payment_gateway.Services.Engine;
using payment_gateway_core.Helper;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway.Services.Action.User
{
    public class LoginAction : IAction
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IValidationService _validationService;
        private readonly IValidatorManager<LoginValidator, payment_gateway_repository.Model.User> _validator;
        private readonly ILogger<LoginAction> _log;

        public LoginAction(IUserRepository userRepository,
            IUserService userService, 
            IValidationService validationService, 
            IValidatorManager<LoginValidator, payment_gateway_repository.Model.User> validator,
            ILogger<LoginAction> log)
        {
            _userRepository = userRepository;
            _userService = userService;
            _validationService = validationService;
            _validator = validator;
            _log = log;
        }

        public async Task<object> ProcessAction(object value)
        {
            var model = value as Model.UserLogin ?? throw new ArgumentNullException(value.GetType().ToString(), "The value is not implemented.");
            _log.LogInformation($"[Started] User Login for: Username: {model.Username}");
            
            //The User/Merchant will log in order to process a payment and/or make requests for other actions
            var user = await _userRepository.GetItemAsync(u => u.UserLogin.Username == model.Username && u.UserLogin.Password == model.Password);

            var validation = await _validationService.ProcessValidation(_validator, user);
            var result = validation as Result;
            if (result?.StatusCode == StatusCode.Failed)
            {
                _log.LogError(result.ErrorCode.ConvertToInt(), $"User Error Details: {result.StatusDetail} - User: {user.UserId} - Username: {user.UserLogin.Username}");
                return result;
            }

            //Validates Auth Token
            var token = _userService.Authenticate(user, true);
            if (token != user.UserLogin.Token)
            {
                user.UserLogin.Token = token;
                var saved = await _userRepository.UpdateItemAsync(user, u => u.UserLogin.Token, token);
                result = new NoSavedToStorage(saved, "User").Process();
                if (result?.StatusCode == StatusCode.Failed)
                    _log.LogCritical(result.ErrorCode.ConvertToInt(), $"User Error Details: {result.StatusDetail} - User: {user.UserId} - Username: {user.UserLogin.Username}");
                else
                    _log.LogInformation($"Token updated for User: {model.Username}");
            }

            _log.LogInformation($"[Finished] User Login for: Username: {model.Username}");
            return user;
        }
    }
}
