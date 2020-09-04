using AutoMapper;
using payment_gateway.Mapper.Engine;
using payment_gateway_core.Helper;
using payment_gateway_repository.Model;

namespace payment_gateway.Mapper
{
    public class PaymentRepoToApi : IMapperModel<Payment, Model.Payment>
    {
        public Model.Payment MapToDestination(Payment source)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Payment, Model.Payment>();
                cfg.CreateMap<CardDetails, Model.CardDetails>()
                    .ForMember(x => x.Cvv, opt => opt.Ignore())
                    .ForMember(y => y.CardNumber, opt => opt.MapFrom(d => d.CardNumber.MaskStringValue(4)));
                cfg.CreateMap<Price, Model.Price>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<Model.Payment>(source);
        }
    }
}
