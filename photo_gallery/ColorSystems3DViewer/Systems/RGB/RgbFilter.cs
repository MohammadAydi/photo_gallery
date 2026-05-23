namespace PixelLab;

public class RgbFilter : ColorFilter
{
    public RgbFilter() : base(3)
    {
    }

    public override string[] ChannelNames => ["R", "G", "B"];
}