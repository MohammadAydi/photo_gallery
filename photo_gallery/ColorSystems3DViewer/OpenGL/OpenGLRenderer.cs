using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;

namespace PixelLab;

public class OpenGLRenderer : IDisposable
{
    private readonly GLControl _control;

    public OpenGLRenderer(GLControl control)
    {
        _control = control;
    }

    public Camera Camera { get; } = new();

    public float Aspect => (float)_control.Width / Math.Max(_control.Height, 1);
    public int Width => _control.Width;
    public int Height => _control.Height;

    public void Dispose()
    {
    }

    public void Setup()
    {
        _control.MakeCurrent();
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.ProgramPointSize);
    }

    public void BeginFrame()
    {
        _control.MakeCurrent();
        GL.Viewport(0, 0, _control.Width, _control.Height);
        GL.ClearColor(0.20f, 0.22f, 0.26f, 1f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void EndFrame()
    {
        _control.SwapBuffers();
    }
}