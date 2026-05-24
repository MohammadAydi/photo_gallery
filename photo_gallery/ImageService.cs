// هذا الملف كان في مشروعك الآخر باسم TESTcolorSystem.Services
// التعديلات:
// 1. تغيير الـ namespace إلى photo_gallery
// 2. إزالة using TESTcolorSystem.Enums و TESTcolorSystem.Models (لم تعودا ضروريتين)
// 3. إضافة case جديد: HLS  [إضافة]

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace photo_gallery;

public class ColorSpaceConverterService
{
    public Mat Convert(Mat original, ColorSpaceType target)
    {
        if (original == null || original.IsEmpty)
            throw new ArgumentException("Invalid image");

        return target switch
        {
            ColorSpaceType.RGB   => ToRGB(original),
            ColorSpaceType.HSV   => ToHSV(original),
            ColorSpaceType.HLS   => ToHLS(original),   // [إضافة] case جديد للـ HLS
            ColorSpaceType.LAB   => ToLAB(original),
            ColorSpaceType.YUV   => ToYUV(original),
            ColorSpaceType.YCbCr => ToYCbCr(original),
            ColorSpaceType.CMYK  => ToCMYK(original),
            _ => throw new NotSupportedException($"Color space {target} is not supported.")
        };
    }

    private Mat ToRGB(Mat original)
    {
        return original.Clone();
    }

    private Mat ToHSV(Mat original)
    {
        Mat result = new Mat();
        CvInvoke.CvtColor(original, result, ColorConversion.Bgr2Hsv);
        return result;    }

    // [إضافة] دالة ToHLS جديدة — نفس نمط باقي الدوال
    private Mat ToHLS(Mat original)
    {
        Mat result = new Mat();
        CvInvoke.CvtColor(original, result, ColorConversion.Bgr2Hls);
        return result;    }

    private Mat ToLAB(Mat original)
    {
        Mat result = new Mat();
        CvInvoke.CvtColor(original, result, ColorConversion.Bgr2Lab);
        return result;    }

    private Mat ToYUV(Mat original)
    {
        Mat result = new Mat();
        CvInvoke.CvtColor(original, result, ColorConversion.Bgr2Yuv);
        return result;    }

    private Mat ToYCbCr(Mat original)
    {
        Mat result = new Mat();
        CvInvoke.CvtColor(original, result, ColorConversion.Bgr2YCrCb);
        return result;    }

    private Mat ToCMYK(Mat original)
    {
        Mat floatImg = new Mat();
        original.ConvertTo(floatImg, DepthType.Cv32F, 1.0 / 255.0);

        using VectorOfMat channels = new VectorOfMat();
        CvInvoke.Split(floatImg, channels);

        Mat b = channels[0];
        Mat g = channels[1];
        Mat r = channels[2];

        Mat maxRGB = new Mat();
        CvInvoke.Max(r, g, maxRGB);
        CvInvoke.Max(maxRGB, b, maxRGB);

        Mat ones = Mat.Ones(floatImg.Rows, floatImg.Cols, DepthType.Cv32F, 1);

        Mat k = new Mat();
        CvInvoke.Subtract(ones, maxRGB, k);

        Mat denominator = new Mat();
        CvInvoke.Subtract(ones, k, denominator);

        Mat safeDenom = new Mat();
        CvInvoke.Max(denominator, new ScalarArray(new MCvScalar(1e-6)), safeDenom);

        Mat c = new Mat();
        CvInvoke.Subtract(ones, r, c);
        CvInvoke.Subtract(c, k, c);
        CvInvoke.Divide(c, safeDenom, c);

        Mat m = new Mat();
        CvInvoke.Subtract(ones, g, m);
        CvInvoke.Subtract(m, k, m);
        CvInvoke.Divide(m, safeDenom, m);

        Mat yChan = new Mat();
        CvInvoke.Subtract(ones, b, yChan);
        CvInvoke.Subtract(yChan, k, yChan);
        CvInvoke.Divide(yChan, safeDenom, yChan);

        Mat cmykImage = new Mat();
        using VectorOfMat cmykChannels = new VectorOfMat(c, m, yChan, k);
        CvInvoke.Merge(cmykChannels, cmykImage);

        floatImg.Dispose(); maxRGB.Dispose(); ones.Dispose();
        k.Dispose(); denominator.Dispose(); safeDenom.Dispose();
        c.Dispose(); m.Dispose(); yChan.Dispose();

        return cmykImage;
        
    }
}
