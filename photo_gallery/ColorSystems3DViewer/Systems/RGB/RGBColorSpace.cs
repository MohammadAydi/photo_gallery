using OpenTK.Mathematics;

namespace PixelLab;

public class RgbColorSpace : BaseColorSpace
{
    public override string Name => "RGB Cube";

    public override ColorFilter CreateDefaultFilter()
    {
        return new RgbFilter();
    }
    public override (float r, float g, float b) ChannelsToRgb(float[] ch) =>
        (ch[0], ch[1], ch[2]);
    protected override void BuildPointCloud()
    {
        var data = new List<float>();
        var step = Step;

        for (var r = 0; r <= 255; r += step)
        for (var g = 0; g <= 255; g += step)
        for (var b = 0; b <= 255; b += step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;
            data.Add(rn * 2f - 1f);
            data.Add(gn * 2f - 1f);
            data.Add(bn * 2f - 1f);
            data.Add(rn);
            data.Add(gn);
            data.Add(bn);
        }

        (VaoPoints, VboPoints) = UploadStatic(data.ToArray());
        PointCount = data.Count / 6;
    }

    protected override void BuildOutline()
    {
        var axes = AxisBuilder.BuildCartesianAxes(
            (10f, -1f, -1f), (1f, 0.2f, 0.2f), // R → red
            (-1f, 10f, -1f), (0.2f, 1f, 0.2f), // G → green
            (-1f, -1f, 10f), (0.2f, 0.2f, 1f));

        var cube = BuildCubeEdges();

        float[] combined = [..axes, ..cube];
        (VaoOutline, VboOutline) = UploadStatic(combined);
        OutlineVertCount = combined.Length / 6;
    }

    private float[] BuildCubeEdges()
    {
        var v = new List<float>();
        float[] corners =
        [
            -1, -1, -1, 1, -1, -1, 1, 1, -1, -1, 1, -1,
            -1, -1, 1, 1, -1, 1, 1, 1, 1, -1, 1, 1
        ];
        int[] lines = [0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7];

        foreach (var idx in lines.Chunk(2))
        {
            int a = idx[0] * 3, b = idx[1] * 3;
            var ca = GetCubeCornerColor(corners[a], corners[a + 1], corners[a + 2]);
            var cb = GetCubeCornerColor(corners[b], corners[b + 1], corners[b + 2]);
            v.AddRange([corners[a], corners[a + 1], corners[a + 2], ca[0], ca[1], ca[2]]);
            v.AddRange([corners[b], corners[b + 1], corners[b + 2], cb[0], cb[1], cb[2]]);
        }

        return v.ToArray();
    }

    private float[] GetCubeCornerColor(float x, float y, float z)
    {
        return [(x + 1f) / 2f, (y + 1f) / 2f, (z + 1f) / 2f];
    }

    protected override (float x, float y, float z) ChannelsToPosition(float[] ch)
    {
        return (ch[0] * 2f - 1f, ch[1] * 2f - 1f, ch[2] * 2f - 1f);
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
}