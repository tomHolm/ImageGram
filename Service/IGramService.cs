using ImageGram.Entity;

namespace ImageGram.Service;

public interface IGramService {
    public Task<Post> createPost(string image, string? caption = null);
    public Task<IEnumerable<Post>> getPosts(string? continuationToken);
    public Task<Comment> addComment(string postId, InnerComment comment);
    public string getContinuationToken();
}