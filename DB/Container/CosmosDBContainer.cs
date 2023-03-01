namespace ImageGram.DB.Container;

using Microsoft.Azure.Cosmos;

public class CosmosDBContainer: ICosmosDBContainer {
    public Container container { get; }

    public CosmosDBContainer(CosmosClient client, string dbId, string containerName) {
        this.container = client.GetContainer(dbId, containerName);
    }
}