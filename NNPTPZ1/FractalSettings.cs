using System;

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
            if (args == null || args.Length < 7)
                throw new ArgumentException("Očekáváno: width height xmin xmax ymin ymax output.", nameof(args));

            int width = ParseDimension(args[0], "width");
            int height = ParseDimension(args[1], "height");
            double xmin = ParseCoordinate(args[2], "xmin");
            double xmax = ParseCoordinate(args[3], "xmax");
            double ymin = ParseCoordinate(args[4], "ymin");
            double ymax = ParseCoordinate(args[5], "ymax");

            if (!IsFinite(xmax - xmin))
                throw new ArgumentException("Rozdíl parametrů 'xmax' a 'xmin' musí být konečný.", "xmax");
            if (!IsFinite(ymax - ymin))
                throw new ArgumentException("Rozdíl parametrů 'ymax' a 'ymin' musí být konečný.", "ymax");
            if (Math.Abs(ymin) > float.MaxValue)
                throw new ArgumentException("Parametr 'ymin' je mimo rozsah typu Single.", "ymin");
            if (Math.Abs(ymax) > float.MaxValue)
                throw new ArgumentException("Parametr 'ymax' je mimo rozsah typu Single.", "ymax");
            if (string.IsNullOrWhiteSpace(args[6]))
                throw new ArgumentException("Cesta k výstupnímu souboru nesmí být prázdná.", "output");

            return new FractalSettings(width, height, xmin, xmax, ymin, ymax, args[6]);
        }

        private static int ParseDimension(string value, string name)
        {
            int dimension;
            try
            {
                dimension = int.Parse(value);
            }
            catch (FormatException error)
            {
                throw new FormatException($"Parametr '{name}' musí být celé číslo.", error);
            }
            catch (OverflowException error)
            {
                throw new OverflowException($"Parametr '{name}' je mimo rozsah typu Int32.", error);
            }

            if (dimension <= 0)
                throw new ArgumentException($"Parametr '{name}' musí být kladný.", name);
            return dimension;
        }

        private static double ParseCoordinate(string value, string name)
        {
            double coordinate;
            try
            {
                coordinate = double.Parse(value);
            }
            catch (FormatException error)
            {
                throw new FormatException($"Parametr '{name}' musí být číslo.", error);
            }
            catch (OverflowException error)
            {
                throw new OverflowException($"Parametr '{name}' je mimo rozsah typu Double.", error);
            }

            if (!IsFinite(coordinate))
                throw new ArgumentException($"Parametr '{name}' musí být konečný.", name);
            return coordinate;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
