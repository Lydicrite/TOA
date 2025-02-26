using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheoryOfAutomatons.Utils.UI.Controls;
using TOA.TheoryOfAutomatons.Utils.UI.Controls.LogicalExpressionParser.ParserSystem;

namespace UnitTestProject
{
    [TestClass]
    public class LEPTests
    {

        [TestMethod]
        public void SimpleExpression_FastCompilation()
        {
            // Arrange
            var parser = new LogicalExpressionParser();
            var expr = new LogicalExpression(parser.Parse("(A & B) | !(C => false)"));
            bool[] inputs = { true, false, true }; // A = true, B = false, C = true

            // Act
            expr.SetVariableOrder(new string[] { "A", "B", "C" });
            expr.CompileFast();
            var result = expr.EvaluateFast(inputs);

            // Assert
            Console.WriteLine($"Результат: {result}\nВыражение: {expr.ToString()}");
            Assert.IsTrue(result);

            // Дополнительная проверка с выводом
            Assert.AreEqual(
                expected: true,
                actual: result,
                message: $"Ожидалось true при A = {inputs[0]}, B = {inputs[1]}, C = {inputs[2]}"
            );
        }

        [TestMethod]
        public void SimpleExpression_Interpretation()
        {
            // Arrange
            var parser = new LogicalExpressionParser();
            var expr = new LogicalExpression(parser.Parse("(A & B) | !(C => false)"));
            bool[] inputs = { true, false, true }; // A = true, B = false, C = true

            // Act
            expr.SetVariableOrder(new string[] { "A", "B", "C" });
            var result = expr.Evaluate(inputs);

            // Assert
            Console.WriteLine($"Результат: {result}\nВыражение: {expr.ToString()}");
            Assert.IsTrue(result);

            // Дополнительная проверка с выводом
            Assert.AreEqual(
                expected: true,
                actual: result,
                message: $"Ожидалось true при A = {inputs[0]}, B = {inputs[1]}, C = {inputs[2]}"
            );
        }
    }
}
