using AutoMapper;
using payment_gateway.Mapper.Engine;
using payment_gateway.Model;
using Core = payment_gateway_core.Model;

namespace payment_gateway.Mapper
{
    public class PaymentFilterApiToCore: IMapperModel<PaymentsFilter, Core.PaymentsFilter>
    {
        public Core.PaymentsFilter MapToDestination(PaymentsFilter source)
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<PaymentsFilter, Core.PaymentsFilter>(); });
            var mapper = config.CreateMapper();
            return mapper.Map<Core.PaymentsFilter>(source);
        }
    }
}
