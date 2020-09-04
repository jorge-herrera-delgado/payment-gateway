using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway.Services.Action.Engine;
using payment_gateway.Services.Engine;
using payment_gateway_core.Helper;
using payment_gateway_core.Validation.Engine;

namespace payment_gateway.Services.Action.User
{
    public class LoginAction : IAction
    {
        private readonly IUserService _userService;
        private readonly IValidationService _validationService;
        private readonly IValidatorManager<payment_gateway_repository.Model.User> _validator;
        private readonly ILogger<LoginAction> _log;

        public LoginAction(IUserService userService, 
            IValidationService validationService, 
            IValidatorManager<payment_gateway_repository.Model.User> validator,
            ILogger<LoginAction> log)
        {
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
            var user = _userService.Authenticate(model.Username, model.Password, true);

            var validation = await _validationService.ProcessValidation(_validator, user);
            var result = validation as Result;
            if (result?.StatusCode == StatusCode.Failed)
            {
                _log.LogError(result.ErrorCode.ConvertToInt(), $"User Error Details: {result.StatusDetail} - User: {user.UserId} - Username: {user.UserLogin.Username}");
                return result;
            }

            _log.LogInformation($"[Finished] User Login for: Username: {model.Username}");
            return validation;
        }
    }
}
