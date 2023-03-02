using ImageGram.DB.Container;
using ImageGram.DB.Repository;
using ImageGram.Entity;

namespace ImageGram.Service;

public class ImageGramService: IGramService {
    private PostsRepository postsRepo { get; set; }
    private CommentsRepository commentsRepo { get; set; }

    public ImageGramService(IContainerFactory factory) {
        this.postsRepo = new PostsRepository(factory);
        this.commentsRepo = new CommentsRepository(factory);
    }

    public async Task<IEnumerable<Post>> getPosts(string? continuationToken = null) {
        return await this.postsRepo.getItemsAsync("select * from c order by c.commentsCount desc", continuationToken);
    }

    public async Task<IEnumerable<Comment>> getComments(string? continuationToken = null) {
        return await this.commentsRepo.getItemsAsync("select * from c", continuationToken);
    }

    public string getContinuationToken() {
        return this.postsRepo.continuationToken;
    }
}