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

    public bool Passes(ReadOnlySpan<float> normalizedChannels)
    {
        var peelAxis = normalizedChannels[0];
        if (peelAxis > Peel) return false;

        var insideAll = true;
        for (var i = 0; i < Ranges.Length; i++)
        {
            var v = normalizedChannels[i];
            if (v < Ranges[i].Min || v > Ranges[i].Max)
            {
                insideAll = false;
                break;
            }
        }

        return SubtractInner ? !insideAll : insideAll;
    }


    public void PeelToPoint()
    {
        if (PointChannels == null) return;
        Peel = PointChannels[0];
    }
}