using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.LogicalExpressionParser.ParserSystem
{
    internal class LogicalExpression
    {
        private Node _root;
        private List<string> _variables;
        private Dictionary<string, int> _variableIndices;

        public LogicalExpression(Node root)
        {
            _root = root;
            _variables = new List<string>();
            _variableIndices = new Dictionary<string, int>();
            CollectVariables();
            SetVariableIndices(_root);
        }

        private void CollectVariables()
        {
            var variables = new HashSet<string>();
            _root.CollectVariables(variables);
            _variables = variables.OrderBy(v => v).ToList();
            for (int i = 0; i < _variables.Count; i++)
                _variableIndices[_variables[i]] = i;
        }

        private void SetVariableIndices(Node node)
        {
            switch (node)
            {
                case VariableNode varNode:
                    varNode.SetIndex(_variableIndices[varNode.Name]);
                    break;
                case UnaryNode unaryNode:
                    SetVariableIndices(unaryNode.Operand);
                    break;
                case BinaryNode binaryNode:
                    SetVariableIndices(binaryNode.Left);
                    SetVariableIndices(binaryNode.Right);
                    break;
            }
        }

        public bool Evaluate(bool[] inputs)
        {
            if (inputs.Length != _variables.Count)
                throw new ArgumentException("Invalid number of inputs.");
            return _root.Evaluate(inputs);
        }

        public Func<bool[], bool> Compile()
        {
            return Evaluate;
        }
    }
}
