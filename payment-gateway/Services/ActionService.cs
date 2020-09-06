using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using payment_gateway.Services.Action.Engine;
using payment_gateway.Services.Engine;

namespace payment_gateway.Services
{
    public class ActionService : IActionService
    {
        private readonly IEnumerable<object> _listActions;

        public ActionService(IServiceProvider provider)
        {
            _listActions = provider.GetServices(typeof(IAction));
        }

        public async Task<object> ProcessAction<TAction>(object model) where TAction : IAction
        {
            var t = typeof(TAction);

            var result = _listActions.FirstOrDefault(x => x.GetType() == t);

            return result is IAction action
                ? await action.ProcessAction(model)
                : throw new ArgumentNullException($"'{t}' hasn't been found or hasn't been implemented.");
        }
    }
}
