using System.Threading.Tasks;

namespace payment_gateway_core.Payment.Engine
{
    public interface IBankProcessor
    {
        Task<object> Process(payment_gateway_repository.Model.Payment payment);
    }
}
