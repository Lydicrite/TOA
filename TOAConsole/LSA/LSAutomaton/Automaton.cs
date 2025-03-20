using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.Elements.Jumps;
using TOAConsole.LSA.Elements.Vertexes;

namespace TOAConsole.LSA.LSAutomaton
{
    internal class Automaton
    {
        public Dictionary<int, ILSAElement> JumpPoints { get; private set; } = new();
        public ILSAElement? CurrentElement { get; private set; }
        public List<ILSAElement> Elements { get; private set; } = new();

        public void AddElement(ILSAElement element)
        {
            Elements.Add(element);
            if (element is JumpPoint jp)
                JumpPoints[jp.JumpIndex] = jp;
        }

        public void SetConditionalValue(int xIndex, bool value)
        {
            var cond = Elements.OfType<ConditionalVertex>().FirstOrDefault(x => x.Index == xIndex);
            if (cond != null)
                cond.Value = value;
        }

        public IEnumerable<string> Run()
        {
            var outputs = new List<string>();
            CurrentElement = Elements.OfType<StartVertex>().FirstOrDefault();

            while (CurrentElement != null && CurrentElement is not EndVertex)
            {
                outputs.Add(CurrentElement.GetOutput());
                CurrentElement = CurrentElement.GetNext(this);
            }

            if (CurrentElement is EndVertex)
                outputs.Add(CurrentElement.GetOutput());

            return outputs;
        }
    }
}
