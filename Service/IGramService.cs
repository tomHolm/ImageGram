using ImageGram.Entity;

namespace ImageGram.Service;

public interface IGramService {
    public Task<IEnumerable<Post>> getPosts(string? continuationToken);
    public Task<IEnumerable<Comment>> getComments(string? continuationToken);
    public string getContinuationToken();
}