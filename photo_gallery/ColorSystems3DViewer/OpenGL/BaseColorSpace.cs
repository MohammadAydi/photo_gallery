using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PixelLab;

public abstract class BaseColorSpace : IColorSpace
{
    
    private float _cachedPeel = -1f;
    
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

      
        BuildPointCloud();
    }

    public abstract string Name { get; }
    public abstract ColorFilter CreateDefaultFilter();

    public virtual void Init()
    {
        
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
    

    protected abstract void BuildPointCloud();
    protected abstract void BuildOutline();


    protected (int vao, int vbo) UploadStatic(float[] data)
    {
        int vao = GL.GenVertexArray();
        int vbo = GL.GenBuffer();

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
        return (vao, vbo);
    }
    
    protected (int vao, int vbo) UploadPointCloud(float[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw)
    {
        int vao = GL.GenVertexArray();
        int vbo = GL.GenBuffer();

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            data.Length * sizeof(float), data, hint);

        int stride = 9 * sizeof(float);

       
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
       
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(0);
        return (vao, vbo);
    }


    protected void BuildHighlightVao()
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

    protected void DrawOutline(Matrix4 mvp)
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

    protected virtual void SetFilterUniforms(ColorFilter filter)
    {
        ShaderPoints.SetVec3("uRangeMin",
            new Vector3(filter.Ranges[0].Min, filter.Ranges[1].Min, filter.Ranges[2].Min));
        ShaderPoints.SetVec3("uRangeMax",
            new Vector3(filter.Ranges[0].Max, filter.Ranges[1].Max, filter.Ranges[2].Max));
        ShaderPoints.SetFloat("uPeel", filter.Peel);
        ShaderPoints.SetBool("uSubtract", filter.SubtractInner);
    }

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

    protected float[] BuildCubeEdges()
    {
        var v = new List<float>();
        float[] corners =
        [
            -1, -1, -1, 1, -1, -1, 1, 1, -1, -1, 1, -1,
            -1, -1, 1, 1, -1, 1, 1, 1, 1, -1, 1, 1
        ];
        int[] lines = [0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7];

        foreach (var idx in lines.Chunk(2))
        {
            int a = idx[0] * 3, b = idx[1] * 3;
            var ca = GetCubeCornerColor(corners[a], corners[a + 1], corners[a + 2]);
            var cb = GetCubeCornerColor(corners[b], corners[b + 1], corners[b + 2]);
            v.AddRange([corners[a], corners[a + 1], corners[a + 2], ca[0], ca[1], ca[2]]);
            v.AddRange([corners[b], corners[b + 1], corners[b + 2], cb[0], cb[1], cb[2]]);
        }

        return v.ToArray();
    }
    
    public virtual float[] GetCubeCornerColor(float x, float y, float z)
    {
        return [(x + 1f) / 2f, (y + 1f) / 2f, (z + 1f) / 2f];
    }
    
}