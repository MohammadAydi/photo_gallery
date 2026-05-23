using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace photo_gallery
{
       public class ColorQuantizer
    {
        private readonly int _maxIterations = 100;
        private readonly double _convergenceThreshold = 0.5;
        private readonly Random _random = new(42);

        public QuantizationResult Quantize(Bitmap sourceBitmap, int colorCount)
        {
            if (sourceBitmap == null) throw new ArgumentNullException(nameof(sourceBitmap));

            var pixels = ExtractPixels(sourceBitmap);
            var sampledPixels = SamplePixels(pixels, sampleRate: 0.15);
            var centroids = RunKMeans(sampledPixels, colorCount);
            var resultPixels = MapPixelsToCentroids(pixels, centroids);
            var resultBitmap = BuildBitmap(resultPixels, sourceBitmap.Width, sourceBitmap.Height);

            return new QuantizationResult
            {
                ResultBitmap = resultBitmap,
                Palette = centroids.Select(c => Color.FromArgb(
                    Clamp(c[0]), Clamp(c[1]), Clamp(c[2]))).ToArray(),
                ColorCount = colorCount
            };
        }

        private double[][] RunKMeans(double[][] pixels, int k)
        {
            var centroids = InitializeCentroidsKMeansPlusPlus(pixels, k);

            for (int iter = 0; iter < _maxIterations; iter++)
            {
                var assignments = AssignPixelsToCentroids(pixels, centroids);

                var newCentroids = ComputeNewCentroids(pixels, assignments, k);

                double shift = Enumerable.Range(0, k).Sum(i => Math.Sqrt(SquaredDist(centroids[i], newCentroids[i])));
                centroids = newCentroids;

                if (shift < _convergenceThreshold)
                    break;
            }

            return centroids;
        }

        private double[][] InitializeCentroidsKMeansPlusPlus(double[][] pixels, int k)
        {
            var centroids = new List<double[]>();
            var distances = new double[pixels.Length];

            int firstIdx = _random.Next(pixels.Length);
            centroids.Add(pixels[firstIdx]);
 
            for (int i = 0; i < pixels.Length; i++)
                distances[i] = SquaredDist(pixels[i], centroids[0]);

            for (int c = 1; c < k; c++)
            {
                double total = distances.Sum();
                double threshold = _random.NextDouble() * total;
                double cumulative = 0;
                int chosen = pixels.Length - 1;

                for (int i = 0; i < pixels.Length; i++)
                {
                    cumulative += distances[i];
                    if (cumulative >= threshold) { chosen = i; break; }
                }
                centroids.Add(pixels[chosen]);
 
                for (int i = 0; i < pixels.Length; i++)
                    distances[i] = Math.Min(distances[i], SquaredDist(pixels[i], centroids[c]));
            }

            return centroids.ToArray();
        }

        private int[] AssignPixelsToCentroids(double[][] pixels, double[][] centroids)
        {
            var assignments = new int[pixels.Length];

            Parallel.For(0, pixels.Length, i =>
            {
                double minDist = double.MaxValue;
                int best = 0;

                for (int c = 0; c < centroids.Length; c++)
                {
                    double dist = SquaredDist(pixels[i], centroids[c]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        best = c;
                    }
                }

                assignments[i] = best;
            });
            return assignments;
        }

        private double[][] ComputeNewCentroids(double[][] pixels, int[] assignments, int k)
        {
            var sums = new double[k][];
            var counts = new int[k];

            for (int c = 0; c < k; c++)
                sums[c] = new double[3];

            for (int i = 0; i < pixels.Length; i++)
            {
                int c = assignments[i];
                sums[c][0] += pixels[i][0];
                sums[c][1] += pixels[i][1];
                sums[c][2] += pixels[i][2];
                counts[c]++;
            }

            var centroids = new double[k][];
            for (int c = 0; c < k; c++)
            {
                if (counts[c] == 0)
                {
                    centroids[c] = pixels[_random.Next(pixels.Length)];
                }
                else
                {
                    centroids[c] =
                    [
                        sums[c][0] / counts[c],
                        sums[c][1] / counts[c],
                        sums[c][2] / counts[c]
                    ];
                }
            }

            return centroids;
        }

        private double[][] ExtractPixels(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            var pixels = new double[width * height][];

            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            int bytesPerPixel = 4;
            int stride = bitmapData.Stride;
            byte[] buffer = new byte[stride * height];
            Marshal.Copy(bitmapData.Scan0, buffer, 0, buffer.Length);
            bitmap.UnlockBits(bitmapData);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = y * stride + x * bytesPerPixel;
                    pixels[y * width + x] =
                    [
                        buffer[idx + 2],
                        buffer[idx + 1],
                        buffer[idx + 0]
                    ];
                }
            }

            return pixels;
        }

        private double[][] SamplePixels(double[][] pixels, double sampleRate)
        {
            int sampleSize = Math.Max(1, (int)(pixels.Length * sampleRate));
            var result = new double[sampleSize][];
     
            var chosenIndices = new HashSet<int>(sampleSize);

            for (int i = 0; i < sampleSize; i++)
            {
                int idx; 
                do
                {
                    idx = _random.Next(pixels.Length);
                } 
                while (!chosenIndices.Add(idx)); 

                result[i] = pixels[idx];
            }

            return result;
        }

        private Color[] MapPixelsToCentroids(double[][] pixels, double[][] centroids)
        {
            var result = new Color[pixels.Length];

            Parallel.For(0, pixels.Length, i =>
            {
                double minDist = double.MaxValue;
                int best = 0;

                for (int c = 0; c < centroids.Length; c++)
                {
                    double dist = SquaredDist(pixels[i], centroids[c]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        best = c;
                    }
                }

                result[i] = Color.FromArgb(Clamp(centroids[best][0]), Clamp(centroids[best][1]),
                    Clamp(centroids[best][2]));
            });

            return result;
        }

        private Bitmap BuildBitmap(Color[] pixels, int width, int height)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            int stride = bitmapData.Stride;
            byte[] buffer = new byte[stride * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var c = pixels[y * width + x];
                    int idx = y * stride + x * 4;
                    buffer[idx + 3] = 255;
                    buffer[idx + 2] = c.R;
                    buffer[idx + 1] = c.G;
                    buffer[idx + 0] = c.B;
                }
            }

            Marshal.Copy(buffer, 0, bitmapData.Scan0, buffer.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        private static double SquaredDist(double[] a, double[] b)
            => (a[0] - b[0]) * (a[0] - b[0]) + (a[1] - b[1]) * (a[1] - b[1]) + (a[2] - b[2]) * (a[2] - b[2]);

        private static int Clamp(double value)
            => Math.Max(0, Math.Min(255, (int)Math.Round(value)));
    }

    public class QuantizationResult
    {
        public required Bitmap ResultBitmap { get; init; }
        public required Color[] Palette { get; init; }
        public int ColorCount { get; init; }
    }
}