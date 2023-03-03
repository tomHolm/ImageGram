using ImageGram.Entity;

namespace ImageGram.DB.Repository;

public interface IPostsRepository {
    public string getContinuationToken();
    public Task<Post> addPost(Post post);
    public Task<Comment> addComment(string postId, InnerComment comment);
    public Task<IEnumerable<Post>> getPosts(string? continuationToken);
    public Task<Post> getPostById(string id);
    public Task<Comment> GetCommentById(string id, string partitionKey);
}