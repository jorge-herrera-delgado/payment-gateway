using payment_gateway_repository.Engine.Model;

namespace payment_gateway_repository.Engine.Base
{
    public abstract class NonSqlBase<T>
    {
        protected ClientModel Client;
        public ClientModel ClientBase
        {
            set => Client = value;
        }
        public abstract T OpenConnection();
    }
}
