using OpenTK.Mathematics;

namespace PixelLab;

public class LabColorSpace : BaseColorSpace
{
    public override string Name => "L*a*b";

    public override ColorFilter CreateDefaultFilter()
    {
        return new LabFilter();
    }
// ch = [L 0-100, a -128-127, b -128-127]
    public override (float r, float g, float b) ChannelsToRgb(float[] ch) =>
        ColorMath.LabToRgb(ch[0], ch[1], ch[2]);


    protected override void BuildPointCloud()
    {
        var data = new List<float>();
        var step = Step;

        for (var r = 0; r <= 255; r += step)
        for (var g = 0; g <= 255; g += step)
        for (var b = 0; b <= 255; b += step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;
            var (L, A, B2) = ColorMath.RgbToLab(rn, gn, bn);

            var x = A / 128f;
            var y = L / 50f - 1f;
            var z = B2 / 128f;

            data.Add(x);
            data.Add(y);
            data.Add(z);
            data.Add(rn);
            data.Add(gn);
            data.Add(bn);
        }

        (VaoPoints, VboPoints) = UploadStatic(data.ToArray());
        PointCount = data.Count / 6;
    }

    protected override void BuildOutline()
    {
        var sphere = BuildSphereOutline(64);

        var axes = AxisBuilder.BuildCartesianAxes(
            (1f, 0f, 0f), (1f, 0.2f, 0.2f), // +a → red
            (0f, 1f, 0f), (1f, 1f, 1f), // +L → white
            (0f, 0f, 1f), (1f, 1f, 0.2f), // +b → yellow
            0f, 0f, 0f); // origin at center

        float[] combined = [..sphere, ..axes];
        (VaoOutline, VboOutline) = UploadStatic(combined);
        OutlineVertCount = combined.Length / 6;
    }

    private float[] BuildSphereOutline(int seg)
    {
        var v = new List<float>();

        for (var i = 0; i < seg; i++)
        {
            var a0 = 2f * MathF.PI * i / seg;
            var a1 = 2f * MathF.PI * (i + 1) / seg;

            // XY plane
            v.AddRange([MathF.Cos(a0), MathF.Sin(a0), 0f, 0.7f, 0.7f, 0.7f]);
            v.AddRange([MathF.Cos(a1), MathF.Sin(a1), 0f, 0.7f, 0.7f, 0.7f]);

            // XZ plane
            v.AddRange([MathF.Cos(a0), 0f, MathF.Sin(a0), 0.7f, 0.7f, 0.7f]);
            v.AddRange([MathF.Cos(a1), 0f, MathF.Sin(a1), 0.7f, 0.7f, 0.7f]);

            // YZ plane
            v.AddRange([0f, MathF.Cos(a0), MathF.Sin(a0), 0.7f, 0.7f, 0.7f]);
            v.AddRange([0f, MathF.Cos(a1), MathF.Sin(a1), 0.7f, 0.7f, 0.7f]);
        }

        return v.ToArray();
    }

    protected override void SetFilterUniforms(ColorFilter filter)
    {
        ShaderPoints.SetVec3("uRangeMin",
            new Vector3(filter.Ranges[0].Min, filter.Ranges[1].Min, filter.Ranges[2].Min));
        ShaderPoints.SetVec3("uRangeMax",
            new Vector3(filter.Ranges[0].Max, filter.Ranges[1].Max, filter.Ranges[2].Max));
        ShaderPoints.SetFloat("uPeel", filter.Peel);
        ShaderPoints.SetBool("uSubtract", filter.SubtractInner);
        ShaderPoints.SetBool("uNoFilter", false);
    }

    protected override (float x, float y, float z) ChannelsToPosition(float[] ch)
    {
        return (ch[1] / 128f, ch[0] / 50f - 1f, ch[2] / 128f);
        // ch = [L, a, b]
    }
}