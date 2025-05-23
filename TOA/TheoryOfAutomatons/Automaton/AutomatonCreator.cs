﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheoryOfAutomatons.Utils;
using TheoryOfAutomatons.Automaton.MealyAutomaton;
using TheoryOfAutomatons.Automaton.MooreAutomaton;
using Newtonsoft.Json;
using System.IO;
using TheoryOfAutomatons.Utils.Containers;
using TheoryOfAutomatons.Utils.UI.Controls;
using TheoryOfAutomatons.Utils.UI.Forms.Adders;
using Syncfusion.DocIO.DLS;
using TheoryOfAutomatons.Utils.Helpers;
using TheoryOfAutomatons.Automaton.Common;
using TheoryOfAutomatons.Utils.UI.Controls.Terminal;
using TOA.TheoryOfAutomatons.Automaton.Common;
using TOA.TheoryOfAutomatons.Automaton;

namespace TheoryOfAutomatons.Automaton
{
    internal class AutomatonCreator
    {
        #region Поля

        private IDFAutomaton DFAutomaton;     

        private ToolStripButton SaveToFileTSB;
        private ToolStripButton LoadFromFileTSB;
        private ToolStripButton ClearSettingsTSB;

        private ComboBox DFATypeSelectorCB;

        private TypedListBox InputAlphabetTLB;
        private TypedListBox OutputAlphabetTLB;

        private Button AddStateB;
        private TextBox InputSequenceTB;
        private Button GetRandomSequenceB;
        private Button AnalyzeSequenceB;

        private NumericUpDown CircleDiameterNUD;
        private NumericUpDown BorderNUD;
        private NumericUpDown DrawStepDelayNUD;
        private NumericUpDown TransitionsBPNUD;
        private NumericUpDown TransitionsLPNUD;

        private PictureBox ContainerCP;
        private PictureBox ActiveBorderCP;
        private PictureBox InactiveBorderCP;
        private PictureBox HighlightedBorderCP;
        private PictureBox InnerStateCP;
        private PictureBox TransitionsBPCP;
        private PictureBox TransitionsLPCP;

        private CheckBox PreventPathsInersectionsChB;
        private CheckBox DeveloperModeChB;

        public Terminal Terminal;
        public PictureBox Container;
        public MainForm Form { get; private set; }
        private ColorDialog ColorPicker;

        private List<Point> ExistingInitialPoints;
        private static Random random = new Random();

        #endregion

