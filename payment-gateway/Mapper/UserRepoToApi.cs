using AutoMapper;
using payment_gateway.Mapper.Engine;
using payment_gateway_repository.Model;

namespace payment_gateway.Mapper
{
    public class UserRepoToApi : IMapperModel<User, Model.User>
    {
        public Model.User MapToDestination(User source)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, Model.User>();
                cfg.CreateMap<UserLogin, Model.UserLogin>().ForMember(x => x.Password, opt => opt.Ignore());
            });
            var mapper = config.CreateMapper();
            return mapper.Map<Model.User>(source);
        }
    }
}
