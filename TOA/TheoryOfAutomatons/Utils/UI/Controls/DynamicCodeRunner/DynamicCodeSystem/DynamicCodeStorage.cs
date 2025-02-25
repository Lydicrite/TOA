using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.DynamicCodeRunner.DynamicCodeSystem
{
    internal class DynamicCodeStorage
    {
        private readonly string storagePath;

        public DynamicCodeStorage(string basePath)
        {
            storagePath = Path.Combine(basePath, "DynamicCode");
            Directory.CreateDirectory(storagePath);
        }

        public void SaveCode(string name, string code)
        {
            File.WriteAllText(Path.Combine(storagePath, $"{name}.cs"), code);
        }

        public string LoadCode(string name)
        {
            return File.ReadAllText(Path.Combine(storagePath, $"{name}.cs"));
        }
    }
}
