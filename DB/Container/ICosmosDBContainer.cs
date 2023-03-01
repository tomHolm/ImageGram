namespace ImageGram.DB.Container;

using Microsoft.Azure.Cosmos;

public interface ICosmosDBContainer {
    Container container { get; }
}