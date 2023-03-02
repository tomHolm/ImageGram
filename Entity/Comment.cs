namespace ImageGram.Entity;

public class Comment: InnerComment {
    public string postId { get; set; } = string.Empty;

    protected override string makeNewId() {
        string postfix = Guid.NewGuid().ToString();
        return $"{postId}:{authorId}:{timestamp}:{postfix}";
    }
}