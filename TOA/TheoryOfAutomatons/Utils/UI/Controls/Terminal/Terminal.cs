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

namespace TheoryOfAutomatons.Utils.UI.Controls.Terminal
{
    internal class Terminal : UserControl
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

        // Команды
        private bool _isProcessingInput = false;
        private CommandHandler _commandHandler;
        private List<string> _commandHistory = new List<string>();
        private int _historyIndex = -1;
        private SyntaxHighlighter _highlighter = new SyntaxHighlighter();
        private StringBuilder _inputBuffer = new StringBuilder();
        public CommandHandler CommandHandler => _commandHandler;


        // Ошибки
        private readonly BindingList<ExecutionIssue> _executionIssues = new BindingList<ExecutionIssue>();
        private DataGridView issuesDataGridView;
        private DataGridViewImageColumn iconColumn;
        private DataGridViewTextBoxColumn typeColumn;
        private DataGridViewTextBoxColumn timeColumn;
        private DataGridViewTextBoxColumn sourceColumn;
        private DataGridViewTextBoxColumn messageColumn;
        private ExecutionIssueType _visibleIssueTypes = ExecutionIssueType.All;

        #endregion

        public Terminal()
        {
            InitializeComponent();
            InitializeCommandSystem();
            ConfigureIssuePanel();
        }

 

        #region Работа с текстом

        private string GetCurrentLine()
        {
            var lineIndex = pshTerminal.GetLineFromCharIndex(pshTerminal.SelectionStart);
            return (lineIndex >= 0 && lineIndex < pshTerminal.Lines.Length)
                ? pshTerminal.Lines[lineIndex]
                : string.Empty;
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
            AppendText(CommandHandler.CommandChar + "> ", Color.Green);
            pshTerminal.ScrollToCaret();
            _isProcessingInput = false;
        }

        public void AppendText(string text, Color color)
        {
            if (pshTerminal.InvokeRequired)
            {
                pshTerminal.Invoke(new System.Action(() => AppendText(text, color)));
                return;
            }

            pshTerminal.SelectionStart = pshTerminal.TextLength;
            pshTerminal.SelectionColor = color;
            pshTerminal.AppendText(text);
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

        #endregion



        #region Команды

        private void InitializeCommandSystem()
        {
            _commandHandler = new CommandHandler();
            _commandHandler.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            AppendPrompt();
        }

        private void ProcessCommand(string input)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                input = input.TrimStart(CommandHandler.CommandChar, '>', ' ');
                if (string.IsNullOrWhiteSpace(input)) return;

                _commandHistory.Insert(0, input);
                _historyIndex = -1;

                AppendText($" --- [{DateTime.Now:HH:mm:ss}] --- ", Color.Gray);
                // _highlighter.HighlightInput(pshTerminal, input);

                var result = _commandHandler.Execute(input, this);
                if (input != "clear")
                {
                    var s = result.ExecutionTimeToString();
                    AppendText(s, Color.Gray);
                    AppendText(Environment.NewLine + result.ToString() + Environment.NewLine, result.Success ? Color.Lime : Color.Red);
                }
            }
            catch (Exception ex)
            {
                AppendText($"ERROR: {ex.Message}\n", Color.Red);
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
            issuesDataGridView.Font = new Font("Arial", 7f, FontStyle.Bold);
            issuesDataGridView.VirtualMode = true;
            issuesDataGridView.AllowDrop = false;
            issuesDataGridView.AutoGenerateColumns = true;
            issuesDataGridView.AutoResizeColumnHeadersHeight();
            issuesDataGridView.Refresh();
            issuesDataGridView.CellValueNeeded += IssuesDataGridView_CellValueNeeded;
            issuesDataGridView.CellFormatting += IssuesDataGridView_CellFormatting;

            // Пример
            try
            {
                ExceptionGenerator.ThrowRandomException();
            }
            catch (Exception ex)
            {
                LogExecIssue(ExecutionIssueType.Error, "Lydicrite", $"Это пример поимки ошибки", ex);
            }
            finally
            {
                LogExecIssue(ExecutionIssueType.Warning, "Was", "Это пример вывода предупреждения");
                LogExecIssue(ExecutionIssueType.Information, "Here", "Это пример вывода сообщения");
            }
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
                    case ExecutionIssueType.Information:
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
            if (showInfoMessages) _visibleIssueTypes |= ExecutionIssueType.Information;

            UpdateFilteredView();
        }

