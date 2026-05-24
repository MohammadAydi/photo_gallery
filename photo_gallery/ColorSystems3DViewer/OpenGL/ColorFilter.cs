namespace PixelLab;

public abstract class ColorFilter
{
    protected ColorFilter(int channelCount)
    {
        Ranges = new (float, float)[channelCount];
        for (var i = 0; i < channelCount; i++)
            Ranges[i] = (0f, 1f);
    }

    public float Peel { get; set; } = 1f;
    public bool SubtractInner { get; set; } = false;
    public float[]? PointChannels { get; set; } = null;

    public abstract string[] ChannelNames { get; }
    public (float Min, float Max)[] Ranges { get; }
    
    public void PeelToPoint()
    {
        if (PointChannels == null) return;
        Peel = PointChannels[0];
    }
}