using ColorMine.ColorSpaces;

namespace PixelLab;

public static class ColorMath
{
    // ── Forward conversions ───────────────────────────────────────

    public static (float H, float S, float V) RgbToHsv(float r, float g, float b)
    {
        var hsb = new Rgb { R = r * 255, G = g * 255, B = b * 255 }.To<Hsb>();
        return ((float)hsb.H, (float)hsb.S, (float)hsb.B);
    }

    public static (float H, float S, float L) RgbToHsl(float r, float g, float b)
    {
        var hsl = new Rgb { R = r * 255, G = g * 255, B = b * 255 }.To<Hsl>();
        return ((float)hsl.H, (float)(hsl.S / 100.0), (float)(hsl.L / 100.0));
    }

    public static (float L, float A, float B) RgbToLab(float r, float g, float b)
    {
        var lab = new Rgb { R = r * 255, G = g * 255, B = b * 255 }.To<Lab>();
        return ((float)lab.L, (float)lab.A, (float)lab.B);
    }

    public static (float Y, float Cb, float Cr) RgbToYCbCr(float r, float g, float b)
    {
        float y  =  0.299f    * r + 0.587f    * g + 0.114f    * b;
        float cb = -0.168736f * r - 0.331264f * g + 0.5f      * b;
        float cr =  0.5f      * r - 0.418688f * g - 0.081312f * b;
        return (y, cb, cr);
    }

    public static (float Y, float U, float V) RgbToYuv(float r, float g, float b)
    {
        float y =  0.299f    * r + 0.587f    * g + 0.114f    * b;
        float u = -0.14713f  * r - 0.28886f  * g + 0.436f    * b;
        float v =  0.615f    * r - 0.51499f  * g - 0.10001f  * b;
        return (y, u, v);
    }

    // ── Inverse conversions ───────────────────────────────────────

    public static (float R, float G, float B) HsvToRgb(float h, float s, float v)
    {
        var rgb = new Hsb { H = h, S = s, B = v }.To<Rgb>();
        return ((float)rgb.R / 255f, (float)rgb.G / 255f, (float)rgb.B / 255f);
    }

    public static (float R, float G, float B) HslToRgb(float h, float s, float l)
    {
        var rgb = new Hsl { H = h, S = s * 100.0, L = l * 100.0 }.To<Rgb>();
        return ((float)rgb.R / 255f, (float)rgb.G / 255f, (float)rgb.B / 255f);
    }

    public static (float R, float G, float B) LabToRgb(float L, float a, float b)
    {
        var rgb = new Lab { L = L, A = a, B = b }.To<Rgb>();
        return (Math.Clamp((float)rgb.R / 255f, 0f, 1f),
                Math.Clamp((float)rgb.G / 255f, 0f, 1f),
                Math.Clamp((float)rgb.B / 255f, 0f, 1f));
    }

    public static (float R, float G, float B) YCbCrToRgb(float y, float cb, float cr)
    {
        float r = y + 1.402f    * cr;
        float g = y - 0.344136f * cb - 0.714136f * cr;
        float b = y + 1.772f    * cb;
        return (Math.Clamp(r, 0f, 1f),
                Math.Clamp(g, 0f, 1f),
                Math.Clamp(b, 0f, 1f));
    }

    public static (float R, float G, float B) YuvToRgb(float y, float u, float v)
    {
        float r = y + 1.13983f  * v;
        float g = y - 0.39465f  * u - 0.58060f * v;
        float b = y + 2.03211f  * u;
        return (Math.Clamp(r, 0f, 1f),
                Math.Clamp(g, 0f, 1f),
                Math.Clamp(b, 0f, 1f));
    }

    // ── All spaces from RGB ───────────────────────────────────────

    public static Dictionary<string, (string[] names, float[] values)> AllFromRgb(
        float r, float g, float b)
    {
        var (h,  s,  v)  = RgbToHsv(r, g, b);
        var (hl, sl, l)  = RgbToHsl(r, g, b);
        var (L,  a,  bL) = RgbToLab(r, g, b);
        var (y,  cb, cr) = RgbToYCbCr(r, g, b);
        var (yu, u,  uv) = RgbToYuv(r, g, b);

        return new()
        {
            ["RGB"]   = (["R",  "G",  "B" ], [r * 255f, g * 255f, b * 255f]),
            ["CMY"]   = (["C",  "M",  "Y" ], [1f-r,     1f-g,     1f-b    ]),
            ["HSV"]   = (["H",  "S",  "V" ], [h,        s,        v       ]),
            ["HSL"]   = (["H",  "S",  "L" ], [hl,       sl,       l       ]),
            ["L*a*b"] = (["L",  "a",  "b" ], [L,        a,        bL      ]),
            ["YCbCr"] = (["Y",  "Cb", "Cr"], [y,        cb,       cr      ]),
            ["YUV"]   = (["Y",  "U",  "V" ], [yu,       u,        uv      ]),
        };
    }
}