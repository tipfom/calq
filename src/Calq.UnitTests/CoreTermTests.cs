using System;
using Calq.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calq.UnitTests
{
    [TestClass]
    public class CoreTermTests
    {
        [TestMethod]
        public void ParseInfixOpearatos()
        {
            Variable x = new Variable("x");
            Variable y = new Variable("y");

            Term t = Term.Parse("x");

            //operator
            t = Term.Parse("x + y");
            Assert.IsTrue(t == x + y);

            t = Term.Parse("x + -y");
            Assert.IsTrue(t == x - y);

            t = Term.Parse("x - y");
            Assert.IsTrue(t == x - y);

            t = Term.Parse("x * y");
            Assert.IsTrue(t == x * y);

            t = Term.Parse("x / y");
            Assert.IsTrue(t == x / y);

            t = Term.Parse("x ^ y");
            Assert.IsTrue(t == (x ^ y));

            ////correct order
            t = Term.Parse("x + y * x");
            Assert.IsTrue(t == x + (y * x));

            t = Term.Parse("x + y / x");
            Assert.IsTrue(t == x + (y / x));

            t = Term.Parse("x + y / x ^ 2");
            Assert.IsTrue(t == x + (y / (x ^ 2)));
        }

        [TestMethod]
        public void ParseMixedOperators()
        {
            Variable x = new Variable("x");
            Variable y = new Variable("y");

            Term t = Term.Parse("sin(x)");
            Assert.IsTrue(t == new Sin(x));

            t = Term.Parse("sin(x) ^ y");
            Assert.IsTrue(t == (new Sin(x) ^ y));

            t = Term.Parse("sin(x * y)");
            Assert.IsTrue(t == new Sin(x * y));

            t = Term.Parse("sin(sin(x) * y)");
            Assert.IsTrue(t == new Sin(new Sin(x) * y));
        }
    }
}
