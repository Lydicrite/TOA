using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TheoryOfAutomatons.Utils.UI.Controls.Terminal.Shell
{
    public class SyntaxHighlighter
    {
        private Color _commandColor = Color.DarkOrange;
        private Color _paramColor = Color.DodgerBlue;
        private Color _symbolColor = Color.Gray;

        public void HighlightInput(RichTextBox box, string input)
        {
            box.SelectionColor = _symbolColor;
            box.AppendText("\\");

            var parameters = Regex.Matches(input, @"(?<match>\""[^\""]*\"")|(?<match>\S+)")
                .Cast<Match>()
                .Select(m => m.Groups["match"].Value.Trim('"'))
                .ToList();

            if (parameters.Count == 0) return;

            // Command name
            box.SelectionColor = _commandColor;
            box.AppendText(parameters[0]);

            // Parameters
            if (parameters.Count > 1)
            {
                box.SelectionColor = _paramColor;
                box.AppendText(" " + string.Join(" ", parameters.Skip(1)));
            }
        }
    }
}
