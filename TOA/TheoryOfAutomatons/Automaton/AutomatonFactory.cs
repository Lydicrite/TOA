using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheoryOfAutomatons.Automaton.MealyAutomaton;
using TheoryOfAutomatons.Automaton.MooreAutomaton;
using TheoryOfAutomatons.Automaton;
using TOA.TheoryOfAutomatons.Automaton.Common;
using System.ComponentModel;

namespace TOA.TheoryOfAutomatons.Automaton
{
    internal static class AutomatonFactory
    {
        public static IDFAutomaton CreateAutomaton
        (
            AutomatonType type,
            PictureBox container,
            AutomatonCreator creator
        )
        {
            return type switch
            {
                AutomatonType.Mealy => new DFMealyAutomaton(container, creator),
                AutomatonType.Moore => new DFMooreAutomaton(container, creator),
                _ => throw new NotImplementedException()
            };
        }

        public static AutomatonType FromInt(int type)
        {
            return type switch
            {
                0 => AutomatonType.Mealy,
                1 => AutomatonType.Moore,
                _ => throw new NotImplementedException()
            };
        }

        public static int ToInt(this AutomatonType type)
        {
            return type switch
            {
                AutomatonType.Mealy => 0,
                AutomatonType.Moore => 1,
                _ => throw new NotImplementedException()
            };
        }
    }

    internal enum AutomatonType
    {
        Mealy = 0,
        Moore = 1
    }
}
