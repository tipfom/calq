using System;
using Calq.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calq.UnitTests
{
    [TestClass]
    public class CoreTermTests
    {
        [TestMethod]
        public void Parse_AllOperators_Term()
        {
            Variable x = new Variable("x");
            Variable y = new Variable("y");

            Term t = Term.Parse("x");
            Assert.IsTrue(t == new Variable("x"));

            //operator
            t = Term.Parse("x + y");
            Assert.IsTrue(t == x + y);

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
    }
}
