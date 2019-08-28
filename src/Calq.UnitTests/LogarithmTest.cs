using System;
using Calq.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calq.UnitTests
{
    [TestClass]
    public class LogarithmTest
    {
        [TestMethod]
        public void ReduceTestMethod()
        {
            Assert.IsTrue(new Logarithm(new Power(new Variable("x"), 2)).Reduce() == 2 * new Logarithm(new Variable("x")));

            Assert.IsTrue(new Logarithm(new Power(new Variable("x"), 2), new Variable("x")).Reduce() == 2);

            Assert.IsTrue(new Logarithm(new Power(new Constant(Constant.ConstType.E), 3)).Reduce() == 3);

            Assert.IsTrue(new Logarithm(new Variable("x") + new Variable("y")).Reduce() == new Logarithm(new Variable("x") + new Variable("y")));

            Assert.IsTrue(new Logarithm(new Variable("x") * new Variable("y")).Reduce() == new Logarithm(new Variable("x") * new Variable("y")));
        }
    }
}
