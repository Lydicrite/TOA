using System;
using TOAConsole.LSA.LSAutomaton.ParserSystem;
using TOAConsole.LSA.LSAutomaton.Utils;

public class Program
{
    public static void Main()
    {
        try
        {
            /*
            
            TestParser("1", "Yн X0 ↑1 Y0 w↑2 ↓1 Y1 ↓2 Yк", "1");                                                                // Корректно
            TestParser("1", "Yн X0 ( ↑1 | Y0 w↑2 ) ↓1 Y1 ↓2 Yк", "1");                                                          // Корректно
            TestParser("2", "Yн ↓1 Y0 X0 ↑1 Y1 Yк", "1");                                                                       // Корректно
            TestParser("2", "Yн ↓1 Y0 X0 ( ↑1 | Y1 Yк )", "1");                                                                 // Корректно
            TestParser("3", "Yн ↓1 Y0 X0 ↑2 w↑1 ↓2 Y1 Yк", "0");                                                                // Корректно
            TestParser("3", "Yн ↓1 Y0 X0 ( ↑2 | w↑1 ) ↓2 Y1 Yк", "0");                                                          // Корректно
            TestParser("4", "Yн Y0 X0 ↑1 w↑2 ↓1 Y1 ↓2 Y2 Yк", "0");                                                             // Корректно
            TestParser("4", "Yн Y0 X0 ( ↑1 | w↑2 ) ↓1 Y1 ↓2 Y2 Yк", "0");                                                       // Корректно
            TestParser("моя ЛСА", "Yн X0 ↑1 X1 ↑2 Y1 w↑2 ↓1 Y0 ↓2 Y2 X2 ↑2 Yк", "111");                                         // Корректно
            TestParser("моя ЛСА", "Yн X0 ( ↑1 | X1 ( ↑2 | Y1 w↑2 ) ) ↓1 Y0 ↓2 Y2 X2 ( ↑2 | Yк )", "111");                       // Корректно
            TestParser("5", "Yн Y1 X0 ↑1 Y2 w↑2 ↓1 X1 ↑3 w↑2 ↓2 X2 ↑3 Y4 w↑4 ↓3 Y3 ↓4 Yк", "111");                              // Корректно
            TestParser("5", "Yн Y1 X0 ( ↑1 | Y2 w↑2 ) ↓1 X1 ( ↑3 | w↑2 ) ↓2 X2 ( ↑3 | Y4 w↑4 ) ↓3 Y3 ↓4 Yк", "111");            // Корректно
            TestParser("ЛСА из учебника", "Yн ↓1 X1 ↑1 Y1 ↓2 Y2 X2 ↑2 Y4 X3 ↑3 Y3 ↓3 Yк", "111");                               // Корректно
            TestParser("ЛСА из учебника", "Yн ↓1 X1 ( ↑1 | Y1 ↓2 Y2 X2 ( ↑2 | Y4 X3 ( ↑3 | Y3 ) ↓3 Yк ) )", "111");             // Корректно
            TestParser("ЛСА из учебника", "Yн ↓1 X1 ( ↑1 | Y1 ↓2 Y2 X2 ( ↑2 | Y4 X3 ( ↑3 | Y3 w↑3 ) ↓3 Yк ) )", "111");         // Корректно
            TestParser("Ира", "Yн Y0 X0 ↑1 Y1 w↑1 ↓1 X1 ↑2 w↑1 ↓2 X2 ↑3 Y3 w↑4 ↓3 Y2 ↓4 Yк", "101");                            // Корректно
            TestParser("Стёпа", "Yн ↓1 Y0 ↓2 Y1 X0 ↑2 X1 ↑3 Y2 w↑4 ↓3 X2 ↑4 w↑1 ↓4 Y3 Yк", "100");                              // Корректно
            TestParser("Макс", "Yн ↓1 X0 ( ↑2 | Y0 X1 ( ↑4 | Y2 w↑5 ) ) ↓2 X2 ( ↑3 | w↑1 ) ↓3 Y1 ↓4 Y3 ↓5 Yк", "100");          // Корректно
            TestParser("Андрей", "Yн ↓1 X0 ( ↑2 | Y0 X2 ( ↑1 | Y3 w↑4) ) ↓2 X1 ( ↑3 | Y1 w↑4 ) ↓3 Y2 ↓4 Yк", "001");            // Корректно
            TestParser("Илья", "Yн ↓1 Y0 X0 ( ↑2 | Y1 w↑3 ) ↓2 Y2 ↓3 X1 ( ↑4 | w↑5 ) ↓4 Y3 ↓5 X2 ( ↑6 | w↑1 ) ↓6 Yк", "000");  // Корректно
            TestParser("Лёха", "Yн ↓1 X0 ( ↑2 | X1 ( ↑3 | Y1 w↑3 ) ) ↓2 Y0 ↑1 ↓3 X2 ( ↑4 | w↑5 ) ↓4 Y2 ↓5 Y3 Yк", "110");       // Корректно
            TestParser("Миша", "Yн ↓1 Y0 ↓2 X0 ( ↑1 | X1 ( ↑3 | w↑2 ) ) ↓3 X2 ( ↑4 | Y1 w↑5 ) ↓4 Y2 ↓5 Y3 Yк", "100");          // Корректно

            Console.Write("\n\n");
            Console.ReadKey();
            Console.Write("\n\n");

            */

            LSAInputHandler.ProgramCycle();

            /*
            while (true)
                Console.ReadLine();
            */
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: -> {ex.Message}");
        }
    }

    private static void TestParser(string name, string lsaString, string inputs)
    {
        Console.WriteLine($"Тест: {name}");
        Console.WriteLine($"--------------- Парсинг: \"{lsaString}\" ---------------\n\nИнформация: ");
        var automaton = LSAParser.Parse(lsaString);

        Console.WriteLine($"\n\n{automaton.GetTokens()}");
        Console.WriteLine($"\n\n{automaton.GetAllLoops()}");
        Console.WriteLine($"\n\n{automaton.GetResults()}");


        Console.Write($"\n\nПодробный вывод работы алгоритма по входам {inputs}: \n");
        var steps = automaton.Run(inputs, true);
        foreach (var step in steps)
            Console.WriteLine(step);

        Console.WriteLine($"\n--------------- Конец парсинга \"{lsaString}\" ---------------\n");
        Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n");
    }
}