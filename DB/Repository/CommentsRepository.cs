using ImageGram.DB.Container;
using ImageGram.Entity;

namespace ImageGram.DB.Repository;

public class CommentsRepository: CosmosDBRepository<Comment> {
    public override string containerName { get; } = "comments";

    public CommentsRepository(IContainerFactory factory): base(factory) {}

    protected override string generateId(Comment item) {
        return $"{item.authorId.ToString()}:{item.postId}:{DateTimeOffset.Now.ToUnixTimeSeconds().ToString()}";
    }
}