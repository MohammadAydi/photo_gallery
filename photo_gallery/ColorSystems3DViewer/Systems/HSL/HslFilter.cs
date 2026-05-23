namespace PixelLab;

public class HslFilter : ColorFilter
{
    public HslFilter() : base(3)
    {
    }

    public override string[] ChannelNames => ["H", "S", "L"];
}