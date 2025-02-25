using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.DynamicCodeRunner.DynamicCodeSystem
{
    internal class ExecutionValidator
    {
        public void Validate(ExecutionContext context, ParameterSchema schema)
        {
            foreach (var kvp in schema.InputTypes)
            {
                if (!context.Inputs.ContainsKey(kvp.Key))
                    throw new ArgumentException($"Missing input parameter: {kvp.Key}");

                if (context.Inputs[kvp.Key]?.GetType() != kvp.Value)
                    throw new ArgumentException($"Invalid type for parameter {kvp.Key}");
            }
        }
    }
}
