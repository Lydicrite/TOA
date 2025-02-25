using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.DynamicCodeRunner.DynamicCodeSystem
{
    internal interface IDynamicModule
    {
        void Execute(ExecutionContext context);
    }

    internal class ExecutionContext
    {
        public Dictionary<string, object> Inputs { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> Outputs { get; } = new Dictionary<string, object>();
    }

    internal class ExecutionResult
    {
        public Dictionary<string, object> Outputs { get; set; } = new Dictionary<string, object>();
        public Exception Error { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public List<string> CompilationErrors { get; set; } = new List<string>();
        public ResourceUsage ResourceUsage { get; set; }
    }

    internal class ResourceUsage
    {
        public long MemoryUsed { get; set; }
        public TimeSpan ProcessorTime { get; set; }
    }
}
