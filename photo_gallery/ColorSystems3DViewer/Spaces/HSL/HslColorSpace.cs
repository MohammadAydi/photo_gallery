using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PixelLab;

public class HslColorSpace : BaseColorSpace
{
    
  
    public override string Name => "HSL Bicone";

    public override ColorFilter CreateDefaultFilter()
    {
        return new HslFilter();
    }


    protected override void BuildPointCloud()
    {
        var data = new List<float>();

        for (int r = 0; r <= 255; r += Step)
        for (int g = 0; g <= 255; g += Step)
        for (int b = 0; b <= 255; b += Step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;
            var (h, s, l) = ColorMath.RgbToHsl(rn, gn, bn);

            float rad    = h * MathF.PI / 180f;
            float radius = s * (1f - MathF.Abs(2f * l - 1f));
            float x = MathF.Cos(rad) * radius;
            float y = l * 2f - 1f;
            float z = MathF.Sin(rad) * radius;

            data.Add(x); data.Add(y); data.Add(z);
            data.Add(rn); data.Add(gn); data.Add(bn);
            data.Add(h / 360f); data.Add(s); data.Add(l);
        }

        (VaoPoints, VboPoints) = UploadPointCloud(data.ToArray());
        PointCount = data.Count / 9;
    }


    public override (float r, float g, float b) ChannelsToRgb(float[] ch) =>
        ColorMath.HslToRgb(ch[0] * 360f, ch[1], ch[2]);

    protected override void BuildOutline()
    {
        var bicone = ConeOutlineBuilder.BuildHslBicone();
        var axes = AxisBuilder.BuildPolarAxes(2f);
        float[] combined = [..bicone, ..axes];

        (VaoOutline, VboOutline) = UploadStatic(combined);
        OutlineVertCount = combined.Length / 6;
    }




    protected override (float x, float y, float z) ChannelsToPosition(float[] ch)
    {
        var rad = ch[0] * 2f * MathF.PI;
        var radius = ch[1] * (1f - MathF.Abs(2f * ch[2] - 1f));
        return (MathF.Cos(rad) * radius, ch[2] * 2f - 1f, MathF.Sin(rad) * radius);
    }

   
}