        public AutomatonCreator
        (
            ToolStripButton stfb, ToolStripButton lffb, ToolStripButton clb,
            ComboBox cb,
            TypedListBox ia, TypedListBox oa,
            Button asb,
            TextBox istb, Button grsb, Button anb,
            NumericUpDown circleDiameter, NumericUpDown border, NumericUpDown dSDNUD, NumericUpDown tLPNUD, NumericUpDown tBPNUD,
            PictureBox cCP, PictureBox aBCP, PictureBox iBCP, PictureBox hBCP, PictureBox iSCP, PictureBox tLPCP, PictureBox tBPCP,
            CheckBox intersections, CheckBox devMode,
            PictureBox container, MainForm form, ColorDialog cp, Terminal terminal
        )
        {
            #region Установка полей

            SaveToFileTSB = stfb;
            LoadFromFileTSB = lffb;
            ClearSettingsTSB = clb;

            DFATypeSelectorCB = cb;
            InputAlphabetTLB = ia;
            OutputAlphabetTLB = oa;

            AddStateB = asb;
            InputSequenceTB = istb;
            GetRandomSequenceB = grsb;
            AnalyzeSequenceB = anb;

            CircleDiameterNUD = circleDiameter;
            BorderNUD = border;
            DrawStepDelayNUD = dSDNUD;
            TransitionsLPNUD = tLPNUD;
            TransitionsBPNUD = tBPNUD;

            ContainerCP = cCP;
            ActiveBorderCP = aBCP;
            InactiveBorderCP = iBCP;
            HighlightedBorderCP = hBCP;
            InnerStateCP = iSCP;
            TransitionsLPCP = tLPCP;
            TransitionsBPCP = tBPCP;

            PreventPathsInersectionsChB = intersections;
            DeveloperModeChB = devMode;

            Container = container;
            Form = form;
            ColorPicker = cp;
            Terminal = terminal;

            #endregion



            #region Установка обработчиков событий

            SaveToFileTSB.Click += SaveToFileB_Click;
            LoadFromFileTSB.Click += LoadFromFileB_Click;
            ClearSettingsTSB.Click += ClearSettingsB_Click;

            DFATypeSelectorCB.SelectedIndexChanged += DFATypeSelectorCB_SelectedIndexChanged;
            InputAlphabetTLB.ItemAdded += InputAlphabetTLB_ItemAdded;
            InputAlphabetTLB.ItemRemoved += InputAlphabetTLB_ItemRemoved;
            OutputAlphabetTLB.ItemAdded += OutputAlphabetTLB_ItemAdded;          
            OutputAlphabetTLB.ItemRemoved += OutputAlphabetTLB_ItemRemoved;

            AddStateB.Click += AddStateB_Click;
            InputSequenceTB.TextChanged += InputSequenceTB_TextChanged;
            GetRandomSequenceB.Click += GetRandomSequenceB_Click;
            AnalyzeSequenceB.Click += AnalyzeSequenceB_Click;
           
            CircleDiameterNUD.ValueChanged += CircleDiameterNUD_ValueChanged;
            BorderNUD.ValueChanged += BorderNUD_ValueChanged;
            DrawStepDelayNUD.ValueChanged += DrawStepDelayNUD_ValueChanged;
            TransitionsLPNUD.ValueChanged += TransitionsLPNUD_ValueChanged;
            TransitionsBPNUD.ValueChanged += TransitionsBPNUD_ValueChanged;

            ContainerCP.Click += ContainerCP_Click;
            ActiveBorderCP.Click += ActiveBorderCP_Click;
            InactiveBorderCP.Click += InactiveBorderCP_Click;
            HighlightedBorderCP.Click += HighlightedBorderCP_Click;
            InnerStateCP.Click += InnerStateCP_Click;
            TransitionsLPCP.Click += TransitionsLPCP_Click;
            TransitionsBPCP.Click += TransitionsBPCP_Click;
    
            PreventPathsInersectionsChB.CheckedChanged += PreventPathsInersections_CheckedChanged;
            DeveloperModeChB.CheckedChanged += DeveloperModeChB_CheckedChanged;

            #endregion

            ExistingInitialPoints = new List<Point>();
            Check();
        }



        #region Обработчики событий UI-элементов

        private void SaveToFileB_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON files (*.json)|*.json";
                saveFileDialog.Title = "Сохранение Автомата в JSON-файл";

