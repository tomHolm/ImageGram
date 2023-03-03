namespace ImageGram.DB;

public class CosmosDBOptions {
    public string connectionString { get; set; } = string.Empty;
    public string primaryKey { get; set; } = string.Empty;
    public string dbId { get; set; } = string.Empty;
    public string containerId { get; set; } = string.Empty;
    public string partitionKey { get; set; } = string.Empty;
    public string storedProcName { get; set; } = string.Empty;
}