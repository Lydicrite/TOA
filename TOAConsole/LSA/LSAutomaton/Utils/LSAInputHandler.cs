using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.LSAutomaton.ParserSystem;
using Windows.ApplicationModel.DataTransfer;

namespace TOAConsole.LSA.LSAutomaton.Utils
{
    internal static class LSAInputHandler
    {
        private static Automaton? _automaton;
        private static MenuState _currentMenuState = MenuState.Main;

        internal enum MenuState
        {
            Main,
            Modes
        }

        #region Константы и импорт библиотек

        // Импорт необходимых функций WinAPI
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint flags);

        [DllImport("user32.dll")]
        private static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, uint flags);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        // Константы для сообщений и флагов
        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private const uint KLF_ACTIVATE = 0x00000001;

        #endregion





        #region Ввод информации

        [STAThread]
        public static void ProgramCycle()
        {
            Console.Clear();
            Console.OutputEncoding = Encoding.Unicode;
            LSAInputHandler.PrintInputRules();

            while (true)
            {
                RenderMenu();
                HandleInput();
            }
        }

        [STAThread]
        public static string ReadLSAString()
        {
            ChangeLayout();

            var input = new StringBuilder();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\n►► Введите ЛСА: \n");
            Console.ResetColor();

            ConsoleKeyInfo keyInfo;
            char keyChar;
            int cursorPos = 0;

            do
            {
                keyInfo = Console.ReadKey(true);
                keyChar = keyInfo.KeyChar;

                // Обработка специальных клавиш
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (input.Length != 0 && input[cursorPos - 1] != '↑')
                            InsertSymbol("↑", ref cursorPos, input);
                        break;

                    case ConsoleKey.DownArrow:
                        if (input.Length != 0 && input[cursorPos - 1] != '↓')
                            InsertSymbol("↓", ref cursorPos, input);
                        break;

                    case ConsoleKey.W:
                        // Проверяем, нет ли уже "w↑" после текущей позиции
                        if ((cursorPos == 0 || input[cursorPos - 1] != 'w') && (cursorPos >= input.Length || input[cursorPos] != '↑'))
                        {
                            InsertSymbol("w↑", ref cursorPos, input);
                        }
                        break;

                    case ConsoleKey.LeftArrow when cursorPos > 0:
                        cursorPos--;
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        break;

                    case ConsoleKey.RightArrow when cursorPos < input.Length:
                        cursorPos++;
                        Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                        break;

                    case ConsoleKey.Backspace when cursorPos > 0:
                        input.Remove(cursorPos - 1, 1);
                        cursorPos--;
                        RedrawInput(input, cursorPos);
                        break;

                    case ConsoleKey.Enter:
                        break;

                    default:
                        // Проверка допустимых символов
                        if (IsValidLSASymbol(keyInfo.KeyChar))
                        {
                            var symbol = keyChar.ToString().ToUpper();

                            // Обработка меток 'н' и 'к'
                            if (symbol == "Н")
                            {
                                symbol = symbol.ToLower();
                                if (input.Length != 0 && !input.ToString().StartsWith("Yн ") && input[cursorPos - 1] == 'Y' && input.ToString().Count(c => c == 'н') == 0)
                                    InsertSymbol(symbol, ref cursorPos, input);
                                break;
                            }
                            else if (symbol == "К")
                            {
                                symbol = symbol.ToLower();
                                if (input.Length != 0 && input[cursorPos - 1] == 'Y' && input.ToString().Count(c => c == 'к') == 0)
                                    InsertSymbol(symbol, ref cursorPos, input);
                                break;
                            }

                            // Вставка Yн в начало
                            else if (symbol == "Y" && input.Length == 0)
                            {
                                InsertSymbol("Yн ", ref cursorPos, input);
                                break;
                            }

                            // Обработка цифр: добавляем пробел только если следующий символ не пробел
                            else if (char.IsDigit(keyChar))
                            {
                                if (cursorPos < input.Length && input[cursorPos] == ' ')
                                    InsertSymbol($"{keyChar}", ref cursorPos, input);
                                else
                                    InsertSymbol($"{keyChar} ", ref cursorPos, input);
                                break;
                            }

                            // Обработка пробелов: пропускаем дубликаты
                            else if (char.IsWhiteSpace(keyChar))
                            {
                                if (cursorPos > 0 && input[cursorPos - 1] != ' ')
                                    InsertSymbol(" ", ref cursorPos, input);
                                else if (cursorPos == 0)
                                {
                                    // Пробел в начале не допускается
                                }
                                break;
                            }

                            else if (symbol == "↑" && input.Length != 0 && input[cursorPos - 1] != '↑')
                            {
                                char cc = input[cursorPos - 1];
                                InsertSymbol("↑", ref cursorPos, input);
                                break;
                            }

                            else if (symbol != "↑")
                                InsertSymbol(symbol, ref cursorPos, input);
                        }
                        break;
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            Console.Write('\n');
            return input.ToString();
        }

        #endregion





        #region Вывод текста и информации

        private static void PrintInputRules()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('\n');
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  ПРАВИЛА РАБОТЫ С ПРОГРАММОЙ                 ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Допустимые элементы:                                        ║");
            Console.WriteLine("║  ┌───────────────────────────────────────────────────────┐   ║");
            Console.WriteLine("║  │ • Y{i}  - операторная вершина с индексом i            │   ║");
            Console.WriteLine("║  │ • X{i}  - условная вершина с индексом i               │   ║");
            Console.WriteLine("║  │ • ↑{i}  - оператор условного перехода                 │   ║");
            Console.WriteLine("║  │ • w↑{i} - оператор безусловного перехода              │   ║");
            Console.WriteLine("║  │ • ↓{i}  - точка перехода с индексом i                 │   ║");
            Console.WriteLine("║  │ • i     - неотрицательное число (индекс элемента)     │   ║");
            Console.WriteLine("║  └───────────────────────────────────────────────────────┘   ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Допустимые символы:                                         ║");
            Console.WriteLine("║  ┌───────────────────────────────────────────────────────┐   ║");
            Console.WriteLine("║  │  Y,  X,  |,  (,  ),  w,  ↑,  ↓,  цифры,  пробелы      │   ║");
            Console.WriteLine("║  └───────────────────────────────────────────────────────┘   ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Особенности ввода:                                          ║");
            Console.WriteLine("║  ┌──────────────────────────────────────────────────────────┐║");
            Console.WriteLine("║  │ 1) Стрелки ↑/↓ вводятся клавишами ▲/▼                    │║");
            Console.WriteLine("║  │ 2) Присутствует 'умное' автоматическое дополнение        │║");
            Console.WriteLine("║  │вводимого текста ЛСА                                      │║");
            Console.WriteLine("║  │ 3) Вставка через Ctrl + V работает только на английской  │║");
            Console.WriteLine("║  │раскладке, она устанавливается автоматически              │║");
            Console.WriteLine("║  │ 4) Автоматическая коррекция формата                      │║");
            Console.WriteLine("║  └──────────────────────────────────────────────────────────┘║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Примеры корректных ЛСА:                                     ║");
            Console.WriteLine("║  ┌──────────────────────────────────────────────────┐        ║");
            Console.WriteLine("║  │ 1) Yн X0 (↑1 | Y0 w↑2) ↓1 Y1 ↓2 Yк               │        ║");
            Console.WriteLine("║  │ 2) Yн ↓1 X1 ↑1 Y1 ↓2 Y2 X2 ↑2 Y4 X3 ↑3 Y3 ↓3 Yк  │        ║");
            Console.WriteLine("║  │ 3) Yн X0 ↑1 X1 ↑2 Y1 w↑2 ↓1 Y0 ↓2 Y2 X2 ↑2 Yк    │        ║");
            Console.WriteLine("║  └──────────────────────────────────────────────────┘        ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Режимы работы с автоматами                                  ║");
            Console.WriteLine("║  ┌────────────────────────────────────────────────────────┐  ║");
            Console.WriteLine("║  │ 1) Последовательный ввод значений логических условий X │  ║");
            Console.WriteLine("║  │и вывод результата каждого шага моделирования       (M1)│  ║");
            Console.WriteLine("║  │ 2) Ввод значений всех логических условий Х и вывод     │  ║");
            Console.WriteLine("║  │результата моделирования                            (M2)│  ║");
            Console.WriteLine("║  │ 3) Полный перебор всех значений логических условий Х   │  ║");
            Console.WriteLine("║  │и вывод результата моделирования                    (M3)│  ║");
            Console.WriteLine("║  └────────────────────────────────────────────────────────┘  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("\n►► Нажмите любую клавишу... ◄◄\n");
            Console.ResetColor();
            Console.ReadKey(true);
            ClearLastLine();
            Console.WriteLine();
        }



        private static void HandleInput()
        {
            var key = Console.ReadKey(true).Key;

            switch (_currentMenuState)
            {
                case MenuState.Main:
                    HandleMainMenu(key);
                    break;

                case MenuState.Modes:
                    HandleModesMenu(key);
                    break;
            }
        }

        private static void RenderMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            switch (_currentMenuState)
            {
                case MenuState.Main:
                    Console.WriteLine("╔═══════════════════════════════════╗");
                    Console.WriteLine("║           ГЛАВНОЕ МЕНЮ            ║");
                    Console.WriteLine("╠═══════════════════════════════════╣");
                    Console.WriteLine("║ '1': Новый алгоритм               ║");
                    Console.WriteLine("║ '2': Выбор режима работы          ║");
                    Console.WriteLine("║ '3': Информация об алгоритме      ║");
                    Console.WriteLine("║ '4': Правила работы с программой  ║");
                    Console.WriteLine("║ '5', 'Esc': Выход из программы    ║");
                    Console.WriteLine("╚═══════════════════════════════════╝");
                    break;

                case MenuState.Modes:
                    Console.WriteLine("╔═══════════════════════════════════╗");
                    Console.WriteLine("║           РЕЖИМЫ РАБОТЫ           ║");
                    Console.WriteLine("╠═══════════════════════════════════╣");
                    Console.WriteLine("║ '1': M1  - Последовательный ввод  ║");
                    Console.WriteLine("║ '2': M2  - Однократный ввод       ║");
                    Console.WriteLine("║ '3': M3  - Полный перебор         ║");
                    Console.WriteLine("║ '4', 'Esc': Выход в главное меню  ║");
                    Console.WriteLine("╚═══════════════════════════════════╝");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("\n►► Нажмите на клавишу в ' ' для выбора опции: ");
            Console.ResetColor();
        }

        private static void HandleMainMenu(ConsoleKey key)
        {
            ClearLastLine();
            Console.WriteLine();

            switch (key)
            {
                case ConsoleKey.D1:
                    LoadNewLSA();
                    break;

                case ConsoleKey.D2 when _automaton != null:
                    _currentMenuState = MenuState.Modes;
                    break;

                case ConsoleKey.D3 when _automaton != null:
                    _automaton.PrintAlgorithmInfo();
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\nНажмите любую клавишу...\n");
                    Console.ReadKey(true);
                    break;

                case ConsoleKey.D4:
                    PrintInputRules();
                    break;

                case ConsoleKey.D5:
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
            }
        }

        private static void HandleModesMenu(ConsoleKey key)
        {
            if (_automaton == null) 
                return;

            ClearLastLine();
            Console.WriteLine();

            switch (key)
            {
                case ConsoleKey.D1:
                    _automaton.RunInteractive();
                    ReturnToMain();
                    break;

                case ConsoleKey.D2:
                    _automaton.RunOnetime();
                    ReturnToMain();
                    break;

                case ConsoleKey.D3:
                    _automaton.RunToGetAllResults();
                    ReturnToMain();
                    break;

                case ConsoleKey.D4:
                case ConsoleKey.Escape:
                    _currentMenuState = MenuState.Main;
                    break;
            }
        }

        #endregion





        #region Вспомогательные методы

        private static void InsertSymbol(string symbol, ref int cursorPos, StringBuilder input)
        {
            input.Insert(cursorPos, symbol);
            cursorPos += symbol.Length;
            RedrawInput(input, cursorPos);
        }

        private static void RedrawInput(StringBuilder input, int cursorPos)
        {
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, currentTop);
            Console.Write(input.ToString());
            Console.SetCursorPosition(Math.Min(cursorPos, input.Length), currentTop);
        }

        private static void ChangeLayout()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Загружаем английскую раскладку (en-US)
            IntPtr hkl = LoadKeyboardLayout("00000409", KLF_ACTIVATE);

            // Получаем хэндл активного окна (не обязательно консольного)
            IntPtr hWnd = GetForegroundWindow();

            // Отправляем сообщение о смене раскладки
            PostMessage(hWnd, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, hkl);

            Console.Write("\n►► Раскладка клавиатуры была автоматически изменена на английскую (\"en-US\") ◄◄\n\n");
            Console.ResetColor();
        }

        private static bool IsValidLSASymbol(char c)
        {
            return 
                c.ToString().ToUpper() == "Y" || c.ToString().ToUpper() == "X" ||
                c.ToString().ToLower() == "н" || c.ToString().ToLower() == "к" ||
                c == ' ' || c == '(' || c == ')' || c == '|' ||
                char.IsDigit(c) || c == '↑' || c == '↓';
        }



        private static void LoadNewLSA()
        {
            try
            {
                Console.Clear();
                var lsaString = LSAInputHandler.ReadLSAString();
                _automaton = LSAParser.Parse(lsaString);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"\n►► ЛСА успешно загружена! ◄◄\n");
                Console.ResetColor();
            }
            catch (ParsingAggregateException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"\n►► Ошибка парсинга: {ex.Message}◄◄\n");
                Console.ResetColor();
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("\n►► Нажмите любую клавишу...\n");
                Console.ResetColor();
                Console.ReadKey(true);
            }
        }

        private static void ReturnToMain()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("\n►► Нажмите любую клавишу для возврата...\n");
            Console.ResetColor();
            Console.ReadKey(true);
            ClearLastLine();
            Console.WriteLine();
            _currentMenuState = MenuState.Main;
        }

        private static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        #endregion
    }
}
