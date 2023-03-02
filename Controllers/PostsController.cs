using ImageGram.Entity;
using ImageGram.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ImageGram.Controllers;

[ApiController]
[Route("posts")]
public class PostsController: ControllerBase {
    private readonly ILogger<PostsController> logger;
    private readonly IGramService service;

    public PostsController(ILogger<PostsController> logger, IGramService service) {
        this.logger = logger;
        this.service = service;
    }

    [HttpPost]
    public async Task<Post> addPost([FromForm] int authorId, [FromForm] string image, [FromForm] string? caption) {
        if (authorId == 0 || image == string.Empty) {
            throw new InvalidDataException("Parameter is missing");
        }
        return await this.service.createPost(authorId, image, caption);
    }

    [HttpGet]
    public async Task<IEnumerable<Post>> Get() {
        string? cToken = Request.Headers["x-ig-continuation-token"];
        var result = await this.service.getPosts(cToken);
        Response.Headers.Add("x-ig-continuation-token", this.service.getContinuationToken());

        return result;
    }
}