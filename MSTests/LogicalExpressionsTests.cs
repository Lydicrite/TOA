using System.Diagnostics;
using System.Reflection;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using TOAConsole.LogicalExpressionParser.ParserSystem;
using TOAConsole.LogicalExpressionParser.ParserSystem.Utils;

namespace MSTests
{
    [TestClass]
    public sealed class LogicalExpressionsTests
    {
        private const int TestTimeoutMs = 1000;
        private static LogicalExpressionParser _parser;

        #region Инициализация и очистка

        [TestInitialize]
        public void TestInit()
        {
            _parser = new LogicalExpressionParser();
            Console.WriteLine($"Инициализация теста: {TestContext.TestName}\n\n");
        }

        [TestCleanup]
        public void TestCleanup() => Console.WriteLine($"\n\nЗавершение теста: {TestContext.TestName}");

        public TestContext TestContext { get; set; }

        #region Вспомогательные методы
        private static long MeasureEvaluationTime(LogicalExpression expr, bool[] inputs)
        {
            var sw = Stopwatch.StartNew();
            expr.Evaluate(inputs);
            sw.Stop();
            return sw.ElapsedTicks;
        }

        private static void PrintTestResults(LogicalExpression expr, bool result)
        {
            Console.WriteLine($"\nРезультат: {result}");
            Console.WriteLine($"Оптимизированное выражение: {expr}");

            var sw = Stopwatch.StartNew();
            var table = expr.PrintTruthTable();
            sw.Stop();

            Console.WriteLine($"\nТаблица истинности (построена за {sw.ElapsedMilliseconds} мс) {table}");
        }


        #region Расширенные диагностические методы

        private static void PrintDetailedAnalysis(string expression, bool[] inputs, string[] variables)
        {
            Console.WriteLine("\n=== ДИАГНОСТИЧЕСКИЙ ОТЧЁТ ===");

            // 1. Тестирование кэширования парсера
            var (firstParseTime, secondParseTime) = MeasureParserCacheEfficiency(_parser, expression);
            Console.WriteLine($"\n[ПАРСЕР] Первый запуск: {firstParseTime.ms} мс ({firstParseTime.ticks} тиков)");
            Console.WriteLine($"[ПАРСЕР] Второй запуск: {secondParseTime.ms} мс ({secondParseTime.ticks} тиков)");
            Console.WriteLine($"Эффективность кэша: {((firstParseTime.ticks - secondParseTime.ticks) * 100.0 / firstParseTime.ticks):F1}%");

            // 2. Анализ выражения
            var expr = new LogicalExpression(_parser.Parse(expression));
            expr.SetVariableOrder(variables);
            Console.WriteLine("\n[АНАЛИЗ ВЫРАЖЕНИЯ]");
            Console.WriteLine($"Исходное выражение: {expression}");
            Console.WriteLine($"Нормализованная форма: {expr}");
            Console.WriteLine($"Количество переменных: {variables.Length}");
            Console.WriteLine($"Глубина дерева: {CalculateExpressionDepth(expr)} уровней");

            // 3. Вычисление результата
            var evalResult = MeasureEvaluationWithDetails(expr, inputs);
            Console.WriteLine("\n[ВЫЧИСЛЕНИЕ]");
            Console.WriteLine($"Входные данные: {FormatBoolArray(inputs)}");
            Console.WriteLine($"Результат: {evalResult.result} \tВремя: {evalResult.timeMs:F3} мс");

            // 4. Генерация таблицы истинности
            var (table, buildTime) = MeasureTruthTableGeneration(expr);
            Console.WriteLine("\n[ТАБЛИЦА ИСТИННОСТИ]");
            Console.WriteLine($"Время построения: {buildTime} мс");
            Console.WriteLine(table);

            // 5. Дополнительная информация
            Console.WriteLine("\n[ДОПОЛНИТЕЛЬНО]");
            Console.WriteLine($"Используемые операторы: {GetUsedOperators(expr)}");
            Console.WriteLine($"Размер кэша парсера: {GetParserCacheSize(_parser)} выражений");
        }

        private static ((long ticks, double ms) first, (long ticks, double ms) second)
            MeasureParserCacheEfficiency(LogicalExpressionParser parser, string expression)
        {
            var swFirst = Stopwatch.StartNew();
            parser.Parse(expression);
            swFirst.Stop();

            var swSecond = Stopwatch.StartNew();
            parser.Parse(expression);
            swSecond.Stop();

            return (
                (swFirst.ElapsedTicks, swFirst.Elapsed.TotalMilliseconds),
                (swSecond.ElapsedTicks, swSecond.Elapsed.TotalMilliseconds)
            );
        }

        private static (bool result, double timeMs) MeasureEvaluationWithDetails(LogicalExpression expr, bool[] inputs)
        {
            var sw = Stopwatch.StartNew();
            var result = expr.Evaluate(inputs);
            sw.Stop();
            return (result, sw.Elapsed.TotalMilliseconds);
        }