                if (DFAutomaton != null)
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFileName = saveFileDialog.FileName;
                        DFAutomaton.Save(selectedFileName);
                    }
                }
            }
        }

        private void LoadFromFileB_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON files (*.json)|*.json";
                openFileDialog.Title = "Выберите JSON-файл, содержащий описание Автомата";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var file = openFileDialog.FileName;
                    var json = File.ReadAllText(file);
                    var automatonData = JsonConvert.DeserializeObject<dynamic>(json);
                    int type = (int)automatonData.TypeIndex;

                    DFAutomaton?.Dispose();
                    DFAutomaton = null;

                    DFAutomaton = AutomatonFactory.CreateAutomaton
                    (
                        AutomatonFactory.FromInt(type),
                        file,
                        Container,
                        this
                    );

                    Check();
                }
            }
        }

        private void ClearSettingsB_Click(object sender, EventArgs e)
        {
            foreach (var c in Form.Controls)
            {
                ((Control)c).Enabled = false;
            }

            DFAutomaton?.Dispose();
            DFAutomaton = null;

            DFATypeSelectorCB.SelectedIndex = -1;
            DFATypeSelectorCB.Refresh();
            DFATypeSelectorCB.Invalidate();

            InputAlphabetTLB.GetData().Clear();
            InputAlphabetTLB.Refresh();
            InputAlphabetTLB.Invalidate();
            InputAlphabetTLB.RefreshListBox();

            OutputAlphabetTLB.GetData().Clear();
            OutputAlphabetTLB.Refresh();
            OutputAlphabetTLB.Invalidate();
            OutputAlphabetTLB.RefreshListBox();

            InputSequenceTB.Text = string.Empty;

            CircleDiameterNUD.Value = 50;
            BorderNUD.Value = 5;
            TransitionsLPNUD.Value = 3.0M;
            TransitionsBPNUD.Value = 3.0M;
            DrawStepDelayNUD.Value = 0.75M;

            Container.BackColor = Color.FromArgb(96, 96, 96);
            ContainerCP.BackColor = Color.FromArgb(96, 96, 96);
            ActiveBorderCP.BackColor = Color.LimeGreen;
            InactiveBorderCP.BackColor = Color.Black;
            HighlightedBorderCP.BackColor = Color.DarkGray;
            InnerStateCP.BackColor = Color.LightGray;
            TransitionsLPCP.BackColor = Color.LimeGreen;
            TransitionsBPCP.BackColor = Color.Black;

            PreventPathsInersectionsChB.Checked = true;
            DeveloperModeChB.Checked = false;

            foreach (var c in Form.Controls)
            {
                ((Control)c).Enabled = true;
            }

            Check();
        }



        private void DFATypeSelectorCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            DFAutomaton?.Dispose();
            DFAutomaton = null;

            DFAutomaton = AutomatonFactory.CreateAutomaton
            (
                AutomatonFactory.FromInt(DFATypeSelectorCB.SelectedIndex),
                Container,
                this
            );

            Check();
        }

        private void InputAlphabetTLB_ItemAdded(object sender, TypedListBox.ItemAddedEventArgs e)
        {
            char c = (char)e.AddedItem.Value;
            string s = e.AddedItem.Description;

            if (DFAutomaton != null)
            {
                if (!DFAutomaton.InputAlphabet.Contains(c))
                {
                    DFAutomaton.InputAlphabet.Add(c);
                    DFAutomaton.InputsDescription.Add(s);
                }
            }

            Check();
        }

        private void InputAlphabetTLB_ItemRemoved(object sender, TypedListBox.ItemRemovedEventArgs e)
        {
            char c = (char)e.RemovedItem.Value;
            string s = e.RemovedItem.Description;

            if (DFAutomaton != null)
            {
                if (DFAutomaton.InputAlphabet.Contains(c))
                {
                    DFAutomaton.InputAlphabet.Remove(c);
                    DFAutomaton.InputsDescription.Remove(s);
                }
            }

            Check();
        }

        private void OutputAlphabetTLB_ItemAdded(object sender, TypedListBox.ItemAddedEventArgs e)
        {
            char c = (char)e.AddedItem.Value;
            string s = e.AddedItem.Description;

            if (DFAutomaton != null)
            {
                if (!DFAutomaton.OutputAlphabet.Contains(c))
                {
                    DFAutomaton.OutputAlphabet.Add(c);
                    DFAutomaton.OutputsDescription.Add(s);
                }
            }

            Check();
        }

        private void OutputAlphabetTLB_ItemRemoved(object sender, TypedListBox.ItemRemovedEventArgs e)
        {
            char c = (char)e.RemovedItem.Value;
            string s = e.RemovedItem.Description;

            if (DFAutomaton != null)
            {
                if (DFAutomaton.OutputAlphabet.Contains(c))
                {
                    DFAutomaton.OutputAlphabet.Remove(c);
                    DFAutomaton.OutputsDescription.Remove(s);
                }
            }

            Check();
        }



        private void AddStateB_Click(object sender, EventArgs e)
        {
            using (AddStateForm form = new AddStateForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (DFAutomaton != null)
                    {
                        if (DFAutomaton.StatesAlphabet.Find(s => s.Index == DFAutomaton.StatesAlphabet.Count()) == null)
                        {
                            Point p = FindFreeInitialPoint();
                            if (p != Point.Empty)
                            {
                                DFAutomaton.StatesAlphabet.Add(AutomatonFactory.CreateState(DFAutomaton.Type, DFAutomaton, DFAutomaton.StatesAlphabet.Count, form.StateDescription.ToString(), p) );
                                Check();
                            }
                        }
                    }
                }
            }
        }



        /// TODO
        #region Обработка последовательностей  

        private void InputSequenceTB_TextChanged(object sender, EventArgs e)
        {
            CheckReadyToAnalyze();
        }

        private void GetRandomSequenceB_Click(object sender, EventArgs e)
        {
            InputSequenceTB.Text = GenerateRandomString(25, InputAlphabetTLB.GetData().Select(o => (char)o.Value).ToList());
            CheckReadyToAnalyze();
        }

        private async void AnalyzeSequenceB_Click(object sender, EventArgs e)
        {
            foreach (var c in Form.Controls)
            {
                ((Control)c).Enabled = false;
            }

            if (DFAutomaton != null)
                MessageBox.Show(await DFAutomaton.ProcessInputSequence(InputSequenceTB.Text));

            foreach (var c in Form.Controls)
            {
                ((Control)c).Enabled = true;
            }
            Check();
        }

        #endregion



        private void CircleDiameterNUD_ValueChanged(object sender, EventArgs e)
        {
            int cD = (int)CircleDiameterNUD.Value;
            if (DFAutomaton != null)
                DFAutomaton.SetStateDiameter(cD);

            Check();
        }

        private void BorderNUD_ValueChanged(object sender, EventArgs e)
        {
            int b = (int)BorderNUD.Value;
            if (DFAutomaton != null)
                DFAutomaton.SetStateBorder(b);

            Check();
        }

        private void DrawStepDelayNUD_ValueChanged(object sender, EventArgs e)
        {
            int timeMS = Convert.ToInt32(Convert.ToSingle(DrawStepDelayNUD.Value) * 1000);

            if (DFAutomaton != null)
                DFAutomaton.SetDrawStepDelay(timeMS);

            Check();
        }

        private void TransitionsLPNUD_ValueChanged(object sender, EventArgs e)
        {
            float width = Convert.ToSingle(TransitionsLPNUD.Value);

            if (DFAutomaton != null)
                DFAutomaton.SetTransitionLightPen(TransitionsLPCP.BackColor, width);
            
            Check();
        }

        private void TransitionsBPNUD_ValueChanged(object sender, EventArgs e)
        {
            float width = Convert.ToSingle(TransitionsBPNUD.Value);

            if (DFAutomaton != null)
                DFAutomaton.SetTransitionBlackPen(TransitionsBPCP.BackColor, width);
           
            Check();
        }

        private void ContainerCP_Click(object sender, EventArgs e)
        {
            Color c = DrawHelper.ShowColorDialog(Form, ContainerCP.BackColor, true);

            if (DFAutomaton != null)
                DFAutomaton.SetContainerBackColor(c);
            
            ContainerCP.BackColor = c;
            Check();
        }

        private void ActiveBorderCP_Click(object sender, EventArgs e)
        {
            Color c = DrawHelper.ShowColorDialog(Form, ActiveBorderCP.BackColor, true);

            if (DFAutomaton != null)
                DFAutomaton.SetActiveBorderColor(c);
            
            ActiveBorderCP.BackColor = c;
            Check();
        }

        private void InactiveBorderCP_Click(object sender, EventArgs e)
        {
            Color c = DrawHelper.ShowColorDialog(Form, InactiveBorderCP.BackColor, true);

            if (DFAutomaton != null)
                DFAutomaton.SetInactiveBorderColor(c);
           
            InactiveBorderCP.BackColor = c;
            Check();
        }

        private void HighlightedBorderCP_Click(object sender, EventArgs e)
        {
            Color c = DrawHelper.ShowColorDialog(Form, HighlightedBorderCP.BackColor, true);

            if (DFAutomaton != null)
                DFAutomaton.SetHighlightedBorderColor(c);
            
            HighlightedBorderCP.BackColor = c;
            Check();
        }

        private void InnerStateCP_Click(object sender, EventArgs e)
        {
            Color c = DrawHelper.ShowColorDialog(Form, InnerStateCP.BackColor, true);

            if (DFAutomaton != null)
                DFAutomaton.SetInnerStateColor(c);
           
            InnerStateCP.BackColor = c;
            Check();
        }

        private void TransitionsLPCP_Click(object sender, EventArgs e)
        {
            Color c = DrawHelper.ShowColorDialog(Form, TransitionsLPCP.BackColor, true);
            float width = Convert.ToSingle(TransitionsLPNUD.Value);

            if (DFAutomaton != null)
                DFAutomaton.SetTransitionLightPen(c, width);
            
            TransitionsLPCP.BackColor = c;
            Check();
        }

        private void TransitionsBPCP_Click(object sender, EventArgs e)
        {
            Color c = DrawHelper.ShowColorDialog(Form, TransitionsBPCP.BackColor, true);
            float width = Convert.ToSingle(TransitionsBPNUD.Value);

            if (DFAutomaton != null)
                DFAutomaton.SetTransitionBlackPen(c, width);
          
            TransitionsBPCP.BackColor = c;
            Check();
        }



        private void PreventPathsInersections_CheckedChanged(object sender, EventArgs e)
        {
            bool m = PreventPathsInersectionsChB.Checked;
            if (DFAutomaton != null)
                DFAutomaton.SetIntersectionsMode(m);

            Check();
        }

        private void DeveloperModeChB_CheckedChanged(object sender, EventArgs e)
        {
            bool m = DeveloperModeChB.Checked;
            if (DFAutomaton != null)
                DFAutomaton.SetDevMode(m);

            Check();
        }
 
        #endregion





        #region Методы проверки

        private void CheckReadyToSaveDFA()
        {
            SaveToFileTSB.Enabled = DFAutomaton != null;
        }

        private void CheckReadyToChangeDFAType()
        {
            DFATypeSelectorCB.Enabled = DFAutomaton == null;
        }

        private void CheckReadyToAddIA()
        {
            InputAlphabetTLB.Enabled = DFATypeSelectorCB.SelectedIndex != -1;
        }

        private void CheckReadyToAddOA()
        {
            OutputAlphabetTLB.Enabled = DFATypeSelectorCB.SelectedIndex != -1 && InputAlphabetTLB.GetData().Count >= 2;
        }

        private void CheckReadyToSave()
        {
            if (DFAutomaton != null)
                SaveToFileTSB.Enabled = DFAutomaton.StatesAlphabet.Count >= 2;
        }

        private void CheckReadyToAddStates()
        {
            bool a = InputAlphabetTLB.GetData().Count >= 2 && OutputAlphabetTLB.GetData().Count >= 2;
            bool b = DFATypeSelectorCB.SelectedIndex != -1;
            bool c = DFAutomaton != null;
            AddStateB.Enabled = a && b && c;
        }

        private void CheckReadyToInputAS()
        {
            bool a = false;
            if (DFAutomaton != null)
                a = DFAutomaton.StatesAlphabet.Count >= 2;

            InputSequenceTB.Enabled = a;
            GetRandomSequenceB.Enabled = a;
        }

        private void CheckReadyToAnalyze()
        {
            bool a = false, b = false, c = false;
            if (DFAutomaton != null)
            {
                a = DFAutomaton.StatesAlphabet.Count >= 2;
                b = InputSequenceTB.Text.Length > 0 && InputSequenceTB.Text.All(ch => DFAutomaton.InputAlphabet.Contains(ch));
                c = DFAutomaton != null && DFAutomaton.IsReady();
            }
           
            AnalyzeSequenceB.Enabled = a && b && c;
        }

        private void CheckReadyToChangeVisParams()
        {
            bool a = false;
            if (DFAutomaton != null)
                a = DFAutomaton.StatesAlphabet.Count >= 1;

            CircleDiameterNUD.Enabled = a;
            BorderNUD.Enabled = a;
            DrawStepDelayNUD.Enabled = a;
            TransitionsLPNUD.Enabled = a;
            TransitionsBPNUD.Enabled = a;

            ContainerCP.Enabled = a;
            ActiveBorderCP.Enabled = a;
            InactiveBorderCP.Enabled = a;
            HighlightedBorderCP.Enabled = a;
            InnerStateCP.Enabled = a;
            TransitionsLPCP.Enabled = a;
            TransitionsBPCP.Enabled = a;

            PreventPathsInersectionsChB.Enabled = a;
        }

        public void Check()
        {
            CheckReadyToSaveDFA();
            CheckReadyToChangeDFAType();
            CheckReadyToAddIA();
            CheckReadyToAddOA();
            CheckReadyToSave();
            CheckReadyToAddStates();
            CheckReadyToInputAS();
            CheckReadyToAnalyze();
            CheckReadyToChangeVisParams();
        }

        #endregion





        #region Вспомогательное

        public void LoadParameters
        (
             int type, int cD, int bW,
             int dTSms,
             Color aB, Color iB, Color hB, Color iS, Color cC,
             float bPW, Color bPC,
             float lPW, Color lPC,
             IDFAutomaton automaton
        )
        {
            DFATypeSelectorCB.SelectedIndex = type;
            CircleDiameterNUD.Value = cD;
            BorderNUD.Value = bW;
            DrawStepDelayNUD.Value = Convert.ToDecimal(dTSms / 1000D);

            ActiveBorderCP.BackColor = aB;
            InactiveBorderCP.BackColor = iB;
            HighlightedBorderCP.BackColor = hB;
            InnerStateCP.BackColor = iS;
            Container.BackColor = cC;
            ContainerCP.BackColor = cC;

            TransitionsBPNUD.Value = Convert.ToDecimal(bPW);
            TransitionsBPCP.BackColor = bPC;

            TransitionsLPNUD.Value = Convert.ToDecimal(lPW);
            TransitionsLPCP.BackColor = lPC;

            automaton.SetStateDiameter(cD);
            automaton.SetStateBorder(bW);
            automaton.SetDrawStepDelay(dTSms);

            automaton.SetActiveBorderColor(aB);
            automaton.SetInactiveBorderColor(iB);
            automaton.SetHighlightedBorderColor(hB);
            automaton.SetInnerStateColor(iS);
            automaton.SetContainerBackColor(cC);

            automaton.SetTransitionBlackPen(bPC, bPW);
            automaton.SetTransitionLightPen(lPC, lPW);

            Check();
        }

        public void SetDescriptions(List<DescribedObject<object>> ia, List<DescribedObject<object>> ib)
        {
            InputAlphabetTLB.SetData(ia);
            OutputAlphabetTLB.SetData(ib);
        }

        private Point FindFreeInitialPoint()
        {
            int containerWidth = Container.Width;
            int containerHeight = Container.Height;

            for (int x = 100; x < containerWidth - 100; x++)
            {
                for (int y = 100; y < containerHeight - 100; y++)
                {
                    Point newPoint = new Point(x, y);
                    if (IsPointFree(newPoint))
                    {
                        ExistingInitialPoints.Add(newPoint);
                        return newPoint;
                    }
                }
            }

            MessageBox.Show("Не удалось найти свободную точку.");
            Check();
            return Point.Empty;
        }

        private bool IsPointFree(Point point)
        {
            foreach (var existingPoint in ExistingInitialPoints)
            {
                if (Math.Abs(existingPoint.X - point.X) < 150 && Math.Abs(existingPoint.Y - point.Y) < 150)
                {
                    return false;
                }
            }
            return true;
        }

        private static string GenerateRandomString(int n, List<char> symbols)
        {
            if (n <= 0 || symbols == null || symbols.Count == 0)
            {
                return string.Empty;
            }

            int length = random.Next(1, n + 1);
            char[] randomChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomChars[i] = symbols[random.Next(symbols.Count)];
            }

            return new string(randomChars);
        }

        #endregion
    }
}
