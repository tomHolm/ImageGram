using ImageGram.DB;
using ImageGram.DB.Container;
using ImageGram.Entity;
using Microsoft.Azure.Cosmos;

namespace ImageGram.Extension;

public static class IServiceCollectionExtensions {

    private static void checkInitialDB(CosmosClient client, CosmosDBOptions options) {
        DatabaseResponse response = client.CreateDatabaseIfNotExistsAsync(options.dbId).Result;
        Database db = response.Database;

        List<Container> conts = new List<Container>();
        foreach (ContainerInfo cInfo in options.containers) {
            conts.Add(db.CreateContainerIfNotExistsAsync(cInfo.name, cInfo.partitionKey).Result.Container);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Created) {
            long curTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            int firstAuthorId = 1;
            int secondAuthorId = 2;
            int thirdAuthorId = 3;

            long firstTime = curTime - 86400;
            long secondTime = firstTime + 3600;
            long thirdTime = secondTime + 7200;

            string firstPostId = $"{firstAuthorId}:{firstTime}:atata";
            string secondPostId = $"{firstAuthorId}:{secondTime}:tratata";
            string thirdPostId = $"{secondAuthorId}:{thirdTime}:ololo";

            List<Post> posts = new List<Post>{
                new Post {id = firstPostId, authorId = firstAuthorId, image = "image.jpg", caption = "What a beautiful image!"},
                new Post {id = secondPostId, authorId = firstAuthorId, image = "pepe.jpg"},
                new Post {id = thirdPostId, authorId = secondAuthorId, image = "qwerty.jpg", caption = "Previous author is Pepe)) hehe"}
            };

            string firstCommentId = $"{thirdAuthorId}:{firstPostId}:{firstTime + 1800}:a";
            string secondCommentId = $"{thirdAuthorId}:{secondPostId}:{secondTime + 60}:b";
            string thirdCommentId = $"{firstAuthorId}:{secondPostId}:{secondTime + 600}:c";
            string forthCommentId = $"{firstAuthorId}:{thirdPostId}:{thirdTime + 1800}:d";
            string fifthCommentId = $"{secondAuthorId}:{firstPostId}:{firstTime + 1900}:e";
            string sixthCommentId = $"{secondAuthorId}:{secondPostId}:{secondTime + 1800}:f";
            string seventhCommentId = $"{firstAuthorId}:{secondPostId}:{secondTime + 2000}:g";
            string eighthCommentId = $"{firstAuthorId}:{firstPostId}:{firstTime + 2100}:h";

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

            foreach (Post item in posts) {
                IEnumerable<Comment> total = comments.Where(
                    (Comment comment) => { return comment.postId == item.id; }
                );
                int totalCount = total.Count();
                item.comments.AddRange(total.Take(2));
                item.commentsCount = totalCount;
            } 

            foreach (Container container in conts) {
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
        }
    }

        
    public static IServiceCollection AddCosmosDB(this IServiceCollection services, CosmosDBOptions options) {
        CosmosClient client = new CosmosClient(options.connectionString, options.primaryKey);

        checkInitialDB(client, options);

        CosmosDBContainerFactory factory = new CosmosDBContainerFactory(client, options);
        services.AddSingleton<IContainerFactory>(factory);
        return services;
    }
}