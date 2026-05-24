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

        for (int r = 0; r <= 255; r += Step)
        for (int g = 0; g <= 255; g += Step)
        for (int b = 0; b <= 255; b += Step)
        {
            float rn = r / 255f, gn = g / 255f, bn = b / 255f;

          
            data.Add(rn * 2f - 1f);
            data.Add(gn * 2f - 1f);
            data.Add(bn * 2f - 1f);
            
            data.Add(rn); data.Add(gn); data.Add(bn);
            
            data.Add(rn); data.Add(gn); data.Add(bn);
        }

        (VaoPoints, VboPoints) = UploadPointCloud(data.ToArray());
        PointCount = data.Count / 9;
    }

    protected override void BuildOutline()
    {
        var axes = AxisBuilder.BuildCartesianAxes(
            (10f, -1f, -1f), (1f, 0.2f, 0.2f),
            (-1f, 10f, -1f), (0.2f, 1f, 0.2f),
            (-1f, -1f, 10f), (0.2f, 0.2f, 1f));

        var cube = BuildCubeEdges();

        float[] combined = [..axes, ..cube];
        (VaoOutline, VboOutline) = UploadStatic(combined);
        OutlineVertCount = combined.Length / 6;
    }

  
    

    protected override (float x, float y, float z) ChannelsToPosition(float[] ch)
    {
        return (ch[0] * 2f - 1f, ch[1] * 2f - 1f, ch[2] * 2f - 1f);
    }

   
}