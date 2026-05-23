using Emgu.CV;

namespace photo_gallery;

public class ImageModel {
    public Mat? OriginalImage { get; set; }
    public Mat? EditedImage  { get; set; }
}