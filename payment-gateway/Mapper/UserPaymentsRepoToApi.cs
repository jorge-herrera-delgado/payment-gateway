using System.Collections.Generic;
using AutoMapper;
using payment_gateway.Mapper.Engine;
using payment_gateway_core.Helper;
using payment_gateway_repository.Model;

namespace payment_gateway.Mapper
{
    public class UserPaymentsRepoToApi : IMapperModel<IEnumerable<UserPayment>, IEnumerable<Model.UserPayment>>
    {
        public IEnumerable<Model.UserPayment> MapToDestination(IEnumerable<UserPayment> source)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserPayment, Model.UserPayment>();
                cfg.CreateMap<CardDetails, Model.CardDetails>()
                    .ForMember(x => x.Cvv, opt => opt.Ignore())
                    .ForMember(y => y.CardNumber, opt => opt.MapFrom(d => d.CardNumber.MaskStringValue(4)));
                cfg.CreateMap<Payment, Model.Payment>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<IEnumerable<Model.UserPayment>>(source);
        }
    }
}
