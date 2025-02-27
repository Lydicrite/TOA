using TOA.TheoryOfAutomatons.Utils.UI.Controls.LogicalExpressionParser.ParserSystem;

namespace TOATests
{
    [TestClass]
    public sealed class LogicalParserTests
    {
        #region Свойства

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // This method is called once for the test assembly, before any tests are run.
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // This method is called once for the test assembly, after all tests are run.
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // This method is called once for the test class, before any tests of the class are run.
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // This method is called once for the test class, after all tests of the class are run.
        }

        [TestInitialize]
        public void TestInit()
        {
            // This method is called before each test method.
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
        }

        #endregion



        #region Тестовые методы

        [TestMethod]
        public void SimpleNestedExpression()
        {
            // Arrange
            var parser = new LogicalExpressionParser();
            var expr = new LogicalExpression(parser.Parse("(A & B) | !(C => false)"));
            bool[] inputs = { false, false, false };

            // Act
            expr.SetVariableOrder(new[] { "A", "B", "C" });
            var result = expr.Evaluate(inputs);
            expr.CompileFast();
            var fastResult = expr.EvaluateFast(inputs);

            // Assert
            Console.WriteLine($"Результат интерпретации: {result}");
            Console.WriteLine($"Результат компиляции: {fastResult}");
            Console.WriteLine($"Выражение: {expr.ToString()}");
            Console.WriteLine($"\n{expr.PrintTruthTable()}");
            Assert.IsFalse(result, "Интерпретация");
            Assert.IsFalse(fastResult, "Компиляция");

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
            Console.WriteLine($"\n{expr.PrintTruthTable()}");
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
            Console.WriteLine($"Результат интерпретации: {result}");
            Console.WriteLine($"Результат компиляции: {fastResult}");
            Console.WriteLine($"Выражение: {expr.ToString()}");
            Console.WriteLine($"\n{expr.PrintTruthTable()}");
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
            Console.WriteLine($"Результат 1: {result1}");
            Console.WriteLine($"Результат 2: {result2}");
            Console.WriteLine($"\nВыражение 1: {expr1.ToString()}");
            Console.WriteLine($"Выражение 2: {expr2.ToString()}");
            Console.WriteLine($"\n\n\n{expr1.PrintTruthTable()}");
            Console.WriteLine($"{expr2.PrintTruthTable()}");
            Assert.AreEqual(result1, result2, "Результат должен быть одинаковым независимо от порядка переменных");
        }

        #endregion
    }
}
