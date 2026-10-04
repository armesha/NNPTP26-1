namespace NNPTPZ1
{
    internal sealed class FractalSettings
    {
        internal int Width { get; }
        internal int Height { get; }
        internal double XMin { get; }
        internal double XMax { get; }
        internal double YMin { get; }
        internal double YMax { get; }
        internal string OutputPath { get; }

        private FractalSettings(int width, int height, double xmin, double xmax,
            double ymin, double ymax, string outputPath)
        {
            Width = width;
            Height = height;
            XMin = xmin;
            XMax = xmax;
            YMin = ymin;
            YMax = ymax;
            OutputPath = outputPath;
        }

        internal static FractalSettings Parse(string[] args)
        {
            int width = int.Parse(args[0]);
            int height = int.Parse(args[1]);
            double xmin = double.Parse(args[2]);
            double xmax = double.Parse(args[3]);
            double ymin = double.Parse(args[4]);
            double ymax = double.Parse(args[5]);
            return new FractalSettings(width, height, xmin, xmax, ymin, ymax, args[6]);
        }
    }
}
