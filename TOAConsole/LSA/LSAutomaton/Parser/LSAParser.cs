using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.Elements.Jumps;
using TOAConsole.LSA.Elements.Vertexes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TOAConsole.LSA.LSAutomaton.Parser
{
    internal static class LSAParser
    {
        /// <summary>
        /// Преобразует входную строку <paramref name="input"/> в объект <see cref="Automaton"/>.
        /// </summary>
        /// <param name="input">Входная строка.</param>
        /// <returns>Объект <see cref="Automaton"/>, заполненный по указанному во входной строке алгоритму</returns>
        /// <exception cref="FormatException"></exception>
        public static Automaton Parse(string input)
        {
            var errors = new List<ParsingError>();

            input = Regex.Replace(input, @"\s*([()|])\s*", " $1 ");
            var tokenPattern = @"(Yн|Yк|w↑\d+|↑\d+|↓\d+|X\d+|Y\d+|\(|\)|\|)";
            var tokens = Regex.Matches(input, tokenPattern)
                        .Cast<Match>()
                        .Select(m => m.Value.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();

            var automaton = new Automaton(tokens);

            try
            {
                if (tokens.FirstOrDefault() != "Yн")
                    AddError(errors, "ЛСА должна начинаться с Yн", 0);

                int position = 1;
                var start = new StartVertex(position);
                automaton.AddElement(start);
                ILSAElement? previousElement = start;

                ParseElements(automaton, ref position, ref previousElement, ref errors);
                
                ValidateStartEndUniqueness(automaton, ref errors);
                ValidateJumpOperators(automaton, ref errors);
                ValidateConditionalVertices(automaton, ref errors);
            }
            catch (ParsingAggregateException) { }

            if (errors.Count > 0)
                throw new ParsingAggregateException(errors);

            return automaton;
        }



        #region Вспомогательные методы парсинга

        /// <summary>
        /// Создаёт и связывает между собой все элементы ЛСА в соответствии с устройством токенов, представляющих их.
        /// <br></br> Созданные элементы добавляются в объект <see cref="Automaton"/>.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="previousElement">Ссылка на предыдущий элемент ЛСА (возможно значение <see cref="null"/>).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void ParseElements(Automaton automaton, ref int position, ref ILSAElement? previousElement, ref List<ParsingError> errors)
        {
            while (position < automaton.Tokens.Count)
            {
                var token = automaton.Tokens[position];
                if (token == "Yк")
                {
                    var end = new EndVertex(position);
                    automaton.AddElement(end);
                    LinkPrevious(previousElement, end, ref errors);
                    position++;
                    break;
                }

                var element = ParseElement(automaton, ref position, ref errors);
                if (element != null)
                {
                    LinkPrevious(previousElement, element, ref errors);
                    previousElement = element;

                    // Добавлено: Рекурсивная обработка цепочки элементов
                    while (position < automaton.Tokens.Count && !new[] { ")", "|", "Yк" }.Contains(automaton.Tokens[position]))
                    {
                        var nextElement = ParseElement(automaton, ref position, ref errors);
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
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Новый элемент ЛСА (возможно значение <see cref="null"/>).</returns>
        /// <exception cref="FormatException"></exception>
        private static ILSAElement? ParseElement(Automaton automaton, ref int position, ref List<ParsingError> errors)
        {
            if (position >= automaton.Tokens.Count) return null;

            var token = automaton.Tokens[position++];
            switch (token)
            {
                case "Yн":
                    AddError(errors, "Yн может быть только в начале ЛСА", position);
                    return null;

                case "Yк":
                    return HandleEndVertex(automaton, position - 1, ref errors);

                case var x when x.StartsWith("X"):
                    return ParseConditionalVertex(automaton, ref position, int.Parse(x[1..]), ref errors);

                case var y when y.StartsWith("Y"):
                    if (y == "Yн")
                    {
                        AddError(errors, "Yн может быть только в начале", position - 1);
                        return null;
                    }

                    var opVertex = CreateOperatorVertex(automaton, position - 1, int.Parse(y[1..]), ref errors);
                    return opVertex;

                case var jp when jp.StartsWith("↓"):
                    return CreateJumpPoint(automaton, position - 1, int.Parse(jp[1..]), ref errors);

                case var jo when jo.StartsWith("↑") || jo.StartsWith("w↑"):
                    var isUnconditional = jo.StartsWith("w↑");
                    return CreateJumpOperator(automaton, position - 1, int.Parse(jo[(isUnconditional ? 2 : 1)..]), isUnconditional, ref errors);

                case "(":
                    return ParseSubAlgorithm(automaton, ref position, ref errors);

                case ")":
                case "|":
                    return null;

                default:
                    AddError(errors, $"Неизвестный токен: \"{token}\"", position);
                    return null;
            }
        }

        /// <summary>
        /// Создаёт и возвращает условную вершину на основе токена, стоящего на позиции <paramref name="position"/> в списке токенов объекта <paramref name="automaton"/>.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="index">Индекс этой условной вершины.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Новая условная вершина с индексом <paramref name="index"/>.</returns>
        /// <exception cref="FormatException"></exception>
        private static ConditionalVertex ParseConditionalVertex(Automaton automaton, ref int position, int index, ref List<ParsingError> errors)
        {
            var vertex = new ConditionalVertex(index, position);
            automaton.AddElement(vertex);

            if (position < automaton.Tokens.Count && automaton.Tokens[position] == "(")
            {
                position++;
                vertex.LBS = ParseSubAlgorithm(automaton, ref position, ref errors);
                if (position >= automaton.Tokens.Count || automaton.Tokens[position++] != "|")
                    AddError(errors, $"Ожидается | после LBS \"X{index}\"", position - 1);

                vertex.RBS = ParseSubAlgorithm(automaton, ref position, ref errors);
                if (position >= automaton.Tokens.Count || automaton.Tokens[position++] != ")")
                    AddError(errors, $"Ожидается ) после RBS \"X{index}\"", position - 1);
            }
            else
            {
                vertex.LBS = ParseElement(automaton, ref position, ref errors);
                if (vertex.LBS == null)
                    AddError(errors, $"Ожидается LBS для \"X{index}\"", position - 1);

                vertex.RBS = ParseElement(automaton, ref position, ref errors);
                if (vertex.RBS == null)
                    AddError(errors, $"Ожидается RBS для \"X{index}\"", position - 1);
            }

            return vertex;
        }

        /// <summary>
        /// Работает со вложенным алгоритмом, являющимся ветвью какой-либо условной вершины.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Новый элемент ЛСА (возможно значение <see cref="null"/>), являющийся потомком или началом одной из двух ветвей условной вершины.</returns>
        /// <exception cref="FormatException"></exception>
        private static ILSAElement? ParseSubAlgorithm(Automaton automaton, ref int position, ref List<ParsingError> errors)
        {
            if (position >= automaton.Tokens.Count)
            {
                AddError(errors, "Неожиданный конец данных в субалгоритме", position);
                return null;
            }

            var startElement = ParseElement(automaton, ref position, ref errors);
            var current = startElement;

            while (position < automaton.Tokens.Count && !new[] { ")", "|" }.Contains(automaton.Tokens[position]))
            {
                var next = ParseElement(automaton, ref position, ref errors);
                if (current is LSABaseElement baseCurrent)
                {
                    if (baseCurrent.Next != null)
                    {
                        AddError(errors, $"Элемент \"{baseCurrent.Id}\" уже имеет следующий элемент", position - 1);
                        return null;
                    }
                    baseCurrent.Next = next;
                }
                current = next;
            }

            return startElement;
        }



        #region Методы, поддерживающие создание, обработку и связывание вершин.

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="automaton"/> конечную вершину.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="pos">Текущая позицию в списке токенов для прохода по нему.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенная конечная вершина ЛСА.</returns>
        /// <exception cref="FormatException"></exception>
        private static ILSAElement? HandleEndVertex(Automaton automaton, int pos, ref List<ParsingError> errors)
        {
            if (automaton.Elements.OfType<EndVertex>().Any())
            {
                AddError(errors, "ЛСА должна содержать только одну \"Yк\"", pos);
                return null;
            }

            var end = new EndVertex(pos);
            automaton.AddElement(end);
            return end;
        }

        /// <summary>
        /// Связывает между собой предыдущий (<paramref name="previous"/>) и текущий (<paramref name="current"/>) элементы ЛСА.
        /// </summary>
        /// <param name="previous">Предыдущий элемент ЛСА.</param>
        /// <param name="current">Текущий элемент ЛСА.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param> 
        /// <exception cref="FormatException"></exception>
        private static void LinkPrevious(ILSAElement? previous, ILSAElement current, ref List<ParsingError> errors)
        {
            if (previous is LSABaseElement prevBase)
            {
                if (prevBase.Next != null)
                {
                    AddError(errors, $"Ошибка связки: элемент \"{prevBase.Id}\" уже имеет следующий элемент (потомка), его позиция: {prevBase.Next.Position}", prevBase.Next.Position);
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
        /// <param name="index">Индекс этой операторной вершины.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенная операторная вершина с индексом <paramref name="index"/>.</returns>
        private static OperatorVertex CreateOperatorVertex(Automaton automaton, int pos, int index, ref List<ParsingError> errors)
        {
            var vertex = new OperatorVertex(index, pos);
            automaton.AddElement(vertex);
            return vertex;
        }

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="automaton"/> точку перехода.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="pos">Текущая позиция в списке токенов для прохода по нему.</param>
        /// <param name="index">Индекс этой точки перехода.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенная точка перехода с индексом <paramref name="index"/>.</returns>
        private static JumpPoint CreateJumpPoint(Automaton automaton, int pos, int index, ref List<ParsingError> errors)
        {
            var jp = new JumpPoint(index, pos);
            automaton.AddElement(jp);
            return jp;
        }

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="automaton"/> оператор перехода.
        /// </summary>
        /// <param name="automaton">Объект <see cref="Automaton"/>, содержащий заполненный список токенов.</param>
        /// <param name="pos">Текущая позиция в списке токенов для прохода по нему.</param>
        /// <param name="index">Индекс этоого оператора перехода.</param>
        /// <param name="isUnconditional">Определяет, является ли переход по данному оператору безусловным.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенный оператор перехода с индексом <paramref name="index"/>.</returns>
        private static JumpOperator CreateJumpOperator(Automaton automaton, int pos, int index, bool isUnconditional, ref List<ParsingError> errors)
        {
            var jo = new JumpOperator(index, pos, isUnconditional);
            automaton.AddElement(jo);

            if (pos - 1 < automaton.Elements.Count() && automaton.Elements[pos - 1] is OperatorVertex ov)
                LinkPrevious(ov, jo, ref errors);
            return jo;
        }

        #endregion

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
                    AddError(errors, $"Точка перехода \"↓{jo.JumpIndex}\" для оператора \"{jo.Id}\" не найдена", jo.Position);
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
                    string ex = $"Условная вершина \"{cv.Id}\" имеет неопределённых потомков: ";
                    if (cv.LBS == null) ex += "\n\tОтсутствует левый потомок";
                    if (cv.RBS == null) ex += "\n\tОтсутствует правый потомок";
                    AddError(errors, ex, cv.Position);
                }
            }
        }

        #endregion
    }
}
