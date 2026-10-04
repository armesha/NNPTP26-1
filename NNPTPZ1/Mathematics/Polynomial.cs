using System.Collections.Generic;
using System.Text;

namespace NNPTPZ1.Mathematics
{
    public class Polynomial
    {
        /// <summary>
        /// Měnitelné koeficienty od konstantního členu po nejvyšší mocninu.
        /// </summary>
        public List<ComplexNumber> Coefficients { get; set; }

        public Polynomial() => Coefficients = new List<ComplexNumber>();

        public void Add(ComplexNumber coefficient) =>
            Coefficients.Add(coefficient);

        /// <summary>
        /// Vytvoří derivaci bez změny původního polynomu nebo jeho koeficientů.
        /// </summary>
        public Polynomial Differentiate()
        {
            var derivative = new Polynomial();
            for (int degree = 1; degree < Coefficients.Count; degree++)
            {
                derivative.Add(Coefficients[degree].Multiply(new ComplexNumber { Real = degree }));
            }

            return derivative;
        }

        public ComplexNumber Evaluate(double x)
        {
            return Evaluate(new ComplexNumber { Real = x });
        }

        public ComplexNumber Evaluate(ComplexNumber x)
        {
            var sum = ComplexNumber.Zero;
            var power = x;
            // Mocniny počítáme postupně; pořadí násobení a sčítání zůstává stejné.
            for (int degree = 0; degree < Coefficients.Count; degree++)
            {
                var coefficient = Coefficients[degree];
                if (degree > 0)
                {
                    if (degree > 1)
                        power = power.Multiply(x);
                    coefficient = coefficient.Multiply(power);
                }

                sum = sum.Add(coefficient);
            }

            return sum;
        }

        /// <summary>
        /// Vypíše všechny koeficienty; mocninu označuje opakováním znaku x.
        /// </summary>
        public override string ToString()
        {
            var result = new StringBuilder();
            for (int degree = 0; degree < Coefficients.Count; degree++)
            {
                result.Append(Coefficients[degree]);
                result.Append('x', degree);
                if (degree + 1 < Coefficients.Count)
                    result.Append(" + ");
            }

            return result.ToString();
        }
    }
}
