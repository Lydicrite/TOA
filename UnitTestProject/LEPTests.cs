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
        public void SimpleExpressionFastCompilation()
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
        public void SimpleExpressionInterpretation()
        {
            // Arrange
            var parser = new LogicalExpressionParser();
            var expr = new LogicalExpression(parser.Parse("(A & B) | !(C => false)"));
            bool[] inputs = { true, false, true };

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

        [TestMethod]
        public void ComplexNestedExpression()
        {
            // Arrange
            var parser = new LogicalExpressionParser();
            var expr = new LogicalExpression(parser.Parse("((A | B) ^ (C & D)) => (E <=> !(F | G))"));
            bool[] inputs = { true, true, false, true, false, true, false };

            // Act
            expr.SetVariableOrder(new[] { "A", "B", "C", "D", "E", "F", "G" });
            var result = expr.Evaluate(inputs);
            expr.CompileFast();
            var fastResult = expr.EvaluateFast(inputs);

            // Assert
            Console.WriteLine($"Результат интерпретации: {result}");
            Console.WriteLine($"Результат компиляции: {fastResult}");
            Console.WriteLine($"Выражение: {expr.ToString()}");
            Console.WriteLine($"\n\n\n{expr.PrintTruthTable()}");

            Assert.IsTrue(result, "Интерпретация");
            Assert.IsTrue(fastResult, "Компиляция");
        }

        [TestMethod]
        public void MixedConstantsEvaluation()
        {
            // Arrange
            var parser = new LogicalExpressionParser();
            var expr = new LogicalExpression(parser.Parse("(true & A) | (B ^ false)"));
            bool[] inputs = { false, true };

            // Act
            expr.SetVariableOrder(new[] { "A", "B" });
            var result = expr.Evaluate(inputs);
            expr.CompileFast();
            var fastResult = expr.EvaluateFast(inputs);

            // Assert
            Assert.IsTrue(result, "Интерпретация");
            Assert.IsTrue(fastResult, "Компиляция");
        }

        [TestMethod]
        public void VariableOrderIndependence()
        {
            // Arrange
            var parser = new LogicalExpressionParser();
            var expr1 = new LogicalExpression(parser.Parse("A | B"));
            var expr2 = new LogicalExpression(parser.Parse("B | A"));
            bool[] inputs = { false, true };

            // Act
            expr1.SetVariableOrder(new[] { "A", "B" });
            expr2.SetVariableOrder(new[] { "B", "A" });

            var result1 = expr1.Evaluate(inputs);
            var result2 = expr2.Evaluate(inputs);

            // Assert
            Assert.AreEqual(result1, result2, "Результат должен быть одинаковым независимо от порядка переменных");
        }

        [TestMethod]
        public void XorOperatorBehavior()
        {
            // Arrange
            var parser = new LogicalExpressionParser();
            var expr = new LogicalExpression(parser.Parse("A ^ B ^ C"));
            bool[] testCase1 = { true, true, true };
            bool[] testCase2 = { true, false, false };

            // Act & Assert
            expr.SetVariableOrder(new[] { "A", "B", "C" });
            Assert.IsTrue(expr.Evaluate(testCase1), "Case 1 - Интерпретация");
            Assert.IsTrue(expr.EvaluateFast(testCase1), "Case 1 - Компиляция");
            Assert.IsTrue(expr.Evaluate(testCase2), "Case 2 - Интерпретация");
            Assert.IsTrue(expr.EvaluateFast(testCase2), "Case 2 - Компиляция");
        }

    }
}
