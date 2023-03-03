using ImageGram.DB.Repository;
using ImageGram.Entity;

namespace ImageGram.Service;

public class ImageGramService: IGramService {
    private IPostsRepository repo { get; set; }

    public ImageGramService(IPostsRepository postsRepository) {
        this.repo = postsRepository;
    }

    public async Task<IEnumerable<Post>> getPosts(string? continuationToken = null) {
        return await this.repo.getPosts(continuationToken);
    }

    public async Task<Post> createPost(string image, string? caption) {
        return await this.repo.addPost(
            new Post {
                image = image,
                caption = caption
            }
        );
    }

    public async Task<Comment> addComment(string postId, InnerComment comment) {
        return await this.repo.addComment(postId, comment);
    }

    public string getContinuationToken() {
        return this.repo.getContinuationToken();
    }

    public async Task<bool> deleteComment(string postId, string commentId) {
        var response = await this.repo.deleteComment(postId, commentId);
        return response.success;
    }
}