using ImageGram.DB.Container;
using ImageGram.Entity;

namespace ImageGram.DB.Repository;

public class CommentsRepository: CosmosDBRepository<Comment> {
    public override string containerName { get; } = "comments";
    public override int pageSize { get; } = 2;

    public CommentsRepository(IContainerFactory factory): base(factory) {}
}