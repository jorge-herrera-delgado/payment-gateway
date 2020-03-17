using AutoMapper;
using payment_gateway.Mapper.Engine;
using payment_gateway_repository.Model;

namespace payment_gateway.Mapper
{
    public class UserApiToRepo : IMapperModel<Model.User, User>
    {
        public User MapToDestination(Model.User source)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Model.User, User>();
                cfg.CreateMap<Model.UserLogin, UserLogin>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<User>(source);
        }
    }
}