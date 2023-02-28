using ImageGram.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageGram.Controllers;

[ApiController]
[Route("api")]
public class PersonController : ControllerBase {

    private readonly ILogger<PersonController> logger;
    private ICDBService dbService;

    public PersonController(ILogger<PersonController> logger, ICDBService dbService) {
        this.logger = logger;
        this.dbService = dbService;
    }

    [HttpGet("say")]
    public Person Get() {
        return new Person(this.dbService.GetData());
    }
}