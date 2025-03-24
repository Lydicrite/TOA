using TOAConsole.LSA.LSAutomaton.Parser;

public class Program
{
    public static void Main()
    {
        try
        {
            TestParser("Yн X0 ↑1 Y0 w↑2 ↓1 Y1 ↓2 Yк", "1");             // Корректно
            TestParser("Yн X0 ( ↑1 | Y0 w↑2 ) ↓1 Y1 ↓2 Yк", "1");       // Корректно

            TestParser("Yн ↓1 Y0 X0 ↑1 Y1 Yк", "1");                    // Некорректная работа - Yк не становится следующим элементом для Y1, хотя должна.
            TestParser("Yн ↓1 Y0 X0 ( ↑1 | Y1 Yк )", "1");              // Корректно

            TestParser("Yн ↓1 Y0 X0 ↑2 w↑1 ↓2 Y1 Yк", "0");             // Корректно
            TestParser("Yн ↓1 Y0 X0 ( ↑2 | w↑1 ) ↓2 Y1 Yк", "0");       // Корректно

            TestParser("Yн Y0 X0 ↑1 w↑2 ↓1 Y1 ↓2 Y2 Yк", "0");          // Корректно
            TestParser("Yн Y0 X0 ( ↑1 | w↑2 ) ↓1 Y1 ↓2 Y2 Yк", "0");    // Корректно

            TestParser("Yн X0 ↑1 X1 ↑2 Y1 w↑2 ↓1 Y0 ↓2 Y2 X2 ↑2 Yк", "111");                    // Корректно
            TestParser("Yн X0 ( ↑1 | X1 ( ↑2 | Y1 w↑2 ) ) ↓1 Y0 ↓2 Y2 X2 ( ↑2 | Yк )", "111");  // Корректно

            TestParser("Yн Y1 X0 ↑1 Y2 w↑2 ↓1 X1 ↑3 ↑2 ↓2 X2 ↑3 Y4 w↑4 ↓3 Y3 ↓4 Yк", "111");                        // Корректно
            TestParser("Yн Y1 X0 ( ↑1 | Y2 w↑2 ) ↓1 X1 ( ↑3 | ↑2 ) ↓2 X2 ( ↑3 | Y4 w↑4 ) ↓3 Y3 ↓4 Yк", "111");      // Корректно
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: -> {ex.Message}");
        }
    }

    private static void TestParser(string lsaString, string inputs)
    {
        Console.WriteLine($"--------------- Парсинг: {lsaString} ---------------\n\nИнформация: ");
        var automaton = LSAParser.Parse(lsaString);

        Console.WriteLine($"\n\n{automaton.GetTokens()}");
        Console.WriteLine($"\n\n{automaton.GetLoops()}");

        var table = automaton.GenerateFinalsTable();

        Console.Write($"\n\nПодробный вывод работы алгоритма по входам {inputs}: ");
        var steps = automaton.Run(inputs, true);
        foreach (var step in steps)
            Console.WriteLine(step);

        Console.WriteLine($"\n--------------- Конец парсинга {lsaString} ---------------\n");
        Console.WriteLine("\n\n\n\n\n\n");
    }
}