using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TOAConsole.LogicalAA.Elements.Common;
using TOAConsole.LogicalAA.Elements.Jumps;
using TOAConsole.LogicalAA.Elements.Vertexes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TOAConsole.LogicalAA.Automaton.ParserSystem
{
    internal static class LASParser
    {
        /// <summary>
        /// Количество условных вершин в текущем выражении, подвергающемуся парсингу.
        /// </summary>
        private static int _conditionalVertexCounter = 0;

        /// <summary>
        /// Преобразует входную строку <paramref name="input"/> в объект <see cref="Automaton"/>.
        /// </summary>
        /// <param name="input">Входная строка.</param>
        /// <returns>Объект <see cref="Automaton"/>, заполненный по указанному во входной строке алгоритму</returns>
        /// <exception cref="FormatException"></exception>
        public static Automaton Parse(string input)
        {
            _conditionalVertexCounter = 0;
            var errors = new List<ParsingError>();
            var parsedPositions = new Dictionary<int, bool>();

            input = PreprocessInput(input);
            var tokens = Tokenize(input, ref errors);
            for (int i = 0; i < tokens.Count(); i++)
                parsedPositions.Add(i, false);

            var automaton = new Automaton(tokens, input);

            try
            {
                if (tokens.FirstOrDefault() != "Yн")
                    AddError(errors, "ЛСА должна начинаться с Yн", 0);

                if (!tokens.Contains("Yк"))
                    AddError(errors, "ЛСА должна содержать Yк", 0);

                int position = 1;
                var start = new StartVertex(position);
                automaton.AddElement(start);
                ILAAElement? previousElement = start;

                ParseElements(automaton, ref position, ref parsedPositions, ref previousElement, ref errors);

                ValidateJumpPoints(automaton, ref errors);
                ValidateStartEndUniqueness(automaton, ref errors);
                ValidateJumpOperators(automaton, ref errors);
                ValidateConditionalVertices(automaton, ref errors);
            }
            catch (ParsingAggregateException) { }

            if (errors.Count > 0)
                throw new ParsingAggregateException(errors);

            return automaton;
        }



        #region Предподготовка

        /// <summary>
        /// Выполняет подготовку строки к парсингу, нормализуя регистр и удаляя те символы, что не отвечают за создание элементов автомата.
        /// </summary>
        /// <param name="input">Строка, содержащая ЛСА.</param>
        /// <returns>Строка, содержащая ЛСА, отредактированная нужным образом.</returns>
        private static string PreprocessInput(string input)
        {
            // Удаляем скобки и сепараторы
            input = Regex.Replace(input, @"[()|]", " ");

            // Нормализуем регистр
            input = Regex.Replace(input, @"\b([yxp])(\d+)\b", m =>
                m.Groups[1].Value.ToUpper() + m.Groups[2].Value);

            input = Regex.Replace(input, @"\b(w↑\d+)\b", m => m.Value.ToLower());

            return Regex.Replace(input, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Выполняет токенизацию для строки с ЛСА.
        /// </summary>
        /// <param name="input">Строка, содержащая ЛСА, отредактированная нужным образом.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Список токенов ЛСА, используемых для создания элементов автомата.</returns>
        private static List<string> Tokenize(string input, ref List<ParsingError> errors)
        {
            var tokenPattern = @"(Yн|Yк|w↑\d+|↑\d+|↓\d+|X\d+|P\d+|Y\d+)";
            var matches = Regex.Matches(input, tokenPattern, RegexOptions.IgnoreCase);

            if (matches.Count == 0)
                AddError(errors, "Некорректный формат ЛСА", 0);

            return matches.Cast<Match>()
                .Select(m => m.Value)
                .ToList();
        }

        #endregion





        #region Методы парсинга элементов

        /// <summary>
        /// Создаёт и связывает между собой все элементы ЛСА в соответствии с устройством токенов, представляющих их.
        /// <br></br> Созданные элементы добавляются в объект <see cref="Automaton"/>.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="previousElement">Ссылка на предыдущий элемент ЛСА (возможно значение <see cref="null"/>).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void ParseElements 
        (
            Automaton automaton, ref int position, ref Dictionary<int, bool> parsedPositions, 
            ref ILAAElement? previousElement, ref List<ParsingError> errors
        )
        {
            while (position < automaton.Tokens.Count)
            {
                var token = automaton.Tokens[position];
                if (token == "Yк")
                {
                    var end = HandleEndVertex(automaton, position - 1, ref parsedPositions, ref errors);
                    LinkPrevious(previousElement, end, ref errors);
                    position++;
                    break;
                }

                var element = ParseElement(automaton, ref position, ref parsedPositions, ref errors);
                if (element != null)
                {
                    LinkPrevious(previousElement, element, ref errors);
                    previousElement = element;

                    while (position < automaton.Tokens.Count)
                    {
                        var nextElement = ParseElement(automaton, ref position, ref parsedPositions, ref errors);
                        if (nextElement != null)
                        {
                            LinkPrevious(element, nextElement, ref errors);
                            previousElement = nextElement;
                            element = nextElement;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Создаёт и возвращает элемент ЛСА на основе токена, стоящего на позиции <paramref name="position"/> в списке токенов объекта <paramref name="automaton"/>.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Новый элемент ЛСА (возможно значение <see cref="null"/>).</returns>
        /// <exception cref="FormatException"></exception>
        private static ILAAElement? ParseElement(Automaton automaton, ref int position, ref Dictionary<int, bool> parsedPositions, ref List<ParsingError> errors)
        {
            if (position >= automaton.Tokens.Count) return null;

            var token = automaton.Tokens[position++];
            switch (token)
            {
                case "Yн":
                    AddError(errors, "Yн может быть только в начале ЛСА", position);
                    return null;

                case "Yк":
                    return HandleEndVertex(automaton, position - 1, ref parsedPositions, ref errors);

                case var x when x.StartsWith("X") || x.StartsWith("P"):
                    string prefix = x.Substring(0, 1);
                    int originalNumber = int.Parse(x.Substring(1));
                    return ParseConditionalVertex(automaton, ref position, ref parsedPositions, prefix, originalNumber, ref errors);

                case var y when y.StartsWith("Y"):
                    if (y == "Yн")
                    {
                        AddError(errors, "Yн может быть только в начале", position - 1);
                        return null;
                    }

                    var opVertex = CreateOperatorVertex(automaton, position - 1, ref parsedPositions, int.Parse(y[1..]), ref errors);
                    return opVertex;

                case var jp when jp.StartsWith("↓"):
                    return CreateJumpPoint(automaton, position - 1, ref parsedPositions, int.Parse(jp[1..]), ref errors);

                case var jo when jo.StartsWith("↑") || jo.StartsWith("w↑"):
                    var isUnconditional = jo.StartsWith("w↑");
                    return CreateJumpOperator(automaton, position - 1, ref parsedPositions, int.Parse(jo[(isUnconditional ? 2 : 1)..]), isUnconditional, ref errors);

                default:
                    AddError(errors, $"Неизвестный токен: \"{token}\"", position);
                    return null;
            }
        }

        #endregion





        #region Парсинг условной вершины

        /// <summary>
        /// Создаёт и возвращает условную вершину на основе токена, стоящего на позиции <paramref name="position"/> в списке токенов объекта <paramref name="automaton"/>.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="index">Индекс этой условной вершины.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Новая условная вершина с индексом <paramref name="index"/>.</returns>
        /// <exception cref="FormatException"></exception>
        private static ConditionalVertex ParseConditionalVertex
        (
            Automaton automaton, ref int position, ref Dictionary<int, bool> parsedPositions,
            string prefix, int originalNumber, ref List<ParsingError> errors
        )
        {
            int index = ++_conditionalVertexCounter;
            var vertex = new ConditionalVertex(prefix, originalNumber, index, position - 1);
            automaton.AddElement(vertex);

            // Парсим LBS (условный оператор ↑j)
            vertex.LBS = ParseJumpOperatorForCondition(automaton, ref position, ref parsedPositions, ref errors);
            if (vertex.LBS == null)
            {
                AddError(errors, $"Ожидается условный оператор перехода ↑j для \"X{index}\"", position);
                return vertex;
            }

            // Парсим RBS (субалгоритм)
            int initialPosition = position;
            vertex.RBS = ParseSubAlgorithm(automaton, ref position, ref parsedPositions, ref errors);

            if (vertex.RBS == null)
                AddError(errors, $"Ожидается субалгоритм для \"X{index}\"", initialPosition);

            return vertex;
        }

        /// <summary>
        /// Парсит оператор условного перехода (↑j) для Xi.LBS.
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Оператор условного перехода, являющийся левым потомком Xi.</returns>
        /// </summary>
        private static JumpOperator ParseJumpOperatorForCondition
        (
            Automaton automaton, ref int position, ref Dictionary<int, bool> parsedPositions, 
            ref List<ParsingError> errors
        )
        {
            if (position >= automaton.Tokens.Count) return null;

            var token = automaton.Tokens[position];
            if (!token.StartsWith("↑"))
            {
                AddError(errors, $"Ожидается условный оператор перехода, получен: {token}", position);
                return null;
            }

            var jo = CreateJumpOperator(automaton, position, ref parsedPositions, int.Parse(token[1..]), false, ref errors);
            parsedPositions[position] = true;
            position++;
            return jo;
        }

        /// <summary>
        /// Работает со вложенным алгоритмом, являющимся ветвью какой-либо условной вершины.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Новый элемент ЛСА (возможно значение <see cref="null"/>), являющийся потомком или началом одной из двух ветвей условной вершины.</returns>
        private static ILAAElement ParseSubAlgorithm(Automaton automaton, ref int position, ref Dictionary<int, bool> parsedPositions, ref List<ParsingError> errors)
        {
            int startPos = position;
            ILAAElement firstElement = null;
            ILAAElement currentElement = null;
            bool exitFlag = false;

            while (position < automaton.Tokens.Count && !exitFlag)
            {
                var currentToken = automaton.Tokens[position];

                // Явные выходы: ↓ или внешний Yк
                if (currentToken.StartsWith("↓") || (currentToken == "Yк" && currentElement != null))
                {
                    currentElement.Next = ParseElement(automaton, ref position, ref parsedPositions, ref errors);
                    parsedPositions[position] = true;
                    break;
                }

                if (parsedPositions.TryGetValue(position, out var isParsed) && isParsed)
                    break;

                var element = ParseElement(automaton, ref position, ref parsedPositions, ref errors);
                if (element == null) break;
                parsedPositions[position - 1] = true;

                // Обработка Yк как допустимого завершения субалгоритма
                if (element is EndVertex)
                {
                    if (firstElement == null) firstElement = element;
                    if (currentElement is LAABaseElement baseElement)
                        baseElement.Next = element;
                    exitFlag = true;
                    continue;
                }

                // Обработка безусловного перехода
                if (element is JumpOperator { IsUnconditional: true })
                {
                    if (firstElement == null) firstElement = element;
                    if (currentElement is LAABaseElement baseElement)
                        baseElement.Next = element;
                    exitFlag = true;
                    continue;
                }

                // Стандартное связывание элементов
                if (firstElement == null)
                {
                    firstElement = element;
                    currentElement = element;
                }
                else if (currentElement is LAABaseElement baseElement)
                {
                    baseElement.Next = element;
                    currentElement = element;
                }
            }

            // Обработка случая, когда Yк - единственный элемент
            if (firstElement is EndVertex)
                return firstElement;

            if (firstElement == null) {
                AddError(errors, "Субалгоритм не может быть пустым", startPos);
                return null;
            }

            return firstElement;
        }

        #endregion





        #region Методы, поддерживающие создание, обработку и связывание вершин.

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="automaton"/> конечную вершину.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="pos">Текущая позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенная конечная вершина ЛСА.</returns>
        /// <exception cref="FormatException"></exception>
        private static ILAAElement? HandleEndVertex(Automaton automaton, int pos, ref Dictionary<int, bool> parsedPositions, ref List<ParsingError> errors)
        {
            if (automaton.Elements.OfType<EndVertex>().Any())
            {
                AddError(errors, "ЛСА должна содержать только одну \"Yк\"", pos);
                return null;
            }

            var end = new EndVertex(pos);
            automaton.AddElement(end);
            parsedPositions[pos] = true;
            return end;
        }

        /// <summary>
        /// Связывает между собой предыдущий (<paramref name="previous"/>) и текущий (<paramref name="current"/>) элементы ЛСА.
        /// </summary>
        /// <param name="previous">Предыдущий элемент ЛСА.</param>
        /// <param name="current">Текущий элемент ЛСА.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param> 
        /// <exception cref="FormatException"></exception>
        private static void LinkPrevious(ILAAElement? previous, ILAAElement current, ref List<ParsingError> errors)
        {
            if (previous is LAABaseElement prevBase)
            {
                if (prevBase.Next != null && prevBase.Next != current)
                {
                    AddError(errors, $"Ошибка связки: элемент \"{prevBase.ID}\" уже имеет следующий элемент (потомка), его позиция: {prevBase.Next.Position}", prevBase.Next.Position);
                    return;
                }
                prevBase.Next = current;
            }
        }

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="automaton"/> операторную вершину.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="pos">Текущая позиция в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="index">Индекс этой операторной вершины.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенная операторная вершина с индексом <paramref name="index"/>.</returns>
        private static OperatorVertex CreateOperatorVertex
        (
            Automaton automaton, int pos, ref Dictionary<int, bool> parsedPositions, 
            int index, ref List<ParsingError> errors
        )
        {
            var vertex = new OperatorVertex(index, pos);
            automaton.AddElement(vertex);
            parsedPositions[pos] = true;
            return vertex;
        }

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="automaton"/> точку перехода.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="pos">Текущая позиция в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="index">Индекс этой точки перехода.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенная точка перехода с индексом <paramref name="index"/>.</returns>
        private static JumpPoint CreateJumpPoint(Automaton automaton, int pos, ref Dictionary<int, bool> parsedPositions, int index, ref List<ParsingError> errors)
        {
            var jp = new JumpPoint(index, pos);
            automaton.AddElement(jp);
            parsedPositions[pos] = true;
            return jp;
        }

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="automaton"/> оператор перехода.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="pos">Текущая позиция в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="index">Индекс этоого оператора перехода.</param>
        /// <param name="isUnconditional">Определяет, является ли переход по данному оператору безусловным.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенный оператор перехода с индексом <paramref name="index"/>.</returns>
        private static JumpOperator CreateJumpOperator
        (
            Automaton automaton, int pos, ref Dictionary<int, bool> parsedPositions, 
            int index, bool isUnconditional, ref List<ParsingError> errors
        )
        {
            var jo = new JumpOperator(index, pos, isUnconditional);
            automaton.AddElement(jo);
            parsedPositions[pos] = true;

            if (pos - 1 < automaton.Elements.Count() && automaton.Elements[pos - 1] is OperatorVertex ov)
                LinkPrevious(ov, jo, ref errors);
            return jo;
        }

        #endregion





        #region Проверки и обработка ошибок

        /// <summary>
        /// Добавляет в список ошибок <paramref name="errors"/> новую ошибку на позиции <paramref name="position"/> с сообщением <paramref name="message"/>.
        /// </summary>
        /// <param name="errors">Список ошибок парсинга.</param>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="position">Позиция в списке токенов, на которой произошла ошибка.</param>
        private static void AddError(List<ParsingError> errors, string message, int position)
        {
            errors.Add(new ParsingError(message, position));
        }

        /// <summary>
        /// Проверяет единственность существования начальной и конечной вершин в списке элементов <paramref name="automaton"/>.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/> для проверки.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <exception cref="FormatException"></exception>
        private static void ValidateStartEndUniqueness(Automaton automaton, ref List<ParsingError> errors)
        {
            var startVertexes = automaton.Elements.OfType<StartVertex>();
            var startCount = startVertexes.Count();
            if (startCount != 1)
                AddError
                (
                    errors,
                        $"ЛСА должна содержать ровно одну \"Yн\", найдено: {startCount} на позициях: " +
                        $"{{ " + string.Join(", ", startVertexes.Select(v => v.Position)) + $" }}",
                    0
                );

            var endVertexes = automaton.Elements.OfType<EndVertex>();
            var endCount = endVertexes.Count();
            if (endCount != 1)
                AddError
                (
                    errors,
                        $"ЛСА должна содержать ровно одну \"Yк\", найдено: {endCount} на позициях: " +
                        $"{{ " + string.Join(", ", endVertexes.Select(v => v.Position)) + $" }}",
                    0
                );
        }

        /// <summary>
        /// Проверяет, для всех ли операторов перехода определены соответствующие им точки перехода.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/> для проверки.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param> 
        /// <exception cref="InvalidOperationException"></exception>
        private static void ValidateJumpOperators(Automaton automaton, ref List<ParsingError> errors)
        {
            foreach (var jo in automaton.Elements.OfType<JumpOperator>())
            {
                if (!automaton.JumpPoints.ContainsKey(jo.JumpIndex))
                    AddError(errors, $"Точка перехода \"↓{jo.JumpIndex}\" для оператора \"{jo.ID}\" не найдена", jo.Position);
            }
        }

        /// <summary>
        /// Проверяет, для всех ли точек перехода определены следующие элементы.
        /// <br>Этот метод существует как костыль для парсера.</br>
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/> для проверки.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param> 
        /// <exception cref="InvalidOperationException"></exception>
        private static void ValidateJumpPoints(Automaton automaton, ref List<ParsingError> errors)
        {
            foreach (var jp in automaton.Elements.OfType<JumpPoint>())
            {
                if (jp.Next == null && automaton.Elements.Count > jp.Position + 1)
                    jp.Next = automaton.Elements[jp.Position + 1];
            }
        }

        /// <summary>
        /// Проверяет, для всех ли условных вершин определены потомки по логическим значениям 0 и 1.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/> для проверки.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param> 
        /// <exception cref="InvalidOperationException"></exception>
        private static void ValidateConditionalVertices(Automaton automaton, ref List<ParsingError> errors)
        {
            foreach (var cv in automaton.Elements.OfType<ConditionalVertex>())
            {
                if (cv.LBS == null || cv.RBS == null)
                {
                    string ex = $"Условная вершина \"{cv.ID}\" имеет неопределённых потомков: ";
                    if (cv.LBS == null) ex += "\n\tОтсутствует левый потомок";
                    if (cv.RBS == null) ex += "\n\tОтсутствует правый потомок";
                    AddError(errors, ex, cv.Position);
                }
            }
        }

        #endregion
    }
}
