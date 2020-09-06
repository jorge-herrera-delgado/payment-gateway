using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using payment_gateway_core.Helper;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Model;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway_core.Validation
{
    public class RegistrationValidator : IValidatorManager<RegistrationValidator, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegistrationValidator> _log;

        public object ReturnObject { get; private set; }
        
        public RegistrationValidator(IUserRepository userRepository, ILogger<RegistrationValidator> log)
        {
            _userRepository = userRepository;
            _log = log;
        }

        //if it needs to add more security and/or validation, we can add them based on requirements.
        public async Task<IEnumerable<Func<Result>>> GetValidatorsResult(User model)
         {
             _log.LogInformation($"[Adding] Registration Validator results for: UserId: {model.UserId}");
            //we can filter properties to validate if are null or empty
            var properties = model.FilteredProperties().ToList();
            properties.AddRange(model.UserLogin.FilteredProperties());
            var listFunc = properties.Select(property => (Func<Result>)(() => new PropertyNullOrEmpty(property).Process())).ToList();

            var resultModel = await _userRepository.GetItemAsync(x => x.UserLogin.Username == model.UserLogin.Username);
             var list = new List<IValidator>
            {
                new UserIncorrectAlreadyExists(resultModel != null)
            };
            listFunc.AddRange(list.Select(validator => (Func<Result>)validator.Process));
            //we can add a validator to verify if password has min length
            //we can add a validator to verify if password has numbers
            //we can add a validator to verify if password has at least 1 uppercase
            //we can add a validator to verify if password has at least 1 special character
            //we can add other validators to verify the data
            
            ReturnObject = model;

            _log.LogInformation($"[Finished] Registration Validator results for: UserId: {model.UserId}");

            return listFunc;
        }
    }
}
