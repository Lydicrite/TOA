using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheoryOfAutomatons.Utils.UI.Controls.Terminal.Shell
{
    internal class CommandArgs
    {
        public string[] Arguments { get; }
        public Terminal Terminal { get; }

        public CommandArgs(string[] args, Terminal terminal)
        {
            Arguments = args;
            Terminal = terminal;
        }
    }
}
