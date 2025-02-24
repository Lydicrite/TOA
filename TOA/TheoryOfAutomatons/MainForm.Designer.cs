using static Syncfusion.Windows.Forms.Tools.Navigation.Bar;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Windows.Forms;
using TheoryOfAutomatons.Utils;
using TheoryOfAutomatons.Utils.UI.Controls;

namespace TheoryOfAutomatons
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.loadFromFileTSB = new System.Windows.Forms.ToolStripButton();
            this.saveToFileTSB = new System.Windows.Forms.ToolStripButton();
            this.clearTSB = new System.Windows.Forms.ToolStripButton();
            this.mainHorizontalSplitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxesPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.generateRandomSequence = new System.Windows.Forms.Button();
            this.sequenceTextBox = new System.Windows.Forms.TextBox();
            this.analyze = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.addState = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.typeBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.settingsFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.innerStateCP = new System.Windows.Forms.PictureBox();
            this.highlightedBorderCP = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.inactiveBorderCP = new System.Windows.Forms.PictureBox();
            this.activeBorderCP = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.borderNUD = new System.Windows.Forms.NumericUpDown();
            this.cirlceDiameterNUD = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.prohibitIntersectingPaths = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.transitionBlackPenCP = new System.Windows.Forms.PictureBox();
            this.transitionBlackPenNUD = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.transitionLightPenCP = new System.Windows.Forms.PictureBox();
            this.transitionLightPenNUD = new System.Windows.Forms.NumericUpDown();
            this.developerMode = new System.Windows.Forms.CheckBox();
            this.container = new System.Windows.Forms.PictureBox();
            this.mainGB = new System.Windows.Forms.GroupBox();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.containerCP = new System.Windows.Forms.PictureBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.drawStepDelayNUD = new System.Windows.Forms.NumericUpDown();
            this.outputAlphabet = new TheoryOfAutomatons.Utils.UI.Controls.TypedListBox();
            this.inputAlphabet = new TheoryOfAutomatons.Utils.UI.Controls.TypedListBox();
            this.terminal1 = new TheoryOfAutomatons.Utils.UI.Controls.Terminal.Terminal();
            this.mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainHorizontalSplitContainer)).BeginInit();
            this.mainHorizontalSplitContainer.Panel1.SuspendLayout();
            this.mainHorizontalSplitContainer.Panel2.SuspendLayout();
            this.mainHorizontalSplitContainer.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.groupBoxesPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.settingsFLP.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.innerStateCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.highlightedBorderCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inactiveBorderCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.activeBorderCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.borderNUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cirlceDiameterNUD)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transitionBlackPenCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transitionBlackPenNUD)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transitionLightPenCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transitionLightPenNUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.container)).BeginInit();
            this.mainGB.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.containerCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.drawStepDelayNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 175);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(150, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(150, 25);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.RightToolStripPanel.Location = new System.Drawing.Point(150, 25);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 150);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 25);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 150);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(150, 150);
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.mainToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFromFileTSB,
            this.saveToFileTSB,
            this.clearTSB});
            this.mainToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mainToolStrip.Size = new System.Drawing.Size(864, 25);
            this.mainToolStrip.TabIndex = 1;
            this.mainToolStrip.Text = "Меню";
            // 
            // loadFromFileTSB
            // 
            this.loadFromFileTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadFromFileTSB.Image = global::TOA.Properties.Resources.LoadButton;
            this.loadFromFileTSB.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.loadFromFileTSB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadFromFileTSB.Name = "loadFromFileTSB";
            this.loadFromFileTSB.Size = new System.Drawing.Size(23, 22);
            this.loadFromFileTSB.Text = "Загрузить автомат из файла...";
            // 
            // saveToFileTSB
            // 
            this.saveToFileTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToFileTSB.Image = global::TOA.Properties.Resources.SaveButton;
            this.saveToFileTSB.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.saveToFileTSB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToFileTSB.Name = "saveToFileTSB";
            this.saveToFileTSB.Size = new System.Drawing.Size(23, 22);
            this.saveToFileTSB.Text = "Сохранить автомат в файл...";
            // 
            // clearTSB
            // 
            this.clearTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearTSB.Image = global::TOA.Properties.Resources.ResetButton;
            this.clearTSB.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.clearTSB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearTSB.Name = "clearTSB";
            this.clearTSB.Size = new System.Drawing.Size(23, 22);
            this.clearTSB.Text = "toolStripButton1";
            // 
            // mainHorizontalSplitContainer
            // 
            this.mainHorizontalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainHorizontalSplitContainer.IsSplitterFixed = true;
            this.mainHorizontalSplitContainer.Location = new System.Drawing.Point(3, 16);
            this.mainHorizontalSplitContainer.Name = "mainHorizontalSplitContainer";
            this.mainHorizontalSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainHorizontalSplitContainer.Panel1
            // 
            this.mainHorizontalSplitContainer.Panel1.Controls.Add(this.tableLayoutPanel);
            // 
            // mainHorizontalSplitContainer.Panel2
            // 
            this.mainHorizontalSplitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.mainHorizontalSplitContainer.Panel2.Controls.Add(this.terminal1);
            this.mainHorizontalSplitContainer.Size = new System.Drawing.Size(834, 980);
            this.mainHorizontalSplitContainer.SplitterDistance = 672;
            this.mainHorizontalSplitContainer.TabIndex = 0;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 273F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.groupBoxesPanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.container, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 672F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(834, 672);
            this.tableLayoutPanel.TabIndex = 3;
            // 
            // groupBoxesPanel
            // 
            this.groupBoxesPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxesPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.groupBoxesPanel.ColumnCount = 1;
            this.groupBoxesPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.groupBoxesPanel.Controls.Add(this.groupBox3, 0, 3);
            this.groupBoxesPanel.Controls.Add(this.groupBox1, 0, 1);
            this.groupBoxesPanel.Controls.Add(this.groupBox2, 0, 2);
            this.groupBoxesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxesPanel.Location = new System.Drawing.Point(3, 3);
            this.groupBoxesPanel.Name = "groupBoxesPanel";
            this.groupBoxesPanel.RowCount = 4;
            this.groupBoxesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.groupBoxesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.groupBoxesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 340F));
            this.groupBoxesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.groupBoxesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.groupBoxesPanel.Size = new System.Drawing.Size(267, 666);
            this.groupBoxesPanel.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.generateRandomSequence);
            this.groupBox3.Controls.Add(this.sequenceTextBox);
            this.groupBox3.Controls.Add(this.analyze);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox3.ForeColor = System.Drawing.Color.Aquamarine;
            this.groupBox3.Location = new System.Drawing.Point(3, 611);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(264, 52);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Анализ входной строки";
            // 
            // generateRandomSequence
            // 
            this.generateRandomSequence.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.generateRandomSequence.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.generateRandomSequence.Enabled = false;
            this.generateRandomSequence.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.generateRandomSequence.ForeColor = System.Drawing.Color.Gainsboro;
            this.generateRandomSequence.Image = global::TOA.Properties.Resources.RandomSequence;
            this.generateRandomSequence.Location = new System.Drawing.Point(212, 20);
            this.generateRandomSequence.Name = "generateRandomSequence";
            this.generateRandomSequence.Size = new System.Drawing.Size(20, 20);
            this.generateRandomSequence.TabIndex = 4;
            this.generateRandomSequence.UseVisualStyleBackColor = false;
            // 
            // sequenceTextBox
            // 
            this.sequenceTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.sequenceTextBox.Enabled = false;
            this.sequenceTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sequenceTextBox.ForeColor = System.Drawing.Color.Gainsboro;
            this.sequenceTextBox.Location = new System.Drawing.Point(7, 20);
            this.sequenceTextBox.Name = "sequenceTextBox";
            this.sequenceTextBox.Size = new System.Drawing.Size(199, 20);
            this.sequenceTextBox.TabIndex = 3;
            // 
            // analyze
            // 
            this.analyze.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.analyze.Enabled = false;
            this.analyze.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.analyze.Font = new System.Drawing.Font("Microsoft Tai Le", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.analyze.ForeColor = System.Drawing.Color.Gainsboro;
            this.analyze.Image = global::TOA.Properties.Resources.AnalyzeButton;
            this.analyze.Location = new System.Drawing.Point(238, 20);
            this.analyze.Name = "analyze";
            this.analyze.Size = new System.Drawing.Size(20, 20);
            this.analyze.TabIndex = 2;
            this.analyze.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.outputAlphabet);
            this.groupBox1.Controls.Add(this.inputAlphabet);
            this.groupBox1.Controls.Add(this.addState);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.typeBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.ForeColor = System.Drawing.Color.Aquamarine;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 262);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры автомата";
            // 
            // addState
            // 
            this.addState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.addState.Enabled = false;
            this.addState.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addState.Font = new System.Drawing.Font("Microsoft Tai Le", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addState.ForeColor = System.Drawing.Color.Gainsboro;
            this.addState.Location = new System.Drawing.Point(6, 233);
            this.addState.Name = "addState";
            this.addState.Size = new System.Drawing.Size(252, 23);
            this.addState.TabIndex = 8;
            this.addState.Text = "Добавить состояние";
            this.addState.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Gainsboro;
            this.label3.Location = new System.Drawing.Point(3, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Выходной алфавит:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Gainsboro;
            this.label2.Location = new System.Drawing.Point(3, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Входной алфавит:";
            // 
            // typeBox
            // 
            this.typeBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.typeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.typeBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.typeBox.ForeColor = System.Drawing.Color.Gainsboro;
            this.typeBox.FormattingEnabled = true;
            this.typeBox.Items.AddRange(new object[] {
            "Мили",
            "Мура"});
            this.typeBox.Location = new System.Drawing.Point(44, 20);
            this.typeBox.Name = "typeBox";
            this.typeBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.typeBox.Size = new System.Drawing.Size(214, 21);
            this.typeBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label1.Location = new System.Drawing.Point(3, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Тип: ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.settingsFLP);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.ForeColor = System.Drawing.Color.Aquamarine;
            this.groupBox2.Location = new System.Drawing.Point(3, 271);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(264, 334);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Параметры отрисовки";
            // 
            // settingsFLP
            // 
            this.settingsFLP.AutoScroll = true;
            this.settingsFLP.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settingsFLP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.settingsFLP.Controls.Add(this.groupBox8);
            this.settingsFLP.Controls.Add(this.groupBox4);
            this.settingsFLP.Controls.Add(this.groupBox5);
            this.settingsFLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsFLP.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.settingsFLP.Location = new System.Drawing.Point(3, 19);
            this.settingsFLP.Name = "settingsFLP";
            this.settingsFLP.Size = new System.Drawing.Size(258, 312);
            this.settingsFLP.TabIndex = 0;
            this.settingsFLP.WrapContents = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.innerStateCP);
            this.groupBox4.Controls.Add(this.highlightedBorderCP);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.inactiveBorderCP);
            this.groupBox4.Controls.Add(this.activeBorderCP);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.borderNUD);
            this.groupBox4.Controls.Add(this.cirlceDiameterNUD);
            this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox4.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox4.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox4.Location = new System.Drawing.Point(3, 101);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(230, 173);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Параметры отрисовки состояний";
            // 
            // innerStateCP
            // 
            this.innerStateCP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.innerStateCP.BackColor = System.Drawing.Color.LightGray;
            this.innerStateCP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.innerStateCP.Enabled = false;
            this.innerStateCP.Location = new System.Drawing.Point(149, 147);
            this.innerStateCP.MaximumSize = new System.Drawing.Size(77, 20);
            this.innerStateCP.MinimumSize = new System.Drawing.Size(77, 20);
            this.innerStateCP.Name = "innerStateCP";
            this.innerStateCP.Size = new System.Drawing.Size(77, 20);
            this.innerStateCP.TabIndex = 30;
            this.innerStateCP.TabStop = false;
            // 
            // highlightedBorderCP
            // 
            this.highlightedBorderCP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.highlightedBorderCP.BackColor = System.Drawing.Color.DarkGray;
            this.highlightedBorderCP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.highlightedBorderCP.Enabled = false;
            this.highlightedBorderCP.Location = new System.Drawing.Point(149, 121);
            this.highlightedBorderCP.MaximumSize = new System.Drawing.Size(77, 20);
            this.highlightedBorderCP.MinimumSize = new System.Drawing.Size(77, 20);
            this.highlightedBorderCP.Name = "highlightedBorderCP";
            this.highlightedBorderCP.Size = new System.Drawing.Size(77, 20);
            this.highlightedBorderCP.TabIndex = 29;
            this.highlightedBorderCP.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.ForeColor = System.Drawing.Color.Gainsboro;
            this.label8.Location = new System.Drawing.Point(3, 149);
            this.label8.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(136, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Цвет заливки состояния:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.ForeColor = System.Drawing.Color.Gainsboro;
            this.label9.Location = new System.Drawing.Point(3, 123);
            this.label9.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(137, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Цвет подсветки границы:";
            // 
            // inactiveBorderCP
            // 
            this.inactiveBorderCP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inactiveBorderCP.BackColor = System.Drawing.Color.Black;
            this.inactiveBorderCP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.inactiveBorderCP.Enabled = false;
            this.inactiveBorderCP.Location = new System.Drawing.Point(149, 95);
            this.inactiveBorderCP.MaximumSize = new System.Drawing.Size(77, 20);
            this.inactiveBorderCP.MinimumSize = new System.Drawing.Size(77, 20);
            this.inactiveBorderCP.Name = "inactiveBorderCP";
            this.inactiveBorderCP.Size = new System.Drawing.Size(77, 20);
            this.inactiveBorderCP.TabIndex = 26;
            this.inactiveBorderCP.TabStop = false;
            // 
            // activeBorderCP
            // 
            this.activeBorderCP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activeBorderCP.BackColor = System.Drawing.Color.LimeGreen;
            this.activeBorderCP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.activeBorderCP.Enabled = false;
            this.activeBorderCP.Location = new System.Drawing.Point(149, 69);
            this.activeBorderCP.MaximumSize = new System.Drawing.Size(77, 20);
            this.activeBorderCP.MinimumSize = new System.Drawing.Size(77, 20);
            this.activeBorderCP.Name = "activeBorderCP";
            this.activeBorderCP.Size = new System.Drawing.Size(77, 20);
            this.activeBorderCP.TabIndex = 25;
            this.activeBorderCP.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.ForeColor = System.Drawing.Color.Gainsboro;
            this.label6.Location = new System.Drawing.Point(3, 97);
            this.label6.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(146, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Цвет неактивной границы: ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.ForeColor = System.Drawing.Color.Gainsboro;
            this.label7.Location = new System.Drawing.Point(3, 71);
            this.label7.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(131, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Цвет активной границы:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.Gainsboro;
            this.label5.Location = new System.Drawing.Point(3, 45);
            this.label5.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Ширина границы:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.Gainsboro;
            this.label4.Location = new System.Drawing.Point(3, 19);
            this.label4.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Диаметр состояния:";
            // 
            // borderNUD
            // 
            this.borderNUD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.borderNUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.borderNUD.Enabled = false;
            this.borderNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.borderNUD.ForeColor = System.Drawing.Color.Gainsboro;
            this.borderNUD.Location = new System.Drawing.Point(149, 43);
            this.borderNUD.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.borderNUD.MaximumSize = new System.Drawing.Size(77, 0);
            this.borderNUD.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.borderNUD.MinimumSize = new System.Drawing.Size(77, 0);
            this.borderNUD.Name = "borderNUD";
            this.borderNUD.Size = new System.Drawing.Size(77, 20);
            this.borderNUD.TabIndex = 15;
            this.borderNUD.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // cirlceDiameterNUD
            // 
            this.cirlceDiameterNUD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cirlceDiameterNUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.cirlceDiameterNUD.Enabled = false;
            this.cirlceDiameterNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cirlceDiameterNUD.ForeColor = System.Drawing.Color.Gainsboro;
            this.cirlceDiameterNUD.Location = new System.Drawing.Point(149, 17);
            this.cirlceDiameterNUD.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.cirlceDiameterNUD.MaximumSize = new System.Drawing.Size(77, 0);
            this.cirlceDiameterNUD.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.cirlceDiameterNUD.MinimumSize = new System.Drawing.Size(77, 0);
            this.cirlceDiameterNUD.Name = "cirlceDiameterNUD";
            this.cirlceDiameterNUD.Size = new System.Drawing.Size(77, 20);
            this.cirlceDiameterNUD.TabIndex = 14;
            this.cirlceDiameterNUD.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.prohibitIntersectingPaths);
            this.groupBox5.Controls.Add(this.groupBox7);
            this.groupBox5.Controls.Add(this.groupBox6);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox5.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox5.Location = new System.Drawing.Point(3, 280);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(230, 192);
            this.groupBox5.TabIndex = 23;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Отрисовка переходов";
            // 
            // prohibitIntersectingPaths
            // 
            this.prohibitIntersectingPaths.AutoSize = true;
            this.prohibitIntersectingPaths.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.prohibitIntersectingPaths.Checked = true;
            this.prohibitIntersectingPaths.CheckState = System.Windows.Forms.CheckState.Checked;
            this.prohibitIntersectingPaths.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.prohibitIntersectingPaths.Enabled = false;
            this.prohibitIntersectingPaths.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.prohibitIntersectingPaths.ForeColor = System.Drawing.Color.Gainsboro;
            this.prohibitIntersectingPaths.Location = new System.Drawing.Point(3, 172);
            this.prohibitIntersectingPaths.Name = "prohibitIntersectingPaths";
            this.prohibitIntersectingPaths.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.prohibitIntersectingPaths.Size = new System.Drawing.Size(224, 17);
            this.prohibitIntersectingPaths.TabIndex = 26;
            this.prohibitIntersectingPaths.Text = "Запрет самопересечений переходов: ";
            this.prohibitIntersectingPaths.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.label13);
            this.groupBox7.Controls.Add(this.transitionBlackPenCP);
            this.groupBox7.Controls.Add(this.transitionBlackPenNUD);
            this.groupBox7.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox7.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox7.Location = new System.Drawing.Point(6, 93);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(218, 70);
            this.groupBox7.TabIndex = 25;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Неактивные переходы";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.ForeColor = System.Drawing.Color.Gainsboro;
            this.label12.Location = new System.Drawing.Point(3, 45);
            this.label12.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(68, 13);
            this.label12.TabIndex = 34;
            this.label12.Text = "Цвет линии:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.ForeColor = System.Drawing.Color.Gainsboro;
            this.label13.Location = new System.Drawing.Point(3, 19);
            this.label13.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(89, 13);
            this.label13.TabIndex = 33;
            this.label13.Text = "Толщина линии:";
            // 
            // transitionBlackPenCP
            // 
            this.transitionBlackPenCP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transitionBlackPenCP.BackColor = System.Drawing.Color.Black;
            this.transitionBlackPenCP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.transitionBlackPenCP.Enabled = false;
            this.transitionBlackPenCP.Location = new System.Drawing.Point(135, 43);
            this.transitionBlackPenCP.MaximumSize = new System.Drawing.Size(77, 20);
            this.transitionBlackPenCP.MinimumSize = new System.Drawing.Size(77, 20);
            this.transitionBlackPenCP.Name = "transitionBlackPenCP";
            this.transitionBlackPenCP.Size = new System.Drawing.Size(77, 20);
            this.transitionBlackPenCP.TabIndex = 32;
            this.transitionBlackPenCP.TabStop = false;
            // 
            // transitionBlackPenNUD
            // 
            this.transitionBlackPenNUD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transitionBlackPenNUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.transitionBlackPenNUD.DecimalPlaces = 1;
            this.transitionBlackPenNUD.Enabled = false;
            this.transitionBlackPenNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.transitionBlackPenNUD.ForeColor = System.Drawing.Color.Gainsboro;
            this.transitionBlackPenNUD.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.transitionBlackPenNUD.Location = new System.Drawing.Point(135, 17);
            this.transitionBlackPenNUD.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            65536});
            this.transitionBlackPenNUD.MaximumSize = new System.Drawing.Size(77, 0);
            this.transitionBlackPenNUD.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.transitionBlackPenNUD.MinimumSize = new System.Drawing.Size(77, 0);
            this.transitionBlackPenNUD.Name = "transitionBlackPenNUD";
            this.transitionBlackPenNUD.Size = new System.Drawing.Size(77, 20);
            this.transitionBlackPenNUD.TabIndex = 31;
            this.transitionBlackPenNUD.Value = new decimal(new int[] {
            30,
            0,
            0,
            65536});
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.transitionLightPenCP);
            this.groupBox6.Controls.Add(this.transitionLightPenNUD);
            this.groupBox6.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox6.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox6.Location = new System.Drawing.Point(6, 17);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(218, 70);
            this.groupBox6.TabIndex = 24;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Активные переходы";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.ForeColor = System.Drawing.Color.Gainsboro;
            this.label10.Location = new System.Drawing.Point(3, 45);
            this.label10.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = "Цвет линии:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.ForeColor = System.Drawing.Color.Gainsboro;
            this.label11.Location = new System.Drawing.Point(3, 19);
            this.label11.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 13);
            this.label11.TabIndex = 29;
            this.label11.Text = "Толщина линии:";
            // 
            // transitionLightPenCP
            // 
            this.transitionLightPenCP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transitionLightPenCP.BackColor = System.Drawing.Color.LimeGreen;
            this.transitionLightPenCP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.transitionLightPenCP.Enabled = false;
            this.transitionLightPenCP.Location = new System.Drawing.Point(135, 43);
            this.transitionLightPenCP.MaximumSize = new System.Drawing.Size(77, 20);
            this.transitionLightPenCP.MinimumSize = new System.Drawing.Size(77, 20);
            this.transitionLightPenCP.Name = "transitionLightPenCP";
            this.transitionLightPenCP.Size = new System.Drawing.Size(77, 20);
            this.transitionLightPenCP.TabIndex = 27;
            this.transitionLightPenCP.TabStop = false;
            // 
            // transitionLightPenNUD
            // 
            this.transitionLightPenNUD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transitionLightPenNUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.transitionLightPenNUD.DecimalPlaces = 1;
            this.transitionLightPenNUD.Enabled = false;
            this.transitionLightPenNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.transitionLightPenNUD.ForeColor = System.Drawing.Color.Gainsboro;
            this.transitionLightPenNUD.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.transitionLightPenNUD.Location = new System.Drawing.Point(135, 17);
            this.transitionLightPenNUD.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            65536});
            this.transitionLightPenNUD.MaximumSize = new System.Drawing.Size(77, 0);
            this.transitionLightPenNUD.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.transitionLightPenNUD.MinimumSize = new System.Drawing.Size(77, 0);
            this.transitionLightPenNUD.Name = "transitionLightPenNUD";
            this.transitionLightPenNUD.Size = new System.Drawing.Size(77, 20);
            this.transitionLightPenNUD.TabIndex = 26;
            this.transitionLightPenNUD.Value = new decimal(new int[] {
            30,
            0,
            0,
            65536});
            // 
            // developerMode
            // 
            this.developerMode.AutoSize = true;
            this.developerMode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.developerMode.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.developerMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.developerMode.ForeColor = System.Drawing.Color.Gainsboro;
            this.developerMode.Location = new System.Drawing.Point(3, 72);
            this.developerMode.Name = "developerMode";
            this.developerMode.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.developerMode.Size = new System.Drawing.Size(224, 17);
            this.developerMode.TabIndex = 22;
            this.developerMode.Text = "Режим разработчика:";
            this.developerMode.UseVisualStyleBackColor = true;
            // 
            // container
            // 
            this.container.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(276, 3);
            this.container.MinimumSize = new System.Drawing.Size(555, 666);
            this.container.Name = "container";
            this.container.Size = new System.Drawing.Size(555, 666);
            this.container.TabIndex = 2;
            this.container.TabStop = false;
            // 
            // mainGB
            // 
            this.mainGB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainGB.Controls.Add(this.mainHorizontalSplitContainer);
            this.mainGB.Location = new System.Drawing.Point(12, 30);
            this.mainGB.Name = "mainGB";
            this.mainGB.Size = new System.Drawing.Size(840, 999);
            this.mainGB.TabIndex = 2;
            this.mainGB.TabStop = false;
            // 
            // colorPicker
            // 
            this.colorPicker.AnyColor = true;
            this.colorPicker.Color = System.Drawing.Color.Lime;
            this.colorPicker.FullOpen = true;
            this.colorPicker.ShowHelp = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label15);
            this.groupBox8.Controls.Add(this.drawStepDelayNUD);
            this.groupBox8.Controls.Add(this.containerCP);
            this.groupBox8.Controls.Add(this.developerMode);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox8.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox8.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(230, 92);
            this.groupBox8.TabIndex = 24;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Параметры рабочей области";
            // 
            // containerCP
            // 
            this.containerCP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.containerCP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this.containerCP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.containerCP.Enabled = false;
            this.containerCP.Location = new System.Drawing.Point(147, 20);
            this.containerCP.MaximumSize = new System.Drawing.Size(77, 20);
            this.containerCP.MinimumSize = new System.Drawing.Size(77, 20);
            this.containerCP.Name = "containerCP";
            this.containerCP.Size = new System.Drawing.Size(77, 20);
            this.containerCP.TabIndex = 30;
            this.containerCP.TabStop = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label14.ForeColor = System.Drawing.Color.Gainsboro;
            this.label14.Location = new System.Drawing.Point(3, 22);
            this.label14.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(123, 13);
            this.label14.TabIndex = 28;
            this.label14.Text = "Цвет рабочей области:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label15.ForeColor = System.Drawing.Color.Gainsboro;
            this.label15.Location = new System.Drawing.Point(3, 48);
            this.label15.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(141, 13);
            this.label15.TabIndex = 32;
            this.label15.Text = "Шаг отрисовки работы (c):";
            // 
            // drawStepDelayNUD
            // 
            this.drawStepDelayNUD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.drawStepDelayNUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.drawStepDelayNUD.DecimalPlaces = 2;
            this.drawStepDelayNUD.Enabled = false;
            this.drawStepDelayNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.drawStepDelayNUD.ForeColor = System.Drawing.Color.Gainsboro;
            this.drawStepDelayNUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.drawStepDelayNUD.Location = new System.Drawing.Point(147, 46);
            this.drawStepDelayNUD.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            65536});
            this.drawStepDelayNUD.MaximumSize = new System.Drawing.Size(77, 0);
            this.drawStepDelayNUD.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            131072});
            this.drawStepDelayNUD.MinimumSize = new System.Drawing.Size(77, 0);
            this.drawStepDelayNUD.Name = "drawStepDelayNUD";
            this.drawStepDelayNUD.Size = new System.Drawing.Size(77, 20);
            this.drawStepDelayNUD.TabIndex = 31;
            this.drawStepDelayNUD.Value = new decimal(new int[] {
            30,
            0,
            0,
            65536});
            // 
            // outputAlphabet
            // 
            this.outputAlphabet.Location = new System.Drawing.Point(7, 153);
            this.outputAlphabet.Name = "outputAlphabet";
            this.outputAlphabet.Size = new System.Drawing.Size(252, 74);
            this.outputAlphabet.SupportedType = TheoryOfAutomatons.Utils.UI.Controls.SupportedTypes.Char;
            this.outputAlphabet.TabIndex = 11;
            // 
            // inputAlphabet
            // 
            this.inputAlphabet.Location = new System.Drawing.Point(6, 60);
            this.inputAlphabet.Name = "inputAlphabet";
            this.inputAlphabet.Size = new System.Drawing.Size(252, 74);
            this.inputAlphabet.SupportedType = TheoryOfAutomatons.Utils.UI.Controls.SupportedTypes.Char;
            this.inputAlphabet.TabIndex = 10;
            // 
            // terminal1
            // 
            this.terminal1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.terminal1.Location = new System.Drawing.Point(0, 0);
            this.terminal1.Margin = new System.Windows.Forms.Padding(13);
            this.terminal1.Name = "terminal1";
            this.terminal1.Size = new System.Drawing.Size(834, 304);
            this.terminal1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(864, 1041);
            this.Controls.Add(this.mainGB);
            this.Controls.Add(this.mainToolStrip);
            this.MinimumSize = new System.Drawing.Size(880, 1080);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Теория Автоматов";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form_Load);
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.mainHorizontalSplitContainer.Panel1.ResumeLayout(false);
            this.mainHorizontalSplitContainer.Panel1.PerformLayout();
            this.mainHorizontalSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainHorizontalSplitContainer)).EndInit();
            this.mainHorizontalSplitContainer.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.groupBoxesPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.settingsFLP.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.innerStateCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.highlightedBorderCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inactiveBorderCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.activeBorderCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.borderNUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cirlceDiameterNUD)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transitionBlackPenCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transitionBlackPenNUD)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transitionLightPenCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transitionLightPenNUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.container)).EndInit();
            this.mainGB.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.containerCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.drawStepDelayNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton loadFromFileTSB;
        private System.Windows.Forms.ToolStripButton saveToFileTSB;
        private System.Windows.Forms.SplitContainer mainHorizontalSplitContainer;
        private System.Windows.Forms.GroupBox mainGB;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel groupBoxesPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button addState;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox typeBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button generateRandomSequence;
        private System.Windows.Forms.TextBox sequenceTextBox;
        private System.Windows.Forms.Button analyze;
        private System.Windows.Forms.PictureBox container;
        private ToolStripButton clearTSB;
        private TypedListBox inputAlphabet;
        private TypedListBox outputAlphabet;
        private GroupBox groupBox2;
        private FlowLayoutPanel settingsFLP;
        private GroupBox groupBox4;
        private Label label5;
        private Label label4;
        private NumericUpDown borderNUD;
        private NumericUpDown cirlceDiameterNUD;
        private CheckBox developerMode;
        private PictureBox activeBorderCP;
        private Label label6;
        private Label label7;
        private PictureBox innerStateCP;
        private PictureBox highlightedBorderCP;
        private Label label8;
        private Label label9;
        private PictureBox inactiveBorderCP;
        private GroupBox groupBox5;
        private GroupBox groupBox6;
        private GroupBox groupBox7;
        private CheckBox prohibitIntersectingPaths;
        private PictureBox transitionLightPenCP;
        private NumericUpDown transitionLightPenNUD;
        private Label label12;
        private Label label13;
        private PictureBox transitionBlackPenCP;
        private NumericUpDown transitionBlackPenNUD;
        private Label label10;
        private Label label11;
        private Utils.UI.Controls.Terminal.Terminal terminal1;
        private ColorDialog colorPicker;
        private GroupBox groupBox8;
        private PictureBox containerCP;
        private Label label14;
        private Label label15;
        private NumericUpDown drawStepDelayNUD;
    }
}

