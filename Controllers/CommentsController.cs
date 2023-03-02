using ImageGram.DB.Container;
using ImageGram.DB.Repository;
using ImageGram.Entity;
using ImageGram.Service;
using Microsoft.AspNetCore.Mvc;

namespace ImageGram.Controllers;

[ApiController]
[Route("comments")]
public class CommentsController: ControllerBase {
    private readonly ILogger<CommentsController> logger;
    private readonly IGramService service;

    public CommentsController(ILogger<CommentsController> logger, IGramService service) {
        this.logger = logger;
        this.service = service;
    }

    [HttpGet]
    public async Task<IEnumerable<Comment>> Get() {
        string? cToken = Request.Headers["x-ig-continuation-token"];
        var result = await this.service.getComments(cToken);
        Response.Headers.Add("x-ig-continuation-token", this.service.getContinuationToken());

        return result;
    }
}