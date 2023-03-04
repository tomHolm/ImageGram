namespace ImageGram.DB.Repository;

public interface IImageRepository {
    public string storeImage(IFormFile file);
}