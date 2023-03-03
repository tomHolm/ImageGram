using ImageGram.Entity;
using Microsoft.Azure.Cosmos;

namespace ImageGram.DB.Repository;

public class PostsRepository: CosmosDBRepository<Post>, IPostsRepository {
    protected override int pageSize { get; } = 2;
    private string storedProcName { get; set; }

    public PostsRepository(CosmosClient client, CosmosDBOptions options): base(client, options) {
        this.storedProcName = options.storedProcName;
    }

    public string getContinuationToken() {
        return this.continuationToken;
    }

    public async Task<Post> addPost(Post post) {
        var response = await this.AddItemAsync(post);
        return response.Resource;
    }

    public async Task<Comment> addComment(string postId, InnerComment comment) {
        comment.timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        comment.generateId();
        dynamic[] inParams = { postId, comment };
        return await this.executeStoredProcedure<Comment>(
            this.storedProcName,
            postId,
            inParams
        );
    }

    public async Task<IEnumerable<Post>> getPosts(string? continuationToken) {
        return await this.getItemsAsync(
            "select * from posts c where c.type = 'post' order by c.commentsCount desc",
            continuationToken
        );
    }

    public async Task<Post> getPostById(string id) {
        return await this.GetItemAsync<Post>(id, id);
    }
    public async Task<Comment> GetCommentById(string id, string partitionKey) {
        return await this.GetItemAsync<Comment>(id, partitionKey);
    }
}