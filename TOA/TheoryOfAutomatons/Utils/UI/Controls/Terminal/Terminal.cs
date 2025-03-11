using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TheoryOfAutomatons.Utils.UI.Controls.Terminal.Shell;
using TheoryOfAutomatons.Utils.UI.Controls.Terminal.ExecutionIssues;
using static Syncfusion.Windows.Forms.Tools.Navigation.Bar;
using System.IO;
using TheoryOfAutomatons.Utils.UI.Forms.Adders;
using TOA.TheoryOfAutomatons.Utils.UI.Controls.Terminal.Shell;

namespace TheoryOfAutomatons.Utils.UI.Controls.Terminal
{
    [ToolboxItem(true)]
    public class Terminal : UserControl
    {
        #region Поля класса

        public RichTextBox pshTerminal;
        private Panel suggestionsPanel;
        private ListBox suggestionsList;
        private FlowLayoutPanel issueMainFLP;
        private TableLayoutPanel issueMainTLP;
        private CheckBox showErrorsFilter;
        private CheckBox showWarningsFilter;
        private CheckBox showInfoMessagesFilter;
        private ImageList issueIcons;
        private ContextMenuStrip issueContextMenu;
        private ToolStripMenuItem detailsMenuItem;
        private ToolStripMenuItem copyMessageMenuItem;
        private ToolStripMenuItem copyDetailsMenuItem;
        private DataGridView issuesDataGridView;
        private DataGridViewImageColumn iconColumn;
        private DataGridViewTextBoxColumn typeColumn;
        private DataGridViewTextBoxColumn categoryColumn;
        private DataGridViewTextBoxColumn timeColumn;
        private DataGridViewTextBoxColumn sourceColumn;
        private DataGridViewTextBoxColumn messageColumn;
        private ToolStripMenuItem openLogMenuItem;

        // Команды
        private bool _isProcessingInput = false;
        private CommandHandler _commandHandler;
        private List<string> _commandHistory = new List<string>();
        private int _historyIndex = -1;
        private SyntaxHighlighter _highlighter = new SyntaxHighlighter();
        public CommandHandler CommandHandler => _commandHandler;


        // Режимы ввода
        private InputRule _activeInputRule;


        // Ошибки
        private readonly BindingList<ExecutionIssue> _executionIssues = new BindingList<ExecutionIssue>();
        private ExecutionIssueType _visibleIssueTypes = ExecutionIssueType.All;

        #endregion

        public Terminal()
        {
            InitializeComponent();
            InitializeCommandSystem();
            ConfigureIssuePanel();
            ProcessCommand("\\set_input_rule commands");
        }

 

        #region Работа с текстом

        private string GetCurrentLine()
        {
            int lineIndex = pshTerminal.GetLineFromCharIndex(pshTerminal.SelectionStart);
            return lineIndex >= 0 && lineIndex < pshTerminal.Lines.Length
                ? pshTerminal.Lines[lineIndex]
                : "";
        }

        private string GetCurrentInputLine()
        {
            var line = GetCurrentLine();
            return line.Length > 3 ? line.Substring(3) : string.Empty;
        }

        private void SetCurrentInput(string text)
        {
            int currentLineStart = pshTerminal.GetFirstCharIndexOfCurrentLine();
            pshTerminal.Select(currentLineStart + 2, pshTerminal.TextLength - currentLineStart - 2);
            pshTerminal.SelectedText = text;
        }

        public void AppendPrompt()
        {
            if (pshTerminal.Text.EndsWith(CommandHandler.CommandChar + "> ")) return;

            _isProcessingInput = true;
            pshTerminal.SelectionStart = pshTerminal.TextLength;
            pshTerminal.SelectionColor = Color.Green;
            AppendText(CommandHandler.CommandChar + "> ", Color.Green, false);
            _isProcessingInput = false;
        }

        public void AppendText(string text, Color color, bool autoPrompt = true)
        {
            if (pshTerminal.InvokeRequired)
            {
                pshTerminal.Invoke(new System.Action(() => AppendText(text, color)));
                return;
            }

            pshTerminal.SelectionStart = pshTerminal.TextLength;
            pshTerminal.SelectionColor = color;
            pshTerminal.AppendText(text);
            if (autoPrompt)
                AppendPrompt();
            pshTerminal.ScrollToCaret();
        }

        public void AppendText(RichTextBox terminal, string text, Color color)
        {
            if (terminal.InvokeRequired)
            {
                terminal.Invoke(new System.Action(() => AppendText(terminal, text, color)));
            }
            else
            {
                terminal.SelectionStart = terminal.TextLength;
                terminal.SelectionLength = 0;
                terminal.SelectionColor = color;
                terminal.AppendText(text);
                terminal.SelectionColor = terminal.ForeColor;
                terminal.ScrollToCaret();
            }
        }



        public void SetInputRule(string ruleName)
        {
            if (InputRulePresets.Rules.TryGetValue(ruleName, out var rule))
            {
                _activeInputRule = rule;
            }
        }

        public void SetInputRule(InputRule rule)
        {
            _activeInputRule = rule;
        }

        private bool ValidateInput(char enteredChar)
        {
            if (_activeInputRule == null) return true;

            string newText = GetCurrentInputLine() + enteredChar;
            return _activeInputRule.ValidateFuture(newText);
        }

