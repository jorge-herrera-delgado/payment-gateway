using Microsoft.Extensions.DependencyInjection;
using payment_gateway.Services;
using payment_gateway.Services.Engine;
using payment_gateway_core.Validation;
using payment_gateway_core.Validation.Engine;
using RepoModel = payment_gateway_repository.Model;
using CoreModel = payment_gateway_core.Model;

namespace payment_gateway.AssemblyRegister
{
    public static class ValidationAssembliesRegister
    {
        public static void RegisterValidation(this IServiceCollection service)
        {
            service.AddSingleton<IValidationService, ValidationService>();

            service.AddSingleton<IValidatorManager<PaymentValidator, RepoModel.Payment>, PaymentValidator>()
                .AddSingleton<IValidatorManager<LoginValidator, RepoModel.User>, LoginValidator>()
                .AddSingleton<IValidatorManager<RegistrationValidator, RepoModel.User>, RegistrationValidator>()
                .AddSingleton<IValidatorManager<LastPaymentValidator, string>, LastPaymentValidator>()
                .AddSingleton<IValidatorManager<AllPaymentsValidator, CoreModel.PaymentsFilter>, AllPaymentsValidator>();
        }
    }
}