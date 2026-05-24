using Emgu.CV;
using OpenCvSharp.Extensions;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace photo_gallery;

public class ImageController
{
    private readonly ImageModel _model;
    private readonly ColorSpaceConverterService _imageService;

    // [تعديل #1] إضافة الخدمة المسؤولة عن تحويل الأنظمة اللونية
    private readonly ColorSpaceConverterService _converterService;

    // [تعديل #2] حفظ نتيجة آخر تحويل لوني (تحتوي على الصورة المحوّلة + أسماء القنوات)
    private ColorConversionResult? _lastConversion;

    public ImageController()
    {
        _model = new ImageModel();
        _imageService = new ColorSpaceConverterService();

        // [تعديل #3] تهيئة الخدمة عند إنشاء الـ Controller
        _converterService = new ColorSpaceConverterService();
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
    public string ImageName        => _model.GetImageName();
    public double FileSizeKB       => _model.GetFileSizeKB();
    public double FileSizeMB       => _model.GetFileSizeMB();
    public int    Width            => _model.GetWidth();
    public int    Height           => _model.GetHeight();
    public string Extension        => _model.GetExtension();
    public int    NumberOfChannels => _model.GetNumberOfChannels();
    public float  DpiX             => _model.GetDpiX();
    public float  DpiY             => _model.GetDpiY();
    public double AspectRatio      => _model.GetAspectRatio();
    public string ResolutionText   => _model.GetResolutionText();
    public string ChannelType      => _model.GetChannelType();

    // [تعديل #4] خاصية جديدة: تُرجع أسماء قنوات النظام اللوني الحالي
    // مثال: بعد HSV تُرجع ["Hue", "Saturation", "Value"]
    public string[] CurrentChannelNames =>
        _lastConversion?.ChannelNames ?? new[] { "R", "G", "B" };

    // ---------------------------
    // LOAD IMAGE
    // ---------------------------
    public void LoadImage(string path)
    {
        _model.FilePath = path;
        _model.OriginalImage = CvInvoke.Imread(path);
        _model.EditedImage = _model.OriginalImage.Clone();

        // [تعديل #5] مسح آخر تحويل عند تحميل صورة جديدة
        _lastConversion = null;
    }

    // ---------------------------
    // RESET IMAGE
    // ---------------------------
    public void ResetImage()
    {
        _model.EditedImage = _model.OriginalImage?.Clone();

        // [تعديل #6] مسح آخر تحويل عند إعادة التعيين
        _lastConversion = null;
    }

    // ---------------------------
    // SAVE IMAGE
    // ---------------------------
    public void SaveImage(string path)
    {
        if (_model.EditedImage != null)
            CvInvoke.Imwrite(path, _model.EditedImage);
    }

    // ---------------------------
    // [تعديل #7] دالة جديدة: تحويل الصورة إلى نظام لوني مختار
    // - تأخذ النظام المطلوب (HSV, LAB, YUV ...)
    // - تستخدم ColorSpaceConverterService لإجراء التحويل
    // - تحفظ النتيجة في _lastConversion
    // - تضع الصورة المحوّلة في EditedImage لعرضها
    // - تُرجع أسماء القنوات لتحديث labels الـ sliders في الـ UI
    // ---------------------------
    public string[] ConvertColorSpace(ColorSpaceType target)
    {
        if (_model.OriginalImage == null)
            throw new InvalidOperationException("No image loaded.");

        _lastConversion = _converterService.Convert(_model.OriginalImage, target);
        _model.EditedImage = _lastConversion.Image.Clone();
        return _lastConversion.ChannelNames;
    }

    // ---------------------------
    // [تعديل #8] دالة جديدة: إخفاء/إظهار قنوات (الـ checkboxes في الـ UI)
    // - تأخذ مصفوفة bool بطول عدد القنوات
    // - القنوات التي قيمتها false تُصفَّر (تُخفى)
    // - تعمل على _lastConversion (الصورة المحوّلة) وليس الأصلية
    // ---------------------------
    public void ApplyChannelFilter(bool[] channelMask)
    {
        if (_lastConversion == null || _lastConversion.Image.IsEmpty)
            return;

        Mat source = _lastConversion.Image;
        int numChannels = source.NumberOfChannels;

        if (channelMask.Length != numChannels)
            throw new ArgumentException(
                $"channelMask length ({channelMask.Length}) does not match channel count ({numChannels}).");

        using VectorOfMat splitChannels = new VectorOfMat();
        CvInvoke.Split(source, splitChannels);

        for (int i = 0; i < numChannels; i++)
        {
            if (!channelMask[i])
                splitChannels[i].SetTo(new Emgu.CV.Structure.MCvScalar(0));
        }

        Mat filtered = new Mat();
        CvInvoke.Merge(splitChannels, filtered);
        _model.EditedImage = filtered;
    }
    
    private readonly ColorQuantizer _quantizer = new();
    private Bitmap? _quantizedBitmap;

    /*public void QuantizeColors(int colorCount)
    {
        if (!HasImage) return;
        var result = _quantizer.Quantize(_model.OriginalImage.ToBitmap(), colorCount);
        _quantizedBitmap = result.ResultBitmap;
        _model.EditedImage = result.ResultBitmap.ToMat();
    }*/
    
public void QuantizeColors(int colorCount)
    {
        if (!HasImage) return;

        Bitmap bmp = _model.OriginalImage.ToBitmap();

        OpenCvSharp.Mat srcMat = BitmapConverter.ToMat(bmp);

        OpenCvSharp.Mat resultMat = _quantizer.Quantize(srcMat, colorCount);

        Bitmap resultBmp = BitmapConverter.ToBitmap(resultMat);

        _model.EditedImage = Emgu.CV.BitmapExtension.ToMat(resultBmp);
    }
}
