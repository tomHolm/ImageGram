namespace ImageGram.Services;

using ImageGram.Repository;

public class CosmosService: ICDBService {
    private IRepository repo;

    public CosmosService(IRepository repo) {
        this.repo = repo;
    }

    public bool Check() {
        return this.repo.Check();
    }

    public string GetData() {
        return this.repo.GetData();
    }
}