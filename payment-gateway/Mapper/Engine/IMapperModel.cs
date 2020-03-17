namespace payment_gateway.Mapper.Engine
{
    public interface IMapperModel<in TSource, out TDestination>
    {
        TDestination MapToDestination(TSource source);
    }
}
