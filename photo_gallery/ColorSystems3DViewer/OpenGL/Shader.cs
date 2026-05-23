using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PixelLab;

public class Shader : IDisposable
{
    public readonly int Handle;

    public Shader(string vertPath, string fragPath)
    {
        var vertSrc = File.ReadAllText(vertPath);
        var fragSrc = File.ReadAllText(fragPath);

        var vert = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vert, vertSrc);
        GL.CompileShader(vert);
        CheckShader(vert, vertPath);

        var frag = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(frag, fragSrc);
        GL.CompileShader(frag);
        CheckShader(frag, fragPath);

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vert);
        GL.AttachShader(Handle, frag);
        GL.LinkProgram(Handle);
        CheckProgram(Handle);

        GL.DeleteShader(vert);
        GL.DeleteShader(frag);
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public void SetMatrix4(string name, Matrix4 mat)
    {
        var loc = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(loc, false, ref mat);
    }

    public void SetVec3(string name, Vector3 v)
    {
        var loc = GL.GetUniformLocation(Handle, name);
        GL.Uniform3(loc, v);
    }

    public void SetFloat(string name, float v)
    {
        var loc = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(loc, v);
    }

    public void SetBool(string name, bool v)
    {
        var loc = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(loc, v ? 1 : 0);
    }

    private static void CheckShader(int handle, string name)
    {
        GL.GetShader(handle, ShaderParameter.CompileStatus, out var ok);
        if (ok == 0)
            throw new Exception($"Shader '{name}' error:\n{GL.GetShaderInfoLog(handle)}");
    }

    private static void CheckProgram(int handle)
    {
        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out var ok);
        if (ok == 0)
            throw new Exception($"Program link error:\n{GL.GetProgramInfoLog(handle)}");
    }
}