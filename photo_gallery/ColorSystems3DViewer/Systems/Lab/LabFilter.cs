namespace PixelLab;

public class LabFilter : ColorFilter
{
    public LabFilter() : base(3)
    {
    }

    public override string[] ChannelNames => ["L", "a", "b"];
}