        private bool ValidateTextInsertion(string textToInsert)
        {
            if (_activeInputRule == null) return true;

            string newText = GetCurrentInputLine() + textToInsert;
            return _activeInputRule.ValidateStrong(newText);
        }

        #endregion



        #region Команды

        private void InitializeCommandSystem()
        {
            _commandHandler = new CommandHandler();
            _commandHandler.RegisterFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ProcessCommand(string input)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                if (input.StartsWith(CommandHandler.CommandChar.ToString()))
                {
                    input = input.TrimStart(CommandHandler.CommandChar, '>', ' ');
                    if (string.IsNullOrWhiteSpace(input)) return;

                    _commandHistory.Insert(0, input);
                    _historyIndex = -1;

                    AppendText($" --- [{DateTime.Now:HH:mm:ss}] --- ", Color.Gray, false);
                    // _highlighter.HighlightInput(pshTerminal, input);

                    var result = _commandHandler.Execute(input, this);
                    if (input != "clear")
                    {
                        var s = result.ExecutionTimeToString();
                        AppendText(s, Color.Gray, false);
                        AppendText(Environment.NewLine + result.ToString() + Environment.NewLine + Environment.NewLine, result.Success ? Color.Lime : Color.Red, true);
                    }
                } 
                else
                {
                    AppendText($"\n{input} не является командной\n\n", Color.Green, false);
                }

            }
            catch (Exception ex)
            {
                AppendText($"ERROR: {ex.Message}\n\n", Color.Red, true);
            }
            finally
            {
                AppendPrompt();
            }
        }

