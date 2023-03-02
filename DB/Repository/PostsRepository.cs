using ImageGram.DB.Container;
using ImageGram.Entity;

namespace ImageGram.DB.Repository;

public class PostsRepository: CosmosDBRepository<Post> {
    public override string containerName { get; } = "posts";
    public override int pageSize { get; } = 2;

    public PostsRepository(IContainerFactory factory): base(factory) {}
}