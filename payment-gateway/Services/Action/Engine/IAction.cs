using System.Threading.Tasks;

namespace payment_gateway.Services.Action.Engine
{
    public interface IAction
    {
        Task<object> ProcessAction(object value);
    }
}
