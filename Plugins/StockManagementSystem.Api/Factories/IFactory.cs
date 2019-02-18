namespace StockManagementSystem.Api.Factories
{
    public interface IFactory<T>
    {
        T Initialize();
    }
}