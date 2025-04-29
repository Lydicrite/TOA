using static Syncfusion.Windows.Forms.Tools.Navigation.Bar;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Windows.Forms;
using TheoryOfAutomatons.Utils;
using TheoryOfAutomatons.Utils.UI.Controls;
using System;

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
            components = new System.ComponentModel.Container();
            toolTip = new ToolTip(components);
            BottomToolStripPanel = new ToolStripPanel();
            TopToolStripPanel = new ToolStripPanel();
            RightToolStripPanel = new ToolStripPanel();
            LeftToolStripPanel = new ToolStripPanel();
            ContentPanel = new ToolStripContentPanel();
            mainToolStrip = new ToolStrip();
            loadFromFileTSB = new ToolStripButton();
            saveToFileTSB = new ToolStripButton();
            clearTSB = new ToolStripButton();
            mainHorizontalSplitContainer = new SplitContainer();
            tableLayoutPanel = new TableLayoutPanel();
            groupBoxesPanel = new TableLayoutPanel();
            groupBox3 = new GroupBox();
            generateRandomSequence = new Button();
            sequenceTextBox = new TextBox();
            analyze = new Button();
            groupBox1 = new GroupBox();
            outputAlphabet = new TypedListBox();
            inputAlphabet = new TypedListBox();
            addState = new Button();
            label3 = new Label();
            label2 = new Label();
            typeBox = new ComboBox();
            label1 = new Label();
            groupBox2 = new GroupBox();
            settingsFLP = new FlowLayoutPanel();
            groupBox8 = new GroupBox();
            label15 = new Label();
            drawStepDelayNUD = new NumericUpDown();
            containerCP = new PictureBox();
            developerMode = new CheckBox();
            label14 = new Label();
            groupBox4 = new GroupBox();
            innerStateCP = new PictureBox();
            highlightedBorderCP = new PictureBox();
            label8 = new Label();
            label9 = new Label();
            inactiveBorderCP = new PictureBox();
            activeBorderCP = new PictureBox();
            label6 = new Label();
            label7 = new Label();
            label5 = new Label();
            label4 = new Label();
            borderNUD = new NumericUpDown();
            cirlceDiameterNUD = new NumericUpDown();
            groupBox5 = new GroupBox();
            prohibitIntersectingPaths = new CheckBox();
            groupBox7 = new GroupBox();
            label12 = new Label();
            label13 = new Label();
            transitionBlackPenCP = new PictureBox();
            transitionBlackPenNUD = new NumericUpDown();
            groupBox6 = new GroupBox();
            label10 = new Label();
            label11 = new Label();
            transitionLightPenCP = new PictureBox();
            transitionLightPenNUD = new NumericUpDown();
            containerMenuSplitter = new SplitContainer();
            containerPanel = new Panel();
            container = new PictureBox();
            zoomPanel = new Panel();
            groupBox10 = new GroupBox();
            clearZoom = new Button();
            zoomTrackBar = new Syncfusion.Windows.Forms.Tools.TrackBarEx(10, 1000);
            groupBox9 = new GroupBox();
            labelScreenCoords = new Label();
            labelRealCoords = new Label();
            MainTerminal = new TheoryOfAutomatons.Utils.UI.Controls.Terminal.Terminal();
            mainGB = new GroupBox();
            colorPicker = new ColorDialog();
            mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainHorizontalSplitContainer).BeginInit();
            mainHorizontalSplitContainer.Panel1.SuspendLayout();
            mainHorizontalSplitContainer.Panel2.SuspendLayout();
            mainHorizontalSplitContainer.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            groupBoxesPanel.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            settingsFLP.SuspendLayout();
            groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)drawStepDelayNUD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)containerCP).BeginInit();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)innerStateCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)highlightedBorderCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inactiveBorderCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)activeBorderCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)borderNUD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cirlceDiameterNUD).BeginInit();
            groupBox5.SuspendLayout();
            groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)transitionBlackPenCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)transitionBlackPenNUD).BeginInit();
            groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)transitionLightPenCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)transitionLightPenNUD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)containerMenuSplitter).BeginInit();
            containerMenuSplitter.Panel1.SuspendLayout();
            containerMenuSplitter.Panel2.SuspendLayout();
            containerMenuSplitter.SuspendLayout();
            containerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)container).BeginInit();
            zoomPanel.SuspendLayout();
            groupBox10.SuspendLayout();
            groupBox9.SuspendLayout();
            mainGB.SuspendLayout();
            SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            BottomToolStripPanel.Dock = DockStyle.Bottom;
            BottomToolStripPanel.Location = new Point(0, 175);
            BottomToolStripPanel.Name = "BottomToolStripPanel";
            BottomToolStripPanel.Orientation = Orientation.Horizontal;
            BottomToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
            BottomToolStripPanel.Size = new Size(150, 0);
            // 
            // TopToolStripPanel
            // 
            TopToolStripPanel.Dock = DockStyle.Top;
            TopToolStripPanel.Location = new Point(0, 0);
            TopToolStripPanel.Name = "TopToolStripPanel";
            TopToolStripPanel.Orientation = Orientation.Horizontal;
            TopToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
            TopToolStripPanel.Size = new Size(150, 25);
            // 
            // RightToolStripPanel
            // 
            RightToolStripPanel.Dock = DockStyle.Right;
            RightToolStripPanel.Location = new Point(150, 25);
            RightToolStripPanel.Name = "RightToolStripPanel";
            RightToolStripPanel.Orientation = Orientation.Vertical;
            RightToolStripPanel.RowMargin = new Padding(0, 3, 0, 0);
            RightToolStripPanel.Size = new Size(0, 150);
            // 
            // LeftToolStripPanel
            // 
            LeftToolStripPanel.Dock = DockStyle.Left;
            LeftToolStripPanel.Location = new Point(0, 25);
            LeftToolStripPanel.Name = "LeftToolStripPanel";
            LeftToolStripPanel.Orientation = Orientation.Vertical;
            LeftToolStripPanel.RowMargin = new Padding(0, 3, 0, 0);
            LeftToolStripPanel.Size = new Size(0, 150);
            // 
            // ContentPanel
            // 
            ContentPanel.Size = new Size(150, 150);
            // 
            // mainToolStrip
            // 
            mainToolStrip.BackColor = Color.FromArgb(31, 31, 31);
            mainToolStrip.ImageScalingSize = new Size(20, 20);
            mainToolStrip.Items.AddRange(new ToolStripItem[] { loadFromFileTSB, saveToFileTSB, clearTSB });
            mainToolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            mainToolStrip.Location = new Point(0, 0);
            mainToolStrip.Name = "mainToolStrip";
            mainToolStrip.RenderMode = ToolStripRenderMode.Professional;
            mainToolStrip.Size = new Size(864, 25);
            mainToolStrip.TabIndex = 1;
            mainToolStrip.Text = "Меню";
            // 
            // loadFromFileTSB
            // 
            loadFromFileTSB.DisplayStyle = ToolStripItemDisplayStyle.Image;
            loadFromFileTSB.Image = TOA.Properties.Resources.LoadButton;
            loadFromFileTSB.ImageScaling = ToolStripItemImageScaling.None;
            loadFromFileTSB.ImageTransparentColor = Color.Magenta;
            loadFromFileTSB.Name = "loadFromFileTSB";
            loadFromFileTSB.Size = new Size(23, 22);
            loadFromFileTSB.Text = "Загрузить автомат из файла...";
            // 
            // saveToFileTSB
            // 
            saveToFileTSB.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveToFileTSB.Image = TOA.Properties.Resources.SaveButton;
            saveToFileTSB.ImageScaling = ToolStripItemImageScaling.None;
            saveToFileTSB.ImageTransparentColor = Color.Magenta;
            saveToFileTSB.Name = "saveToFileTSB";
            saveToFileTSB.Size = new Size(23, 22);
            saveToFileTSB.Text = "Сохранить автомат в файл...";
            // 
            // clearTSB
            // 
            clearTSB.DisplayStyle = ToolStripItemDisplayStyle.Image;
            clearTSB.Image = TOA.Properties.Resources.ResetButton;
            clearTSB.ImageScaling = ToolStripItemImageScaling.None;
            clearTSB.ImageTransparentColor = Color.Magenta;
            clearTSB.Name = "clearTSB";
            clearTSB.Size = new Size(23, 22);
            clearTSB.Text = "toolStripButton1";
            // 
            // mainHorizontalSplitContainer
            // 
            mainHorizontalSplitContainer.Dock = DockStyle.Fill;
            mainHorizontalSplitContainer.FixedPanel = FixedPanel.Panel2;
            mainHorizontalSplitContainer.IsSplitterFixed = true;
            mainHorizontalSplitContainer.Location = new Point(3, 19);
            mainHorizontalSplitContainer.Name = "mainHorizontalSplitContainer";
            mainHorizontalSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // mainHorizontalSplitContainer.Panel1
            // 
            mainHorizontalSplitContainer.Panel1.Controls.Add(tableLayoutPanel);
            // 
            // mainHorizontalSplitContainer.Panel2
            // 
            mainHorizontalSplitContainer.Panel2.BackColor = Color.FromArgb(26, 26, 26);
            mainHorizontalSplitContainer.Panel2.Controls.Add(MainTerminal);
            mainHorizontalSplitContainer.Size = new Size(834, 877);
            mainHorizontalSplitContainer.SplitterDistance = 643;
            mainHorizontalSplitContainer.TabIndex = 0;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel.BackColor = Color.FromArgb(41, 41, 41);
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 273F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(groupBoxesPanel, 0, 0);
            tableLayoutPanel.Controls.Add(containerMenuSplitter, 1, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 672F));
            tableLayoutPanel.Size = new Size(834, 643);
            tableLayoutPanel.TabIndex = 3;
            // 
            // groupBoxesPanel
            // 
            groupBoxesPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxesPanel.BackColor = Color.FromArgb(32, 32, 32);
            groupBoxesPanel.ColumnCount = 1;
            groupBoxesPanel.ColumnStyles.Add(new ColumnStyle());
            groupBoxesPanel.Controls.Add(groupBox3, 0, 3);
            groupBoxesPanel.Controls.Add(groupBox1, 0, 1);
            groupBoxesPanel.Controls.Add(groupBox2, 0, 2);
            groupBoxesPanel.Dock = DockStyle.Fill;
            groupBoxesPanel.Location = new Point(3, 3);
            groupBoxesPanel.Name = "groupBoxesPanel";
            groupBoxesPanel.RowCount = 4;
            groupBoxesPanel.RowStyles.Add(new RowStyle());
            groupBoxesPanel.RowStyles.Add(new RowStyle());
            groupBoxesPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 308F));
            groupBoxesPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            groupBoxesPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            groupBoxesPanel.Size = new Size(267, 637);
            groupBoxesPanel.TabIndex = 1;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(generateRandomSequence);
            groupBox3.Controls.Add(sequenceTextBox);
            groupBox3.Controls.Add(analyze);
            groupBox3.Dock = DockStyle.Bottom;
            groupBox3.FlatStyle = FlatStyle.Popup;
            groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBox3.ForeColor = Color.Aquamarine;
            groupBox3.Location = new Point(3, 579);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(264, 55);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Анализ входной строки";
            // 
            // generateRandomSequence
            // 
            generateRandomSequence.BackColor = Color.FromArgb(48, 48, 48);
            generateRandomSequence.BackgroundImageLayout = ImageLayout.Center;
            generateRandomSequence.Enabled = false;
            generateRandomSequence.FlatStyle = FlatStyle.Flat;
            generateRandomSequence.ForeColor = Color.Gainsboro;
            generateRandomSequence.Image = TOA.Properties.Resources.RandomSequence;
            generateRandomSequence.Location = new Point(212, 20);
            generateRandomSequence.Name = "generateRandomSequence";
            generateRandomSequence.Size = new Size(20, 20);
            generateRandomSequence.TabIndex = 4;
            generateRandomSequence.UseVisualStyleBackColor = false;
            // 
            // sequenceTextBox
            // 
            sequenceTextBox.BackColor = Color.FromArgb(48, 48, 48);
            sequenceTextBox.Enabled = false;
            sequenceTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            sequenceTextBox.ForeColor = Color.Gainsboro;
            sequenceTextBox.Location = new Point(7, 20);
            sequenceTextBox.Name = "sequenceTextBox";
            sequenceTextBox.Size = new Size(199, 20);
            sequenceTextBox.TabIndex = 3;
            // 
            // analyze
            // 
            analyze.BackColor = Color.FromArgb(48, 48, 48);
            analyze.Enabled = false;
            analyze.FlatStyle = FlatStyle.Flat;
            analyze.Font = new System.Drawing.Font("Microsoft Tai Le", 6.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            analyze.ForeColor = Color.Gainsboro;
            analyze.Image = TOA.Properties.Resources.AnalyzeButton;
            analyze.Location = new Point(238, 20);
            analyze.Name = "analyze";
            analyze.Size = new Size(20, 20);
            analyze.TabIndex = 2;
            analyze.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(outputAlphabet);
            groupBox1.Controls.Add(inputAlphabet);
            groupBox1.Controls.Add(addState);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(typeBox);
            groupBox1.Controls.Add(label1);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.FlatStyle = FlatStyle.Popup;
            groupBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBox1.ForeColor = Color.Aquamarine;
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(264, 262);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Параметры автомата";
            // 
            // outputAlphabet
            // 
            outputAlphabet.Font = new System.Drawing.Font("Microsoft YaHei UI", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            outputAlphabet.ForeColor = Color.Gainsboro;
            outputAlphabet.Location = new Point(7, 153);
            outputAlphabet.Name = "outputAlphabet";
            outputAlphabet.Size = new Size(252, 74);
            outputAlphabet.TabIndex = 11;
            // 
            // inputAlphabet
            // 
            inputAlphabet.Font = new System.Drawing.Font("Microsoft YaHei UI", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            inputAlphabet.ForeColor = Color.Gainsboro;
            inputAlphabet.Location = new Point(6, 60);
            inputAlphabet.Name = "inputAlphabet";
            inputAlphabet.Size = new Size(252, 74);
            inputAlphabet.TabIndex = 10;
            // 
            // addState
            // 
            addState.BackColor = Color.FromArgb(48, 48, 48);
            addState.Enabled = false;
            addState.FlatStyle = FlatStyle.Flat;
            addState.Font = new System.Drawing.Font("Microsoft Tai Le", 6.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            addState.ForeColor = Color.Gainsboro;
            addState.Location = new Point(6, 233);
            addState.Name = "addState";
            addState.Size = new Size(252, 23);
            addState.TabIndex = 8;
            addState.Text = "Добавить состояние";
            addState.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label3.ForeColor = Color.Gainsboro;
            label3.Location = new Point(3, 137);
            label3.Name = "label3";
            label3.Size = new Size(106, 13);
            label3.TabIndex = 5;
            label3.Text = "Выходной алфавит:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label2.ForeColor = Color.Gainsboro;
            label2.Location = new Point(3, 44);
            label2.Name = "label2";
            label2.Size = new Size(98, 13);
            label2.TabIndex = 3;
            label2.Text = "Входной алфавит:";
            // 
            // typeBox
            // 
            typeBox.BackColor = Color.FromArgb(48, 48, 48);
            typeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            typeBox.FlatStyle = FlatStyle.Popup;
            typeBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            typeBox.ForeColor = Color.Gainsboro;
            typeBox.FormattingEnabled = true;
            typeBox.Items.AddRange(new object[] { "Мили", "Мура" });
            typeBox.Location = new Point(44, 20);
            typeBox.Name = "typeBox";
            typeBox.RightToLeft = RightToLeft.No;
            typeBox.Size = new Size(214, 21);
            typeBox.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.ForeColor = Color.Gainsboro;
            label1.Location = new Point(3, 23);
            label1.Name = "label1";
            label1.Size = new Size(32, 13);
            label1.TabIndex = 1;
            label1.Text = "Тип: ";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox2.Controls.Add(settingsFLP);
            groupBox2.FlatStyle = FlatStyle.Popup;
            groupBox2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBox2.ForeColor = Color.Aquamarine;
            groupBox2.Location = new Point(3, 271);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(264, 302);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Параметры отрисовки";
            // 
            // settingsFLP
            // 
            settingsFLP.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            settingsFLP.AutoScroll = true;
            settingsFLP.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            settingsFLP.BorderStyle = BorderStyle.FixedSingle;
            settingsFLP.Controls.Add(groupBox8);
            settingsFLP.Controls.Add(groupBox4);
            settingsFLP.Controls.Add(groupBox5);
            settingsFLP.FlowDirection = FlowDirection.TopDown;
            settingsFLP.Location = new Point(3, 19);
            settingsFLP.Name = "settingsFLP";
            settingsFLP.Size = new Size(258, 280);
            settingsFLP.TabIndex = 0;
            settingsFLP.WrapContents = false;
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(label15);
            groupBox8.Controls.Add(drawStepDelayNUD);
            groupBox8.Controls.Add(containerCP);
            groupBox8.Controls.Add(developerMode);
            groupBox8.Controls.Add(label14);
            groupBox8.FlatStyle = FlatStyle.Popup;
            groupBox8.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox8.ForeColor = Color.LightGray;
            groupBox8.Location = new Point(3, 3);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(230, 92);
            groupBox8.TabIndex = 24;
            groupBox8.TabStop = false;
            groupBox8.Text = "Параметры рабочей области";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label15.ForeColor = Color.Gainsboro;
            label15.Location = new Point(3, 48);
            label15.Margin = new Padding(1, 0, 1, 0);
            label15.Name = "label15";
            label15.Size = new Size(141, 13);
            label15.TabIndex = 32;
            label15.Text = "Шаг отрисовки работы (c):";
            // 
            // drawStepDelayNUD
            // 
            drawStepDelayNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            drawStepDelayNUD.BackColor = Color.FromArgb(48, 48, 48);
            drawStepDelayNUD.DecimalPlaces = 2;
            drawStepDelayNUD.Enabled = false;
            drawStepDelayNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            drawStepDelayNUD.ForeColor = Color.Gainsboro;
            drawStepDelayNUD.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            drawStepDelayNUD.Location = new Point(147, 46);
            drawStepDelayNUD.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            drawStepDelayNUD.MaximumSize = new Size(77, 0);
            drawStepDelayNUD.Minimum = new decimal(new int[] { 10, 0, 0, 131072 });
            drawStepDelayNUD.MinimumSize = new Size(77, 0);
            drawStepDelayNUD.Name = "drawStepDelayNUD";
            drawStepDelayNUD.Size = new Size(77, 20);
            drawStepDelayNUD.TabIndex = 31;
            drawStepDelayNUD.Value = new decimal(new int[] { 75, 0, 0, 131072 });
            // 
            // containerCP
            // 
            containerCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            containerCP.BackColor = Color.FromArgb(96, 96, 96);
            containerCP.BorderStyle = BorderStyle.Fixed3D;
            containerCP.Enabled = false;
            containerCP.Location = new Point(147, 20);
            containerCP.MaximumSize = new Size(77, 20);
            containerCP.MinimumSize = new Size(77, 20);
            containerCP.Name = "containerCP";
            containerCP.Size = new Size(77, 20);
            containerCP.TabIndex = 30;
            containerCP.TabStop = false;
            // 
            // developerMode
            // 
            developerMode.AutoSize = true;
            developerMode.CheckAlign = ContentAlignment.MiddleRight;
            developerMode.Dock = DockStyle.Bottom;
            developerMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            developerMode.ForeColor = Color.Gainsboro;
            developerMode.Location = new Point(3, 72);
            developerMode.Name = "developerMode";
            developerMode.Padding = new Padding(0, 0, 1, 0);
            developerMode.Size = new Size(224, 17);
            developerMode.TabIndex = 22;
            developerMode.Text = "Режим разработчика:";
            developerMode.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label14.ForeColor = Color.Gainsboro;
            label14.Location = new Point(3, 22);
            label14.Margin = new Padding(1, 0, 1, 0);
            label14.Name = "label14";
            label14.Size = new Size(123, 13);
            label14.TabIndex = 28;
            label14.Text = "Цвет рабочей области:";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(innerStateCP);
            groupBox4.Controls.Add(highlightedBorderCP);
            groupBox4.Controls.Add(label8);
            groupBox4.Controls.Add(label9);
            groupBox4.Controls.Add(inactiveBorderCP);
            groupBox4.Controls.Add(activeBorderCP);
            groupBox4.Controls.Add(label6);
            groupBox4.Controls.Add(label7);
            groupBox4.Controls.Add(label5);
            groupBox4.Controls.Add(label4);
            groupBox4.Controls.Add(borderNUD);
            groupBox4.Controls.Add(cirlceDiameterNUD);
            groupBox4.FlatStyle = FlatStyle.Popup;
            groupBox4.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox4.ForeColor = Color.LightGray;
            groupBox4.Location = new Point(3, 101);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(230, 173);
            groupBox4.TabIndex = 21;
            groupBox4.TabStop = false;
            groupBox4.Text = "Параметры отрисовки состояний";
            // 
            // innerStateCP
            // 
            innerStateCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            innerStateCP.BackColor = Color.LightGray;
            innerStateCP.BorderStyle = BorderStyle.Fixed3D;
            innerStateCP.Enabled = false;
            innerStateCP.Location = new Point(149, 147);
            innerStateCP.MaximumSize = new Size(77, 20);
            innerStateCP.MinimumSize = new Size(77, 20);
            innerStateCP.Name = "innerStateCP";
            innerStateCP.Size = new Size(77, 20);
            innerStateCP.TabIndex = 30;
            innerStateCP.TabStop = false;
            // 
            // highlightedBorderCP
            // 
            highlightedBorderCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            highlightedBorderCP.BackColor = Color.DarkGray;
            highlightedBorderCP.BorderStyle = BorderStyle.Fixed3D;
            highlightedBorderCP.Enabled = false;
            highlightedBorderCP.Location = new Point(149, 121);
            highlightedBorderCP.MaximumSize = new Size(77, 20);
            highlightedBorderCP.MinimumSize = new Size(77, 20);
            highlightedBorderCP.Name = "highlightedBorderCP";
            highlightedBorderCP.Size = new Size(77, 20);
            highlightedBorderCP.TabIndex = 29;
            highlightedBorderCP.TabStop = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label8.ForeColor = Color.Gainsboro;
            label8.Location = new Point(3, 149);
            label8.Margin = new Padding(1, 0, 1, 0);
            label8.Name = "label8";
            label8.Size = new Size(136, 13);
            label8.TabIndex = 28;
            label8.Text = "Цвет заливки состояния:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label9.ForeColor = Color.Gainsboro;
            label9.Location = new Point(3, 123);
            label9.Margin = new Padding(1, 0, 1, 0);
            label9.Name = "label9";
            label9.Size = new Size(137, 13);
            label9.TabIndex = 27;
            label9.Text = "Цвет подсветки границы:";
            // 
            // inactiveBorderCP
            // 
            inactiveBorderCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inactiveBorderCP.BackColor = Color.Black;
            inactiveBorderCP.BorderStyle = BorderStyle.Fixed3D;
            inactiveBorderCP.Enabled = false;
            inactiveBorderCP.Location = new Point(149, 95);
            inactiveBorderCP.MaximumSize = new Size(77, 20);
            inactiveBorderCP.MinimumSize = new Size(77, 20);
            inactiveBorderCP.Name = "inactiveBorderCP";
            inactiveBorderCP.Size = new Size(77, 20);
            inactiveBorderCP.TabIndex = 26;
            inactiveBorderCP.TabStop = false;
            // 
            // activeBorderCP
            // 
            activeBorderCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            activeBorderCP.BackColor = Color.LimeGreen;
            activeBorderCP.BorderStyle = BorderStyle.Fixed3D;
            activeBorderCP.Enabled = false;
            activeBorderCP.Location = new Point(149, 69);
            activeBorderCP.MaximumSize = new Size(77, 20);
            activeBorderCP.MinimumSize = new Size(77, 20);
            activeBorderCP.Name = "activeBorderCP";
            activeBorderCP.Size = new Size(77, 20);
            activeBorderCP.TabIndex = 25;
            activeBorderCP.TabStop = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label6.ForeColor = Color.Gainsboro;
            label6.Location = new Point(3, 97);
            label6.Margin = new Padding(1, 0, 1, 0);
            label6.Name = "label6";
            label6.Size = new Size(146, 13);
            label6.TabIndex = 24;
            label6.Text = "Цвет неактивной границы: ";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label7.ForeColor = Color.Gainsboro;
            label7.Location = new Point(3, 71);
            label7.Margin = new Padding(1, 0, 1, 0);
            label7.Name = "label7";
            label7.Size = new Size(131, 13);
            label7.TabIndex = 23;
            label7.Text = "Цвет активной границы:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label5.ForeColor = Color.Gainsboro;
            label5.Location = new Point(3, 45);
            label5.Margin = new Padding(1, 0, 1, 0);
            label5.Name = "label5";
            label5.Size = new Size(95, 13);
            label5.TabIndex = 17;
            label5.Text = "Ширина границы:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label4.ForeColor = Color.Gainsboro;
            label4.Location = new Point(3, 19);
            label4.Margin = new Padding(1, 0, 1, 0);
            label4.Name = "label4";
            label4.Size = new Size(112, 13);
            label4.TabIndex = 16;
            label4.Text = "Диаметр состояния:";
            // 
            // borderNUD
            // 
            borderNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            borderNUD.BackColor = Color.FromArgb(48, 48, 48);
            borderNUD.Enabled = false;
            borderNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            borderNUD.ForeColor = Color.Gainsboro;
            borderNUD.Location = new Point(149, 43);
            borderNUD.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            borderNUD.MaximumSize = new Size(77, 0);
            borderNUD.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            borderNUD.MinimumSize = new Size(77, 0);
            borderNUD.Name = "borderNUD";
            borderNUD.Size = new Size(77, 20);
            borderNUD.TabIndex = 15;
            borderNUD.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // cirlceDiameterNUD
            // 
            cirlceDiameterNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cirlceDiameterNUD.BackColor = Color.FromArgb(48, 48, 48);
            cirlceDiameterNUD.Enabled = false;
            cirlceDiameterNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            cirlceDiameterNUD.ForeColor = Color.Gainsboro;
            cirlceDiameterNUD.Location = new Point(149, 17);
            cirlceDiameterNUD.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            cirlceDiameterNUD.MaximumSize = new Size(77, 0);
            cirlceDiameterNUD.Minimum = new decimal(new int[] { 30, 0, 0, 0 });
            cirlceDiameterNUD.MinimumSize = new Size(77, 0);
            cirlceDiameterNUD.Name = "cirlceDiameterNUD";
            cirlceDiameterNUD.Size = new Size(77, 20);
            cirlceDiameterNUD.TabIndex = 14;
            cirlceDiameterNUD.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(prohibitIntersectingPaths);
            groupBox5.Controls.Add(groupBox7);
            groupBox5.Controls.Add(groupBox6);
            groupBox5.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox5.ForeColor = Color.LightGray;
            groupBox5.Location = new Point(3, 280);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(230, 192);
            groupBox5.TabIndex = 23;
            groupBox5.TabStop = false;
            groupBox5.Text = "Отрисовка переходов";
            // 
            // prohibitIntersectingPaths
            // 
            prohibitIntersectingPaths.AutoSize = true;
            prohibitIntersectingPaths.CheckAlign = ContentAlignment.MiddleRight;
            prohibitIntersectingPaths.Checked = true;
            prohibitIntersectingPaths.CheckState = CheckState.Checked;
            prohibitIntersectingPaths.Dock = DockStyle.Bottom;
            prohibitIntersectingPaths.Enabled = false;
            prohibitIntersectingPaths.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            prohibitIntersectingPaths.ForeColor = Color.Gainsboro;
            prohibitIntersectingPaths.Location = new Point(3, 172);
            prohibitIntersectingPaths.Name = "prohibitIntersectingPaths";
            prohibitIntersectingPaths.Padding = new Padding(0, 0, 1, 0);
            prohibitIntersectingPaths.Size = new Size(224, 17);
            prohibitIntersectingPaths.TabIndex = 26;
            prohibitIntersectingPaths.Text = "Запрет самопересечений переходов: ";
            prohibitIntersectingPaths.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(label12);
            groupBox7.Controls.Add(label13);
            groupBox7.Controls.Add(transitionBlackPenCP);
            groupBox7.Controls.Add(transitionBlackPenNUD);
            groupBox7.Font = new System.Drawing.Font("Times New Roman", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox7.ForeColor = Color.LightGray;
            groupBox7.Location = new Point(6, 93);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(218, 70);
            groupBox7.TabIndex = 25;
            groupBox7.TabStop = false;
            groupBox7.Text = "Неактивные переходы";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label12.ForeColor = Color.Gainsboro;
            label12.Location = new Point(3, 45);
            label12.Margin = new Padding(1, 0, 1, 0);
            label12.Name = "label12";
            label12.Size = new Size(68, 13);
            label12.TabIndex = 34;
            label12.Text = "Цвет линии:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label13.ForeColor = Color.Gainsboro;
            label13.Location = new Point(3, 19);
            label13.Margin = new Padding(1, 0, 1, 0);
            label13.Name = "label13";
            label13.Size = new Size(89, 13);
            label13.TabIndex = 33;
            label13.Text = "Толщина линии:";
            // 
            // transitionBlackPenCP
            // 
            transitionBlackPenCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            transitionBlackPenCP.BackColor = Color.Black;
            transitionBlackPenCP.BorderStyle = BorderStyle.Fixed3D;
            transitionBlackPenCP.Enabled = false;
            transitionBlackPenCP.Location = new Point(135, 43);
            transitionBlackPenCP.MaximumSize = new Size(77, 20);
            transitionBlackPenCP.MinimumSize = new Size(77, 20);
            transitionBlackPenCP.Name = "transitionBlackPenCP";
            transitionBlackPenCP.Size = new Size(77, 20);
            transitionBlackPenCP.TabIndex = 32;
            transitionBlackPenCP.TabStop = false;
            // 
            // transitionBlackPenNUD
            // 
            transitionBlackPenNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            transitionBlackPenNUD.BackColor = Color.FromArgb(48, 48, 48);
            transitionBlackPenNUD.DecimalPlaces = 1;
            transitionBlackPenNUD.Enabled = false;
            transitionBlackPenNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            transitionBlackPenNUD.ForeColor = Color.Gainsboro;
            transitionBlackPenNUD.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            transitionBlackPenNUD.Location = new Point(135, 17);
            transitionBlackPenNUD.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            transitionBlackPenNUD.MaximumSize = new Size(77, 0);
            transitionBlackPenNUD.Minimum = new decimal(new int[] { 10, 0, 0, 65536 });
            transitionBlackPenNUD.MinimumSize = new Size(77, 0);
            transitionBlackPenNUD.Name = "transitionBlackPenNUD";
            transitionBlackPenNUD.Size = new Size(77, 20);
            transitionBlackPenNUD.TabIndex = 31;
            transitionBlackPenNUD.Value = new decimal(new int[] { 30, 0, 0, 65536 });
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(label10);
            groupBox6.Controls.Add(label11);
            groupBox6.Controls.Add(transitionLightPenCP);
            groupBox6.Controls.Add(transitionLightPenNUD);
            groupBox6.Font = new System.Drawing.Font("Times New Roman", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox6.ForeColor = Color.LightGray;
            groupBox6.Location = new Point(6, 17);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(218, 70);
            groupBox6.TabIndex = 24;
            groupBox6.TabStop = false;
            groupBox6.Text = "Активные переходы";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label10.ForeColor = Color.Gainsboro;
            label10.Location = new Point(3, 45);
            label10.Margin = new Padding(1, 0, 1, 0);
            label10.Name = "label10";
            label10.Size = new Size(68, 13);
            label10.TabIndex = 30;
            label10.Text = "Цвет линии:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label11.ForeColor = Color.Gainsboro;
            label11.Location = new Point(3, 19);
            label11.Margin = new Padding(1, 0, 1, 0);
            label11.Name = "label11";
            label11.Size = new Size(89, 13);
            label11.TabIndex = 29;
            label11.Text = "Толщина линии:";
            // 
            // transitionLightPenCP
            // 
            transitionLightPenCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            transitionLightPenCP.BackColor = Color.LimeGreen;
            transitionLightPenCP.BorderStyle = BorderStyle.Fixed3D;
            transitionLightPenCP.Enabled = false;
            transitionLightPenCP.Location = new Point(135, 43);
            transitionLightPenCP.MaximumSize = new Size(77, 20);
            transitionLightPenCP.MinimumSize = new Size(77, 20);
            transitionLightPenCP.Name = "transitionLightPenCP";
            transitionLightPenCP.Size = new Size(77, 20);
            transitionLightPenCP.TabIndex = 27;
            transitionLightPenCP.TabStop = false;
            // 
            // transitionLightPenNUD
            // 
            transitionLightPenNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            transitionLightPenNUD.BackColor = Color.FromArgb(48, 48, 48);
            transitionLightPenNUD.DecimalPlaces = 1;
            transitionLightPenNUD.Enabled = false;
            transitionLightPenNUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            transitionLightPenNUD.ForeColor = Color.Gainsboro;
            transitionLightPenNUD.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            transitionLightPenNUD.Location = new Point(135, 17);
            transitionLightPenNUD.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            transitionLightPenNUD.MaximumSize = new Size(77, 0);
            transitionLightPenNUD.Minimum = new decimal(new int[] { 10, 0, 0, 65536 });
            transitionLightPenNUD.MinimumSize = new Size(77, 0);
            transitionLightPenNUD.Name = "transitionLightPenNUD";
            transitionLightPenNUD.Size = new Size(77, 20);
            transitionLightPenNUD.TabIndex = 26;
            transitionLightPenNUD.Value = new decimal(new int[] { 30, 0, 0, 65536 });
            // 
            // containerMenuSplitter
            // 
            containerMenuSplitter.Dock = DockStyle.Fill;
            containerMenuSplitter.FixedPanel = FixedPanel.Panel2;
            containerMenuSplitter.IsSplitterFixed = true;
            containerMenuSplitter.Location = new Point(276, 3);
            containerMenuSplitter.Name = "containerMenuSplitter";
            containerMenuSplitter.Orientation = Orientation.Horizontal;
            // 
            // containerMenuSplitter.Panel1
            // 
            containerMenuSplitter.Panel1.Controls.Add(containerPanel);
            // 
            // containerMenuSplitter.Panel2
            // 
            containerMenuSplitter.Panel2.Controls.Add(zoomPanel);
            containerMenuSplitter.Size = new Size(555, 637);
            containerMenuSplitter.SplitterDistance = 571;
            containerMenuSplitter.TabIndex = 2;
            // 
            // containerPanel
            // 
            containerPanel.AutoScroll = true;
            containerPanel.AutoSize = true;
            containerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            containerPanel.BorderStyle = BorderStyle.Fixed3D;
            containerPanel.Controls.Add(container);
            containerPanel.Dock = DockStyle.Fill;
            containerPanel.Location = new Point(0, 0);
            containerPanel.Margin = new Padding(7);
            containerPanel.Name = "containerPanel";
            containerPanel.Size = new Size(555, 571);
            containerPanel.TabIndex = 3;
            // 
            // container
            // 
            container.BackColor = Color.FromArgb(96, 96, 96);
            container.BorderStyle = BorderStyle.Fixed3D;
            container.Location = new Point(5, 7);
            container.Margin = new Padding(7);
            container.Name = "container";
            container.Padding = new Padding(7);
            container.Size = new Size(541, 555);
            container.SizeMode = PictureBoxSizeMode.AutoSize;
            container.TabIndex = 3;
            container.TabStop = false;
            container.Paint += Container_Paint;
            container.MouseWheel += Container_MouseWheel;
            // 
            // zoomPanel
            // 
            zoomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            zoomPanel.BorderStyle = BorderStyle.Fixed3D;
            zoomPanel.Controls.Add(groupBox10);
            zoomPanel.Controls.Add(groupBox9);
            zoomPanel.Dock = DockStyle.Fill;
            zoomPanel.Location = new Point(0, 0);
            zoomPanel.Name = "zoomPanel";
            zoomPanel.Size = new Size(555, 62);
            zoomPanel.TabIndex = 0;
            // 
            // groupBox10
            // 
            groupBox10.Controls.Add(clearZoom);
            groupBox10.Controls.Add(zoomTrackBar);
            groupBox10.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, FontStyle.Bold);
            groupBox10.ForeColor = Color.LightGray;
            groupBox10.Location = new Point(3, 1);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new Size(310, 56);
            groupBox10.TabIndex = 4;
            groupBox10.TabStop = false;
            groupBox10.Text = "Масштаб";
            // 
            // clearZoom
            // 
            clearZoom.BackColor = Color.FromArgb(48, 48, 48);
            clearZoom.BackgroundImageLayout = ImageLayout.Center;
            clearZoom.FlatStyle = FlatStyle.Flat;
            clearZoom.Font = new System.Drawing.Font("Arial Narrow", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            clearZoom.ForeColor = Color.Gainsboro;
            clearZoom.Location = new Point(212, 17);
            clearZoom.Name = "clearZoom";
            clearZoom.Size = new Size(92, 29);
            clearZoom.TabIndex = 5;
            clearZoom.Text = "Сброс масштаба";
            clearZoom.UseVisualStyleBackColor = false;
            // 
            // zoomTrackBar
            // 
            zoomTrackBar.BackColor = Color.FromArgb(64, 64, 64);
            zoomTrackBar.BeforeTouchSize = new Size(200, 20);
            zoomTrackBar.ButtonColor = Color.FromArgb(64, 64, 64);
            zoomTrackBar.ButtonSignColor = Color.Red;
            zoomTrackBar.CanOverrideStyle = true;
            zoomTrackBar.ForeColor = Color.Black;
            zoomTrackBar.LargeChange = 100;
            zoomTrackBar.Location = new Point(6, 22);
            zoomTrackBar.Name = "zoomTrackBar";
            zoomTrackBar.Size = new Size(200, 20);
            zoomTrackBar.SliderSize = new Size(7, 14);
            zoomTrackBar.SmallChange = 10;
            zoomTrackBar.Style = Syncfusion.Windows.Forms.Tools.TrackBarEx.Theme.Metro;
            zoomTrackBar.TabIndex = 1;
            zoomTrackBar.ThemeName = "Metro";
            zoomTrackBar.TimerInterval = 100;
            zoomTrackBar.TrackBarGradientEnd = Color.FromArgb(32, 32, 32);
            zoomTrackBar.TrackBarGradientStart = Color.FromArgb(32, 32, 32);
            zoomTrackBar.Value = 100;
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(labelScreenCoords);
            groupBox9.Controls.Add(labelRealCoords);
            groupBox9.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, FontStyle.Bold);
            groupBox9.ForeColor = Color.LightGray;
            groupBox9.Location = new Point(319, 1);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(150, 56);
            groupBox9.TabIndex = 3;
            groupBox9.TabStop = false;
            groupBox9.Text = "Координаты";
            // 
            // labelScreenCoords
            // 
            labelScreenCoords.AutoSize = true;
            labelScreenCoords.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelScreenCoords.ForeColor = Color.Gainsboro;
            labelScreenCoords.Location = new Point(4, 38);
            labelScreenCoords.Margin = new Padding(1, 0, 1, 0);
            labelScreenCoords.Name = "labelScreenCoords";
            labelScreenCoords.Size = new Size(90, 13);
            labelScreenCoords.TabIndex = 25;
            labelScreenCoords.Text = "Экранные: (X; Y)";
            // 
            // labelRealCoords
            // 
            labelRealCoords.AutoSize = true;
            labelRealCoords.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelRealCoords.ForeColor = Color.Gainsboro;
            labelRealCoords.Location = new Point(4, 17);
            labelRealCoords.Margin = new Padding(1, 0, 1, 0);
            labelRealCoords.Name = "labelRealCoords";
            labelRealCoords.Size = new Size(90, 13);
            labelRealCoords.TabIndex = 24;
            labelRealCoords.Text = "Реальные: (X; Y)";
            // 
            // MainTerminal
            // 
            MainTerminal.Dock = DockStyle.Fill;
            MainTerminal.Location = new Point(0, 0);
            MainTerminal.Margin = new Padding(13);
            MainTerminal.Name = "MainTerminal";
            MainTerminal.Size = new Size(834, 230);
            MainTerminal.TabIndex = 0;
            // 
            // mainGB
            // 
            mainGB.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mainGB.Controls.Add(mainHorizontalSplitContainer);
            mainGB.Location = new Point(12, 30);
            mainGB.Name = "mainGB";
            mainGB.Size = new Size(840, 899);
            mainGB.TabIndex = 2;
            mainGB.TabStop = false;
            // 
            // colorPicker
            // 
            colorPicker.AnyColor = true;
            colorPicker.Color = Color.Lime;
            colorPicker.FullOpen = true;
            colorPicker.ShowHelp = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(864, 941);
            Controls.Add(mainGB);
            Controls.Add(mainToolStrip);
            MinimumSize = new Size(880, 980);
            Name = "MainForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Теория Автоматов";
            Load += Form_Load;
            mainToolStrip.ResumeLayout(false);
            mainToolStrip.PerformLayout();
            mainHorizontalSplitContainer.Panel1.ResumeLayout(false);
            mainHorizontalSplitContainer.Panel1.PerformLayout();
            mainHorizontalSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainHorizontalSplitContainer).EndInit();
            mainHorizontalSplitContainer.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            groupBoxesPanel.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            settingsFLP.ResumeLayout(false);
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)drawStepDelayNUD).EndInit();
            ((System.ComponentModel.ISupportInitialize)containerCP).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)innerStateCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)highlightedBorderCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)inactiveBorderCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)activeBorderCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)borderNUD).EndInit();
            ((System.ComponentModel.ISupportInitialize)cirlceDiameterNUD).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)transitionBlackPenCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)transitionBlackPenNUD).EndInit();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)transitionLightPenCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)transitionLightPenNUD).EndInit();
            containerMenuSplitter.Panel1.ResumeLayout(false);
            containerMenuSplitter.Panel1.PerformLayout();
            containerMenuSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)containerMenuSplitter).EndInit();
            containerMenuSplitter.ResumeLayout(false);
            containerPanel.ResumeLayout(false);
            containerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)container).EndInit();
            zoomPanel.ResumeLayout(false);
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            groupBox9.ResumeLayout(false);
            groupBox9.PerformLayout();
            mainGB.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private Utils.UI.Controls.Terminal.Terminal MainTerminal;
        private ColorDialog colorPicker;
        private GroupBox groupBox8;
        private PictureBox containerCP;
        private Label label14;
        private Label label15;
        private NumericUpDown drawStepDelayNUD;
        private SplitContainer containerMenuSplitter;
        private Panel containerPanel;
        public PictureBox container;
        private Panel zoomPanel;
        private GroupBox groupBox10;
        private Button clearZoom;
        private Syncfusion.Windows.Forms.Tools.TrackBarEx zoomTrackBar;
        private GroupBox groupBox9;
        private Label labelScreenCoords;
        private Label labelRealCoords;
    }
}