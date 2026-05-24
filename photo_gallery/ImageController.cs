using Emgu.CV;
using OpenCvSharp.Extensions;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace photo_gallery;

public class ImageController {
    private readonly ImageModel _model;
    private readonly ColorQuantizer _quantizer = new();
    private readonly ColorSpaceConverterService _converterService;
    private readonly ImageCompoundsService _compoundsService;
    private ColorSpaceType? _activeColorSpace;


    /// Index order mirrors OpenCV channel order after conversion:
    ///   RGB   → [R, G, B]
    ///   HSV   → [H, S, V]
    ///   YUV   → [Y, U(Cb-blue), V(Cr-red)]
    ///   LAB   → [L, A(green), B(blue)]
    ///   YCbCr → [Y, Cb, Cr]
    ///   CMYK  → [C, M, Y, K]
    private bool[] _channelEnabled = Array.Empty<bool>();

    private double[] _channelScales = Array.Empty<double>();


    public ImageController() {
        _model = new ImageModel();
        _converterService = new ColorSpaceConverterService();
        _compoundsService = new ImageCompoundsService();
    }


    public Mat? CurrentImage => _model.EditedImage;
    public Mat? OriginalImage => _model.OriginalImage;
    public bool HasImage => _model.OriginalImage != null;

    // -----------------------------------------------------------------------
    // IMAGE INFORMATION
    // -----------------------------------------------------------------------
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

    // -----------------------------------------------------------------------
    // LOAD / RESET / SAVE
    // -----------------------------------------------------------------------
    public void LoadImage(string path) {
        _model.FilePath = path;
        _model.OriginalImage = CvInvoke.Imread(path);
        _model.EditedImage = _model.OriginalImage.Clone();
        _activeColorSpace = null;
        ResetChannelState(3);
    }

    public void ResetImage() {
        _model.EditedImage = _model.OriginalImage?.Clone();
        _activeColorSpace = null;
        ResetChannelState(3);
    }

    public void SaveImage(string path) {
        if (_model.EditedImage == null) return;

        Mat toSave = _model.EditedImage;
        Mat converted = null;

        try {
            if (_model.EditedImage.Depth == DepthType.Cv32F) {
                converted = new Mat();
                _model.EditedImage.ConvertTo(converted, DepthType.Cv8U, 255.0);
                toSave = converted;
            }

            CvInvoke.Imwrite(path, toSave);
        }
        finally {
            converted?.Dispose();
        }
    }

    // -----------------------------------------------------------------------
    // COLOR SPACE CONVERSION
    // -----------------------------------------------------------------------

    public void ConvertColorSpace(ColorSpaceType target) {
        if (_model.OriginalImage == null)
            throw new InvalidOperationException("No image loaded.");

        _activeColorSpace = target;
        int channelCount = target == ColorSpaceType.CMYK ? 4 : 3;
        ResetChannelState(channelCount);

        // Convert and apply
        ApplyCurrentChannelState();
    }

    // -----------------------------------------------------------------------
    // CHANNEL ENABLE / DISABLE
    // -----------------------------------------------------------------------

    /// Enable or disable a single channel (0-based index).
    public void SetChannelEnabled(int channelIndex, bool enabled) {
        if (!HasImage || _activeColorSpace == null) return;
        if (channelIndex < 0 || channelIndex >= _channelEnabled.Length) return;

        _channelEnabled[channelIndex] = enabled;
        ApplyCurrentChannelState();
    }


    public void SetChannelScale(int channelIndex, int sliderValue) {
        if (!HasImage || _activeColorSpace == null) return;
        if (channelIndex < 0 || channelIndex >= _channelScales.Length) return;

        _channelScales[channelIndex] = sliderValue / 100.0;
        ApplyCurrentChannelState();
    }

    // -----------------------------------------------------------------------
    // COLOR QUANTIZATION
    // -----------------------------------------------------------------------

    public void QuantizeColors(int colorCount) {
        if (!HasImage) return;

        Bitmap bmp = _model.OriginalImage!.ToBitmap();
        var srcMat = BitmapConverter.ToMat(bmp);
        var resultMat = _quantizer.Quantize(srcMat, colorCount);
        Bitmap resultBmp = BitmapConverter.ToBitmap(resultMat);

        _model.EditedImage = Emgu.CV.BitmapExtension.ToMat(resultBmp);
    }


    private void ApplyCurrentChannelState() {
        if (_model.OriginalImage == null || _activeColorSpace == null) return;

        // 1. Get a fresh conversion from the original
        Mat converted = _converterService.Convert(_model.OriginalImage, _activeColorSpace.Value);

        // 2. Apply channel adjustments
        Mat adjusted = _compoundsService.ApplyChannelAdjustments(
            converted,
            _activeColorSpace.Value,
            _channelEnabled,
            _channelScales);

        converted.Dispose();
        _model.EditedImage = adjusted;
    }

    private void ResetChannelState(int channelCount) {
        _channelEnabled = Enumerable.Repeat(true, channelCount).ToArray();
        _channelScales = Enumerable.Repeat(1.0, channelCount).ToArray();
    }
}