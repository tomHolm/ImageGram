namespace ImageGram.Entity;

public class Post: EntityBase {
    public override string type { get; } = "post";
    public string postId { get; set; } = string.Empty;
    public string image { get; set; } = string.Empty;
    public string? caption { get; set; }
    public int commentsCount { get; set; } = 0;
    public List<InnerComment> comments { get; set; } = new List<InnerComment>();
    public override void generateId() {
        base.generateId();
        this.postId = this.id;
    }
}