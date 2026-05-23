using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PixelLab;

public class HsvColorSpace : BaseColorSpace
{
    private HsvPoint[] _all;

    private ColorFilter? _lastFilter;
    public override string Name => "HSV Cone";

    public override ColorFilter CreateDefaultFilter()
    {
        return new HsvFilter();
    }

    protected override void BuildPointCloud()
    {
        var list = new List<HsvPoint>();
        var step = Step;

        for (var r = 0; r <= 255; r += step)
        for (var g = 0; g <= 255; g += step)
        for (var b = 0; b <= 255; b += step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;
            var (h, s, v) = ColorMath.RgbToHsv(rn, gn, bn);
            list.Add(new HsvPoint(h, s, v, rn, gn, bn));
        }

        _all = list.ToArray();

        // initial upload with no filter
        UploadHsvFiltered(null);
    }

    private void UploadHsvFiltered(ColorFilter? filter)
    {
        var data = new List<float>();

        foreach (var p in _all)
        {
            if (filter != null)
            {
                var hn = p.H / 360f;
                if (!filter.Passes([hn, p.S, p.V])) continue;
            }

            // polar → cartesian: X=cos(H)*S*V, Z=sin(H)*S*V, Y=V*2-1
            var rad = p.H * MathF.PI / 180f;
            var x = MathF.Cos(rad) * p.S * p.V;
            var y = p.V * 2f - 1f;
            var z = MathF.Sin(rad) * p.S * p.V;

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
            // re-upload to existing VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VboPoints);
            GL.BufferData(BufferTarget.ArrayBuffer,
                arr.Length * sizeof(float), arr, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
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

    public override void Draw(ColorFilter filter, Matrix4 mvp)
    {
        if (HasFilterChanged(filter))
        {
            UploadHsvFiltered(filter); // or UploadHslFiltered
            CacheFilter(filter);
        }

        base.Draw(filter, mvp);
    }

    protected override void SetFilterUniforms(ColorFilter filter)
    {
        // filtering is done on CPU, shader just draws everything
        ShaderPoints.SetBool("uNoFilter", true);
        // dummy values to avoid uninitialized uniform warnings
        ShaderPoints.SetVec3("uRangeMin", Vector3.Zero);
        ShaderPoints.SetVec3("uRangeMax", Vector3.One);
        ShaderPoints.SetFloat("uPeel", 1f);
        ShaderPoints.SetBool("uSubtract", false);
    }

    protected override (float x, float y, float z) ChannelsToPosition(float[] ch)
    {
        var rad = ch[0] * 2f * MathF.PI; // H normalized 0-1 → radians
        float s = ch[1], v = ch[2];
        return (MathF.Cos(rad) * s * v, v * 2f - 1f, MathF.Sin(rad) * s * v);
    }

    // pre-computed full list
    private record struct HsvPoint(float H, float S, float V, float R, float G, float B);
}