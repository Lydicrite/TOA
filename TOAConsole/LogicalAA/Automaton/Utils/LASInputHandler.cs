using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LogicalAA.Automaton.ParserSystem;
using TOAConsole.LogicalAA.Automaton.Utils.MAS;
using Windows.ApplicationModel.DataTransfer;

namespace TOAConsole.LogicalAA.Automaton.Utils
{
    /// <summary>
    /// Класс, содержащий методы для удобной консольной работы с моделью абстрактного автомата <see cref="Automaton"/>
    /// </summary>
    internal static class LASInputHandler
    {
        #region Поля обработчика ввода

        /// <summary>
        /// Объект <see cref="Automaton"/>, с которым работает программа.
        /// </summary>
        private static Automaton? _automaton;
        /// <summary>
        /// Текущее состояние меню
        /// </summary>
        private static MenuState _currentMenuState = MenuState.Main;

        /// <summary>
        /// Перечисление состояний меню программы.
        /// </summary>
        internal enum MenuState
        {
            /// <summary>
            /// Главное меню.
            /// </summary>
            Main,
            /// <summary>
            /// Меню моделирования в различных режимах.
            /// </summary>
            Modes
        }

        #endregion



        #region Константы и импорт библиотек WinAPI

        /// <summary>
        /// Получает дескриптор активного окна (окна, которое в данный момент находится в фокусе).
        /// </summary>
        /// <returns>Дескриптор активного окна.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Загружает раскладку клавиатуры по идентификатору.
        /// </summary>
        /// <param name="pwszKLID">Строка идентификатора раскладки (например, "00000409" для английской).</param>
        /// <param name="flags">Флаги загрузки (например, 0 для загрузки без активации).</param>
        /// <returns>Дескриптор загруженной раскладки.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint flags);

        /// <summary>
        /// Активирует указанную раскладку клавиатуры для входного потока.
        /// </summary>
        /// <param name="hkl">Дескриптор раскладки клавиатуры.</param>
        /// <param name="flags">Флаги активации (например, KLF_ACTIVATE).</param>
        /// <returns>True в случае успеха, иначе false.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, uint flags);

        /// <summary>
        /// Помещает сообщение в очередь сообщений указанного окна.
        /// </summary>
        /// <param name="hWnd">Дескриптор окна-получателя.</param>
        /// <param name="Msg">Код сообщения (например, WM_INPUTLANGCHANGEREQUEST).</param>
        /// <param name="wParam">Дополнительные параметры сообщения.</param>
        /// <param name="lParam">Дополнительные параметры сообщения.</param>
        /// <returns>True в случае успеха, иначе false.</returns>
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);



        // Константы для сообщений и флагов
        /// <summary>
        /// Сообщение для запроса смены раскладки клавиатуры.
        /// </summary>
        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        /// <summary>
        /// Флаг для активации раскладки клавиатуры при загрузке.
        /// </summary>
        private const uint KLF_ACTIVATE = 0x00000001;

        #endregion



        #region Ввод информации

        /// <summary>
        /// Запускает основной цикл работы программы.
        /// </summary>
        [STAThread]
        public static void ProgramCycle()
        {
            Console.Clear();
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            LASInputHandler.PrintMainInfo();

            while (true)
            {
                RenderMenu();
                HandleInput();
            }
        }

