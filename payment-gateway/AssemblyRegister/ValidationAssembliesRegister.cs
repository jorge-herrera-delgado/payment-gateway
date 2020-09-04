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

            service.AddSingleton<IValidatorManager<RepoModel.Payment>, PaymentValidator>()
                .AddSingleton<IValidatorManager<RepoModel.User>, LoginValidator>()
                .AddSingleton<IValidatorManager<RepoModel.User>, RegistrationValidator>()
                .AddSingleton<IValidatorManager<string>, LastPaymentValidator>()
                .AddSingleton<IValidatorManager<CoreModel.PaymentsFilter>, AllPaymentsValidator>();
        }
    }
}