using System.Drawing;
using Emgu.CV;

namespace photo_gallery;

public class ImageModel
{
    public Mat? OriginalImage { get; set; }
    public Mat? EditedImage { get; set; }

    public string? FilePath { get; set; }

    // ---------------------------
    // IMAGE NAME
    // ---------------------------
    public string GetImageName()
    {
        if (string.IsNullOrEmpty(FilePath))
            return "Unknown";

        FileInfo file = new FileInfo(FilePath);

        return file.Name;
    }

    // ---------------------------
    // FILE SIZE (KB)
    // ---------------------------
    public double GetFileSizeKB()
    {
        if (string.IsNullOrEmpty(FilePath))
            return 0;

        FileInfo file = new FileInfo(FilePath);

        return file.Length / 1024.0;
    }

    // ---------------------------
    // FILE SIZE (MB)
    // ---------------------------
    public double GetFileSizeMB()
    {
        if (string.IsNullOrEmpty(FilePath))
            return 0;

        FileInfo file = new FileInfo(FilePath);

        return file.Length / 1024.0 / 1024.0;
    }

    // ---------------------------
    // WIDTH
    // ---------------------------
    public int GetWidth()
    {
        if (EditedImage == null)
            return 0;

        return EditedImage.Width;
    }

    // ---------------------------
    // HEIGHT
    // ---------------------------
    public int GetHeight()
    {
        if (EditedImage == null)
            return 0;

        return EditedImage.Height;
    }

    // ---------------------------
    // EXTENSION
    // ---------------------------
    public string GetExtension()
    {
        if (string.IsNullOrEmpty(FilePath))
            return "Unknown";

        return Path.GetExtension(FilePath);
    }

    // ---------------------------
    // NUMBER OF CHANNELS
    // ---------------------------
    public int GetNumberOfChannels()
    {
        if (EditedImage == null)
            return 0;

        return EditedImage.NumberOfChannels;
    }

    // ---------------------------
    // DPI X
    // ---------------------------
    public float GetDpiX()
    {
        if (EditedImage == null)
            return 0;

        using Bitmap bmp = EditedImage.ToBitmap();

        return bmp.HorizontalResolution;
    }

    // ---------------------------
    // DPI Y
    // ---------------------------
    public float GetDpiY()
    {
        if (EditedImage == null)
            return 0;

        using Bitmap bmp = EditedImage.ToBitmap();

        return bmp.VerticalResolution;
    }

    // ---------------------------
    // ASPECT RATIO
    // ---------------------------
    public double GetAspectRatio()
    {
        if (EditedImage == null || EditedImage.Height == 0)
            return 0;

        return (double)EditedImage.Width / EditedImage.Height;
    }

    // ---------------------------
    // RESOLUTION TEXT
    // ---------------------------
    public string GetResolutionText()
    {
        if (EditedImage == null)
            return "0 x 0";

        return $"{EditedImage.Width} x {EditedImage.Height}";
    }

    // ---------------------------
    // CHANNEL TYPE TEXT
    // ---------------------------
    public string GetChannelType()
    {
        int channels = GetNumberOfChannels();

        return channels switch
        {
            1 => "Grayscale",
            3 => "BGR",
            4 => "BGRA",
            _ => "Unknown"
        };
    }
}