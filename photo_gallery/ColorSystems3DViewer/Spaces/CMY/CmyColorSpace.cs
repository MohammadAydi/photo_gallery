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

        for (int r = 0; r <= 255; r += Step)
        for (int g = 0; g <= 255; g += Step)
        for (int b = 0; b <= 255; b += Step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;
            float c = 1f - rn, m = 1f - gn, y = 1f - bn;

            
            data.Add(c * 2f - 1f);
            data.Add(m * 2f - 1f);
            data.Add(y * 2f - 1f);
            
            data.Add(rn); data.Add(gn); data.Add(bn);
           
            data.Add(c); data.Add(m); data.Add(y);
        }

        (VaoPoints, VboPoints) = UploadPointCloud(data.ToArray());
        PointCount = data.Count / 9;
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
    
    
    public override float[] GetCubeCornerColor(float x, float y, float z)
    {
        var c = (x + 1f) / 2f;
        var m = (y + 1f) / 2f;
        var yv = (z + 1f) / 2f;

      
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
    
}