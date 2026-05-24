namespace PixelLab;

public static class ConeOutlineBuilder
{
    public static float[] BuildHsvCone(int segments = 64)
    {
        var verts = new List<float>();
        float r = 1f, top = 1f, bottom = -1f;

        for (var i = 0; i < segments; i++)
        {
            var a0 = 2f * MathF.PI * i / segments;
            var a1 = 2f * MathF.PI * (i + 1) / segments;
            AddLine(verts,
                MathF.Cos(a0) * r, top, MathF.Sin(a0) * r,
                MathF.Cos(a1) * r, top, MathF.Sin(a1) * r);
        }

        for (var i = 0; i < 6; i++)
        {
            var a = 2f * MathF.PI * i / 6f;
            AddLine(verts, 0f, bottom, 0f, MathF.Cos(a) * r, top, MathF.Sin(a) * r);
        }

        
        AddLine(verts, 0f, bottom, 0f, 0f, top, 0f);

        return verts.ToArray();
    }


    public static float[] BuildHslBicone(int segments = 64)
    {
        var verts = new List<float>();
        var r = 1f;

        
        for (var i = 0; i < segments; i++)
        {
            var a0 = 2f * MathF.PI * i / segments;
            var a1 = 2f * MathF.PI * (i + 1) / segments;
            AddLine(verts,
                MathF.Cos(a0) * r, 0f, MathF.Sin(a0) * r,
                MathF.Cos(a1) * r, 0f, MathF.Sin(a1) * r);
        }

       
        for (var i = 0; i < 6; i++)
        {
            var a = 2f * MathF.PI * i / 6f;
            AddLine(verts, 0f, -1f, 0f, MathF.Cos(a) * r, 0f, MathF.Sin(a) * r);
            AddLine(verts, 0f, 1f, 0f, MathF.Cos(a) * r, 0f, MathF.Sin(a) * r);
        }


        AddLine(verts, 0f, -1f, 0f, 0f, 1f, 0f);

        return verts.ToArray();
    }

    private static void AddLine(List<float> v,
        float x0, float y0, float z0,
        float x1, float y1, float z1)
    {
        v.AddRange([x0, y0, z0, 0.8f, 0.8f, 0.8f]);
        v.AddRange([x1, y1, z1, 0.8f, 0.8f, 0.8f]);
    }
}