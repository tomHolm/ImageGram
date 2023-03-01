namespace ImageGram.Entity;

public class Post: EntityBase {
    public int authorId { get; set; } = 0;
    public string image { get; set; } = string.Empty;
    public string? caption { get; set; }
}