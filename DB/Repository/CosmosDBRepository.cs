using ImageGram.DB.Container;
using ImageGram.Entity;

namespace ImageGram.DB.Repository;

using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

public abstract class CosmosDBRepository<T>: IRepository<T> where T: EntityBase {
    public abstract string containerName { get; }
    private readonly Container container;

    public CosmosDBRepository(IContainerFactory factory) {
        this.container = factory.getContainer(this.containerName).container;
    }

    protected abstract string generateId(T entity);

    protected virtual PartitionKey getPartitionKey(string id) {
        return new PartitionKey(id.Split(':')[0]);
    }

    public async Task<IEnumerable<T>> getItemsAsync(string query) {
        FeedIterator<T> iterator = this.container.GetItemQueryIterator<T>(new QueryDefinition(query));
        List<T> result = new List<T>();
        while (iterator.HasMoreResults) {
            FeedResponse<T> response = await iterator.ReadNextAsync();
            result.AddRange(response.ToList());
        }
        return result;
    }

    public async Task<T> GetItemAsync(string id) {
        ItemResponse<T> response = await this.container.ReadItemAsync<T>(id, this.getPartitionKey(id));
        return response.Resource;
    }

    public async Task AddItemAsync(T item) {
        item.id = this.generateId(item);
        await this.container.CreateItemAsync<T>(item, this.getPartitionKey(item.id));
    }

    public async Task UpdateItemAsync(T item) {
        await this.container.UpsertItemAsync<T>(item, this.getPartitionKey(item.id));
    }

    public async Task DeleteItemAsync(string id) {
        await this.container.DeleteItemAsync<T>(id, this.getPartitionKey(id));
    }
}