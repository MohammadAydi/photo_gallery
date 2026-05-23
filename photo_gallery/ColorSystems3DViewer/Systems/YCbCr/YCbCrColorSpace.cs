using OpenTK.Mathematics;

namespace PixelLab;

public class YCbCrColorSpace : BaseColorSpace
{
    public override string Name => "YCbCr";

    public override ColorFilter CreateDefaultFilter()
    {
        return new YCbCrFilter();
    }
// ch = [Y, Cb, Cr]
    public override (float r, float g, float b) ChannelsToRgb(float[] ch) =>
        ColorMath.YCbCrToRgb(ch[0], ch[1], ch[2]);
    protected override void BuildPointCloud()
    {
        var data = new List<float>();
        var step = Step;

        for (var r = 0; r <= 255; r += step)
        for (var g = 0; g <= 255; g += step)
        for (var b = 0; b <= 255; b += step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;
            var (Y, Cb, Cr) = ColorMath.RgbToYCbCr(rn, gn, bn);

            // Y: 0-1 → -1..1, Cb/Cr: -0.5..0.5 → -1..1
            var x = Cb * 2f;
            var y = Y * 2f - 1f;
            var z = Cr * 2f;

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
        var box = BuildBoxOutline();

        var axes = AxisBuilder.BuildCartesianAxes(
            (1f, 0f, 0f), (1f, 0.5f, 0f), // Cb → orange
            (0f, 1f, 0f), (0.8f, 0.8f, 0.8f), // Y  → gray
            (0f, 0f, 1f), (0.2f, 0.2f, 1f), // Cr → blue
            0f, 0f, 0f);

        float[] combined = [..box, ..axes];
        (VaoOutline, VboOutline) = UploadStatic(combined);
        OutlineVertCount = combined.Length / 6;
    }

    private float[] BuildBoxOutline()
    {
        var v = new List<float>();
        float[] c =
        [
            -1, -1, -1, 1, -1, -1, 1, 1, -1, -1, 1, -1,
            -1, -1, 1, 1, -1, 1, 1, 1, 1, -1, 1, 1
        ];
        int[] lines = [0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7];

        foreach (var idx in lines.Chunk(2))
        {
            int a = idx[0] * 3, b = idx[1] * 3;
            v.AddRange([c[a], c[a + 1], c[a + 2], 0.7f, 0.7f, 0.7f]);
            v.AddRange([c[b], c[b + 1], c[b + 2], 0.7f, 0.7f, 0.7f]);
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
        return (ch[1] * 2f, ch[0] * 2f - 1f, ch[2] * 2f);
        // ch = [Y, Cb, Cr]
    }
}