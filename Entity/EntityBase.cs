namespace ImageGram.Entity;

public abstract class EntityBase {
    public string id { get; set; } = string.Empty;
    protected abstract string makeNewId();
    public void generateId() {
        this.id = this.makeNewId();
    }
}