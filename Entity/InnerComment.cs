namespace ImageGram.Entity;

public class InnerComment: EntityBase {
    public int authorId { get; set; } = 0;
    public string text { get; set; } = string.Empty;
    public long timestamp { get; set; } = 0;
    protected override string makeNewId() {
        string postfix = Guid.NewGuid().ToString();
        return $"{authorId}:{postfix}";
    }
}