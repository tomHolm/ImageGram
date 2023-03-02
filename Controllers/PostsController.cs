using ImageGram.Entity;
using ImageGram.Service;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public async Task<IEnumerable<Post>> Get() {
        string? cToken = Request.Headers["x-ig-continuation-token"];
        var result = await this.service.getPosts(cToken);
        Response.Headers.Add("x-ig-continuation-token", this.service.getContinuationToken());

        return result;
    }
}