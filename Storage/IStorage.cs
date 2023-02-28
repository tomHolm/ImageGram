namespace ImageGram.Storage;

public interface IStorage {
    public bool Check();

    public string GetData();
}