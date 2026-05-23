namespace PixelLab;

public static class AxisBuilder
{
    public static float[] BuildCartesianAxes(
        (float x, float y, float z) xEnd, (float r, float g, float b) xColor,
        (float x, float y, float z) yEnd, (float r, float g, float b) yColor,
        (float x, float y, float z) zEnd, (float r, float g, float b) zColor,
        float ox = -1f, float oy = -1f, float oz = -1f)
    {
        return
        [
            ox, oy, oz, xColor.r, xColor.g, xColor.b,
            xEnd.x, xEnd.y, xEnd.z, xColor.r, xColor.g, xColor.b,

            ox, oy, oz, yColor.r, yColor.g, yColor.b,
            yEnd.x, yEnd.y, yEnd.z, yColor.r, yColor.g, yColor.b,

            ox, oy, oz, zColor.r, zColor.g, zColor.b,
            zEnd.x, zEnd.y, zEnd.z, zColor.r, zColor.g, zColor.b
        ];
    }


    public static float[] BuildPolarAxes(float radius = 1f)
    {
        var h0x = radius;
        var h0z = 0f;


        return
        [
            0f, -1f, 0f, 0.3f, 0.3f, 0.3f,
            0f, 1f, 0f, 1.0f, 1.0f, 1.0f,


            0f, 0f, 0f, 1f, 0.2f, 0.2f,
            h0x, 0f, h0z, 1f, 0.2f, 0.2f
        ];
    }
}