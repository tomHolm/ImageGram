using ImageGram.DB.Container;

namespace ImageGram.DB;

public class CosmosDBOptions {
    public string connectionString { get; set; } = String.Empty;
    public string primaryKey { get; set; } = String.Empty;
    public string dbId { get; set; } = String.Empty;
    public List<ContainerInfo> containers { get; set; } = new List<ContainerInfo>();
}