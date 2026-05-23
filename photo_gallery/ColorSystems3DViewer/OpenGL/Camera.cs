using OpenTK.Mathematics;

namespace PixelLab;

public class Camera
{
    private const float ZoomMin = -15f;
    private const float ZoomMax = -2f;
    public float RotX { get; set; } = 20f;
    public float RotY { get; set; } = -30f;
    public float Zoom { get; set; } = -5f;

    public void Pan(float dx, float dy)
    {
        RotY += dx * 0.5f;
        RotX += dy * 0.5f;
    }

    public void Scroll(int delta)
    {
        Zoom = Math.Clamp(Zoom + delta * 0.002f, ZoomMin, ZoomMax);
    }

    public Matrix4 GetMVP(float aspect)
    {
        var model =
            Matrix4.CreateRotationX(MathHelper.DegreesToRadians(RotX)) *
            Matrix4.CreateRotationY(MathHelper.DegreesToRadians(RotY));

        var view = Matrix4.CreateTranslation(0, 0, Zoom);

        var proj = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(60f),
            aspect,
            0.1f,
            100f);

        return model * view * proj;
    }
}