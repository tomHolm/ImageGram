using ImageGram.DB;
using ImageGram.DB.Repository;
using ImageGram.Entity;
using Microsoft.Azure.Cosmos;

namespace ImageGram.Extension;

public static class IServiceCollectionExtensions {

    private static CosmosClient getCosmosClient(CosmosDBOptions options) {
        CosmosClient client = new CosmosClient(options.connectionString, options.primaryKey);
        checkInitialDB(client, options);
        return client;
    }

    private static void checkInitialDB(CosmosClient client, CosmosDBOptions options) {
        DatabaseResponse dbResponse = client.CreateDatabaseIfNotExistsAsync(options.dbId).Result;
        Database db = dbResponse.Database;

        ContainerResponse cResponse = db.CreateContainerIfNotExistsAsync(options.containerId, options.partitionKey).Result;
        Container container = cResponse.Container;

        // TODO Create stored procedure
        container.Scripts.CreateStoredProcedureAsync(new Microsoft.Azure.Cosmos.Scripts.StoredProcedureProperties{
            Id = options.storedProcName,
            Body = File.ReadAllText($@"DB\Stored\{options.storedProcName}.js")
        });

        if (dbResponse.StatusCode == System.Net.HttpStatusCode.Created) {
            long curTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            long firstTime = curTime - 86400;
            long secondTime = firstTime + 3600;
            long thirdTime = secondTime + 7200;

            string firstPostId = "atata";
            string secondPostId = "tratata";
            string thirdPostId = "ololo";

            List<Post> posts = new List<Post>{
                new Post {id = firstPostId, postId = firstPostId, image = "image.jpg", caption = "What a beautiful image!"},
                new Post {id = secondPostId, postId = secondPostId, image = "pepe.jpg"},
                new Post {id = thirdPostId, postId = thirdPostId, image = "qwerty.jpg", caption = "Previous author is Pepe)) hehe"}
            };

            foreach (Post item in posts) {
                Post addedItem = container.CreateItemAsync<Post>(item, new PartitionKey(item.id)).Result.Resource;
            }

            string firstCommentId = "a";
            string secondCommentId = "b";
            string thirdCommentId = "c";
            string forthCommentId = "d";
            string fifthCommentId = "e";
            string sixthCommentId = "f";
            string seventhCommentId = "g";
            string eighthCommentId = "h";

            Dictionary<string, InnerComment[]> comments = new Dictionary<string, InnerComment[]>{
                {
                    firstPostId,
                    new[] { 
                        new InnerComment {id = firstCommentId, text = "Really nice!"},
                        new InnerComment {id = fifthCommentId, text = "wow!!!"},
                        new InnerComment {id = eighthCommentId, text = "Really nice!"}
                    }
                },
                {
                    secondPostId,
                    new[] {
                        new InnerComment {id = secondCommentId, text = "ZHABA!"},
                        new InnerComment {id = thirdCommentId, text = "SAM TY ZHABA!!!"},
                        new InnerComment {id = sixthCommentId, text = "YA/MY ZHABA!"},
                        new InnerComment {id = seventhCommentId, text = "Ahhaha!"}
                    }
                },
                {
                    thirdPostId,
                    new[] {
                        new InnerComment {id = forthCommentId, text = "May be! hehe))"}
                    }
                }
            };

            foreach (KeyValuePair<string, InnerComment[]> pair in comments) {
                foreach (InnerComment comment in pair.Value) {
                    dynamic[] procParams = { pair.Key, comment };
                    var response = container.Scripts.ExecuteStoredProcedureAsync<InnerComment>(
                        options.storedProcName,
                        new PartitionKey(pair.Key),
                        procParams
                    ).Result;
                    InnerComment newCom = response.Resource;
                }
            }
        }
    }

    public static IServiceCollection AddCosmosDB(this IServiceCollection services, CosmosDBOptions options) {
        CosmosClient client = getCosmosClient(options);
        PostsRepository postsRepo = new PostsRepository(client, options);
        services.AddSingleton<IPostsRepository>(postsRepo);
        return services;
    }
}