        /// <summary>
        /// Обеспечивает дружественный ввод и чтение строки, содержащей ЛСА.
        /// </summary>
        /// <returns>Строка, содержащая ЛСА.</returns>
        [STAThread]
        public static string ReadLSAString()
        {
            Console.ResetColor();
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
                        if (IsValidLASSymbol(keyInfo.KeyChar))
                        {
                            var symbol = keyChar.ToString().ToUpper();

                            // Обработка меток 'н' и 'к'
                            if (symbol == "Н")
                            {
                                symbol = symbol.ToLower();
                                if (input.Length != 0 && !input.ToString().StartsWith("Yн ") && input[cursorPos - 1] == 'Y' && !input.ToString().Any(c => c == 'н'))
                                    InsertSymbol(symbol, ref cursorPos, input);
                                break;
                            }
                            else if (symbol == "К")
                            {
                                symbol = symbol.ToLower();
                                if (input.Length != 0 && input[cursorPos - 1] == 'Y' && !input.ToString().Any(c => c == 'к'))
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

            Console.ResetColor();
            Console.Write('\n');
            return input.ToString();
        }

        /// <summary>
        /// Поддерживает обработку ввода в различных ситуациях.
        /// </summary>
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

        /// <summary>
        /// Обрабатывает нажатие ключевых клавиш, отвечающих за начало работы каких-либо модулей программы, описанных в главном меню.
        /// </summary>
        /// <param name="key">Клавиша, нажатая пользователем.</param>
        private static void HandleMainMenu(ConsoleKey key)
        {
            ClearLastLine();
            Console.WriteLine();

            switch (key)
            {
                case ConsoleKey.D1:
                    ReloadAA();
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
                    StartMergeProcess();
                    break;

                case ConsoleKey.D5:
                    PrintMainInfo();
                    break;

                case ConsoleKey.D6:
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
            }
        }

        /// <summary>
        /// Обрабатывает нажатие ключевых клавиш, отвечающих за начало работы режимов моделирования, описанных в меню выбора режимов работы.
        /// </summary>
        /// <param name="key"></param>
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





        #region Вывод информации

        /// <summary>
        /// Выводит основную информацию и правила работы с программой.
        /// </summary>
        private static void PrintMainInfo()
        {
            Console.ResetColor();
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('\n');
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  ПРАВИЛА РАБОТЫ С ПРОГРАММОЙ                 ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Допустимые элементы:                                        ║");
            Console.WriteLine("║  ┌───────────────────────────────────────────────────────┐   ║");
            Console.WriteLine("║  │ • Y{i}        - операторная вершина с индексом i      │   ║");
            Console.WriteLine("║  │ • X{i} / P{i} - условная вершина с индексом i         │   ║");
            Console.WriteLine("║  │ • ↑{i}        - оператор условного перехода           │   ║");
            Console.WriteLine("║  │ • w↑{i}       - оператор безусловного перехода        │   ║");
            Console.WriteLine("║  │ • ↓{i}        - точка перехода с индексом i           │   ║");
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
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("║  Дополнительный функционал                                   ║");
            Console.WriteLine("║  ┌─────────────────────────────────────────────────────────┐ ║");
            Console.WriteLine("║  │ 1) Вывод основной информации об алгоритмах, моделируемых│ ║");
            Console.WriteLine("║  │исследуемыми автоматами                                  │ ║");
            Console.WriteLine("║  │ 2) Инструмент объединения нескольких автоматов на основе│ ║");
            Console.WriteLine("║  │их матричных схем                                        │ ║");
            Console.WriteLine("║  └─────────────────────────────────────────────────────────┘ ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("\n►► Нажмите любую клавишу... ◄◄\n");
            Console.ResetColor();
            Console.ReadKey(true);
            ClearLastLine();
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Выводит главное меню или вспомогательное меню в зависимости от ситуации.
        /// </summary>
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
                    Console.WriteLine("║ '4': Объединение алгоритмов       ║");
                    Console.WriteLine("║ '5': Правила работы с программой  ║");
                    Console.WriteLine("║ '6', 'Esc': Выход из программы    ║");
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

        #endregion





        #region Вспомогательные методы

        /// <summary>
        /// Запускает модуль объединения автоматов.
        /// </summary>
        private static void StartMergeProcess()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═════════════════════════════════════════════════╗");
            Console.WriteLine("║              ОБЪЕДИНЕНИЕ АВТОМАТОВ              ║");
            Console.WriteLine("╠═════════════════════════════════════════════════╣");
            Console.WriteLine("║ Позволяет произвести объединение от 2 до 10     ║");
            Console.WriteLine("║ автоматов на основе объединения их матричных    ║");
            Console.WriteLine("║ схем (МСА).                                     ║");
            Console.WriteLine("║                                                 ║");
            Console.WriteLine("║ В случае успешного ввода ЛСА всех объединяемых  ║");
            Console.WriteLine("║ автоматов программа выводит информацию о каждом ║");
            Console.WriteLine("║ из них, их подготовленные к объединению МСА и   ║");
            Console.WriteLine("║ итоговую ОМСА, выполняя её упрощение и          ║");
            Console.WriteLine("║ минимизацию с помощью законов алгебры логики и  ║");
            Console.WriteLine("║ распределения сдвигов.                          ║");
            Console.WriteLine("║                                                 ║");
            Console.WriteLine("║ МСА в своих ячейках содержит формулы переходов  ║");
            Console.WriteLine("║ из Yi (строки) в Yj (столбцы).                  ║");
            Console.WriteLine("║  - '0' означает отсутствие перехода.            ║");
            Console.WriteLine("║  - '1' означает прямой переход (для стоящих     ║");
            Console.WriteLine("║ подряд Y-вершин, либо разделённых операторами и ║");
            Console.WriteLine("║ (или) точками переходов).                       ║");
            Console.WriteLine("║  - 'логическая формула' означает комбинацию     ║");
            Console.WriteLine("║ значений условных вершин, при которых из Yi в   ║");
            Console.WriteLine("║ Yj можнно перейти, минуя другие Yk.             ║");
            Console.WriteLine("╚═════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("\n►► Нажмите любую клавишу для продолжения... ");
            Console.ResetColor();
            Console.ReadKey(true);
            ClearLastLine();

            int count = ReadInt("►► Введите количество объединяемых автоматов", 2, 10);
            var schemes = new List<MatrixSchema>();
            var automatons = new List<Automaton>();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"\n\n►► --- Ввод ЛСА автомата #{i + 1} --- ◄◄");
                    Console.ResetColor();
                    var automaton = LoadSingleAA();
                    automatons.Add(automaton);
                    schemes.Add(automaton.MatrixSchema);

                }
                catch (ParsingAggregateException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"\n►► Ошибка парсинга: {ex.Message}◄◄\n");
                    Console.ResetColor();
                    i--;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n►► Ошибка: {ex.Message}◄◄\n");
                    Console.ResetColor();
                    i--;
                }
                finally
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\n►► Нажмите любую клавишу для продолжения...");
                    Console.ResetColor();
                    Console.ReadKey(true);
                    ClearLastLine();
                }
            }

