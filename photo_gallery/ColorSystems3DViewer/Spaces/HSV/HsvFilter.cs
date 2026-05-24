namespace PixelLab;

public class HsvFilter : ColorFilter
{
    public HsvFilter() : base(3)
    {
    }

    public override string[] ChannelNames => ["H", "S", "V"];
}