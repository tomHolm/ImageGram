namespace ImageGram.Controllers.Request;

public class AddPostRequest {
    public IFormFile? image { get; set; }
    public string? caption { get; set; }
}