using ImageGram.DB.Container;
using ImageGram.DB.Repository;
using ImageGram.Entity;
using Microsoft.AspNetCore.Mvc;

namespace ImageGram.Controllers;

[ApiController]
[Route("posts")]
public class PostsController: ControllerBase {
    private readonly ILogger<PostsController> logger;
    private readonly PostsRepository repository;

    public PostsController(ILogger<PostsController> logger, IContainerFactory factory) {
        this.logger = logger;
        this.repository = new PostsRepository(factory);
    }

    [HttpGet]
    public async Task<IEnumerable<Post>> Get() {
        var result = await this.repository.getItemsAsync("select * from c");
        return result;
    }
}