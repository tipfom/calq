using System;
using Calq.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calq.UnitTests
{
    [TestClass]
    public class MultiplicationTest
    {
        [TestMethod]
        public void SimpleReduceTestMethod()
        {
            var x = new Multiplication(new Variable("x"), new Variable("x")).Reduce();
            Assert.IsTrue((new Multiplication(new Variable("x"), new Variable("x"))).Reduce() == new Power(new Variable("x"), 2));
        }
    }
}
