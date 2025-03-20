using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.Elements.Jumps;
using TOAConsole.LSA.Elements.Vertexes;

namespace TOAConsole.LSA.LSAutomaton.Parser
{
    internal class LSAParser
    {
        private readonly List<string> tokens;
        private int position;
        private readonly Automaton automaton;
        private ILSAElement previousElement;

        public LSAParser(string input)
        {
            input = Regex.Replace(input, @"\s*([()|])\s*", " $1 ");
            var tokenPattern = @"(Yн|Yк|w↑\d+|↑\d+|↓\d+|X\d+|Y\d+|\(|\)|\|)";
            tokens = Regex.Matches(input, tokenPattern)
                        .Cast<Match>()
                        .Select(m => m.Value.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();
            automaton = new Automaton();
        }

        public Automaton Parse()
        {
            if (tokens.FirstOrDefault() != "Yн")
                throw new FormatException("ЛСА должна начинаться с Yн");

            var start = new StartVertex();
            automaton.AddElement(start);
            previousElement = start;
            position = 1;

            ParseElements();

            if (tokens.LastOrDefault() != "Yк")
                throw new FormatException("ЛСА должна заканчиваться Yк");

            ValidateJumpOperators();
            ValidateConditionalVertices();

            return automaton;
        }

        private void ParseElements()
        {
            while (position < tokens.Count)
            {
                var token = tokens[position];
                if (token == "Yк")
                {
                    var end = new EndVertex();
                    automaton.AddElement(end);
                    LinkPrevious(end);
                    position++;
                    break;
                }

                var element = ParseElement();
                if (element != null)
                {
                    LinkPrevious(element);
                    previousElement = element;
                }
            }
        }

        private ILSAElement ParseElement()
        {
            if (position >= tokens.Count) return null;

            var token = tokens[position++];
            switch (token)
            {
                case "Yк":
                    throw new FormatException("Yк не может находиться внутри алгоритма");
                case var x when x.StartsWith("X"):
                    return ParseConditionalVertex(int.Parse(x[1..]));
                case var y when y.StartsWith("Y"):
                    if (y == "Yн") throw new FormatException("Yн может быть только в начале");
                    if (y == "Yк") throw new FormatException("Yк обрабатывается отдельно");
                    return CreateOperatorVertex(int.Parse(y[1..]));
                case var jp when jp.StartsWith("↓"):
                    return CreateJumpPoint(int.Parse(jp[1..]));
                case var jo when jo.StartsWith("↑") || jo.StartsWith("w↑"):
                    var isUnconditional = jo.StartsWith("w↑");
                    return CreateJumpOperator(int.Parse(jo[(isUnconditional ? 2 : 1)..]), isUnconditional);
                case "(":
                    return ParseSubAlgorithm();
                case ")":
                case "|":
                    return null;
                default:
                    throw new FormatException($"Неизвестный токен: {token}");
            }
        }

        private ConditionalVertex ParseConditionalVertex(int index)
        {
            var vertex = new ConditionalVertex(index);
            automaton.AddElement(vertex);

            if (position < tokens.Count && tokens[position] == "(")
            {
                position++;
                vertex.LBS = ParseSubAlgorithm();
                if (position >= tokens.Count || tokens[position++] != "|")
                    throw new FormatException($"Ожидается | после LBS X{index}");
                vertex.RBS = ParseSubAlgorithm();
                if (position >= tokens.Count || tokens[position++] != ")")
                    throw new FormatException($"Ожидается ) после RBS X{index}");
            }
            else
            {
                if (position + 1 >= tokens.Count)
                    throw new FormatException($"Недостаточно элементов для X{index}");

                vertex.LBS = ParseElement() ?? throw new FormatException($"Ожидается LBS для X{index}");
                vertex.RBS = ParseElement() ?? throw new FormatException($"Ожидается RBS для X{index}");
            }

            return vertex;
        }

        private ILSAElement ParseSubAlgorithm()
        {
            if (position >= tokens.Count)
                throw new FormatException("Неожиданный конец данных в субалгоритме");

            var startElement = ParseElement();
            var current = startElement;

            while (position < tokens.Count && !new[] { ")", "|" }.Contains(tokens[position]))
            {
                var next = ParseElement();
                if (current is LSABaseElement baseCurrent)
                {
                    if (baseCurrent.Next != null)
                        throw new FormatException($"Элемент {baseCurrent.Id} уже имеет следующий элемент");
                    baseCurrent.Next = next;
                }
                current = next;
            }

            return startElement;
        }




        private OperatorVertex CreateOperatorVertex(int index)
        {
            var vertex = new OperatorVertex(index);
            automaton.AddElement(vertex);
            return vertex;
        }

        private JumpPoint CreateJumpPoint(int index)
        {
            var jp = new JumpPoint(index);
            automaton.AddElement(jp);
            return jp;
        }

        private JumpOperator CreateJumpOperator(int index, bool isUnconditional)
        {
            var jo = new JumpOperator(index, isUnconditional);
            automaton.AddElement(jo);
            return jo;
        }

        private void LinkPrevious(ILSAElement current)
        {
            if (previousElement is LSABaseElement prevBase)
            {
                if (prevBase.Next != null)
                    throw new FormatException($"Элемент {prevBase.Id} уже имеет следующий элемент");
                prevBase.Next = current;
            }
        }

        private void ValidateJumpOperators()
        {
            var jumpOperators = automaton.Elements.OfType<JumpOperator>();
            foreach (var jo in jumpOperators)
            {
                if (!automaton.JumpPoints.ContainsKey(jo.JumpIndex))
                    throw new InvalidOperationException($"Точка перехода ↓{jo.JumpIndex} не найдена");
            }
        }

        private void ValidateConditionalVertices()
        {
            var conditionals = automaton.Elements.OfType<ConditionalVertex>();
            foreach (var cv in conditionals)
            {
                if (cv.LBS == null || cv.RBS == null)
                    throw new InvalidOperationException($"Условная вершина {cv.Id} имеет неполные переходы");
            }
        }
    }
}
