using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.LSAutomaton;

namespace TOAConsole.LSA.Elements.Common
{
    internal interface ILSAElement
    {
        string Id { get; }
        ILSAElement Next { get; set; }
        string GetOutput();
        ILSAElement GetNext(Automaton automaton);
    }

    internal abstract class LSABaseElement : ILSAElement
    {
        public string Id { get; protected set; }
        public ILSAElement Next { get; set; }

        public abstract string GetOutput();
        public abstract ILSAElement GetNext(Automaton automaton);
    }
}
