using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_gateway.Services.Action;
using payment_gateway.Services.Action.Engine;

namespace payment_gateway.Services.Engine
{
    public interface IActionService
    {
        Task<object> ProcessAction<TAction>(object model) where TAction : IAction;
    }
}
