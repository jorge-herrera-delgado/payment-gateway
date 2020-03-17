using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_gateway_core.Helper;
using payment_gateway_core.Resolver.Engine;
using payment_gateway_core.Validation.Engine;
using payment_gateway_core.Validation.Validator;
using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;

namespace payment_gateway_core.Validation
{
    public class RegistrationValidator : IValidatorManager
    {
        private readonly User _user;
        private readonly IRepository<User> _repository;

        public object ReturnObject { get; private set; }

        public RegistrationValidator(User user, IRepository<User> repository)
        {
            _user = user;
            _repository = repository;
        }

        //if it needs to add more security and/or validation, we can add them based on requirements.
        //Another way we can achieve this functionality could be by IOC container.
        //All the assemblies under IValidator can be registered and then executed to get results.
        public async Task<IEnumerable<Func<Result>>> GetValidatorsResult()
        {
            //we can filter properties to validate if are null or empty
            var properties = _user.FilteredProperties().ToList();
            properties.AddRange(_user.UserLogin.FilteredProperties());
            var listFunc = properties.Select(property => (Func<Result>)(() => new PropertyNullOrEmpty(property).Process())).ToList();

            var model = await _repository.GetItemAsync(x => x.UserLogin.Username == _user.UserLogin.Username);
            var list = new List<IValidator>
            {
                new UserIncorrectAlreadyExists(model != null)
            };
            listFunc.AddRange(list.Select(validator => (Func<Result>)validator.Process));
            //we can add a validator to verify if password has min length
            //we can add a validator to verify if password has numbers
            //we can add a validator to verify if password has at least 1 uppercase
            //we can add a validator to verify if password has at least 1 special character
            //we can add other validators to verify the data

            //Here we validate if the data has been saved
            listFunc.Add(() => new NoSavedToStorage(_repository.AddItemAsync(_user).Result, "User").Process());

            ReturnObject = _user;

            return listFunc;
        }
    }
}
