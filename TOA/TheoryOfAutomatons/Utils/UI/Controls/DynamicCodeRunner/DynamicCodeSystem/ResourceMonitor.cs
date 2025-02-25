using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.DynamicCodeRunner.DynamicCodeSystem
{
    internal class ResourceMonitor : IDisposable
    {
        private readonly Process _process = Process.GetCurrentProcess();
        private long _startMemory;

        public void Start()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            _startMemory = _process.PrivateMemorySize64;
        }

        public ResourceUsage GetUsage()
        {
            return new ResourceUsage
            {
                MemoryUsed = _process.PrivateMemorySize64 - _startMemory,
                ProcessorTime = _process.TotalProcessorTime
            };
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}
