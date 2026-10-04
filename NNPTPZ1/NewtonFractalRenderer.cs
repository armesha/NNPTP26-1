using System;
using System.Collections.Generic;
using System.Drawing;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1
{
    internal sealed class NewtonFractalRenderer
    {
        private const int AcceptedStepCount = 30;
        private const double StepSquaredThreshold = 0.5;
        private const double RootDistanceSquaredThreshold = 0.01;
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
            for (int row = 0; row < settings.Width; row++)
            {
                for (int column = 0; column < settings.Height; column++)
                {
                    var point = CreateStartingPoint(settings.XMin + column * xStep,
                        settings.YMin + row * yStep);
                    float iterations;
                    var root = Iterate(point, out iterations);
                    int rootIndex = GetOrAddRootIndex(roots, root);
                    bitmap.SetPixel(column, row, GetPixelColor(rootIndex, iterations));
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

        private ComplexNumber Iterate(ComplexNumber point, out float iterations)
        {
            iterations = 0;
            for (int acceptedSteps = 0; acceptedSteps < AcceptedStepCount; acceptedSteps++)
            {
                var difference = Polynomial.Evaluate(point).Divide(Derivative.Evaluate(point));
                point = point.Subtract(difference);
                if (Math.Pow(difference.Real, 2) + Math.Pow(difference.Imaginary, 2) >= StepSquaredThreshold)
                    acceptedSteps--;
                iterations++;
            }
            return point;
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
            return roots.Count;
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
