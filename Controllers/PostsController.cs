using System.Net;
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

    [HttpPost]
    public async Task<Post> addPost([FromForm] string image, [FromForm] string? caption) {
        if (image == string.Empty) {
            throw new InvalidDataException("Parameter is missing");
        }
        return await this.service.createPost(image, caption);
    }

    [HttpPost("{postId}/comments")]
    public async Task<Comment> addComment([FromRoute] string postId, [FromBody] InnerComment comment) {
        if (comment.text == string.Empty) {
            throw new InvalidDataException("Parameter is missing");
        }

        return await this.service.addComment(postId, comment);
    }

    [HttpGet]
    public async Task<IEnumerable<Post>> GetPosts() {
        string? cToken = Request.Headers["x-ig-continuation-token"];
        var result = await this.service.getPosts(cToken);
        Response.Headers.Add("x-ig-continuation-token", this.service.getContinuationToken());

        return result;
    }

    [HttpDelete("{postId}/comments/{commentId}")]
    public async Task deleteComment([FromRoute] string postId, [FromRoute] string commentId) {
        var result = await this.service.deleteComment(postId, commentId);
        Response.StatusCode = result 
            ? (int)HttpStatusCode.NoContent
            : (int)HttpStatusCode.NotFound;
    }
}