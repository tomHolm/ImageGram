using ImageGram.DB;
using ImageGram.DB.Container;
using ImageGram.Entity;
using Microsoft.Azure.Cosmos;

namespace ImageGram.Extension;

public static class IServiceCollectionExtensions {
    public static IServiceCollection AddCosmosDB(this IServiceCollection services, CosmosDBOptions options) {
        CosmosClient client = new CosmosClient(options.connectionString, options.primaryKey);
        // Check and set startup
        Database db = client.CreateDatabaseIfNotExistsAsync(options.dbId).Result.Database;
        long curTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        int firstAuthorId = 1;
        int secondAuthorId = 2;
        int thirdAuthorId = 3;

        long firstTime = curTime - 86400;
        long secondTime = firstTime + 3600;
        long thirdTime = secondTime + 7200;

        string firstPostId = $"{firstAuthorId}:{firstTime.ToString()}";
        string secondPostId = $"{firstAuthorId}:{secondTime.ToString()}";
        string thirdPostId = $"{secondAuthorId}:{thirdTime.ToString()}";

        List<Post> posts = new List<Post>{
            new Post {id = firstPostId, authorId = firstAuthorId, image = "image.jpg", caption = "What a beautiful image!"},
            new Post {id = secondPostId, authorId = firstAuthorId, image = "pepe.jpg"},
            new Post {id = thirdPostId, authorId = secondAuthorId, image = "qwerty.jpg", caption = "Previous author is Pepe)) hehe"}
        };

        string firstCommentId = $"{thirdAuthorId.ToString()}:{firstPostId}:{firstTime + 1800}";
        string secondCommentId = $"{thirdAuthorId.ToString()}:{secondPostId}:{secondTime + 60}";
        string thirdCommentId = $"{firstAuthorId.ToString()}:{secondPostId}:{secondTime + 600}";
        string forthCommentId = $"{firstAuthorId.ToString()}:{thirdPostId}:{thirdTime + 1800}";
        string fifthCommentId = $"{secondAuthorId.ToString()}:{firstPostId}:{firstTime + 1900}";
        string sixthCommentId = $"{secondAuthorId.ToString()}:{secondPostId}:{secondTime + 1800}";
        string seventhCommentId = $"{firstAuthorId.ToString()}:{secondPostId}:{secondTime + 2000}";
        string eighthCommentId = $"{firstAuthorId.ToString()}:{firstPostId}:{firstTime + 2100}";

        List<Comment> comments = new List<Comment>{
            new Comment {id = firstCommentId, postId = firstPostId, authorId = thirdAuthorId, text = "Really nice!"},
            new Comment {id = secondCommentId, postId = secondPostId, authorId = thirdAuthorId, text = "ZHABA!"},
            new Comment {id = thirdCommentId, postId = secondPostId, authorId = firstAuthorId, text = "SAM TY ZHABA!!!"},
            new Comment {id = forthCommentId, postId = thirdPostId, authorId = firstAuthorId, text = "May be! hehe))"},
            new Comment {id = fifthCommentId, postId = firstPostId, authorId = secondAuthorId, text = "wow!!!"},
            new Comment {id = sixthCommentId, postId = secondPostId, authorId = secondAuthorId, text = "YA/MY ZHABA!"},
            new Comment {id = seventhCommentId, postId = secondPostId, authorId = firstAuthorId, text = "Ahhaha!"},
            new Comment {id = eighthCommentId, postId = firstPostId, authorId = firstAuthorId, text = "Really nice!"}
        };

        foreach (ContainerInfo cInfo in options.containers) {
            Container container = db.CreateContainerIfNotExistsAsync(cInfo.name, cInfo.partitionKey).Result.Container;
            switch (container.Id) {
                case "posts":
                    foreach (Post item in posts) {
                        Post addedItem = container.CreateItemAsync<Post>(item).Result.Resource;
                    }
                    break;
                case "comments":
                    foreach (Comment item in comments) {
                        Comment addedComment = container.CreateItemAsync<Comment>(item).Result.Resource;
                    }
                    break;
            }
        }

        CosmosDBContainerFactory factory = new CosmosDBContainerFactory(client, options);
        services.AddSingleton<IContainerFactory>(factory);
        return services;
    }
}