using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace photo_gallery;

public class ImageCompoundsService {
    public Mat ApplyChannelAdjustments(
        Mat source,
        ColorSpaceType colorSpace,
        bool[] channelEnabled,
        double[] channelScales) {
        if (source == null || source.IsEmpty)
            throw new ArgumentNullException(nameof(source));

        int expectedChannels = colorSpace == ColorSpaceType.CMYK ? 4 : 3;
        if (channelEnabled.Length != expectedChannels || channelScales.Length != expectedChannels)
            throw new ArgumentException(
                $"Expected {expectedChannels} channel entries for {colorSpace}.");

        DepthType sourceDepth = source.Depth; // preserve original depth (Cv32F for CMYK, Cv8U for RGB/HSV)

        Mat[] channels = SplitChannels(source);

        for (int i = 0; i < channels.Length; i++) {
            if (!channelEnabled[i]) {
                channels[i].SetTo(new MCvScalar(0));
            }
            else {
                double scale = channelScales[i];
                if (Math.Abs(scale - 1.0) > 1e-6) {
                    Mat scaled = new Mat();
                    channels[i].ConvertTo(scaled, sourceDepth, scale); // ← match source depth
                    channels[i].Dispose();
                    channels[i] = scaled;
                }
            }
        }

        Mat result = MergeChannels(channels);
        foreach (var ch in channels) ch.Dispose();
        return result;
    }


    private static Mat[] SplitChannels(Mat src) {
        using var vec = new Emgu.CV.Util.VectorOfMat();
        CvInvoke.Split(src, vec);

        Mat[] result = new Mat[vec.Size];
        for (int i = 0; i < vec.Size; i++)
            result[i] = vec[i].Clone();

        return result;
    }

    private static Mat MergeChannels(Mat[] channels) {
        using var vec = new Emgu.CV.Util.VectorOfMat(channels);
        Mat dst = new Mat();
        CvInvoke.Merge(vec, dst);
        return dst;
    }
}