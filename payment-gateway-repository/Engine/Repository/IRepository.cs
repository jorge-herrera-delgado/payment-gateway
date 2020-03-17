namespace payment_gateway_repository.Engine.Repository
{
    public interface IRepository<T> : IRepositoryGet<T>, IRepositoryPost<T> { }
}