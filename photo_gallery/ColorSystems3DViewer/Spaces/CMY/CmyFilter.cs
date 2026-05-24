namespace PixelLab;

public class CmyFilter : ColorFilter
{
    public CmyFilter() : base(3)
    {
    }

    public override string[] ChannelNames => ["C", "M", "Y"];
}