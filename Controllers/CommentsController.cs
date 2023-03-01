using ImageGram.DB.Container;
using ImageGram.DB.Repository;
using ImageGram.Entity;
using Microsoft.AspNetCore.Mvc;

namespace ImageGram.Controllers;

[ApiController]
[Route("comments")]
public class CommentsController: ControllerBase {
    private readonly ILogger<CommentsController> logger;
    private readonly CommentsRepository repository;

    public CommentsController(ILogger<CommentsController> logger, IContainerFactory factory) {
        this.logger = logger;
        this.repository = new CommentsRepository(factory);
    }

    [HttpGet]
    public async Task<IEnumerable<Comment>> Get() {
        var result = await this.repository.getItemsAsync("select * from c");
        return result;
    }
}