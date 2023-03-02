using ImageGram.DB.Container;
using ImageGram.Entity;

namespace ImageGram.DB.Repository;

using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

public abstract class CosmosDBRepository<T>: IRepository<T> where T: EntityBase {
    public abstract string containerName { get; }
    public abstract int pageSize { get; }
    public string continuationToken { get; set; } = string.Empty;
    private readonly Container container;

    public CosmosDBRepository(IContainerFactory factory) {
        this.container = factory.getContainer(this.containerName).container;
    }

    protected virtual PartitionKey getPartitionKey(string id) {
        return new PartitionKey(id.Split(':')[0]);
    }

    public async Task<IEnumerable<T>> getItemsAsync(string query, string? continuationToken = null) {
        FeedIterator<T> iterator = this.container.GetItemQueryIterator<T>(
            query,
            continuationToken,
            new QueryRequestOptions{ MaxItemCount = this.pageSize }
            );
        List<T> result = new List<T>();

        FeedResponse<T> response = await iterator.ReadNextAsync();
        result.AddRange(response.ToList());
        this.continuationToken = response.ContinuationToken;

        return result;
    }

    public async Task<T> GetItemAsync(string id) {
        ItemResponse<T> response = await this.container.ReadItemAsync<T>(id, this.getPartitionKey(id));
        return response.Resource;
    }

    public async Task AddItemAsync(T item) {
        item.generateId();
        await this.container.CreateItemAsync<T>(item, this.getPartitionKey(item.id));
    }

    public async Task UpdateItemAsync(T item) {
        await this.container.UpsertItemAsync<T>(item, this.getPartitionKey(item.id));
    }

    public async Task DeleteItemAsync(string id) {
        await this.container.DeleteItemAsync<T>(id, this.getPartitionKey(id));
    }
}