using System.Threading.Tasks;

namespace CosmosDbLibrary
{
    public interface ICosmosDbService<T>
    {
        Task AddItemAsync(T item);
    }
}
