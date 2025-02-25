using System;
using System.Windows.Forms;
using TheoryOfAutomatons.Automaton;

namespace TheoryOfAutomatons
{
    public partial class MainForm: Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        internal static AutomatonCreator AutomatonCreator;
        private void Form_Load(object sender, EventArgs e)
        {
            AutomatonCreator = new AutomatonCreator
            (
                saveToFileTSB, loadFromFileTSB, clearTSB,
                typeBox, 
                inputAlphabet, outputAlphabet, 
                addState,
                sequenceTextBox, generateRandomSequence, analyze, 
                cirlceDiameterNUD, borderNUD, drawStepDelayNUD, transitionLightPenNUD, transitionBlackPenNUD,
                containerCP, activeBorderCP, inactiveBorderCP, highlightedBorderCP, innerStateCP, transitionLightPenCP, transitionBlackPenCP,
                prohibitIntersectingPaths, developerMode, 
                container, this, colorPicker
            );
        }
    }
}
