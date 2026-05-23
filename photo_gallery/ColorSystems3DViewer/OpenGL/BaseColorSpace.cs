using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PixelLab;

public abstract class BaseColorSpace : IColorSpace
{
    private ColorFilter? _cachedFilter;
    private float _cachedPeel = -1f;
    private (float Min, float Max)[]? _cachedRanges;
    private bool _cachedSubtract;
    protected Shader ShaderOutline;
    protected Shader ShaderPoints;
    protected int Step = 2;
    protected int VaoHighlight, VboHighlight;
    protected int VaoOutline, VboOutline, OutlineVertCount;
    protected int VaoPoints, VboPoints, PointCount;

    public virtual void Rebuild(int step)
    {
        Step = step;

        GL.DeleteVertexArray(VaoPoints);
        GL.DeleteBuffer(VboPoints);
        VaoPoints = VboPoints = 0;

        _cachedRanges = null;
        BuildPointCloud();
    }

    public abstract string Name { get; }
    public abstract ColorFilter CreateDefaultFilter();

    public virtual void Init()
    {
        _cachedRanges = null;
        ShaderPoints = new Shader("ColorSystems3DViewer/Shaders/shader.vert",
            "ColorSystems3DViewer/Shaders/shader.frag");
        ShaderOutline = new Shader("ColorSystems3DViewer/Shaders/shader_axes.vert",
            "ColorSystems3DViewer/Shaders/shader.frag");

        BuildPointCloud();
        BuildOutline();
        BuildHighlightVao();
    }

    public virtual void Draw(ColorFilter filter, Matrix4 mvp)
    {
        DrawOutline(mvp);
        DrawPoints(filter, mvp);
        if (filter.PointChannels != null)
            DrawPoint(filter.PointChannels, mvp);
    }


    public virtual void Dispose()
    {
        GL.DeleteVertexArray(VaoPoints);
        GL.DeleteBuffer(VboPoints);
        GL.DeleteVertexArray(VaoOutline);
        GL.DeleteBuffer(VboOutline);
        GL.DeleteVertexArray(VaoHighlight);
        GL.DeleteBuffer(VboHighlight);
        ShaderPoints?.Dispose();
        ShaderOutline?.Dispose();
    }

    protected bool HasFilterChanged(ColorFilter f)
    {
        if (_cachedRanges == null) return true;
        if (f.Peel != _cachedPeel) return true;
        if (f.SubtractInner != _cachedSubtract) return true;
        for (var i = 0; i < f.Ranges.Length; i++)
            if (f.Ranges[i] != _cachedRanges[i])
                return true;
        return false;
    }

    protected void CacheFilter(ColorFilter f)
    {
        _cachedPeel = f.Peel;
        _cachedSubtract = f.SubtractInner;
        _cachedRanges = f.Ranges.ToArray();
    }


    protected abstract void BuildPointCloud();
    protected abstract void BuildOutline();


    protected (int vao, int vbo) UploadStatic(float[] data)
    {
        var vao = GL.GenVertexArray();
        var vbo = GL.GenBuffer();

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float,
            false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float,
            false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
        return (vao, vbo);
    }

    private void BuildHighlightVao()
    {
        VaoHighlight = GL.GenVertexArray();
        VboHighlight = GL.GenBuffer();

        GL.BindVertexArray(VaoHighlight);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VboHighlight);
        GL.BufferData(BufferTarget.ArrayBuffer,
            6 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float,
            false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float,
            false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
    }


    protected abstract (float x, float y, float z) ChannelsToPosition(float[] ch);
    public abstract (float r, float g, float b) ChannelsToRgb(float[] channels);

    private void DrawOutline(Matrix4 mvp)
    {
        ShaderOutline.Use();
        ShaderOutline.SetMatrix4("uMVP", mvp);
        GL.BindVertexArray(VaoOutline);
        GL.DrawArrays(PrimitiveType.Lines, 0, OutlineVertCount);
        GL.BindVertexArray(0);
    }

    protected virtual void DrawPoints(ColorFilter filter, Matrix4 mvp)
    {
        ShaderPoints.Use();
        ShaderPoints.SetMatrix4("uMVP", mvp);
        SetFilterUniforms(filter);

        GL.BindVertexArray(VaoPoints);
        GL.DrawArrays(PrimitiveType.Points, 0, PointCount);
        GL.BindVertexArray(0);
    }

    protected abstract void SetFilterUniforms(ColorFilter filter);

    public virtual void DrawPoint(float[] channels, Matrix4 mvp)
    {
        var (x, y, z) = ChannelsToPosition(channels);
        float[] data = [x, y, z, 1f, 1f, 1f];

        GL.BindBuffer(BufferTarget.ArrayBuffer, VboHighlight);
        GL.BufferSubData(BufferTarget.ArrayBuffer,
            IntPtr.Zero, data.Length * sizeof(float), data);

        GL.Disable(EnableCap.DepthTest);
        ShaderOutline.Use();
        ShaderOutline.SetMatrix4("uMVP", mvp);
        GL.BindVertexArray(VaoHighlight);
        GL.PointSize(10f);
        GL.DrawArrays(PrimitiveType.Points, 0, 1);
        GL.PointSize(3f);
        GL.BindVertexArray(0);
        GL.Enable(EnableCap.DepthTest);
    }
}