﻿using Calq.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calq.UnitTests
{
    [TestClass]
    public class AdditionTest
    {
        [TestMethod]
        public void SimpleReduceTestMethod()
        {
            Assert.IsTrue((new Variable("x") + new Variable("x")).Reduce() == 2 * new Variable("x"));

            Assert.IsTrue((new Real(3) + new Variable("x")).Reduce() == new Real(3) + new Variable("x"));

            Assert.IsTrue((new Real(3) + new Real(2)).Reduce() == new Real(5));

            Assert.IsTrue((new Constant(Constant.ConstType.E) + new Constant(Constant.ConstType.E)).Reduce() == 2 * new Constant(Constant.ConstType.E));
        }

        [TestMethod]
        public void ConcatSimpleTestMethod()
        {
            Assert.IsTrue((new Addition(new Variable("x") + new Real(2), new Variable("x"))).Reduce() == 2 * new Variable("x") + new Real(2));
        }

        [TestMethod]
        public void ConcatMultiplicationTestMethod()
        {
            Variable x = new Variable("x");
            Variable y = new Variable("y");
            Constant pi = new Constant(Constant.ConstType.Pi);

            Assert.IsTrue((new Multiplication(2, x, y) + new Multiplication(new Real(-2), x, y)).Reduce() == 0);

            Assert.IsTrue((new Multiplication(2, x, y) + new Multiplication(new Real(3), x, y)).Reduce() == new Multiplication(new Real(5), x, y));

            Assert.IsTrue((new Multiplication(2, x, y) + new Multiplication(new Real(3), x, y) + new Multiplication(new Real(3), x, y, pi)).Reduce() == new Multiplication(new Real(5), x, y) + new Multiplication(new Real(3), x, y, pi));
        }

        [TestMethod]
        public void ConcatLogarithmTestMethod()
        {
            Variable x = new Variable("x");
            Variable y = new Variable("y");
            
            Assert.IsTrue((new Addition(new Logarithm(x), new Logarithm(y))).Reduce() == new Logarithm(x*y) );
        }
    }
}
