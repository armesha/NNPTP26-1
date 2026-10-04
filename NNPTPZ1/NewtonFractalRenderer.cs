using System;
using System.Collections.Generic;
using System.Drawing;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1
{
    internal sealed class NewtonFractalRenderer
    {
        private const int AcceptedStepCount = 30;
        internal const int MaximumIterations = 500;
        private const double StepSquaredThreshold = 0.5;
        private const double RootDistanceSquaredThreshold = 0.01;
        // Imaginární složka má typ float, proto připouštíme malou nenulovou odchylku.
        private const double RootResidualTolerance = 0.00001;
        private const double AxisOffset = 0.0001;
        private const int BrightnessLossPerIteration = 2;
        private static readonly Color[] RootColors =
        {
            Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange,
            Color.Fuchsia, Color.Gold, Color.Cyan, Color.Magenta
        };

        internal Polynomial Polynomial { get; }
        internal Polynomial Derivative { get; }

        internal NewtonFractalRenderer(Polynomial polynomial)
        {
            Polynomial = polynomial;
            Derivative = polynomial.Differentiate();
        }

        internal void Render(Bitmap bitmap, FractalSettings settings)
        {
            double xStep = (settings.XMax - settings.XMin) / settings.Width;
            double yStep = (settings.YMax - settings.YMin) / settings.Height;
            var roots = new List<ComplexNumber>();
            for (int row = 0; row < settings.Height; row++)
            {
                for (int column = 0; column < settings.Width; column++)
                {
                    var point = CreateStartingPoint(settings.XMin + column * xStep,
                        settings.YMin + row * yStep);
                    ComplexNumber root;
                    int iterations;
                    Color color = Color.Black;
                    if (TryFindRoot(point, out root, out iterations))
                    {
                        int rootIndex = GetOrAddRootIndex(roots, root);
                        color = GetPixelColor(rootIndex, iterations);
                    }
                    bitmap.SetPixel(column, row, color);
                }
            }
        }

        private static ComplexNumber CreateStartingPoint(double real, double imaginary)
        {
            var point = new ComplexNumber { Real = real, Imaginary = (float)imaginary };
            if (point.Real == 0)
                point.Real = AxisOffset;
            if (point.Imaginary == 0)
                point.Imaginary = (float)AxisOffset;
            return point;
        }

        internal bool TryFindRoot(ComplexNumber point, out ComplexNumber root, out int iterations)
        {
            root = null;
            iterations = 0;
            int acceptedSteps = 0;
            while (acceptedSteps < AcceptedStepCount && iterations < MaximumIterations)
            {
                if (!IsFinite(point))
                    return false;
                var derivative = Derivative.Evaluate(point);
                if (!IsFinite(derivative) || (derivative.Real == 0 && derivative.Imaginary == 0))
                    return false;
                var difference = Polynomial.Evaluate(point).Divide(derivative);
                if (!IsFinite(difference))
                    return false;
                point = point.Subtract(difference);
                if (Math.Pow(difference.Real, 2) + Math.Pow(difference.Imaginary, 2) < StepSquaredThreshold)
                    acceptedSteps++;
                iterations++;
            }
            if (acceptedSteps < AcceptedStepCount || !IsFinite(point))
                return false;
            var residual = Polynomial.Evaluate(point);
            if (!IsFinite(residual) || residual.GetMagnitude() > RootResidualTolerance)
                return false;
            root = point;
            return true;
        }

        private static bool IsFinite(ComplexNumber value)
        {
            return !double.IsNaN(value.Real) && !double.IsInfinity(value.Real) &&
                !float.IsNaN(value.Imaginary) && !float.IsInfinity(value.Imaginary);
        }

        internal static int GetOrAddRootIndex(List<ComplexNumber> roots, ComplexNumber root)
        {
            int rootIndex = -1;
            for (int index = 0; index < roots.Count; index++)
            {
                if (Math.Pow(root.Real - roots[index].Real, 2) +
                    Math.Pow(root.Imaginary - roots[index].Imaginary, 2) <= RootDistanceSquaredThreshold)
                    rootIndex = index;
            }
            if (rootIndex >= 0)
                return rootIndex;
            roots.Add(root);
            return roots.Count - 1;
        }

        internal static Color GetPixelColor(int rootIndex, float iterations)
        {
            Color color = RootColors[rootIndex % RootColors.Length];
            int brightnessLoss = (int)iterations * BrightnessLossPerIteration;
            return Color.FromArgb(Darken(color.R, brightnessLoss), Darken(color.G, brightnessLoss),
                Darken(color.B, brightnessLoss));
        }

        private static int Darken(int channel, int brightnessLoss)
        {
            return Math.Min(Math.Max(0, channel - brightnessLoss), 255);
        }
    }
}
