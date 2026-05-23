using OpenTK.Mathematics;

namespace PixelLab;

public interface IColorSpace
{
    string Name { get; }
    ColorFilter CreateDefaultFilter();
    void Init();
    void Rebuild(int step);
    void Draw(ColorFilter filter, Matrix4 mvp);
    (float r, float g, float b) ChannelsToRgb(float[] channels);

    void Dispose();
}