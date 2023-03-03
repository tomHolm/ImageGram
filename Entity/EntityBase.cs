namespace ImageGram.Entity;

public abstract class EntityBase {
    public string id { get; set; } = string.Empty;
    public virtual string type { get; } = string.Empty;
    private string makeNewId() {
        return $"{Guid.NewGuid()}";
    }
    public virtual void generateId() {
        this.id = this.makeNewId();
    }
}