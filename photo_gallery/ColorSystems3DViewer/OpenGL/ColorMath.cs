using ColorMine.ColorSpaces;

namespace PixelLab;

public static class ColorMath
{
 
    public static (float H, float S, float V) RgbToHsv(float r, float g, float b)
    {
        var hsv = new Rgb { R = r * 255, G = g * 255, B = b * 255 }.To<Hsv>();
        return ((float)hsv.H, (float)hsv.S, (float)hsv.V);
    }

    public static (float H, float S, float L) RgbToHsl(float r, float g, float b)
    {
        var hsl = new Rgb { R = r * 255, G = g * 255, B = b * 255 }.To<Hsl>();
        return ((float)hsl.H, (float)(hsl.S / 100.0), (float)(hsl.L / 100.0));
    }
    
   
    

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

    
   

    public static Dictionary<string, (string[] names, float[] values)> AllFromRgb(
        float r, float g, float b)
    {
        var (h,  s,  v)  = RgbToHsv(r, g, b);
        var (hl, sl, l)  = RgbToHsl(r, g, b);
        
       
        return new()
        {
            ["RGB"]   = (["R",  "G",  "B" ], [r * 255f, g * 255f, b * 255f]),
            ["CMY"]   = (["C",  "M",  "Y" ], [1f-r,     1f-g,     1f-b    ]),
            ["HSV"]   = (["H",  "S",  "V" ], [h,        s,        v       ]),
            ["HSL"]   = (["H",  "S",  "L" ], [hl,       sl,       l       ]),
        };
    }
}