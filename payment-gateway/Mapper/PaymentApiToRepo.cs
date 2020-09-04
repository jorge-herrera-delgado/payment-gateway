using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using payment_gateway.Mapper.Engine;
using payment_gateway_repository.Model;

namespace payment_gateway.Mapper
{
    public class PaymentApiToRepo : IMapperModel<Model.Payment, Payment>
    {
        public Payment MapToDestination(Model.Payment source)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Model.Payment, Payment>();
                cfg.CreateMap<Model.CardDetails, CardDetails>();
                cfg.CreateMap<Model.Price, Price>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<Payment>(source);
        }
    }
}
