namespace MSTestProject
{
    using global::TheoryOfAutomatons.Utils.UI.Controls.Log

    [TestClass]
    public sealed class Test1
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
