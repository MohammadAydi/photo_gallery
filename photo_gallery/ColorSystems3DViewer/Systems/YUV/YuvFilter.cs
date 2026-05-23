namespace PixelLab;

public class YuvFilter : ColorFilter
{
    public YuvFilter() : base(3)
    {
    }

    public override string[] ChannelNames => ["Y", "U", "V"];
}