        public void LogExecIssue(ExecutionIssueType issueTupe, string source, string message, Exception ex = null)
        {
            switch (issueTupe)
            {
                case ExecutionIssueType.Error:
                    LogError(source, message, ex); break;
                case ExecutionIssueType.Warning:
                    LogWarning(source, message, ex); break;
                case ExecutionIssueType.Information:
                    LogMessage(source, message, ex); break;
                default: throw new NotImplementedException();
            }
        }

        private void LogError(string source, string message, Exception ex = null)
        {
            var issue = new ExecutionIssue(ExecutionIssueType.Error, source, message, ex);
            AddIssueToList(issue);
        }

        private void LogWarning(string source, string message, Exception ex = null)
        {
            var issue = new ExecutionIssue(ExecutionIssueType.Warning, source, message, ex);
            AddIssueToList(issue);
        }

        private void LogMessage(string source, string message, Exception ex = null)
        {
            var issue = new ExecutionIssue(ExecutionIssueType.Information, source, message, ex);
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
            if (e.KeyChar == CommandHandler.CommandChar)
            {
                int currentPos = pshTerminal.SelectionStart;
                if (currentPos > 0 && pshTerminal.Text[currentPos - 1] == CommandHandler.CommandChar)
                {
                    e.Handled = true; // Блокируем ввод
                }
            }
        }

