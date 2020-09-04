using Microsoft.Extensions.DependencyInjection;
using payment_gateway.Services;
using payment_gateway.Services.Engine;
using payment_gateway_core.Payment;
using payment_gateway_core.Payment.Engine;
using Action = payment_gateway.Services.Action;


namespace payment_gateway.AssemblyRegister
{
    public static class ActionAssembliesRegister
    {
        public static void RegisterActions(this IServiceCollection service)
        {
            service
                .AddSingleton<IActionService, ActionService>()
                .AddSingleton<Action.Engine.IAction, Action.Payment.ProcessPaymentAction>()
                .AddSingleton<Action.Engine.IAction, Action.User.LoginAction>()
                .AddSingleton<Action.Engine.IAction, Action.User.RegistrationAction>()
                .AddSingleton<Action.Engine.IAction, Action.Payment.LastPaymentsActionProvider>()
                .AddSingleton<Action.Engine.IAction, Action.Payment.AllPaymentsActionProvider>()
                .AddSingleton<IBankProcessor, PaypalManager>();
        }
    }
}
