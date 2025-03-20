using TOAConsole.LSA.LSAutomaton.Parser;

public class Program
{
    public static void Main()
    {
        try
        {
            TestParser("Yн X0 ↑1 X1 ↑2 Y1 w↑2 ↓1 Y0 ↓2 Y2 X2 ↑2 ↑3 ↓3 Yк");
            TestParser("Yн X0 ( ↑1 | X1 ( ↑2 | Y1 w↑2 ) ) ↓1 Y0 ↓2 Y2 X2 ( ↑2 | ↑3 ) ↓3 Yк");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private static void TestParser(string input)
    {
        Console.WriteLine($"Парсинг: {input}");
        var parser = new LSAParser(input);
        var automaton = parser.Parse();

        automaton.SetConditionalValue(0, true);
        automaton.SetConditionalValue(1, false);
        automaton.SetConditionalValue(2, true);

        foreach (var step in automaton.Run())
            Console.WriteLine(step);

        Console.WriteLine("\n\n");
    }
}