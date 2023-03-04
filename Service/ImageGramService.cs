using ImageGram.DB.Repository;
using ImageGram.Entity;

namespace ImageGram.Service;

public class ImageGramService: IGramService {
    private IPostsRepository postsRepo { get; set; }
    private IImageRepository imageRepo { get; set; }

    public ImageGramService(IPostsRepository postsRepository, IImageRepository imageRepository) {
        this.postsRepo = postsRepository;
        this.imageRepo = imageRepository;
    }

    public async Task<IEnumerable<Post>> getPosts(string? continuationToken = null) {
        return await this.postsRepo.getPosts(continuationToken);
    }

    public async Task<Post> createPost(IFormFile image, string? caption) {
        string imageFile = this.imageRepo.storeImage(image);
        return await this.postsRepo.addPost(
            new Post {
                image = imageFile,
                caption = caption
            }
        );
    }

    public async Task<Post?> getPostById(string postId) {
        return await this.postsRepo.getPostById(postId);
    }

    public async Task<Comment> addComment(string postId, InnerComment comment) {
        return await this.postsRepo.addComment(postId, comment);
    }

    public async Task<Comment?> getCommentById(string postId, string commentId) {
        return await this.postsRepo.getCommentById(commentId, postId);
    }

    public string getContinuationToken() {
        return this.postsRepo.getContinuationToken();
    }

    public async Task<bool> deleteComment(string postId, string commentId) {
        var response = await this.postsRepo.deleteComment(postId, commentId);
        return response != null && response.success;
    }
}