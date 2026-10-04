using Microsoft.VisualStudio.TestTools.UnitTesting;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1Tests
{
    [TestClass]
    public class OriginalBehaviorTests
    {
        [TestMethod]
        public void Add_ReturnsSumAndPreservesOriginalNumberStrings()
        {
            var left = new ComplexNumber { Real = 10, Imaginary = 20 };
            var right = new ComplexNumber { Real = 1, Imaginary = 2 };

            Assert.AreEqual(new ComplexNumber { Real = 11, Imaginary = 22 }, left.Add(right));
            Assert.AreEqual("(10 + 20i)", left.ToString());
            Assert.AreEqual("(1 + 2i)", right.ToString());

            left = new ComplexNumber { Real = 1, Imaginary = -1 };
            right = new ComplexNumber { Real = 0, Imaginary = 0 };

            Assert.AreEqual(new ComplexNumber { Real = 1, Imaginary = -1 }, left.Add(right));
            Assert.AreEqual("(1 + -1i)", left.ToString());
            Assert.AreEqual("(0 + 0i)", right.ToString());
        }

        [TestMethod]
        public void Evaluate_QuadraticPreservesOriginalValuesAndString()
        {
            var polynomial = new Polynomial();
            polynomial.Add(new ComplexNumber { Real = 1, Imaginary = 0 });
            polynomial.Add(new ComplexNumber { Real = 0, Imaginary = 0 });
            polynomial.Add(new ComplexNumber { Real = 1, Imaginary = 0 });

            Assert.AreEqual(new ComplexNumber { Real = 1 },
                polynomial.Evaluate(new ComplexNumber { Real = 0, Imaginary = 0 }));
            Assert.AreEqual(new ComplexNumber { Real = 2 },
                polynomial.Evaluate(new ComplexNumber { Real = 1, Imaginary = 0 }));
            Assert.AreEqual(new ComplexNumber { Real = 5 },
                polynomial.Evaluate(new ComplexNumber { Real = 2, Imaginary = 0 }));
            Assert.AreEqual("(1 + 0i) + (0 + 0i)x + (1 + 0i)xx", polynomial.ToString());
        }
    }
}
