using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using payment_gateway.Mapper.Engine;
using payment_gateway_repository.Model;

namespace payment_gateway.Mapper
{
    public class UserPaymentApiToRepo : IMapperModel<Model.UserPayment, UserPayment>
    {
        public UserPayment MapToDestination(Model.UserPayment source)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Model.UserPayment, UserPayment>();
                cfg.CreateMap<Model.CardDetails, CardDetails>();
                cfg.CreateMap<Model.Payment, Payment>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<UserPayment>(source);
        }
    }
}
