using ImageGram.Entity;
using Microsoft.Azure.Cosmos;

namespace ImageGram.DB.Repository;

public interface IRepository<T> where T : EntityBase {
    public Task<IEnumerable<T>> getItemsAsync(string query, string? continuationToken = null);
    public Task<T> GetItemAsync(string id);
    public Task<ItemResponse<T>> AddItemAsync(T item);
    public Task UpdateItemAsync(T item);
    public Task DeleteItemAsync(string id);
}