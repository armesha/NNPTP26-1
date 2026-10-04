using System;
using System.Drawing;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var settings = FractalSettings.Parse(args);
            using (var bitmap = new Bitmap(settings.Width, settings.Height))
            {
                var renderer = new NewtonFractalRenderer(CreatePolynomial());
                Console.WriteLine(renderer.Polynomial);
                Console.WriteLine(renderer.Derivative);
                renderer.Render(bitmap, settings);
                bitmap.Save(settings.OutputPath ?? "../../../out.png");
            }
        }

        private static Polynomial CreatePolynomial()
        {
            var polynomial = new Polynomial();
            polynomial.Add(new ComplexNumber { Real = 1 });
            polynomial.Add(ComplexNumber.Zero);
            polynomial.Add(ComplexNumber.Zero);
            polynomial.Add(new ComplexNumber { Real = 1 });
            return polynomial;
        }
    }
}
