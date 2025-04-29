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
using System.Drawing;

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
            IDFAutomaton automaton = type switch
            {
                AutomatonType.Mealy => new DFMealyAutomaton(container, creator),
                AutomatonType.Moore => new DFMooreAutomaton(container, creator),
                _ => null
            };

            return automaton;
        }

        public static IDFAutomaton CreateAutomaton
        (
            AutomatonType type,
            string filePath,
            PictureBox container,
            AutomatonCreator creator
        )
        {
            IDFAutomaton automaton = type switch
            {
                AutomatonType.Mealy => new DFMealyAutomaton(container, creator),
                AutomatonType.Moore => new DFMooreAutomaton(container, creator),
                _ => null
            };

            automaton.Load(filePath, container);

            return automaton;
        }

        public static IAutomatonState CreateState
        (
            AutomatonType type,
            IDFAutomaton automaton, 
            int index, 
            string userDefinedText, 
            Point initialPosition
        )
        {
            return type switch
            {
                AutomatonType.Mealy => new MealyAutomatonState((automaton as DFMealyAutomaton), index, userDefinedText, initialPosition),
                AutomatonType.Moore => new MooreAutomatonState((automaton as DFMooreAutomaton), index, userDefinedText, initialPosition),
                _ => null
            };
        }

        public static AutomatonType FromInt(int type)
        {
            return type switch
            {
                0 => AutomatonType.Mealy,
                1 => AutomatonType.Moore,
                _ => 0
            };
        }

        public static int ToInt(this AutomatonType type)
        {
            return type switch
            {
                AutomatonType.Mealy => 0,
                AutomatonType.Moore => 1,
                _ => 0
            };
        }
    }

    internal enum AutomatonType
    {
        Mealy = 0,
        Moore = 1
    }
}
