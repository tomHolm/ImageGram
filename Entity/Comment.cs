namespace ImageGram.Entity;

public class Comment: EntityBase {
    public string postId { get; set; } = string.Empty;
    public int authorId { get; set; } = 0;
    public string text { get; set; } = string.Empty;
}