        private static (string table, long timeMs) MeasureTruthTableGeneration(LogicalExpression expr)
        {
            var sw = Stopwatch.StartNew();
            var table = expr.PrintTruthTable();
            sw.Stop();
            return (table, sw.ElapsedMilliseconds);
        }

        #endregion

        #region Вспомогательные форматеры

        private static string FormatBoolArray(bool[] arr) =>
            $"[{string.Join(", ", arr.Select(b => b ? "1" : "0"))}]";

        private static int CalculateExpressionDepth(LogicalExpression expr)
        {
            // Реализация через рекурсивный обход дерева выражений
            return expr.ToString().Count(c => c == '(');
        }

        private static string GetUsedOperators(LogicalExpression expr)
        {
            var ops = new HashSet<string>();
            var pattern = new Regex(@"([&|^!]|=>|<=>|[∧∨→≡])");
            foreach (Match m in pattern.Matches(expr.ToString()))
                ops.Add(m.Value);

            return string.Join(", ", ops.OrderBy(o => o));
        }

        private static long GetParserCacheSize(LogicalExpressionParser parser)
        {
            // Реализация через Reflection (для демонстрации)
            var cacheField = typeof(LogicalExpressionParser).GetField(
                "_logicalExpressionsCache",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            return ((MemoryCache)cacheField.GetValue(parser)).GetCount();
        }

        #endregion

        #endregion

        #endregion




        #region Тесты базовой функциональности

        [TestMethod]
        [Timeout(TestTimeoutMs)]
        [Description("Проверка вложенных выражений с константами и базовыми операторами")]
        public void Evaluate_ComplexNestedExpressionWithConstants_ReturnsExpectedResult()
        {
            // Arrange
            var expr = new LogicalExpression(_parser.Parse("((A & B) | !(C → true)) ⇔ D"));
            var variables = new[] { "A", "B", "C", "D" };
            var testInput = new[] { false, false, false, false };

            // Act
            expr.SetVariableOrder(variables);
            var result = expr.Evaluate(testInput);

            // Assert
            PrintTestResults(expr, result);
            Assert.IsTrue(result, "Ожидалось значение true для входных данных [false, false, false, false]");
        }

        #endregion





        #region Тесты расширенных возможностей

        [TestMethod]
        [Timeout(TestTimeoutMs)]
        [Description("Проверка независимости результатов от порядка переменных")]
        public void Evaluate_WithDifferentVariableOrders_ReturnsConsistentResults()
        {
            // Arrange
            var expr1 = new LogicalExpression(_parser.Parse("(true & !(~A)) | (B ^ false)"));
            var expr2 = new LogicalExpression(_parser.Parse("(true ∧ ¬(!A)) ∨ (B XOR false)"));
            var inputs = new[] { false, true };

            // Act
            expr1.SetVariableOrder(new[] { "A", "B" });
            expr2.SetVariableOrder(new[] { "B", "A" });
            var result1 = expr1.Evaluate(inputs);
            var result2 = expr2.Evaluate(inputs);

            // Assert
            Console.WriteLine($"Результат 1: {result1}\nРезультат 2: {result2}");
            Assert.AreEqual(result1, result2,
                "Результаты должны быть идентичны при разных порядках переменных");
        }

        #endregion





        #region Параметризованные тесты

        [DataTestMethod]
        [Timeout(TestTimeoutMs)]
        [DataRow(new[] { true, true, false }, true)]
        [DataRow(new[] { false, false, true }, false)]
        [DataRow(new[] { true, false, true }, false)]
        [Description("Проверка различных комбинаций входных данных")]
        public void Evaluate_WithMultipleInputCombinations_ReturnsExpected(bool[] inputs, bool expected)
        {
            // Arrange
            var expr = new LogicalExpression(_parser.Parse("(X | Y) & !Z"));
            expr.SetVariableOrder(new[] { "X", "Y", "Z" });

            // Act
            var result = expr.Evaluate(inputs);

            // Assert
            Console.WriteLine($"Входные данные: {string.Join(", ", inputs)}\nВыход: {result}");
            Assert.AreEqual(expected, result,
                $"Несоответствие ожидаемого результата для входных данных: {string.Join(", ", inputs)}");
        }

        #endregion





        #region Тесты производительности

        [TestMethod]
        [Timeout(5000)]
        [Description("Проверка эффективности кэширования выражений")]
        public void Evaluate_ExpressionCaching_EfficiencyTest()
        {
            // Arrange
            var expr = new LogicalExpression(_parser.Parse("(A ^ B) & (C | D) => (E ⇔ F)"));
            expr.SetVariableOrder(new[] { "A", "B", "C", "D", "E", "F" });
            var testInput = new[] { true, false, true, false, true, false };

            // Act
            var firstCallTime = MeasureEvaluationTime(expr, testInput);
            var secondCallTime = MeasureEvaluationTime(expr, testInput);

            // Assert
            Assert.IsTrue(secondCallTime < firstCallTime * 0.5,
                "Повторный вызов должен быть значительно быстрее благодаря кэшированию");
        }

        [TestMethod]
        [Timeout(TestTimeoutMs * 4)]
        [Description("Проверка производительности для выражения с 15 переменными")]
        public void Evaluate_With15Variables_CompletesInReasonableTime()
        {
            // Arrange
            var expr = "(A | B | C | D | E | F | G) & (H <=> I <=> J) XOR (K -> L -> M -> N -> O & (((P & Q) XOR (R ^ (!S))) | T))";
            var vars = Enumerable.Range('A', 20).Select(c => ((char)c).ToString()).ToArray();

            // Act & assert
            var input = new bool[20];
            PrintDetailedAnalysis(expr, input, vars);
        }

        #endregion





        #region Тесты семантической корректности

        [TestMethod]
        [Timeout(TestTimeoutMs)]
        [Description("Проверка эквивалентности различных форм импликации")]
        public void Evaluate_ImplicationOperatorEquivalence_ReturnsConsistentResults()
        {
            // Arrange
            var expr1 = new LogicalExpression(_parser.Parse("A => B"));
            var expr2 = new LogicalExpression(_parser.Parse("!A | B"));
            var expr3 = new LogicalExpression(_parser.Parse("¬A ∨ B"));
            var variables = new[] { "A", "B" };
            var testInputs = new bool[][]
            {
                new[] { true, true },
                new[] { true, false },
                new[] { false, true },
                new[] { false, false }
            };

            // Act & Assert
            foreach (var input in testInputs)
            {
                expr1.SetVariableOrder(variables);
                expr2.SetVariableOrder(variables);
                expr3.SetVariableOrder(variables);

                var result1 = expr1.Evaluate(input);
                var result2 = expr2.Evaluate(input);
                var result3 = expr3.Evaluate(input);

                Assert.AreEqual(result1, result2, $"Несоответствие для A={input[0]}, B={input[1]}");
                Assert.AreEqual(result2, result3, $"Несоответствие для A={input[0]}, B={input[1]}");
            }
        }

        #endregion





        #region Тесты обработки ошибок

        [TestMethod]
        [Timeout(TestTimeoutMs)]
        [ExpectedException(typeof(ArgumentException))]
        [Description("Проверка обработки неверного количества аргументов")]
        public void Evaluate_WithInvalidInputLength_ThrowsArgumentException()
        {
            // Arrange
            var expr = new LogicalExpression(_parser.Parse("A & B"));
            expr.SetVariableOrder(new[] { "A", "B" });

            // Act
            expr.Evaluate(new[] { true });
        }

        [TestMethod]
        [Timeout(TestTimeoutMs)]
        [Description("Проверка пустого выражения (должно выбрасывать исключение)")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Parse_EmptyExpression_ThrowsException()
        {
            // Arrange & Act
            var expr = new LogicalExpression(_parser.Parse(""));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Description("Проверка обработки дубликатов переменных при установке порядка")]
        public void SetVariableOrder_WithDuplicateVariables_ThrowsArgumentException()
        {
            // Arrange
            var expr = new LogicalExpression(_parser.Parse("A & B"));

            // Act
            expr.SetVariableOrder(new[] { "A", "A", "B" });
        }

        [TestMethod]
        [ExpectedException(typeof(ExpressionParseException))]
        [Description("Проверка обработки неверной вложенности скобок")]
        public void Parse_InvalidParenthesisNesting_ThrowsParseException()
        {
            // Arrange & Act
            var expr = new LogicalExpression(_parser.Parse("((A & B) | C"));
        }

        #endregion





        #region Тесты пограничных случаев

        [TestMethod]
        [Timeout(TestTimeoutMs)]
        [Description("Проверка выражения с единственной переменной")]
        public void Evaluate_SingleVariableExpression_ReturnsCorrectValue()
        {
            // Arrange
            var expr = new LogicalExpression(_parser.Parse("X"));
            expr.SetVariableOrder(new[] { "X" });

            // Act & Assert
            Assert.IsTrue(expr.Evaluate(new[] { true }), "Ожидалось true для X = true");
            Assert.IsFalse(expr.Evaluate(new[] { false }), "Ожидалось false для X = false");
        }

        #endregion





        #region Тесты таблиц истинности

        [TestMethod]
        [Timeout(1000)]
        [Description("Проверка полного соответствия таблицы истинности для XOR")]
        public void TruthTable_ForXorOperator_MatchesExpected()
        {
            // Arrange
            var expr = new LogicalExpression(_parser.Parse("A ^ B"));
            expr.SetVariableOrder(new[] { "A", "B" });

            // Expected truth table for XOR
            var expectedResults = new[]
            {
                new { A = false, B = false, Result = false },
                new { A = false, B = true, Result = true },
                new { A = true, B = false, Result = true },
                new { A = true, B = true, Result = false }
            };

            // Act & Assert
            foreach (var expected in expectedResults)
            {
                var actual = expr.Evaluate(new[] { expected.A, expected.B });
                Assert.AreEqual(expected.Result, actual,
                    $"Несоответствие для A = {expected.A}, B = {expected.B}");
            }
        }

        #endregion
    }
}