            var binaryCodes = new List<string>();
            var varCodes = new List<string>();
            var newVariables = new HashSet<string>();

            var merged = MASCombiner.PrepareForCombine(schemes);
            var combined = MASCombiner.CombineSchemas(merged, ref binaryCodes, ref varCodes, ref newVariables);

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write
                (
                    $"\n►► В объединении участвуют {count} автомата, отсюда: " +
                    $"\n\t► достаточное количество новых переменных для объединения: {newVariables.Count} ◄" +
                    $"\n\t► новые переменные: {string.Join(", ", newVariables)} ◄\n◄◄\n"
                );

            for (int i = 0; i < count; i++)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write
                        (
                            $"\n►►" +
                            $"\nПодготовленная к объединению МСА автомата #{i + 1}" +
                            $"\n\t► кодирована следующим кодом: \"{binaryCodes[i]}\" ({varCodes[i]}) ◄" +
                            $"\n◄◄\n"
                        );
                    Console.ResetColor();
                    Console.Write($"\n{merged[i]}\n");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n►► Ошибка: {ex.Message}◄◄\n");
                    Console.ResetColor();
                    i--;
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine
                (
                    $"\n►►" +
                    $"\nОбъединённая МСА" +
                    $"\n\t► содержит неупрощённые логические формулы переходов из Yᵢ в Yⱼ ◄" +
                    $"\n◄◄\n"
                );
            Console.ResetColor();
            Console.WriteLine(combined.ToString());
            Console.ResetColor();

            var simplified = combined.Simplify();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine
                (
                    $"\n►►" +
                    $"\nУпрощённая МСА" +
                    $"\n\t► содержит упрощённые по законам алгебры логики логические формулы переходов из Yᵢ в Yⱼ ◄" +
                    $"\n◄◄\n"
                );
            Console.ResetColor();
            Console.WriteLine(simplified.ToString());
            Console.ResetColor();

