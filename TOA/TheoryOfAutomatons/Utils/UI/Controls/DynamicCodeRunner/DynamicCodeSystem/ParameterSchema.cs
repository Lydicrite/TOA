using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.DynamicCodeRunner.DynamicCodeSystem
{
    internal class ParameterSchema
    {
        public Dictionary<string, Type> InputTypes { get; } = new Dictionary<string, Type>();
        public Dictionary<string, Type> OutputTypes { get; } = new Dictionary<string, Type>();
    }
}
