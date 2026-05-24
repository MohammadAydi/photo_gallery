using OpenTK.Mathematics;

namespace PixelLab;

public class Camera
{
    
    public float RotX { get; set; } = 20f;
    public float RotY { get; set; } = -30f;

    public float Distance { get; set; } = 5f;

    public Vector3 Target { get; set; } = Vector3.Zero;

    private const float MinDistance = 2f;
    private const float MaxDistance = 20f;

    private const float MinPitch = -89f;
    private const float MaxPitch = 89f;

    public void Orbit(float dx, float dy)
    {
        RotY += dx * 0.5f;
        RotX += dy * 0.5f;

        RotX = MathHelper.Clamp(RotX, MinPitch, MaxPitch);
    }

    public void Zoom(float delta)
    {
        Distance = Math.Clamp(Distance - delta * 0.01f, MinDistance, MaxDistance);
    }

    private Vector3 GetPosition()
    {
        float pitch = MathHelper.DegreesToRadians(RotX);
        float yaw = MathHelper.DegreesToRadians(RotY);

        float x = Distance * MathF.Cos(pitch) * MathF.Sin(yaw);
        float y = Distance * MathF.Sin(pitch);
        float z = Distance * MathF.Cos(pitch) * MathF.Cos(yaw);

        return new Vector3(x, y, z) + Target;
    }

    public Matrix4 GetViewMatrix()
    {
        var position = GetPosition();
        return Matrix4.LookAt(position, Target, Vector3.UnitY);
    }

    public Matrix4 GetMVP(float aspect)
    {
        var view = GetViewMatrix();

        var projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(60f),
            aspect,
            0.1f,
            100f);

        var model = Matrix4.Identity;

        return model * view * projection;
    }
}