            var minimized = simplified.Minimize();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine
                (
                    $"\n►►" +
                    $"\nМинимизированная МСА" +
                    $"\n\t► содержит упрощённые по законам алгебры логики и оптимизированные с помощью распределения сдвигов логические формулы переходов из Yᵢ в Yⱼ ◄" +
                    $"\n◄◄\n"
                );
            Console.ResetColor();
            Console.WriteLine(minimized.ToString());
            Console.ResetColor();

            ReturnToMain();
        }




        /// <summary>
        /// Вставляет символ <paramref name="symbol"/> в <see cref="StringBuilder"/> <paramref name="input"/> на позицию курсора <paramref name="cursorPos"/>.
        /// </summary>
        /// <param name="symbol">Символ для вставки.</param>
        /// <param name="cursorPos">Ссылка на позицию курсора.</param>
        /// <param name="input"><see cref="StringBuilder"/>, содержащий вводимую строку.</param>
        private static void InsertSymbol(string symbol, ref int cursorPos, StringBuilder input)
        {
            input.Insert(cursorPos, symbol);
            cursorPos += symbol.Length;
            RedrawInput(input, cursorPos);
        }

        /// <summary>
        /// Стирает и заново вводит в консоль строку из <paramref name="input"/>.
        /// </summary>
        /// <param name="input"><see cref="StringBuilder"/>, содержащий вводимую строку.</param>
        /// <param name="cursorPos">Текущая позиция курсора.</param>
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

        /// <summary>
        /// Меняет раскладку клавиатуры на английскую ("en-US").
        /// </summary>
        private static void ChangeLayout()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            Console.ResetColor();
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

        /// <summary>
        /// Проверяет, является ли исследуемый символ <paramref name="c"/> корректным для использования в ЛСА.
        /// </summary>
        /// <param name="c">Исследуемый символ.</param>
        /// <returns><see langword="true"/>, если <paramref name="c"/> является орректным для использования в ЛСА, иначе <see langword="false"/>.</returns>
        private static bool IsValidLASSymbol(char c)
        {
            return 
                c.ToString().ToUpper() == "Y" ||
                c.ToString().ToLower() == "н" || c.ToString().ToLower() == "к" ||
                c.ToString().ToUpper() == "X" || c.ToString().ToUpper() == "P" ||
                c == ' ' || c == '(' || c == ')' || c == '|' ||
                char.IsDigit(c) || c == '↑' || c == '↓';
        }

        /// <summary>
        /// Считывает целое число с проверкой на вход в диапазон [<paramref name="min"/>, <paramref name="max"/>] и выводом приветственного сообщения <paramref name="prompt"/>.
        /// </summary>
        /// <param name="prompt">Приветственное сообщение.</param>
        /// <param name="min">Минимальное число из диапазона.</param>
        /// <param name="max">Максимальное число из диапазона.</param>
        /// <returns></returns>
        private static int ReadInt(string prompt, int min, int max)
        {
            int value;
            do
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.Write($"\n{prompt} (диапазон: [{min}, {max}]): ");

                Console.ResetColor();
            } while (!int.TryParse(Console.ReadLine(), out value) || value < min || value > max);

            return value;
        }



        /// <summary>
        /// Обрабатывает ввод автомата (для режима объединения).
        /// </summary>
        /// <returns>Новый объект <see cref="Automaton"/>.</returns>
        private static Automaton LoadSingleAA()
        {
            var lsaString = LASInputHandler.ReadLSAString();
            var automaton = LASParser.Parse(lsaString);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n►► ЛСА успешно загружена! ◄◄\n");
            Console.ResetColor();
            automaton.PrintAlgorithmInfo();
            return automaton;
        }

        /// <summary>
        /// Обрабатывает пересоздание объекта <see cref="_automaton"/>.
        /// </summary>
        private static void ReloadAA()
        {
            try
            {
                Console.Clear();
                var lsaString = LASInputHandler.ReadLSAString();
                _automaton = LASParser.Parse(lsaString);

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

        /// <summary>
        /// Возвращает программу в главное меню.
        /// </summary>
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

        /// <summary>
        /// Очищает последнюю строку в консоли.
        /// </summary>
        private static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        #endregion
    }
}
