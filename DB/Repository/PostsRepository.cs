using System.Net;
using ImageGram.DB.Stored.Response;
using ImageGram.Entity;
using Microsoft.Azure.Cosmos;

namespace ImageGram.DB.Repository;

public class PostsRepository: IPostsRepository {
    public string continuationToken { get; set; } = string.Empty;
    private readonly Container container;
    private int pageSize { get; } = 2;
    private string addCommentProcName { get; set; }
    private string deleteCommentProcName { get; set; }

    public PostsRepository(CosmosClient client, CosmosDBOptions options) {
        this.container = client.GetContainer(options.dbId, options.containerId);
        this.addCommentProcName = options.addCommentProcName;
        this.deleteCommentProcName = options.deleteCommentProcName;
    }

    public string getContinuationToken() {
        return this.continuationToken;
    }

    private async Task<ItemResponse<T>> AddItemAsync<T>(T item) where T: EntityBase {
        item.generateId();
        return await this.container.CreateItemAsync<T>(item, new PartitionKey(item.id));
    }

    public async Task<Post> addPost(Post post) {
        var response = await this.AddItemAsync<Post>(post);
        return response.Resource;
    }

    private async Task<T> executeStoredProcedure<T>(string procName, string pKey, dynamic[] inputParams) {
        try {
            return await this.container.Scripts.ExecuteStoredProcedureAsync<T>(
                procName,
                new PartitionKey(pKey),
                inputParams
            );
        } catch (CosmosException e) when (e.SubStatusCode == (int)HttpStatusCode.NotFound) {
            return default;
        }
    }

    public async Task<Comment> addComment(string postId, InnerComment comment) {
        comment.timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        comment.generateId();
        dynamic[] inParams = { postId, comment };
        return await this.executeStoredProcedure<Comment>(
            this.addCommentProcName,
            postId,
            inParams
        );
    }

    private async Task<IEnumerable<T>> getItemsAsync<T>(string query, string? continuationToken = null) where T: EntityBase {
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

    public async Task<IEnumerable<Post>> getPosts(string? continuationToken) {
        return await this.getItemsAsync<Post>(
            "select * from posts c where c.type = 'post' order by c.commentsCount desc",
            continuationToken
        );
    }

    private async Task<T> GetItemAsync<T>(string id, string partitionKey) where T: EntityBase {
        try {
            ItemResponse<T> response = await this.container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
            return response.Resource;
        } catch(CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound) {
            return default;
        }
    }

    public async Task<Post?> getPostById(string id) {
        return await this.GetItemAsync<Post>(id, id);
    }
    public async Task<Comment?> getCommentById(string id, string partitionKey) {
        return await this.GetItemAsync<Comment>(id, partitionKey);
    }

    public async Task<DeleteCommentResponse> deleteComment(string postId, string commentId) {
        dynamic[] inParams = { commentId, postId };
        return await this.executeStoredProcedure<DeleteCommentResponse>(
            this.deleteCommentProcName,
            postId,
            inParams
        );
    }
}