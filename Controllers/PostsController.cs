using ImageGram.Controllers.Request;
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
    public async Task<ActionResult<IEnumerable<Post>>> GetPosts() {
        string? cToken = Request.Headers["x-ig-continuation-token"];
        var result = await this.service.getPosts(cToken);
        if (result.Any()) {
            Response.Headers.Add("x-ig-continuation-token", this.service.getContinuationToken());
            return Ok(result);
        } else {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<Post>> addPost([FromForm] AddPostRequest req) {
        if (req.image == null) {
            return BadRequest();
        }

        var result = await this.service.createPost(req.image, req.caption);
        string getUrl = $"{Request.Path}/{result.id}";
        return Created(getUrl, result);
    }

    [HttpGet("{postId}")]
    public async Task<ActionResult<Post>> getPost([FromRoute] string postId) {
        var result = await this.service.getPostById(postId);
        return result != null 
            ? Ok(result)
            : NotFound();
    }

    [HttpPost("{postId}/comments")]
    public async Task<ActionResult<Comment>> addComment([FromRoute] string postId, [FromBody] InnerComment comment) {
        if (comment.text == string.Empty) {
            return BadRequest();
        }

        var result = await this.service.addComment(postId, comment);
        if (result != null) {
            string getUrl = $"{Request.Path}/{result.id}";
            return Created(getUrl, result);
        } else {
            return NotFound();
        }
    }

    [HttpGet("{postId}/comments/{commentId}")]
    public async Task<ActionResult<Comment>> getComment([FromRoute] string postId, [FromRoute] string commentId) {
        var result = await this.service.getCommentById(postId, commentId);
        return result != null
            ? Ok(result)
            : NotFound();
    }

    [HttpDelete("{postId}/comments/{commentId}")]
    public async Task<ActionResult> deleteComment([FromRoute] string postId, [FromRoute] string commentId) {
        var result = await this.service.deleteComment(postId, commentId);
        return result 
            ? NoContent()
            : NotFound();
    }
}