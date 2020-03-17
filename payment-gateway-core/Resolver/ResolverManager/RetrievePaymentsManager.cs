using System;
using System.Linq;
using System.Threading.Tasks;
using payment_gateway_core.Helper;
using payment_gateway_core.Model;
using payment_gateway_core.Validation.Engine;
using payment_gateway_repository.Engine.Repository;
using payment_gateway_repository.Model;

namespace payment_gateway_core.Resolver.ResolverManager
{
    public class RetrievePaymentsManager
    {
        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<UserPayment> _repositoryPayment;

        public RetrievePaymentsManager(IRepository<User> repositoryUser, IRepository<UserPayment> repositoryPayment)
        {
            _repositoryUser = repositoryUser;
            _repositoryPayment = repositoryPayment;
        }

        public async Task<object> GetLastPayment(string userId)
        {
            var user = await _repositoryUser.GetItemAsync(u => u.UserId == new Guid(userId));
            
            if (user != null)
                return _repositoryPayment.GetMongoQueryable().Where(u => u.UserId == user.UserId).OrderByDescending(x => x.Created).FirstOrDefault();
            
            
            return new Result
            {
                ErrorCode = ErrorCode.NoAuthorized,
                StatusCode = StatusCode.Failed,
                StatusDetail = $"The user (UserId: {userId}) is not authorized to get these details."
            };
        }

        public async Task<object> GetPayments(PaymentsFilter filter)
        {
            var user = await _repositoryUser.GetItemAsync(u => u.UserId == new Guid(filter.UserId));
            if (user != null)
                return _repositoryPayment.GetMongoQueryable().GetPaymentsQueryable(filter).ToList();

            return new Result
            {
                ErrorCode = ErrorCode.NoAuthorized,
                StatusCode = StatusCode.Failed,
                StatusDetail = $"The user (UserId: {filter.UserId}) is not authorized to get these details."
            };
        }
    }
}
