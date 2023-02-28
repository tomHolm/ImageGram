using ImageGram.Storage;

namespace ImageGram.Repository;

public class CosmosRepository: IRepository {
    private IStorage storage;

    public CosmosRepository(IStorage storage) {
        this.storage = storage;
    }

    public bool Check() {
        return this.storage.Check();
    }

    public string GetData() {
        return this.storage.GetData();
    }
}