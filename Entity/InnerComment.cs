namespace ImageGram.Entity;

public class InnerComment: EntityBase {
    public override string type { get; } = "comment";
    public string text { get; set; } = string.Empty;
    public long timestamp { get; set; } = 0;
}