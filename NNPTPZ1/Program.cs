using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            FractalSettings settings;
            try
            {
                settings = FractalSettings.Parse(args);
            }
            catch (Exception error) when (error is ArgumentException || error is FormatException ||
                error is OverflowException)
            {
                Console.Error.WriteLine(error.Message);
                Console.Error.WriteLine("Použití: NNPTPZ1 width height xmin xmax ymin ymax output");
                return 1;
            }

            Bitmap bitmap;
            try
            {
                bitmap = new Bitmap(settings.Width, settings.Height);
            }
            catch (Exception error) when (error is ArgumentException || error is OutOfMemoryException)
            {
                Console.Error.WriteLine("Obrázek se nepodařilo vytvořit: " + error.Message);
                return 1;
            }

            using (bitmap)
            {
                var renderer = new NewtonFractalRenderer(CreatePolynomial());
                Console.WriteLine(renderer.Polynomial);
                Console.WriteLine(renderer.Derivative);
                renderer.Render(bitmap, settings);
                try
                {
                    using (var output = File.Create(settings.OutputPath))
                        bitmap.Save(output, ImageFormat.Png);
                }
                catch (Exception error) when (error is IOException || error is UnauthorizedAccessException ||
                    error is ExternalException || error is ArgumentException || error is NotSupportedException)
                {
                    Console.Error.WriteLine("Obrázek se nepodařilo uložit: " + error.Message);
                    return 1;
                }
            }
            return 0;
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
