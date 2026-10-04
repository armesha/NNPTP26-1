using System;

namespace NNPTPZ1.Mathematics
{
    public class ComplexNumber
    {
        private const double DegreesPerRadian = 180.0 / Math.PI;
        private const double MinimumNormalDouble = 2.2250738585072014E-308;

        public double Real { get; set; }
        public float Imaginary { get; set; }

        /// <summary>
        /// Vrací samostatnou nulovou hodnotu, aby její změna neovlivnila další výpočty.
        /// </summary>
        public static ComplexNumber Zero => new ComplexNumber();

        public override bool Equals(object obj)
        {
            var other = obj as ComplexNumber;
            return other != null && Real.Equals(other.Real) && Imaginary.Equals(other.Imaginary);
        }

        /// <summary>
        /// Složky se nesmí měnit, pokud je hodnota použita jako klíč v hashovací kolekci.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return (ComponentHashCode(Real) * 397) ^ ComponentHashCode(Imaginary);
            }
        }

        public ComplexNumber Multiply(ComplexNumber b)
        {
            return new ComplexNumber
            {
                Real = Real * b.Real - (double)Imaginary * b.Imaginary,
                Imaginary = (float)(Real * b.Imaginary + Imaginary * b.Real)
            };
        }

        public double GetMagnitude()
        {
            double realMagnitude = Math.Abs(Real);
            double imaginaryMagnitude = Math.Abs((double)Imaginary);
            double largerMagnitude = Math.Max(realMagnitude, imaginaryMagnitude);
            if (largerMagnitude == 0 || double.IsInfinity(largerMagnitude))
            {
                return largerMagnitude;
            }

            double ratio = Math.Min(realMagnitude, imaginaryMagnitude) / largerMagnitude;
            return largerMagnitude * Math.Sqrt(1 + ratio * ratio);
        }

        public ComplexNumber Add(ComplexNumber b)
        {
            return new ComplexNumber
            {
                Real = Real + b.Real,
                Imaginary = Imaginary + b.Imaginary
            };
        }

        public double GetAngleInDegrees()
        {
            return Math.Atan2(Imaginary, Real) * DegreesPerRadian;
        }

        public ComplexNumber Subtract(ComplexNumber b)
        {
            return new ComplexNumber
            {
                Real = Real - b.Real,
                Imaginary = Imaginary - b.Imaginary
            };
        }

        public override string ToString()
        {
            return $"({Real} + {Imaginary}i)";
        }

        internal ComplexNumber Divide(ComplexNumber b)
        {
            double divisorReal = b.Real;
            double divisorImaginary = b.Imaginary;
            double dividendImaginary = Imaginary;
            if (divisorReal == 0 && divisorImaginary == 0)
            {
                return new ComplexNumber { Real = double.NaN, Imaginary = float.NaN };
            }

            if (divisorImaginary == 0)
            {
                return new ComplexNumber
                {
                    Real = Real / divisorReal,
                    Imaginary = (float)(dividendImaginary / divisorReal)
                };
            }
            if (divisorReal == 0)
            {
                return new ComplexNumber
                {
                    Real = dividendImaginary / divisorImaginary,
                    Imaginary = (float)(-Real / divisorImaginary)
                };
            }

            // Smithova metoda nepočítá druhé mocniny složek dělitele.
            if (Math.Abs(divisorReal) >= Math.Abs(divisorImaginary))
            {
                double ratio = divisorImaginary / divisorReal;
                double denominator = divisorReal + divisorImaginary * ratio;
                return new ComplexNumber
                {
                    Real = (Real + dividendImaginary * ratio) / denominator,
                    Imaginary = (float)((dividendImaginary - Real * ratio) / denominator)
                };
            }

            double imaginaryRatio = divisorReal / divisorImaginary;
            double imaginaryDenominator = divisorImaginary + divisorReal * imaginaryRatio;
            return new ComplexNumber
            {
                Real = DivideRealWithDominantImaginary(divisorReal, divisorImaginary,
                    imaginaryRatio, imaginaryDenominator),
                Imaginary = (float)((dividendImaginary * imaginaryRatio - Real) / imaginaryDenominator)
            };
        }

        private double DivideRealWithDominantImaginary(double divisorReal, double divisorImaginary,
            double ratio, double denominator)
        {
            // Malý poměr může ztratit přesnost ještě před násobením velkou reálnou složkou.
            if (Math.Abs(ratio) < MinimumNormalDouble)
            {
                double directProduct = Real * divisorReal;
                if (Math.Abs(directProduct) >= MinimumNormalDouble)
                {
                    // Normální součin nepotřebuje škálování, které by mohlo přetéct.
                    double directContribution = (directProduct / divisorImaginary) / denominator;
                    return Imaginary / denominator + directContribution;
                }

                double scaledProduct = Real * (divisorReal / MinimumNormalDouble);
                double contribution = ((scaledProduct / divisorImaginary) / denominator) * MinimumNormalDouble;
                return Imaginary / denominator + contribution;
            }

            double product = Real * ratio;
            if (Math.Abs(product) < MinimumNormalDouble && Math.Abs(denominator) < 1)
            {
                // Škálu vracíme až po dělení, aby mezivýsledek nepodtekl na nulu.
                double contribution = (((Real / MinimumNormalDouble) * ratio) / denominator) * MinimumNormalDouble;
                return Imaginary / denominator + contribution;
            }

            return (Imaginary + product) / denominator;
        }

        private static int ComponentHashCode(double value)
        {
            // Equals považuje všechny hodnoty NaN i obě znaménkové nuly za shodné.
            return double.IsNaN(value) || value == 0 ? 0 : value.GetHashCode();
        }
    }
}