        private void ShowSuggestions(string partial)
        {
            var commands = _commandHandler.GetCommandNames()
                .Where(c => c.StartsWith(partial.Trim(CommandHandler.CommandChar, '>', ' '), StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (commands.Any())
            {
                suggestionsList.Items.Clear();
                suggestionsList.Items.AddRange(commands.Cast<object>().ToArray());
                suggestionsPanel.Visible = true;
                suggestionsList.Visible = true;
            }
            else
            {
                suggestionsPanel.Visible = false;
                suggestionsList.Visible = false;
            }
        }

        #endregion



        #region Ошибки выполнения

        private void ConfigureIssuePanel()
        {
            issueIcons = new ImageList
            {
                Images = {
                    SystemIcons.Warning,
                    SystemIcons.Error,
                    SystemIcons.Information
                }
            };

            issuesDataGridView.Rows.Clear();
            issuesDataGridView.RowTemplate.Height = 20;
            iconColumn.Width = 20;
            iconColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            iconColumn.DefaultCellStyle.NullValue = null;
            issuesDataGridView.Font = new Font("Times New Roman", 6f, FontStyle.Bold);
            issuesDataGridView.VirtualMode = true;
            issuesDataGridView.AllowDrop = false;
            issuesDataGridView.AutoGenerateColumns = true;
            issuesDataGridView.AutoResizeColumnHeadersHeight();
            issuesDataGridView.Refresh();
            issuesDataGridView.CellValueNeeded += IssuesDataGridView_CellValueNeeded;
            issuesDataGridView.CellFormatting += IssuesDataGridView_CellFormatting;

            InitializeIssueContextMenu();

            // Пример
            try
            {
                ExceptionGenerator.ThrowRandomException();
            }
            catch (Exception ex)
            {
                LogExecIssue(ExecutionIssueType.Error, ExecutionIssueCategory.UI, "Lydicrite", $"Это пример поимки ошибки", ex);
            }
            finally
            {
                LogExecIssue(ExecutionIssueType.Warning, ExecutionIssueCategory.Security, "Was", "Это пример вывода предупреждения");
                LogExecIssue(ExecutionIssueType.Message, ExecutionIssueCategory.Data, "Here", "Это пример вывода сообщения");
            }
        }

        private void InitializeIssueContextMenu()
        {
            issueContextMenu = new ContextMenuStrip();

            // Пункт "Показать детали"
            detailsMenuItem = new ToolStripMenuItem("Показать детали ошибки");
            detailsMenuItem.Click += (s, e) => ShowSelectedIssueDetails();

            // Пункт "Копировать сообщение"
            copyMessageMenuItem = new ToolStripMenuItem("Копировать сообщение");
            copyMessageMenuItem.Click += (s, e) => CopyIssueMessage();

            // Пункт "Копировать детали"
            copyDetailsMenuItem = new ToolStripMenuItem("Копировать полные данные");
            copyDetailsMenuItem.Click += (s, e) => CopyIssueDetails();

            // Пункт "Открыть в логе"
            openLogMenuItem = new ToolStripMenuItem("Открыть в файле журнала");
            openLogMenuItem.Click += (s, e) => OpenInLogFile();

            issueContextMenu.Items.AddRange(new ToolStripItem[] {
                detailsMenuItem,
                new ToolStripSeparator(),
                copyMessageMenuItem,
                copyDetailsMenuItem,
                new ToolStripSeparator(),
                openLogMenuItem
            });

            issuesDataGridView.ContextMenuStrip = issueContextMenu;
            issuesDataGridView.MouseDown += IssuesDataGridView_MouseDown;
        }

        private void UpdateFilteredView()
        {
            // Фильтруем элементы по видимым типам
            var filtered = _executionIssues.Where(issue => _visibleIssueTypes.HasFlag(issue.Type)).ToList();

            // Группируем элементы по типу
            var groups = _executionIssues.GroupBy(issue => issue.Type);

            foreach (var group in groups)
            {
                int count = group.Count();
                ExecutionIssueType type = group.Key;
                string typeName = type.GetTypeName();
                string text = string.Empty;

                switch (type)
                {
                    case ExecutionIssueType.Error:
                        text = showErrorsFilter.Checked ? $"{typeName} ({count})" : $"{typeName} (0 из {count})";
                        showErrorsFilter.Text = text;
                        break;
                    case ExecutionIssueType.Warning:
                        text = showWarningsFilter.Checked ? $"{typeName} ({count})" : $"{typeName} (0 из {count})";
                        showWarningsFilter.Text = text;
                        break;
                    case ExecutionIssueType.Message:
                        text = showInfoMessagesFilter.Checked ? $"{typeName} ({count})" : $"{typeName} (0 из {count})";
                        showInfoMessagesFilter.Text = text;
                        break;
                }
            }

            // Обновляем размер виртуального списка
            issuesDataGridView.RowCount = filtered.Count;
            issuesDataGridView.Invalidate();
        }

        private void UpdateFilters(bool showErrors, bool showWarnings, bool showInfoMessages)
        {
            _visibleIssueTypes = ExecutionIssueType.None;
            if (showErrors) _visibleIssueTypes |= ExecutionIssueType.Error;
            if (showWarnings) _visibleIssueTypes |= ExecutionIssueType.Warning;
            if (showInfoMessages) _visibleIssueTypes |= ExecutionIssueType.Message;

            UpdateFilteredView();
        }

        public void LogExecIssue(ExecutionIssueType issueTupe, ExecutionIssueCategory cat, string source, string message, Exception ex = null, string codeSnippet = null)
        {
            switch (issueTupe)
            {
                case ExecutionIssueType.Error:
                    LogError(source, message, cat, ex, codeSnippet); break;
                case ExecutionIssueType.Warning:
                    LogWarning(source, message, cat, ex, codeSnippet); break;
                case ExecutionIssueType.Message:
                    LogMessage(source, message, cat, ex, codeSnippet); break;
                default: throw new NotImplementedException();
            }
        }

        private void LogError(string source, string message, ExecutionIssueCategory cat, Exception ex = null, string codeSnippet = null)
        {
            var issue = new ExecutionIssue(ExecutionIssueType.Error, cat, source, message, ex, codeSnippet);
            AddIssueToList(issue);
        }

        private void LogWarning(string source, string message, ExecutionIssueCategory cat, Exception ex = null, string codeSnippet = null)
        {
            var issue = new ExecutionIssue(ExecutionIssueType.Warning, cat, source, message, ex, codeSnippet);
            AddIssueToList(issue);
        }

        private void LogMessage(string source, string message, ExecutionIssueCategory cat, Exception ex = null, string codeSnippet = null)
        {
            var issue = new ExecutionIssue(ExecutionIssueType.Message, cat,source, message, ex, codeSnippet);
            AddIssueToList(issue);
        }

        private void AddIssueToList(ExecutionIssue issue)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddIssueToList(issue)));
                return;
            }

            _executionIssues.Add(issue);
            UpdateFilteredView();
        }

        #endregion



        #region События UI-элементов



        #region PowerShell

        private void pshTerminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            if (!ValidateInput(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == CommandHandler.CommandChar)
            {
                int currentPos = pshTerminal.SelectionStart;
                if (currentPos > 0 && pshTerminal.Text[currentPos - 1] == CommandHandler.CommandChar)
                {
                    e.Handled = true; // Блокируем ввод
                }
            }

            if (e.KeyChar == ((char)Keys.Space))
            {
                int currentPos = pshTerminal.SelectionStart;
                if (currentPos > 0 && pshTerminal.Text[currentPos - 1] == ' ')
                {
                    e.Handled = true; // Блокируем ввод
                }
            }
        }

        private void pshTerminal_KeyDown(object sender, KeyEventArgs e)
        {
            int currentLineStart = pshTerminal.GetFirstCharIndexOfCurrentLine();
            int lastLine = pshTerminal.Lines.Length - 1;

            // Обработка Backspace / Delete
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                // Запрещаем редактирование предыдущих строк
                int currentLineIndex = pshTerminal.GetLineFromCharIndex(pshTerminal.SelectionStart);
                if (currentLineIndex != pshTerminal.Lines.Length - 1)
                {
                    e.SuppressKeyPress = true;
                    return;
                }

                // Работаем только с последней строкой
                string currentLine = GetCurrentLine();
                string inputLine = currentLine.Length > 3 ? currentLine.Substring(3) : "";
                int cursorPositionInLine = pshTerminal.SelectionStart - pshTerminal.GetFirstCharIndexOfCurrentLine();

                // Корректируем позицию с учетом приглашения "> "
                int adjustedCursorPos = cursorPositionInLine - 3;

                // Проверка границ для Backspace / Delete
                bool isValidOperation = true;
                if (e.KeyCode == Keys.Back)
                {
                    isValidOperation = adjustedCursorPos > 0; // Нельзя удалить приглашение
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    isValidOperation = adjustedCursorPos < inputLine.Length;
                }

                if (!isValidOperation)
                {
                    e.SuppressKeyPress = true;
                    return;
                }

                // Симулируем новую строку после удаления
                string newText = inputLine;
                if (pshTerminal.SelectionLength > 0)
                {
                    int start = Math.Max(adjustedCursorPos, 0);
                    int length = Math.Min(pshTerminal.SelectionLength, inputLine.Length - start);
                    newText = inputLine.Remove(start, length);
                }
                else
                {
                    int deletePos = e.KeyCode == Keys.Back
                        ? adjustedCursorPos - 1
                        : adjustedCursorPos;

                    if (deletePos >= 0 && deletePos < inputLine.Length)
                    {
                        newText = inputLine.Remove(deletePos, 1);
                    }
                }

                // Проверяем соответствие правилу ввода
                if (!_activeInputRule.ValidationRegex.IsMatch(newText))
                {
                    if (newText == "" || newText.StartsWith("\\"))
                    {
                        e.SuppressKeyPress = false;
                    }
                    else
                    {
                        e.SuppressKeyPress = true;
                    }
                }
            }

            else if (e.KeyCode == Keys.Enter)
            {
                if (_activeInputRule.Type == InputRuleType.Commands)
                {
                    e.SuppressKeyPress = true;

                    int currentLineIndex = pshTerminal.GetLineFromCharIndex(pshTerminal.SelectionStart);
                    int lastLineIndex = pshTerminal.Lines.Length - 1;
                    bool isCursorAtEnd = pshTerminal.SelectionStart == pshTerminal.TextLength;

                    if (currentLineIndex == lastLineIndex && isCursorAtEnd)
                    {
                        var input = GetCurrentInputLine();
                        if (!string.IsNullOrEmpty(input) && _activeInputRule.ValidateStrong(input))
                        {
                            ProcessCommand(input);
                            AppendPrompt();
                        }
                    }
                }
            }

            else if (e.KeyCode == Keys.Up)
            {
                if (_historyIndex < _commandHistory.Count - 1)
                {
                    _historyIndex = _historyIndex == -1 ? 0 : _historyIndex + 1;
                    SetCurrentInput(CommandHandler.CommandChar + _commandHistory[_historyIndex]);
                }
            }

            else if (e.KeyCode == Keys.Down)
            {
                if (_historyIndex > 0)
                {
                    _historyIndex--;
                    SetCurrentInput(CommandHandler.CommandChar + _commandHistory[_historyIndex]);
                }
                else if (_historyIndex == 0)
                {
                    _historyIndex = -1;
                    SetCurrentInput("");
                }
            }

            // Обработка Ctrl + V
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true;
                string clipboardText = Clipboard.GetText();

                if (ValidateTextInsertion(clipboardText))
                {
                    pshTerminal.SelectedText = clipboardText;
                }
                return;
            }


            else if (e.Control && e.KeyCode == Keys.L)
            {
                ProcessCommand("\\clear");
            }
        }

        private void pshTerminal_TextChanged(object sender, EventArgs e)
        {
            {
                if (_isProcessingInput) return;
                _isProcessingInput = true;

                // Откат изменений, если они затронули не последнюю строку
                int currentLine = pshTerminal.GetLineFromCharIndex(pshTerminal.SelectionStart);
                if (currentLine < pshTerminal.Lines.Length - 1)
                {
                    pshTerminal.Undo();
                }

                // Защита приглашения в текущей строке
                int promptStart = pshTerminal.GetFirstCharIndexOfCurrentLine();
                if (pshTerminal.Lines.Length - 1 >= 0)
                {
                    string currentLineText = pshTerminal.Lines[pshTerminal.Lines.Length - 1];
                    if (!currentLineText.StartsWith(CommandHandler.CommandChar + ">"))
                    {
                        // Восстановление приглашения, если оно удалено
                        pshTerminal.Select(promptStart, pshTerminal.TextLength - promptStart);
                    }

                    _isProcessingInput = false;
                }
            }


            {
                var currentLine = GetCurrentInputLine();

                if (currentLine.StartsWith(CommandHandler.CommandChar.ToString()))
                {
                    ShowSuggestions(currentLine);
                }
                else
                {
                    suggestionsPanel.Visible = false;
                }
            }
        }

        private void suggestionsList_Click(object sender, EventArgs e)
        {
            if (suggestionsList.SelectedItem != null)
            {
                SetCurrentInput(CommandHandler.CommandChar.ToString() + suggestionsList.SelectedItem);
                suggestionsPanel.Visible = false;
            }
        }

        private void pshTerminal_KeyUp(object sender, KeyEventArgs e)
        {
            // Блокировка Backspace/Delete для приглашения
            int currentLineStart = pshTerminal.GetFirstCharIndexOfCurrentLine();
            if (pshTerminal.SelectionStart <= currentLineStart + 2)
            {
                if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                {
                    e.Handled = true;
                }
            }
        }

        private void pshTerminal_SelectionChanged(object sender, EventArgs e)
        {
            int currentLine = pshTerminal.GetLineFromCharIndex(pshTerminal.SelectionStart);
            int promptStart = pshTerminal.GetFirstCharIndexOfCurrentLine();

            // Запрет перемещения курсора до приглашения
            if (pshTerminal.SelectionStart < promptStart + 3)
            {
                pshTerminal.SelectionStart = promptStart + 3;
            }

            // Запрет перехода на предыдущие строки
            if (currentLine < pshTerminal.Lines.Length - 1)
            {
                pshTerminal.SelectionStart = pshTerminal.TextLength;
            }
        }

        #endregion



        #region ExecutionIssues

        private void IssuesDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var filtered = _executionIssues.Where(issue => _visibleIssueTypes.HasFlag(issue.Type)).ToList();
            if (e.RowIndex >= 0 && e.RowIndex < filtered.Count)
            {
                var issue = filtered[e.RowIndex];
                switch (e.ColumnIndex)
                {
                    case 0: // Иконка
                        e.Value = issue.Type.GetCachedIcon();
                        break;
                    case 1: // Тип
                        e.Value = issue.Type.ToFriendlyString();
                        break;
                    case 2: // Тип
                        e.Value = issue.Category.ToFriendlyString();
                        break;
                    case 3: // Время
                        e.Value = issue.Timestamp.ToLocalTime().ToString("G");
                        break;
                    case 4: // Источник
                        e.Value = issue.Source;
                        break;
                    case 5: // Сообщение
                        e.Value = issue.Message;
                        break;
                }
            }
        }

        private void IssuesDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var filtered = _executionIssues.Where(i => _visibleIssueTypes.HasFlag(i.Type)).ToList();
            if (e.RowIndex >= 0 && e.RowIndex < issuesDataGridView.RowCount && filtered.Count > e.RowIndex)
            {     
                var issue = filtered[e.RowIndex];
                e.CellStyle.ForeColor = Color.Gainsboro;
            }
        }

        private void showErrors_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilters(showErrorsFilter.Checked, showWarningsFilter.Checked, showInfoMessagesFilter.Checked);
        }

        private void showWarnings_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilters(showErrorsFilter.Checked, showWarningsFilter.Checked, showInfoMessagesFilter.Checked);
        }

        private void showInfoMessages_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilters(showErrorsFilter.Checked, showWarningsFilter.Checked, showInfoMessagesFilter.Checked);
        }

        private void IssuesDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = issuesDataGridView.HitTest(e.X, e.Y);
                if (hitTest.RowIndex >= 0 && hitTest.ColumnIndex >= 0)
                {
                    issuesDataGridView.ClearSelection();
                    issuesDataGridView.Rows[hitTest.RowIndex].Selected = true;
                }
                else
                {
                    issueContextMenu.Hide();
                }
            }
        }

        private ExecutionIssue GetSelectedIssue()
        {
            if (issuesDataGridView.SelectedRows.Count == 0) return null;
            var filtered = _executionIssues.Where(i => _visibleIssueTypes.HasFlag(i.Type)).ToList();
            int index = issuesDataGridView.SelectedRows[0].Index;
            return filtered.ElementAtOrDefault(index);
        }

        private void ShowSelectedIssueDetails()
        {
            var issue = GetSelectedIssue();
            if (issue == null) return;
            else
            {
                var info = issue.GetTechnicalDetails();
                ShowIssueDetails form = new ShowIssueDetails($"\nПерехвачена проблема {issue.ErrorCode}: \n{info}");
                form.Show(this);
            }
        }

        private void CopyIssueMessage()
        {
            var issue = GetSelectedIssue();
            if (issue != null)
            {
                Clipboard.SetText(issue.Message);
                AppendText($"\nСообщение проблемы {issue.ErrorCode} скопировано в буфер\n", Color.LightBlue);
                AppendPrompt();
            }
        }

        private void CopyIssueDetails()
        {
            var issue = GetSelectedIssue();
            if (issue != null)
            {
                Clipboard.SetText(issue.GetTechnicalDetails());
                AppendText($"\nВсе данные проблемы {issue.ErrorCode} скопированы\n", Color.LightBlue);
                AppendPrompt();
            }
        }

        private void OpenInLogFile()
        {
            var issue = GetSelectedIssue();
            if (issue == null) return;

            try
            {
                string logPath = Path.Combine(Application.StartupPath, "error_log.txt");
                File.AppendAllText(logPath, $"\n\n{issue.GetTechnicalDetails()}");
                Process.Start("notepad.exe", logPath);
            }
            catch (Exception ex)
            {
                LogExecIssue(ExecutionIssueType.Error, ExecutionIssueCategory.IO,
                    "Контекстное меню", "Ошибка открытия лога", ex);
            }
        }

        #endregion



        #endregion



        #region Параметры конструктора

        private Syncfusion.Windows.Forms.Tools.TabControlAdv terminalTab;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv pshPage;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv runtimePage;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv configPage;

        // Метод для инициализации компонентов
        private void InitializeComponent()
        {
            components = new Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            terminalTab = new Syncfusion.Windows.Forms.Tools.TabControlAdv();
            pshPage = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            suggestionsPanel = new Panel();
            suggestionsList = new ListBox();
            pshTerminal = new RichTextBox();
            runtimePage = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            issueMainTLP = new TableLayoutPanel();
            issueMainFLP = new FlowLayoutPanel();
            showErrorsFilter = new CheckBox();
            showWarningsFilter = new CheckBox();
            showInfoMessagesFilter = new CheckBox();
            issuesDataGridView = new DataGridView();
            iconColumn = new DataGridViewImageColumn();
            typeColumn = new DataGridViewTextBoxColumn();
            categoryColumn = new DataGridViewTextBoxColumn();
            timeColumn = new DataGridViewTextBoxColumn();
            sourceColumn = new DataGridViewTextBoxColumn();
            messageColumn = new DataGridViewTextBoxColumn();
            configPage = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            issueIcons = new ImageList(components);
            ((ISupportInitialize)terminalTab).BeginInit();
            terminalTab.SuspendLayout();
            pshPage.SuspendLayout();
            suggestionsPanel.SuspendLayout();
            runtimePage.SuspendLayout();
            issueMainTLP.SuspendLayout();
            issueMainFLP.SuspendLayout();
            ((ISupportInitialize)issuesDataGridView).BeginInit();
            SuspendLayout();
            // 
            // terminalTab
            // 
            terminalTab.Alignment = TabAlignment.Bottom;
            terminalTab.BackColor = Color.FromArgb(32, 32, 32);
            terminalTab.BeforeTouchSize = new Size(623, 345);
            terminalTab.Controls.Add(pshPage);
            terminalTab.Controls.Add(runtimePage);
            terminalTab.Controls.Add(configPage);
            terminalTab.Dock = DockStyle.Fill;
            terminalTab.Location = new Point(0, 0);
            terminalTab.Margin = new Padding(8);
            terminalTab.Name = "terminalTab";
            terminalTab.Size = new Size(623, 345);
            terminalTab.TabIndex = 0;
            terminalTab.ThemeStyle.PrimitiveButtonStyle.DisabledNextPageImage = null;
            // 
            // pshPage
            // 
            pshPage.Controls.Add(suggestionsPanel);
            pshPage.Controls.Add(pshTerminal);
            pshPage.Image = null;
            pshPage.ImageSize = new Size(16, 16);
            pshPage.Location = new Point(1, 2);
            pshPage.Margin = new Padding(4, 3, 4, 3);
            pshPage.Name = "pshPage";
            pshPage.ShowCloseButton = true;
            pshPage.Size = new Size(620, 316);
            pshPage.TabForeColor = Color.FromArgb(224, 224, 224);
            pshPage.TabIndex = 1;
            pshPage.Text = "PowerShell";
            pshPage.ThemesEnabled = false;
            // 
            // suggestionsPanel
            // 
            suggestionsPanel.BackColor = Color.FromArgb(24, 24, 24);
            suggestionsPanel.BorderStyle = BorderStyle.Fixed3D;
            suggestionsPanel.Controls.Add(suggestionsList);
            suggestionsPanel.Dock = DockStyle.Right;
            suggestionsPanel.Location = new Point(446, 0);
            suggestionsPanel.Margin = new Padding(4, 3, 4, 3);
            suggestionsPanel.Name = "suggestionsPanel";
            suggestionsPanel.Size = new Size(174, 316);
            suggestionsPanel.TabIndex = 2;
            suggestionsPanel.Visible = false;
            // 
            // suggestionsList
            // 
            suggestionsList.BackColor = Color.FromArgb(24, 24, 24);
            suggestionsList.Dock = DockStyle.Fill;
            suggestionsList.ForeColor = Color.FromArgb(224, 224, 224);
            suggestionsList.FormattingEnabled = true;
            suggestionsList.Location = new Point(0, 0);
            suggestionsList.Margin = new Padding(4, 3, 4, 3);
            suggestionsList.Name = "suggestionsList";
            suggestionsList.Size = new Size(170, 312);
            suggestionsList.TabIndex = 0;
            suggestionsList.Visible = false;
            suggestionsList.Click += suggestionsList_Click;
            // 
            // pshTerminal
            // 
            pshTerminal.BackColor = Color.Black;
            pshTerminal.Dock = DockStyle.Fill;
            pshTerminal.ForeColor = Color.FromArgb(224, 224, 224);
            pshTerminal.Location = new Point(0, 0);
            pshTerminal.Margin = new Padding(4, 3, 4, 3);
            pshTerminal.Name = "pshTerminal";
            pshTerminal.Size = new Size(620, 316);
            pshTerminal.TabIndex = 0;
            pshTerminal.Text = "";
            pshTerminal.SelectionChanged += pshTerminal_SelectionChanged;
            pshTerminal.TextChanged += pshTerminal_TextChanged;
            pshTerminal.KeyDown += pshTerminal_KeyDown;
            pshTerminal.KeyPress += pshTerminal_KeyPress;
            pshTerminal.KeyUp += pshTerminal_KeyUp;
            // 
            // runtimePage
            // 
            runtimePage.Controls.Add(issueMainTLP);
            runtimePage.Image = null;
            runtimePage.ImageSize = new Size(16, 16);
            runtimePage.Location = new Point(1, 2);
            runtimePage.Margin = new Padding(4, 3, 4, 3);
            runtimePage.Name = "runtimePage";
            runtimePage.ShowCloseButton = true;
            runtimePage.Size = new Size(620, 316);
            runtimePage.TabForeColor = Color.FromArgb(224, 224, 224);
            runtimePage.TabIndex = 2;
            runtimePage.Text = "Проблемы выполнения";
            runtimePage.ThemesEnabled = false;
            // 
            // issueMainTLP
            // 
            issueMainTLP.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            issueMainTLP.ColumnCount = 1;
            issueMainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            issueMainTLP.Controls.Add(issueMainFLP, 0, 0);
            issueMainTLP.Controls.Add(issuesDataGridView, 0, 1);
            issueMainTLP.Dock = DockStyle.Fill;
            issueMainTLP.Location = new Point(0, 0);
            issueMainTLP.Margin = new Padding(4, 3, 4, 3);
            issueMainTLP.Name = "issueMainTLP";
            issueMainTLP.RowCount = 2;
            issueMainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            issueMainTLP.RowStyles.Add(new RowStyle());
            issueMainTLP.Size = new Size(620, 316);
            issueMainTLP.TabIndex = 1;
            // 
            // issueMainFLP
            // 
            issueMainFLP.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            issueMainFLP.Controls.Add(showErrorsFilter);
            issueMainFLP.Controls.Add(showWarningsFilter);
            issueMainFLP.Controls.Add(showInfoMessagesFilter);
            issueMainFLP.Dock = DockStyle.Fill;
            issueMainFLP.Location = new Point(4, 3);
            issueMainFLP.Margin = new Padding(4, 3, 4, 3);
            issueMainFLP.MaximumSize = new Size(6000, 34);
            issueMainFLP.MinimumSize = new Size(10, 34);
            issueMainFLP.Name = "issueMainFLP";
            issueMainFLP.Size = new Size(612, 34);
            issueMainFLP.TabIndex = 0;
            // 
            // showErrorsFilter
            // 
            showErrorsFilter.Appearance = Appearance.Button;
            showErrorsFilter.AutoSize = true;
            showErrorsFilter.Checked = true;
            showErrorsFilter.CheckState = CheckState.Checked;
            showErrorsFilter.FlatAppearance.CheckedBackColor = Color.FromArgb(64, 64, 64);
            showErrorsFilter.FlatStyle = FlatStyle.Flat;
            showErrorsFilter.ForeColor = Color.Gainsboro;
            showErrorsFilter.Image = TOA.Properties.Resources.ErrorIcon;
            showErrorsFilter.ImageAlign = ContentAlignment.BottomRight;
            showErrorsFilter.Location = new Point(4, 3);
            showErrorsFilter.Margin = new Padding(4, 3, 4, 3);
            showErrorsFilter.Name = "showErrorsFilter";
            showErrorsFilter.Size = new Size(79, 25);
            showErrorsFilter.TabIndex = 0;
            showErrorsFilter.Text = "Ошибки";
            showErrorsFilter.TextAlign = ContentAlignment.MiddleCenter;
            showErrorsFilter.TextImageRelation = TextImageRelation.ImageBeforeText;
            showErrorsFilter.UseVisualStyleBackColor = true;
            showErrorsFilter.CheckedChanged += showErrors_CheckedChanged;
            // 
            // showWarningsFilter
            // 
            showWarningsFilter.Appearance = Appearance.Button;
            showWarningsFilter.AutoSize = true;
            showWarningsFilter.Checked = true;
            showWarningsFilter.CheckState = CheckState.Checked;
            showWarningsFilter.FlatAppearance.CheckedBackColor = Color.FromArgb(64, 64, 64);
            showWarningsFilter.FlatStyle = FlatStyle.Flat;
            showWarningsFilter.ForeColor = Color.Gainsboro;
            showWarningsFilter.Image = TOA.Properties.Resources.WarningIcon;
            showWarningsFilter.Location = new Point(91, 3);
            showWarningsFilter.Margin = new Padding(4, 3, 4, 3);
            showWarningsFilter.Name = "showWarningsFilter";
            showWarningsFilter.Size = new Size(127, 25);
            showWarningsFilter.TabIndex = 1;
            showWarningsFilter.Text = "Предупреждения";
            showWarningsFilter.TextImageRelation = TextImageRelation.ImageBeforeText;
            showWarningsFilter.UseVisualStyleBackColor = true;
            showWarningsFilter.CheckedChanged += showWarnings_CheckedChanged;
            // 
            // showInfoMessagesFilter
            // 
            showInfoMessagesFilter.Appearance = Appearance.Button;
            showInfoMessagesFilter.AutoSize = true;
            showInfoMessagesFilter.Checked = true;
            showInfoMessagesFilter.CheckState = CheckState.Checked;
            showInfoMessagesFilter.FlatAppearance.CheckedBackColor = Color.FromArgb(64, 64, 64);
            showInfoMessagesFilter.FlatStyle = FlatStyle.Flat;
            showInfoMessagesFilter.ForeColor = Color.Gainsboro;
            showInfoMessagesFilter.Image = TOA.Properties.Resources.InformationIcon;
            showInfoMessagesFilter.Location = new Point(226, 3);
            showInfoMessagesFilter.Margin = new Padding(4, 3, 4, 3);
            showInfoMessagesFilter.Name = "showInfoMessagesFilter";
            showInfoMessagesFilter.Size = new Size(98, 25);
            showInfoMessagesFilter.TabIndex = 2;
            showInfoMessagesFilter.Text = "Сообщения";
            showInfoMessagesFilter.TextImageRelation = TextImageRelation.ImageBeforeText;
            showInfoMessagesFilter.UseVisualStyleBackColor = true;
            showInfoMessagesFilter.CheckedChanged += showInfoMessages_CheckedChanged;
            // 
            // issuesDataGridView
            // 
            issuesDataGridView.AllowUserToAddRows = false;
            issuesDataGridView.AllowUserToDeleteRows = false;
            issuesDataGridView.AllowUserToResizeRows = false;
            issuesDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            issuesDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            issuesDataGridView.BackgroundColor = Color.FromArgb(32, 32, 32);
            issuesDataGridView.BorderStyle = BorderStyle.Fixed3D;
            issuesDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(64, 64, 64);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle1.SelectionBackColor = Color.MidnightBlue;
            dataGridViewCellStyle1.SelectionForeColor = Color.Gainsboro;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            issuesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            issuesDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            issuesDataGridView.Columns.AddRange(new DataGridViewColumn[] { iconColumn, typeColumn, categoryColumn, timeColumn, sourceColumn, messageColumn });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(64, 64, 64);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle2.SelectionBackColor = Color.MidnightBlue;
            dataGridViewCellStyle2.SelectionForeColor = Color.Gainsboro;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            issuesDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            issuesDataGridView.Dock = DockStyle.Fill;
            issuesDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            issuesDataGridView.EnableHeadersVisualStyles = false;
            issuesDataGridView.GridColor = Color.FromArgb(32, 32, 32);
            issuesDataGridView.Location = new Point(8, 41);
            issuesDataGridView.Margin = new Padding(8, 1, 8, 45);
            issuesDataGridView.Name = "issuesDataGridView";
            issuesDataGridView.ReadOnly = true;
            issuesDataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(64, 64, 64);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle3.SelectionBackColor = Color.MidnightBlue;
            dataGridViewCellStyle3.SelectionForeColor = Color.Gainsboro;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            issuesDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            issuesDataGridView.RowHeadersVisible = false;
            issuesDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(64, 64, 64);
            dataGridViewCellStyle4.ForeColor = Color.Lime;
            dataGridViewCellStyle4.SelectionBackColor = Color.Blue;
            dataGridViewCellStyle4.SelectionForeColor = Color.White;
            issuesDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            issuesDataGridView.RowTemplate.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
            issuesDataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.DimGray;
            issuesDataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            issuesDataGridView.ScrollBars = ScrollBars.Vertical;
            issuesDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            issuesDataGridView.Size = new Size(604, 257);
            issuesDataGridView.TabIndex = 15;
            // 
            // iconColumn
            // 
            iconColumn.FillWeight = 30.45685F;
            iconColumn.HeaderText = "";
            iconColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            iconColumn.Name = "iconColumn";
            iconColumn.ReadOnly = true;
            iconColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            iconColumn.Width = 18;
            // 
            // typeColumn
            // 
            typeColumn.FillWeight = 169.5432F;
            typeColumn.HeaderText = "Тип";
            typeColumn.Name = "typeColumn";
            typeColumn.ReadOnly = true;
            typeColumn.Resizable = DataGridViewTriState.True;
            typeColumn.Width = 51;
            // 
            // categoryColumn
            // 
            categoryColumn.HeaderText = "Категория";
            categoryColumn.Name = "categoryColumn";
            categoryColumn.ReadOnly = true;
            categoryColumn.Width = 87;
            // 
            // timeColumn
            // 
            timeColumn.HeaderText = "Время";
            timeColumn.Name = "timeColumn";
            timeColumn.ReadOnly = true;
            timeColumn.Width = 66;
            // 
            // sourceColumn
            // 
            sourceColumn.HeaderText = "Источник";
            sourceColumn.Name = "sourceColumn";
            sourceColumn.ReadOnly = true;
            sourceColumn.Width = 85;
            // 
            // messageColumn
            // 
            messageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            messageColumn.HeaderText = "Данные";
            messageColumn.Name = "messageColumn";
            messageColumn.ReadOnly = true;
            // 
            // configPage
            // 
            configPage.Image = null;
            configPage.ImageSize = new Size(16, 16);
            configPage.Location = new Point(1, 2);
            configPage.Margin = new Padding(4, 3, 4, 3);
            configPage.Name = "configPage";
            configPage.ShowCloseButton = true;
            configPage.Size = new Size(620, 316);
            configPage.TabForeColor = Color.FromArgb(224, 224, 224);
            configPage.TabIndex = 3;
            configPage.Text = "Проблемы конфигурации";
            configPage.ThemesEnabled = false;
            // 
            // issueIcons
            // 
            issueIcons.ColorDepth = ColorDepth.Depth8Bit;
            issueIcons.ImageSize = new Size(15, 15);
            issueIcons.TransparentColor = Color.Transparent;
            // 
            // Terminal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(terminalTab);
            Margin = new Padding(15);
            Name = "Terminal";
            Size = new Size(623, 345);
            ((ISupportInitialize)terminalTab).EndInit();
            terminalTab.ResumeLayout(false);
            pshPage.ResumeLayout(false);
            suggestionsPanel.ResumeLayout(false);
            runtimePage.ResumeLayout(false);
            issueMainTLP.ResumeLayout(false);
            issueMainFLP.ResumeLayout(false);
            issueMainFLP.PerformLayout();
            ((ISupportInitialize)issuesDataGridView).EndInit();
            ResumeLayout(false);

        }

        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}