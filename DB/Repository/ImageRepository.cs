using System.Drawing;
using System.Drawing.Imaging;

namespace ImageGram.DB.Repository;

public class ImageRepository: IImageRepository {
    private string folderPath { get; set; }

    public ImageRepository(IWebHostEnvironment hostEnvironment) {
        this.folderPath = Path.Combine(hostEnvironment.WebRootPath, "images");
    } 
    public string storeImage(IFormFile file) {
        if (file == null) {
            throw new ArgumentNullException("file");
        }

        string newFileName = $"{Guid.NewGuid()}.jpg";

        using (MemoryStream mStream = new MemoryStream()) {
            file.CopyTo(mStream);
            Image img = new Bitmap(mStream);
            using (FileStream fStream = new FileStream(Path.Combine(this.folderPath, newFileName), FileMode.Create)) {
                img.Save(fStream, ImageFormat.Jpeg);
            }
        }

        return newFileName;
    }
}