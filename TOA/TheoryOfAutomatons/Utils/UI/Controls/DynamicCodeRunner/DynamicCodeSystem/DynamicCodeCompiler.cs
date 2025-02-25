using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.DynamicCodeRunner.DynamicCodeSystem
{
    internal class DynamicCodeCompiler
    {
        public Assembly Compile(string code, IEnumerable<string> additionalReferences = null)
        {
            using (var provider = new CSharpCodeProvider())
            {
                var parameters = new CompilerParameters
                {
                    GenerateInMemory = true,
                    ReferencedAssemblies =
                    {
                        "System.dll",
                        "System.Core.dll",
                        typeof(IDynamicModule).Assembly.Location
                    }
                };

                if (additionalReferences != null)
                    parameters.ReferencedAssemblies.AddRange(additionalReferences.ToArray());

                code = $@"using System;
                using System.Collections.Generic;
                using System.Linq;
                {code}";

                var results = provider.CompileAssemblyFromSource(parameters, code);

                if (results.Errors.HasErrors)
                {
                    var errors = results.Errors.Cast<CompilerError>()
                        .Select(e => $"{e.ErrorText} (Line {e.Line})")
                        .ToList();
                    throw new CompilationException(errors);
                }

                return results.CompiledAssembly;
            }
        }
    }

    internal class CompilationException : Exception
    {
        public List<string> Errors { get; }

        public CompilationException(List<string> errors)
            : base("Compilation failed: " + string.Join("\n", errors))
        {
            Errors = errors;
        }
    }
}
