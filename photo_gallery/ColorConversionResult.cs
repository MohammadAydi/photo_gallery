// هذا الملف كان في مشروعك الآخر باسم TESTcolorSystem.Models
// التعديل الوحيد: تغيير الـ namespace إلى photo_gallery
// المحتوى نفسه بالكامل

using Emgu.CV;

namespace photo_gallery;

public class ColorConversionResult
{
    public Mat Image { get; set; }
    public ColorSpaceType ColorSpace { get; set; }
    public string[] ChannelNames { get; set; }

    public ColorConversionResult(Mat image, ColorSpaceType colorSpace, string[] channelNames)
    {
        Image = image;
        ColorSpace = colorSpace;
        ChannelNames = channelNames;
    }
}