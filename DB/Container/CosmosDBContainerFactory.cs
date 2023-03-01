using Microsoft.Azure.Cosmos;

namespace ImageGram.DB.Container;

public class CosmosDBContainerFactory: IContainerFactory {
    private readonly CosmosClient client;
    private readonly CosmosDBOptions options;

    public CosmosDBContainerFactory(CosmosClient client, CosmosDBOptions options) {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        if (this.options.dbId == string.Empty) {
            throw new ArgumentNullException(nameof(this.options.dbId));
        }
        if (this.options.containers.Count == 0) {
            throw new ArgumentNullException(nameof(this.options.containers));
        }
    }

    public ICosmosDBContainer getContainer(string name) {
        if (this.options.containers.Where(info => info.name == name) == null) {
            throw new ArgumentException($"Unable to find container: {name}");
        }
        return new CosmosDBContainer(this.client, this.options.dbId, name);
    }
}