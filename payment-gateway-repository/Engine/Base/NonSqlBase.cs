namespace payment_gateway_repository.Engine.Base
{
    public abstract class NonSqlBase<T, TSettings>
    {
        public abstract T OpenConnection(TSettings settings);
    }
}