        private void pshTerminal_KeyDown(object sender, KeyEventArgs e)
        {
            int currentLineStart = pshTerminal.GetFirstCharIndexOfCurrentLine();

            if (pshTerminal.SelectionStart <= currentLineStart + 3 && (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete))
            {
                e.SuppressKeyPress = true;
                return;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                // Проверяем, находится ли курсор в конце последней строки
                int currentLineIndex = pshTerminal.GetLineFromCharIndex(pshTerminal.SelectionStart);
                int lastLineIndex = pshTerminal.Lines.Length - 1;
                bool isCursorAtEnd = pshTerminal.SelectionStart == pshTerminal.TextLength;

                if (currentLineIndex == lastLineIndex && isCursorAtEnd)
                {
                    var input = GetCurrentLine().TrimStart(CommandHandler.CommandChar, '>', ' ');
                    if (!string.IsNullOrEmpty(input))
                        ProcessCommand(input);
                    AppendPrompt();
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

            // Горячие клавиши
            else if (e.Control && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true;
                string clipboardText = Clipboard.GetText().Replace("\n", "").Replace("\r", "");
                pshTerminal.SelectedText = clipboardText;
            }
            else if(e.Control && e.KeyCode == Keys.L)
            {
                ProcessCommand("clear");
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
                var currentLine = GetCurrentLine();

                if (currentLine.Length > 3)
                {
                    var commandPart = GetCurrentLine().Substring(3);
                    if (commandPart.StartsWith(CommandHandler.CommandChar.ToString()))
                    {
                        ShowSuggestions(commandPart);
                    }
                    else
                    {
                        suggestionsPanel.Visible = false;
                        suggestionsList.Visible = false;
                    }
                }
                else
                {
                    suggestionsPanel.Visible = false;
                    suggestionsList.Visible = false;
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
                        e.Value = issue.Type.ToString();
                        break;
                    case 2: // Время
                        e.Value = issue.Timestamp.ToString("HH:mm:ss");
                        break;
                    case 3: // Источник
                        e.Value = issue.Source;
                        break;
                    case 4: // Сообщение
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.terminalTab = new Syncfusion.Windows.Forms.Tools.TabControlAdv();
            this.pshPage = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.suggestionsPanel = new System.Windows.Forms.Panel();
            this.suggestionsList = new System.Windows.Forms.ListBox();
            this.pshTerminal = new System.Windows.Forms.RichTextBox();
            this.runtimePage = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.issueMainTLP = new System.Windows.Forms.TableLayoutPanel();
            this.issuesDataGridView = new System.Windows.Forms.DataGridView();
            this.iconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.typeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sourceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.issueMainFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.showErrorsFilter = new System.Windows.Forms.CheckBox();
            this.showWarningsFilter = new System.Windows.Forms.CheckBox();
            this.showInfoMessagesFilter = new System.Windows.Forms.CheckBox();
            this.configPage = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.issueIcons = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.terminalTab)).BeginInit();
            this.terminalTab.SuspendLayout();
            this.pshPage.SuspendLayout();
            this.suggestionsPanel.SuspendLayout();
            this.runtimePage.SuspendLayout();
            this.issueMainTLP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.issuesDataGridView)).BeginInit();
            this.issueMainFLP.SuspendLayout();
            this.SuspendLayout();
            // 
            // terminalTab
            // 
            this.terminalTab.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.terminalTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.terminalTab.BeforeTouchSize = new System.Drawing.Size(617, 281);
            this.terminalTab.Controls.Add(this.pshPage);
            this.terminalTab.Controls.Add(this.runtimePage);
            this.terminalTab.Controls.Add(this.configPage);
            this.terminalTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.terminalTab.Location = new System.Drawing.Point(0, 0);
            this.terminalTab.Margin = new System.Windows.Forms.Padding(7);
            this.terminalTab.Name = "terminalTab";
            this.terminalTab.Size = new System.Drawing.Size(617, 281);
            this.terminalTab.TabIndex = 0;
            this.terminalTab.ThemeStyle.PrimitiveButtonStyle.DisabledNextPageImage = null;
            // 
            // pshPage
            // 
            this.pshPage.Controls.Add(this.suggestionsPanel);
            this.pshPage.Controls.Add(this.pshTerminal);
            this.pshPage.Image = null;
            this.pshPage.ImageSize = new System.Drawing.Size(16, 16);
            this.pshPage.Location = new System.Drawing.Point(1, 2);
            this.pshPage.Name = "pshPage";
            this.pshPage.ShowCloseButton = true;
            this.pshPage.Size = new System.Drawing.Size(614, 254);
            this.pshPage.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pshPage.TabIndex = 1;
            this.pshPage.Text = "PowerShell";
            this.pshPage.ThemesEnabled = false;
            // 
            // suggestionsPanel
            // 
            this.suggestionsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.suggestionsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.suggestionsPanel.Controls.Add(this.suggestionsList);
            this.suggestionsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.suggestionsPanel.Location = new System.Drawing.Point(464, 0);
            this.suggestionsPanel.Name = "suggestionsPanel";
            this.suggestionsPanel.Size = new System.Drawing.Size(150, 254);
            this.suggestionsPanel.TabIndex = 2;
            this.suggestionsPanel.Visible = false;
            // 
            // suggestionsList
            // 
            this.suggestionsList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.suggestionsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.suggestionsList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.suggestionsList.FormattingEnabled = true;
            this.suggestionsList.Location = new System.Drawing.Point(0, 0);
            this.suggestionsList.Name = "suggestionsList";
            this.suggestionsList.Size = new System.Drawing.Size(146, 250);
            this.suggestionsList.TabIndex = 0;
            this.suggestionsList.Visible = false;
            this.suggestionsList.Click += new System.EventHandler(this.suggestionsList_Click);
            // 
            // pshTerminal
            // 
            this.pshTerminal.BackColor = System.Drawing.Color.Black;
            this.pshTerminal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pshTerminal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pshTerminal.Location = new System.Drawing.Point(0, 0);
            this.pshTerminal.Name = "pshTerminal";
            this.pshTerminal.Size = new System.Drawing.Size(614, 254);
            this.pshTerminal.TabIndex = 0;
            this.pshTerminal.Text = "";
            this.pshTerminal.SelectionChanged += new System.EventHandler(this.pshTerminal_SelectionChanged);
            this.pshTerminal.TextChanged += new System.EventHandler(this.pshTerminal_TextChanged);
            this.pshTerminal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pshTerminal_KeyDown);
            this.pshTerminal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pshTerminal_KeyPress);
            this.pshTerminal.KeyUp += new System.Windows.Forms.KeyEventHandler(this.pshTerminal_KeyUp);
            // 
            // runtimePage
            // 
            this.runtimePage.Controls.Add(this.issueMainTLP);
            this.runtimePage.Image = null;
            this.runtimePage.ImageSize = new System.Drawing.Size(16, 16);
            this.runtimePage.Location = new System.Drawing.Point(1, 2);
            this.runtimePage.Name = "runtimePage";
            this.runtimePage.ShowCloseButton = true;
            this.runtimePage.Size = new System.Drawing.Size(614, 254);
            this.runtimePage.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.runtimePage.TabIndex = 2;
            this.runtimePage.Text = "Проблемы выполнения";
            this.runtimePage.ThemesEnabled = false;
            // 
            // issueMainTLP
            // 
            this.issueMainTLP.ColumnCount = 1;
            this.issueMainTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.issueMainTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.issueMainTLP.Controls.Add(this.issuesDataGridView, 0, 1);
            this.issueMainTLP.Controls.Add(this.issueMainFLP, 0, 0);
            this.issueMainTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.issueMainTLP.Location = new System.Drawing.Point(0, 0);
            this.issueMainTLP.Name = "issueMainTLP";
            this.issueMainTLP.RowCount = 2;
            this.issueMainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.56693F));
            this.issueMainTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.43307F));
            this.issueMainTLP.Size = new System.Drawing.Size(614, 254);
            this.issueMainTLP.TabIndex = 1;
            // 
            // issuesDataGridView
            // 
            this.issuesDataGridView.AllowUserToAddRows = false;
            this.issuesDataGridView.AllowUserToDeleteRows = false;
            this.issuesDataGridView.AllowUserToResizeRows = false;
            this.issuesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.issuesDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.issuesDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.issuesDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.issuesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.issuesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.issuesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.issuesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iconColumn,
            this.typeColumn,
            this.timeColumn,
            this.sourceColumn,
            this.messageColumn});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.issuesDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.issuesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.issuesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.issuesDataGridView.EnableHeadersVisualStyles = false;
            this.issuesDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.issuesDataGridView.Location = new System.Drawing.Point(3, 40);
            this.issuesDataGridView.Name = "issuesDataGridView";
            this.issuesDataGridView.ReadOnly = true;
            this.issuesDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.issuesDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.issuesDataGridView.RowHeadersVisible = false;
            this.issuesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            this.issuesDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.issuesDataGridView.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.issuesDataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.DimGray;
            this.issuesDataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.issuesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.issuesDataGridView.Size = new System.Drawing.Size(608, 211);
            this.issuesDataGridView.TabIndex = 15;
            // 
            // iconColumn
            // 
            this.iconColumn.FillWeight = 30.45685F;
            this.iconColumn.HeaderText = "";
            this.iconColumn.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.iconColumn.Name = "iconColumn";
            this.iconColumn.ReadOnly = true;
            this.iconColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.iconColumn.Width = 18;
            // 
            // typeColumn
            // 
            this.typeColumn.FillWeight = 169.5432F;
            this.typeColumn.HeaderText = "Тип";
            this.typeColumn.Name = "typeColumn";
            this.typeColumn.ReadOnly = true;
            this.typeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.typeColumn.Width = 52;
            // 
            // timeColumn
            // 
            this.timeColumn.HeaderText = "Время";
            this.timeColumn.Name = "timeColumn";
            this.timeColumn.ReadOnly = true;
            this.timeColumn.Width = 68;
            // 
            // sourceColumn
            // 
            this.sourceColumn.HeaderText = "Источник";
            this.sourceColumn.Name = "sourceColumn";
            this.sourceColumn.ReadOnly = true;
            this.sourceColumn.Width = 85;
            // 
            // messageColumn
            // 
            this.messageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.messageColumn.HeaderText = "Данные";
            this.messageColumn.Name = "messageColumn";
            this.messageColumn.ReadOnly = true;
            // 
            // issueMainFLP
            // 
            this.issueMainFLP.Controls.Add(this.showErrorsFilter);
            this.issueMainFLP.Controls.Add(this.showWarningsFilter);
            this.issueMainFLP.Controls.Add(this.showInfoMessagesFilter);
            this.issueMainFLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.issueMainFLP.Location = new System.Drawing.Point(3, 3);
            this.issueMainFLP.Name = "issueMainFLP";
            this.issueMainFLP.Size = new System.Drawing.Size(608, 31);
            this.issueMainFLP.TabIndex = 0;
            // 
            // showErrorsFilter
            // 
            this.showErrorsFilter.Appearance = System.Windows.Forms.Appearance.Button;
            this.showErrorsFilter.AutoSize = true;
            this.showErrorsFilter.Checked = true;
            this.showErrorsFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showErrorsFilter.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.showErrorsFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showErrorsFilter.ForeColor = System.Drawing.Color.Gainsboro;
            this.showErrorsFilter.Image = global::TOA.Properties.Resources.ErrorIcon;
            this.showErrorsFilter.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.showErrorsFilter.Location = new System.Drawing.Point(3, 3);
            this.showErrorsFilter.Name = "showErrorsFilter";
            this.showErrorsFilter.Size = new System.Drawing.Size(72, 23);
            this.showErrorsFilter.TabIndex = 0;
            this.showErrorsFilter.Text = "Ошибки";
            this.showErrorsFilter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.showErrorsFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.showErrorsFilter.UseVisualStyleBackColor = true;
            this.showErrorsFilter.CheckedChanged += new System.EventHandler(this.showErrors_CheckedChanged);
            // 
            // showWarningsFilter
            // 
            this.showWarningsFilter.Appearance = System.Windows.Forms.Appearance.Button;
            this.showWarningsFilter.AutoSize = true;
            this.showWarningsFilter.Checked = true;
            this.showWarningsFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showWarningsFilter.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.showWarningsFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showWarningsFilter.ForeColor = System.Drawing.Color.Gainsboro;
            this.showWarningsFilter.Image = global::TOA.Properties.Resources.WarningIcon;
            this.showWarningsFilter.Location = new System.Drawing.Point(81, 3);
            this.showWarningsFilter.Name = "showWarningsFilter";
            this.showWarningsFilter.Size = new System.Drawing.Size(119, 23);
            this.showWarningsFilter.TabIndex = 1;
            this.showWarningsFilter.Text = "Предупреждения";
            this.showWarningsFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.showWarningsFilter.UseVisualStyleBackColor = true;
            this.showWarningsFilter.CheckedChanged += new System.EventHandler(this.showWarnings_CheckedChanged);
            // 
            // showInfoMessagesFilter
            // 
            this.showInfoMessagesFilter.Appearance = System.Windows.Forms.Appearance.Button;
            this.showInfoMessagesFilter.AutoSize = true;
            this.showInfoMessagesFilter.Checked = true;
            this.showInfoMessagesFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showInfoMessagesFilter.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.showInfoMessagesFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showInfoMessagesFilter.ForeColor = System.Drawing.Color.Gainsboro;
            this.showInfoMessagesFilter.Image = global::TOA.Properties.Resources.InformationIcon;
            this.showInfoMessagesFilter.Location = new System.Drawing.Point(206, 3);
            this.showInfoMessagesFilter.Name = "showInfoMessagesFilter";
            this.showInfoMessagesFilter.Size = new System.Drawing.Size(90, 23);
            this.showInfoMessagesFilter.TabIndex = 2;
            this.showInfoMessagesFilter.Text = "Сообщения";
            this.showInfoMessagesFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.showInfoMessagesFilter.UseVisualStyleBackColor = true;
            this.showInfoMessagesFilter.CheckedChanged += new System.EventHandler(this.showInfoMessages_CheckedChanged);
            // 
            // configPage
            // 
            this.configPage.Image = null;
            this.configPage.ImageSize = new System.Drawing.Size(16, 16);
            this.configPage.Location = new System.Drawing.Point(1, 2);
            this.configPage.Name = "configPage";
            this.configPage.ShowCloseButton = true;
            this.configPage.Size = new System.Drawing.Size(614, 254);
            this.configPage.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.configPage.TabIndex = 3;
            this.configPage.Text = "Проблемы конфигурации";
            this.configPage.ThemesEnabled = false;
            // 
            // issueIcons
            // 
            this.issueIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.issueIcons.ImageSize = new System.Drawing.Size(15, 15);
            this.issueIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // Terminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.terminalTab);
            this.Margin = new System.Windows.Forms.Padding(13);
            this.Name = "Terminal";
            this.Size = new System.Drawing.Size(617, 281);
            ((System.ComponentModel.ISupportInitialize)(this.terminalTab)).EndInit();
            this.terminalTab.ResumeLayout(false);
            this.pshPage.ResumeLayout(false);
            this.suggestionsPanel.ResumeLayout(false);
            this.runtimePage.ResumeLayout(false);
            this.issueMainTLP.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.issuesDataGridView)).EndInit();
            this.issueMainFLP.ResumeLayout(false);
            this.issueMainFLP.PerformLayout();
            this.ResumeLayout(false);

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