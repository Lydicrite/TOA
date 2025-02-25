using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.DynamicCodeRunner.DynamicCodeSystem
{
    internal class Sandbox : MarshalByRefObject
    {
        public ExecutionResult Execute(string assemblyPath, ExecutionContext context, ParameterSchema schema)
        {
            var result = new ExecutionResult();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            using (var monitor = new ResourceMonitor())
            {
                monitor.Start();

                try
                {
                    var assembly = Assembly.LoadFrom(assemblyPath);
                    var type = assembly.GetTypes().FirstOrDefault(t => typeof(IDynamicModule).IsAssignableFrom(t));

                    if (type == null)
                        throw new InvalidOperationException("No implementation of IDynamicModule found");

                    var validator = new ExecutionValidator();
                    validator.Validate(context, schema);

                    var instance = (IDynamicModule)Activator.CreateInstance(type);
                    instance.Execute(context);

                    result.Outputs = new Dictionary<string, object>(context.Outputs);
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                }
                finally
                {
                    stopwatch.Stop();
                    result.ExecutionTime = stopwatch.Elapsed;
                    result.ResourceUsage = monitor.GetUsage();
                }

                return result;
            }
        }

        public static AppDomain CreateSandboxDomain(string basePath)
        {
            var setup = new AppDomainSetup
            {
                ApplicationBase = basePath,
                ApplicationName = "Sandbox"
            };

            var permissions = new PermissionSet(PermissionState.None);
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            permissions.AddPermission(new FileIOPermission(
                FileIOPermissionAccess.Read | FileIOPermissionAccess.Write,
                Path.Combine(basePath, "Sandbox")));

            return AppDomain.CreateDomain(
                "SandboxDomain",
                new Evidence(),
                setup,
                permissions);
        }
    }
}
