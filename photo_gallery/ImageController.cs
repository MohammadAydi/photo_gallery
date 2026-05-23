using Emgu.CV;

namespace photo_gallery;

public class ImageController {
    private readonly ImageModel _model;
    private readonly ImageService _imageService;

    public ImageController() {
        _model = new ImageModel();
        _imageService = new ImageService();
    }
    
    public Mat? CurrentImage => _model.EditedImage;
    public Mat? OriginalImage => _model.OriginalImage;

    public bool HasImage => _model.OriginalImage != null;

    public void LoadImage(string path) {
        _model.OriginalImage = CvInvoke.Imread(path);
        _model.EditedImage = _model.OriginalImage.Clone();
    }
    
    public void SaveImage(string path) {
        if (_model.EditedImage != null) {
            CvInvoke.Imwrite(path, _model.EditedImage);
        }
    }
    private readonly ColorQuantizer _quantizer = new();
    private Bitmap? _quantizedBitmap;

    public void QuantizeColors(int colorCount)
    {
        if (!HasImage) return;
        var result = _quantizer.Quantize(_model.OriginalImage.ToBitmap(), colorCount);
        _quantizedBitmap = result.ResultBitmap;
        _model.EditedImage = result.ResultBitmap.ToMat();
    }

}