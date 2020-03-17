using System;
using System.Collections.Generic;
using payment_gateway_repository.Model;

namespace payment_gateway.Mapper.Engine
{
    public static class MapperModelFactory<TSource, TDestination>
        where TDestination : class
    {
        private static readonly Dictionary<Type, Type> RegisterTypes = new Dictionary<Type, Type>();

        static MapperModelFactory()
        {
            RegisterTypes.Add(typeof(Model.User), typeof(UserApiToRepo));
            RegisterTypes.Add(typeof(User), typeof(UserRepoToApi));
            RegisterTypes.Add(typeof(Model.UserPayment), typeof(UserPaymentApiToRepo));
            RegisterTypes.Add(typeof(UserPayment), typeof(UserPaymentRepoToApi));
        }

        public static IMapperModel<TSource, TDestination> GetMapper()
        {
            var t = typeof(TSource);
            if (!RegisterTypes.ContainsKey(t)) throw new NotSupportedException();
            return Activator.CreateInstance(RegisterTypes[t], true) as IMapperModel<TSource, TDestination>;
        }
    }
}
