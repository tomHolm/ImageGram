using ImageGram.DB;
using ImageGram.DB.Repository;
using ImageGram.Entity;
using Microsoft.Azure.Cosmos;

namespace ImageGram.Extension;

public static class IServiceCollectionExtensions {

    private static void createInitialData(Container container, CosmosDBOptions options) {
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
                        new InnerComment {id = firstCommentId, text = "Really nice!", timestamp = firstTime + 30},
                        new InnerComment {id = fifthCommentId, text = "wow!!!", timestamp = firstTime + 300},
                        new InnerComment {id = eighthCommentId, text = "Really nice!", timestamp = firstTime + 333}
                    }
                },
                {
                    secondPostId,
                    new[] {
                        new InnerComment {id = secondCommentId, text = "ZHABA!", timestamp = secondTime + 60},
                        new InnerComment {id = thirdCommentId, text = "SAM TY ZHABA!!!", timestamp = secondTime + 66},
                        new InnerComment {id = sixthCommentId, text = "YA/MY ZHABA!", timestamp = secondTime + 666},
                        new InnerComment {id = seventhCommentId, text = "Ahhaha!", timestamp = secondTime + 999}
                    }
                },
                {
                    thirdPostId,
                    new[] {
                        new InnerComment {id = forthCommentId, text = "May be! hehe))", timestamp = thirdTime + 555}
                    }
                }
            };

            foreach (KeyValuePair<string, InnerComment[]> pair in comments) {
                foreach (InnerComment comment in pair.Value) {
                    dynamic[] procParams = { pair.Key, comment };
                    var response = container.Scripts.ExecuteStoredProcedureAsync<InnerComment>(
                        options.addCommentProcName,
                        new PartitionKey(pair.Key),
                        procParams
                    ).Result;
                    InnerComment newCom = response.Resource;
                }
            }
    }

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

        container.Scripts.CreateStoredProcedureAsync(new Microsoft.Azure.Cosmos.Scripts.StoredProcedureProperties{
            Id = options.addCommentProcName,
            Body = File.ReadAllText($@"DB\Stored\{options.addCommentProcName}.js")
        });

        container.Scripts.CreateStoredProcedureAsync(new Microsoft.Azure.Cosmos.Scripts.StoredProcedureProperties{
            Id = options.deleteCommentProcName,
            Body = File.ReadAllText($@"DB\Stored\{options.deleteCommentProcName}.js")
        });

        if (dbResponse.StatusCode == System.Net.HttpStatusCode.Created) {
            createInitialData(container, options);
        }
    }

    public static IServiceCollection AddCosmosDB(this IServiceCollection services, CosmosDBOptions options) {
        CosmosClient client = getCosmosClient(options);
        PostsRepository postsRepo = new PostsRepository(client, options);
        services.AddSingleton<IPostsRepository>(postsRepo);
        return services;
    }
}