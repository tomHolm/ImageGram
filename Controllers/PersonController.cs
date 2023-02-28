using Microsoft.AspNetCore.Mvc;

namespace ImageGram.Controllers;

[ApiController]
[Route("api")]
public class PersonController : ControllerBase {

    private readonly ILogger<PersonController> _logger;

    public PersonController(ILogger<PersonController> logger) {
        _logger = logger;
    }

    [HttpGet("person/{name}")]
    public Person Get(string name) {
        return new Person(name);
    }
}