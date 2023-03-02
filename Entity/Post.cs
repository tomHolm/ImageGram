namespace ImageGram.Entity;

public class Post: EntityBase {
    public int authorId { get; set; } = 0;
    public string image { get; set; } = string.Empty;
    public string? caption { get; set; }
    public int commentsCount { get; set; } = 0;
    public List<InnerComment> comments { get; set; } = new List<InnerComment>();

    protected override string makeNewId() {
        string postfix = Guid.NewGuid().ToString();
        long curTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        return $"{authorId}:{curTime}:{postfix}";
    }
}