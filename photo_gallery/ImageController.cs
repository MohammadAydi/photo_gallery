using Emgu.CV;

namespace photo_gallery;

public class ImageController
{
    private readonly ImageModel _model;
    private readonly ImageService _imageService;

    public ImageController()
    {
        _model = new ImageModel();
        _imageService = new ImageService();
    }

    // ---------------------------
    // IMAGE ACCESS
    // ---------------------------
    public Mat? CurrentImage => _model.EditedImage;

    public Mat? OriginalImage => _model.OriginalImage;

    public bool HasImage => _model.OriginalImage != null;

    // ---------------------------
    // IMAGE INFORMATION
    // ---------------------------
    public string ImageName => _model.GetImageName();

    public double FileSizeKB => _model.GetFileSizeKB();

    public double FileSizeMB => _model.GetFileSizeMB();

    public int Width => _model.GetWidth();

    public int Height => _model.GetHeight();

    public string Extension => _model.GetExtension();

    public int NumberOfChannels => _model.GetNumberOfChannels();

    public float DpiX => _model.GetDpiX();

    public float DpiY => _model.GetDpiY();

    public double AspectRatio => _model.GetAspectRatio();

    public string ResolutionText => _model.GetResolutionText();

    public string ChannelType => _model.GetChannelType();

    // ---------------------------
    // LOAD IMAGE
    // ---------------------------
    public void LoadImage(string path)
    {
        _model.FilePath = path;

        _model.OriginalImage = CvInvoke.Imread(path);

        _model.EditedImage = _model.OriginalImage.Clone();
    }

    public void ResetImage() {
        _model.EditedImage = _model.OriginalImage?.Clone();
    }

    // ---------------------------
    // SAVE IMAGE
    // ---------------------------
    public void SaveImage(string path)
    {
        if (_model.EditedImage != null)
        {
            CvInvoke.Imwrite(path, _model.EditedImage);
        }
    }
}