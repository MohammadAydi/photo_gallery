using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PixelLab;

public class HsvColorSpace : BaseColorSpace
{
    

    private ColorFilter? _lastFilter;
    public override string Name => "HSV Cone";

    public override ColorFilter CreateDefaultFilter()
    {
        return new HsvFilter();
    }

    protected override void BuildPointCloud()
    {
        var data = new List<float>();

        for (int r = 0; r <= 255; r += Step)
        for (int g = 0; g <= 255; g += Step)
        for (int b = 0; b <= 255; b += Step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;
            var (h, s, v) = ColorMath.RgbToHsv(rn, gn, bn);

            float rad = h * MathF.PI / 180f;
            float x = MathF.Cos(rad) * s * v;
            float y = v * 2f - 1f;
            float z = MathF.Sin(rad) * s * v;

            data.Add(x); data.Add(y); data.Add(z);
            data.Add(rn); data.Add(gn); data.Add(bn);
            data.Add(h / 360f); data.Add(s); data.Add(v);
        }

        (VaoPoints, VboPoints) = UploadPointCloud(data.ToArray());
        PointCount = data.Count / 9;
    }


  
    public override (float r, float g, float b) ChannelsToRgb(float[] ch) =>
        ColorMath.HsvToRgb(ch[0] * 360f, ch[1], ch[2]);
    protected override void BuildOutline()
    {
        var cone = ConeOutlineBuilder.BuildHsvCone();
        var axes = AxisBuilder.BuildPolarAxes();
        float[] combined = [..cone, ..axes];

        (VaoOutline, VboOutline) = UploadStatic(combined);
        OutlineVertCount = combined.Length / 6;
    }

    

    protected override (float x, float y, float z) ChannelsToPosition(float[] ch)
    {
        var rad = ch[0] * 2f * MathF.PI; 
        float s = ch[1], v = ch[2];
        return (MathF.Cos(rad) * s * v, v * 2f - 1f, MathF.Sin(rad) * s * v);
    }
}