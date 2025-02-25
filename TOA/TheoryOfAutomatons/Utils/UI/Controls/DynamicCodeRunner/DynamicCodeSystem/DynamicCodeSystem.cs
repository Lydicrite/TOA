using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.DynamicCodeRunner.DynamicCodeSystem
{
    internal class DynamicCodeSystem
    {
        private readonly string basePath;
        private readonly DynamicCodeCompiler compiler;
        private readonly DynamicCodeStorage storage;

        public DynamicCodeSystem()
        {
            basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            compiler = new DynamicCodeCompiler();
            storage = new DynamicCodeStorage(basePath);
        }

        public ExecutionResult Execute(string code, ExecutionContext context, ParameterSchema schema)
        {
            var domain = Sandbox.CreateSandboxDomain(basePath);
            try
            {
                var assembly = compiler.Compile(code);
                var tempPath = Path.Combine(basePath, "Sandbox", "temp.dll");

                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
                File.WriteAllBytes(tempPath, File.ReadAllBytes(assembly.Location));

                var sandbox = (Sandbox)domain.CreateInstanceAndUnwrap(
                    typeof(Sandbox).Assembly.FullName,
                    typeof(Sandbox).FullName);

                return sandbox.Execute(tempPath, context, schema);
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        public void SaveModule(string name, string code) => storage.SaveCode(name, code);
        public string LoadModule(string name) => storage.LoadCode(name);
    }
}
