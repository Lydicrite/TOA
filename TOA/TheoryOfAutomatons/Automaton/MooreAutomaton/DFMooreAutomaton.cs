using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using TheoryOfAutomatons.Automaton.Common;
using TheoryOfAutomatons.Utils.Helpers;
using TheoryOfAutomatons.Utils.UI.Controls;

namespace TheoryOfAutomatons.Automaton.MooreAutomaton
{
    internal class DFMooreAutomaton : DFAutomaton<MooreAutomatonState>
    {
        private Dictionary<MooreAutomatonState, char> outputFunction;

        public DFMooreAutomaton(PictureBox container, AutomatonCreator aC) : base(container, aC)
        {
            outputFunction = new Dictionary<MooreAutomatonState, char>();
        }

        public override IDictionary<object, char> OutputFunction
        {
            get => outputFunction == null ? new Dictionary<object, char>() : outputFunction.ToDictionary(x => (object)x.Key, x => x.Value);
            protected set
            {
                if (value == null)
                    outputFunction = null;
                else
                    outputFunction = value.ToDictionary(x => (MooreAutomatonState)x.Key, x => x.Value);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (outputFunction != null)
            {
                outputFunction.Clear();
                outputFunction = null;
            }
        }

        ~DFMooreAutomaton()
        {
            Dispose();
        }





        #region Логика работы АА

        public override async Task<string> ProcessInputSequence(string inputSequence)
        {
            if (!IsReady())
            {
                MessageBox.Show("Automaton is not ready. Ensure all states have transitions for each input symbol.");
                return string.Empty;
            }

            AC.Form.MinimizeBox = false;

            var log = new StringBuilder();
            CurrentState = StatesAlphabet[0];
            CurrentState.IsInput = true;

            log.AppendLine($"Обработка входной последовательности {inputSequence}:");
            for (int i = 0; i < inputSequence.Length; i++)
            {
                // FillMaps();

                CurrentInputSymbol = inputSequence[i];
                CurrentState.IsInput = true;

                // Показываем текущее состояние
                OnTransitionsChanged();
                Redraw();
                await Task.Delay(DrawStepDelay);

                // Логируем текущее состояние и выходной сигнал
                log.AppendLine($"\nОбработка входного символа №{i} = {CurrentInputSymbol}: ");
                log.AppendLine($"\tДо получения символа автомат находился в состоянии {CurrentState.Name}. ");

                // Отображение перехода к следующему состоянию
                CurrentOutputSymbol = CurrentState.Output;
                OnTransitionsChanged();
                Redraw();
                await Task.Delay(DrawStepDelay);

                // Логируем новое состояние и выходной сигнал
                var nextState = CurrentState.Transitions[CurrentInputSymbol];
                log.AppendLine($"\tПосле обработки символа автомат перешёл в состояние {nextState.Name} и послал выходной сигнал {CurrentOutputSymbol}");

                // Обновляем текущее состояние и необходимую информацию
                CurrentInputSymbol = '\0';
                CurrentOutputSymbol = '\0';
                CurrentState.IsInput = false;
                CurrentState = nextState;

                // FillMaps();

                // Отображение конца обработки символа
                OnTransitionsChanged();
                Redraw();
                await Task.Delay(DrawStepDelay);
            }

            AC.Form.MinimizeBox = true;
            return log.ToString();
        }

        #endregion





        #region Визуализация

        public override void DeleteState(MooreAutomatonState state)
        {
            StatesAlphabet.Remove(state);

            // Очищаем переходы и выходы в состояниях АА
            foreach (var s in StatesAlphabet)
            {
                var trs = s.Transitions.ToList().FindAll(kvp => kvp.Value == state);
                foreach (var tr in trs)
                {
                    s.Transitions.Remove(tr.Key);
                }
            }

            // Очищаем переходы в АА
            var atrs = Transitions.FindAll(tr => tr.From == state || tr.To == state);
            foreach (var atr in atrs)
            {
                Transitions.Remove(atr);
            }

            // Очищаем функцию переходов
            var atfs = TransitionFunction.ToList().FindAll(kvp => kvp.Value == state || kvp.Key.Item2 == state);
            foreach (var atf in atfs)
            {
                TransitionFunction.Remove(atf.Key);
            }

            // Очищаем функцию выходов
            var aofs = outputFunction.ToList().FindAll(kvp => kvp.Key == state);
            foreach (var aof in aofs)
            {
                outputFunction.Remove(aof.Key);
            }

            OnTransitionsChanged();
        }

        #region Отрисовка путей
        public override void DrawTransitions(Graphics g)
        {
            base.DrawTransitions(g);

            DrawHelper.SetGraphicsParameters(g);

            AutomatonTransition<MooreAutomatonState> tr = null;
            if (CurrentInputSymbol != '\0')
            {
                tr = Transitions.Find(t => t.From == CurrentState && t.To == TransitionFunction[Tuple.Create(CurrentInputSymbol, CurrentState)]);
            }

            foreach (var transition in Transitions)
            {
                if (CurrentOutputSymbol != '\0' && CurrentInputSymbol != '\0')
                {
                    if (transition == tr)
                        transition.DrawTransition(TransitionLightPen, g, TransitionsSpace, false);
                    else
                        transition.DrawTransition(TransitionBlackPen, g, TransitionsSpace, false);
                }
                else
                {
                    transition.DrawTransition(TransitionBlackPen, g, TransitionsSpace, true);
                }
            }
        }
        #endregion

        #endregion





        #region Сохранение и загрузка в файл

        public override void Save(string filePath)
        {
            var automatonData = new
            {
                DrawStepDelay,

                CircleDiameter,
                BorderWidth,
                TransitionBlackPenWidth,
                TransitionLightPenWidth,

                ActiveBorderColor,
                InactiveBorderColor,
                HighlightedBorderColor,
                InnerStateColor,
                ContainerBackColor,
                InactiveTransitionColor,
                ActiveTransitionColor,

                TypeIndex,

                InputAlphabet,
                InputsDescription,
                OutputAlphabet,
                OutputsDescription,
                StatesAlphabet = StatesAlphabet.Select(s => new
                {
                    s.Index,
                    s.Name,
                    s.UserDefinedText,
                    s.StateCenter,
                    s.IsMoving,
                    s.IsCyclic,
                    s.IsInput,
                    s.Output,
                    Transitions = s.Transitions.Select(t => new { t.Key, StateIndex = t.Value.Index }).ToDictionary(t => t.Key, t => t.StateIndex)
                }).ToList()
            };

            var json = JsonConvert.SerializeObject(automatonData, Formatting.Indented);

            if (File.Exists(filePath))
                File.Delete(filePath);

            File.WriteAllText(filePath, json);

            FileInfo fileInfo = new FileInfo(filePath);
            fileInfo.IsReadOnly = true;
        }

        public override void Load(string filePath, PictureBox container)
        {
            var json = File.ReadAllText(filePath);
            var automatonData = JsonConvert.DeserializeObject<dynamic>(json);

            // Установка параметров АА
            int cD, bW, type;
            cD = (int)automatonData.CircleDiameter;
            bW = (int)automatonData.BorderWidth;

            int dTSms = (int)automatonData.DrawStepDelay;
            Color aB = (Color)automatonData.ActiveBorderColor;
            Color iB = (Color)automatonData.InactiveBorderColor;
            Color hB = (Color)automatonData.HighlightedBorderColor;
            Color iS = (Color)automatonData.InnerStateColor;
            Color cC = (Color)automatonData.ContainerBackColor;

            float bPW = (float)automatonData.TransitionBlackPenWidth;
            Color bPC = (Color)automatonData.InactiveTransitionColor;

            float lPW = (float)automatonData.TransitionLightPenWidth;
            Color lPC = (Color)automatonData.ActiveTransitionColor;

            type = (int)automatonData.TypeIndex;

            MainForm.AutomatonCreator.LoadParameters
            (
                type, cD, bW,
                dTSms,
                aB, iB, hB, iS, cC,
                bPW, bPC,
                lPW, lPC,
                this
            );

            // Инициализация текущего экземпляра автомата
            this.InputAlphabet = ((IEnumerable<dynamic>)automatonData.InputAlphabet)
                                    .Select(x => (char)x)
                                    .ToList();
            this.InputsDescription = ((IEnumerable<dynamic>)automatonData.InputsDescription)
                                        .Select(x => (string)x)
                                        .ToList();
            this.OutputAlphabet = ((IEnumerable<dynamic>)automatonData.OutputAlphabet)
                                    .Select(x => (char)x)
                                    .ToList();
            this.OutputsDescription = ((IEnumerable<dynamic>)automatonData.OutputsDescription)
                                        .Select(x => (string)x)
                                        .ToList();

            FillACData(MainForm.AutomatonCreator);

            // Очистка текущего алфавита состояний перед загрузкой новых данных
            this.StatesAlphabet.Clear();

            // Добавление алфавита состояний из JSON
            foreach (var stateData in automatonData.StatesAlphabet)
            {
                var state = new MooreAutomatonState(
                    this,
                    (int)stateData.Index,
                    (string)stateData.UserDefinedText,
                    (Point)stateData.StateCenter
                );

                state.IsMoving = (bool)stateData.IsMoving;
                state.IsCyclic = (bool)stateData.IsCyclic;
                state.IsInput = (bool)stateData.IsInput;
                state.Output = (char)stateData.Output;
                state.Transitions = new Dictionary<char, MooreAutomatonState>(this.InputAlphabet.Count());

                this.AddState(state);
            }

            GeometryHelper.AdaptContainerSize(this, MainForm.AutomatonCreator.Container, MainForm.AutomatonCreator.Form);

            // Добавление функций переходов и выходов для состояний
            for (int i = 0; i < automatonData.StatesAlphabet.Count; i++)
            {
                var stateData = automatonData.StatesAlphabet[i];
                foreach (var kvp in stateData.Transitions)
                {
                    char inputChar = (char)kvp.Name[0];
                    int index = (int)(kvp.Value);
                    MooreAutomatonState aS = this.StatesAlphabet.Find(s => s.Index == index);
                    this.StatesAlphabet[i].AddTransition(inputChar, aS);
                }

                this.StatesAlphabet[i].Output = (char)stateData.Output;
            }

            foreach (var state in this.StatesAlphabet)
            {
                if (state.Output == '\0')
                {
                    state.Output = '\0';
                    this.OutputFunction[state] = '\0';
                }
            }
        }

        #endregion
    }
}