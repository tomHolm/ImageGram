using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace ImageGram.Storage;

public class CosmosStorage: IStorage {
    private CosmosClient client;

    public CosmosStorage(IOptions<Options> options) {
        var opts = options.Value;
        client = new CosmosClient(
            opts.ConnectionString,
            opts.PrimaryKey,
            new CosmosClientOptions() { ApplicationName = "ImageGram" }
        );
    }

    public bool Check() {
        return true;
    }

    public string GetData() {
        return "ATATA TRATATA!";
    }
}