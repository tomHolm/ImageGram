using ImageGram.Entity;

namespace ImageGram.Service;

public interface IGramService {
    public Task<Post> createPost(IFormFile image, string? caption = null);
    public Task<IEnumerable<Post>> getPosts(string? continuationToken);
    public Task<Post?> getPostById(string postId);
    public Task<Comment> addComment(string postId, InnerComment comment);
    public Task<Comment?> getCommentById(string postId, string commentId);
    public Task<bool> deleteComment(string postId, string commentId);
    public string getContinuationToken();
}