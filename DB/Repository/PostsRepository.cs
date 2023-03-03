using ImageGram.DB.Stored.Response;
using ImageGram.Entity;
using Microsoft.Azure.Cosmos;

namespace ImageGram.DB.Repository;

public class PostsRepository: CosmosDBRepository<Post>, IPostsRepository {
    protected override int pageSize { get; } = 2;
    private string addCommentProcName { get; set; }
    private string deleteCommentProcName { get; set; }

    public PostsRepository(CosmosClient client, CosmosDBOptions options): base(client, options) {
        this.addCommentProcName = options.addCommentProcName;
        this.deleteCommentProcName = options.deleteCommentProcName;
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
            this.addCommentProcName,
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
    public async Task<Comment> getCommentById(string id, string partitionKey) {
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