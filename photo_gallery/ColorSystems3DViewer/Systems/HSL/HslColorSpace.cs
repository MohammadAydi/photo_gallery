using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PixelLab;

public class HslColorSpace : BaseColorSpace
{
    private HslPoint[] _all;
    private ColorFilter? _lastFilter;
    public override string Name => "HSL Bicone";

    public override ColorFilter CreateDefaultFilter()
    {
        return new HslFilter();
    }


    protected override void BuildPointCloud()
    {
        var list = new List<HslPoint>();
        var step = Step;

        for (var r = 0; r <= 255; r += step)
        for (var g = 0; g <= 255; g += step)
        for (var b = 0; b <= 255; b += step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;
            var (h, s, l) = ColorMath.RgbToHsl(rn, gn, bn);
            list.Add(new HslPoint(h, s, l, rn, gn, bn));
        }

        _all = list.ToArray();
        UploadHslFiltered(null);
    }

    private void UploadHslFiltered(ColorFilter? filter)
    {
        var data = new List<float>();

        foreach (var p in _all)
        {
            if (filter != null)
            {
                var hn = p.H / 360f;
                if (!filter.Passes([hn, p.S, p.L])) continue;
            }

            // bicone: radius = S * (1 - abs(2L-1))
            var rad = p.H * MathF.PI / 180f;
            var radius = p.S * (1f - MathF.Abs(2f * p.L - 1f));
            var x = MathF.Cos(rad) * radius;
            var y = p.L * 2f - 1f;
            var z = MathF.Sin(rad) * radius;

            data.Add(x);
            data.Add(y);
            data.Add(z);
            data.Add(p.R);
            data.Add(p.G);
            data.Add(p.B);
        }

        PointCount = data.Count / 6;
        var arr = data.ToArray();

        if (VaoPoints == 0)
        {
            (VaoPoints, VboPoints) = UploadStatic(arr);
        }
        else
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboPoints);
            GL.BufferData(BufferTarget.ArrayBuffer,
                arr.Length * sizeof(float), arr, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }

    public override (float r, float g, float b) ChannelsToRgb(float[] ch) =>
        ColorMath.HslToRgb(ch[0] * 360f, ch[1], ch[2]);

    protected override void BuildOutline()
    {
        var bicone = ConeOutlineBuilder.BuildHslBicone();
        var axes = AxisBuilder.BuildPolarAxes(5f);
        float[] combined = [..bicone, ..axes];

        (VaoOutline, VboOutline) = UploadStatic(combined);
        OutlineVertCount = combined.Length / 6;
    }

    public override void Draw(ColorFilter filter, Matrix4 mvp)
    {
        if (HasFilterChanged(filter))
        {
            UploadHslFiltered(filter); // or UploadHslFiltered
            CacheFilter(filter);
        }

        base.Draw(filter, mvp);
    }

    protected override void SetFilterUniforms(ColorFilter filter)
    {
        ShaderPoints.SetBool("uNoFilter", true);
        ShaderPoints.SetVec3("uRangeMin", Vector3.Zero);
        ShaderPoints.SetVec3("uRangeMax", Vector3.One);
        ShaderPoints.SetFloat("uPeel", 1f);
        ShaderPoints.SetBool("uSubtract", false);
    }

    protected override (float x, float y, float z) ChannelsToPosition(float[] ch)
    {
        var rad = ch[0] * 2f * MathF.PI;
        var radius = ch[1] * (1f - MathF.Abs(2f * ch[2] - 1f));
        return (MathF.Cos(rad) * radius, ch[2] * 2f - 1f, MathF.Sin(rad) * radius);
    }

    private record struct HslPoint(float H, float S, float L, float R, float G, float B);
}