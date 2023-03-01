using ImageGram.DB.Container;
using ImageGram.Entity;

namespace ImageGram.DB.Repository;

public class PostsRepository: CosmosDBRepository<Post> {
    public override string containerName { get; } = "posts";

    public PostsRepository(IContainerFactory factory): base(factory) {}

    protected override string generateId(Post item) {
        return $"{item.authorId.ToString()}:{DateTimeOffset.Now.ToUnixTimeSeconds().ToString()}";
    }
}