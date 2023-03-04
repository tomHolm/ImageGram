using ImageGram.Entity;

namespace ImageGram.DB.Repository;

using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;

public abstract class CosmosDBRepository<T> where T: EntityBase {
    protected abstract int pageSize { get; }
    public string continuationToken { get; set; } = string.Empty;
    private readonly Container container;

    public CosmosDBRepository(CosmosClient client, CosmosDBOptions options) {
        this.container = client.GetContainer(options.dbId, options.containerId);
    }

    protected virtual PartitionKey getPartitionKey(string id) {
        return new PartitionKey(id.Split(':')[0]);
    }

    protected async Task<IEnumerable<T>> getItemsAsync(string query, string? continuationToken = null) {
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

    protected async Task<K> GetItemAsync<K>(string id, string partitionKey) where K: EntityBase {
        try {
            ItemResponse<K> response = await this.container.ReadItemAsync<K>(id, this.getPartitionKey(partitionKey));
            return response.Resource;
        } catch(CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound) {
            return default;
        }
    }

    protected async Task<ItemResponse<T>> AddItemAsync(T item) {
        item.generateId();
        return await this.container.CreateItemAsync<T>(item, new PartitionKey(item.id));
    }

    protected async Task DeleteItemAsync<K>(string id, string partitionKey) {
        await this.container.DeleteItemAsync<K>(id, new PartitionKey(partitionKey));
    }

    protected async Task<K> executeStoredProcedure<K>(string procName, string pKey, dynamic[] inputParams) {
        try {
            return await this.container.Scripts.ExecuteStoredProcedureAsync<K>(
                procName,
                new PartitionKey(pKey),
                inputParams
            );
        } catch (CosmosException e) when (e.SubStatusCode == (int)HttpStatusCode.NotFound) {
            return default;
        }
    }
}