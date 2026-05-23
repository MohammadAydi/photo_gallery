using OpenTK.Mathematics;

namespace PixelLab;

public class CmyColorSpace : BaseColorSpace
{
    public override string Name => "CMY Cube";

    public override ColorFilter CreateDefaultFilter()
    {
        return new CmyFilter();
    }

    protected override void BuildPointCloud()
    {
        var data = new List<float>();
        var step = Step;

        for (var r = 0; r <= 255; r += step)
        for (var g = 0; g <= 255; g += step)
        for (var b = 0; b <= 255; b += step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;
            float c = 1f - rn, m = 1f - gn, y = 1f - bn;

            data.Add(c * 2f - 1f);
            data.Add(m * 2f - 1f);
            data.Add(y * 2f - 1f);
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
            (10f, -1f, -1f), (0f, 1f, 1f),
            (-1f, 10f, -1f), (1f, 0f, 1f),
            (-1f, -1f, 10f), (1f, 1f, 0f));

        var cube = BuildCubeEdges();

        float[] combined = [..axes, ..cube];

        (VaoOutline, VboOutline) = UploadStatic(combined);

        OutlineVertCount = combined.Length / 6;
    }
    public override (float r, float g, float b) ChannelsToRgb(float[] ch) =>
        (1f - ch[0], 1f - ch[1], 1f - ch[2]);
    private float[] BuildCubeEdges()
    {
        var v = new List<float>();

        float[] corners =
        [
            -1, -1, -1,
            1, -1, -1,
            1, 1, -1,
            -1, 1, -1,

            -1, -1, 1,
            1, -1, 1,
            1, 1, 1,
            -1, 1, 1
        ];

        int[] lines =
        [
            0, 1, 1, 2, 2, 3, 3, 0,
            4, 5, 5, 6, 6, 7, 7, 4,
            0, 4, 1, 5, 2, 6, 3, 7
        ];

        foreach (var idx in lines.Chunk(2))
        {
            var a = idx[0] * 3;
            var b = idx[1] * 3;

            var ca = GetCubeCornerColor(
                corners[a],
                corners[a + 1],
                corners[a + 2]);

            var cb = GetCubeCornerColor(
                corners[b],
                corners[b + 1],
                corners[b + 2]);

            v.AddRange([
                corners[a], corners[a + 1], corners[a + 2],
                ca[0], ca[1], ca[2]
            ]);

            v.AddRange([
                corners[b], corners[b + 1], corners[b + 2],
                cb[0], cb[1], cb[2]
            ]);
        }

        return v.ToArray();
    }

    private float[] GetCubeCornerColor(float x, float y, float z)
    {
        var c = (x + 1f) / 2f;
        var m = (y + 1f) / 2f;
        var yv = (z + 1f) / 2f;

        // convert CMY -> RGB for display
        return
        [
            1f - c,
            1f - m,
            1f - yv
        ];
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