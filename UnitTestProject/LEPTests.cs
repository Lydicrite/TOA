using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheoryOfAutomatons.Utils.UI.Controls;

namespace UnitTestProject
{
    [TestClass]
    public class LEPTests
    {

        [TestMethod]
        public void TestMethod1()
        {
            var parser = new LogicalExpressionParser();
            var expression = new LogicalExpression(parser.Parse("(A & B) | !C"));
            var func = expression.Compile();

            bool[] inputs = { true, false, true }; // A=true, B=false, C=true
            Console.WriteLine(func(inputs)); // Вывод: False
        }
    }
}
