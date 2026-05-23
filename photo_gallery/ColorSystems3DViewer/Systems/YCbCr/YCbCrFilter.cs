namespace PixelLab;

public class YCbCrFilter : ColorFilter
{
    public YCbCrFilter() : base(3)
    {
    }

    public override string[] ChannelNames => ["Y", "Cb", "Cr"];
}