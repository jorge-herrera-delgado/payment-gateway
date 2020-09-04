using Microsoft.Extensions.DependencyInjection;
using payment_gateway.Model;
using payment_gateway_core.Model;
using payment_gateway_repository.Engine.Base;
using payment_gateway_repository.Repository;
using payment_gateway_repository.Repository.Contract;

namespace payment_gateway.AssemblyRegister
{
    public static class RepositoryAssembliesRegister
    {
        private static MongoRepositoryBase _repositoryBase;

        public static void RegisterRepositories(this IServiceCollection service, AppSettings appSettings)
        {
            _repositoryBase ??= new MongoRepositoryBase(appSettings.ConnectionString);

            service
                .AddSingleton<IUserRepository>(u => new UserRepository(_repositoryBase))
                .AddSingleton<IPaymentRepository>(p => new PaymentRepository(_repositoryBase));
        }

    }
}
