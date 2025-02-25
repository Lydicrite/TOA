using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.DynamicCodeRunner.DynamicCodeSystem
{
    internal class ExecutionHost
    {
        public ExecutionResult Execute(Action action, int timeoutMilliseconds = 5000)
        {
            var result = new ExecutionResult();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var task = Task.Run(action);

                if (!task.Wait(timeoutMilliseconds))
                    throw new TimeoutException("Execution timed out");

                result.ExecutionTime = stopwatch.Elapsed;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }
    }
}
