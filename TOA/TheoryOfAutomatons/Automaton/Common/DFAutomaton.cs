using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

using TheoryOfAutomatons.Utils.Helpers;
using TheoryOfAutomatons.Utils.Containers;
using TheoryOfAutomatons.Automaton.MealyAutomaton;
using TheoryOfAutomatons.Automaton.MooreAutomaton;
using TheoryOfAutomatons.Utils.UI.Controls;
using System.Linq;
using System.Windows.Automation;
using TOA.TheoryOfAutomatons.Automaton.Common;
using TOA.TheoryOfAutomatons.Automaton;

namespace TheoryOfAutomatons.Automaton.Common
{
    /// <summary>
    /// Реализует абстрактный класс Конечного Автомата, содержащий общие поля, члены, методы и функции.
    /// </summary>
    /// <typeparam name="TState">Тип состояния Автомата</typeparam>
    internal abstract class DFAutomaton<TState> : IDFAutomaton
        where TState : class
    {
        #region Параметры для работы

        /// <summary>
        /// Определяет тип Автомата.
        /// </summary>
        public abstract AutomatonType Type { get; }
        /// <summary>
        /// Входной алфавит АА.
        /// </summary>
        public List<char> InputAlphabet { get; protected set; }
        /// <summary>
        /// Выходной алфавит АА.
        /// </summary>
        public List<char> OutputAlphabet { get; protected set; }
        /// <summary>
        /// Алфавит состояний АА.
        /// </summary>
        public List<TState> StatesAlphabet { get; protected set; }
        /// <summary>
        /// Функция переходов АА (фактически - инструкция для функции переходов)<br/>
        /// Ставит в соответствие паре (вход, состояние) соответствующее новое состояние из <see cref="StatesAlphabet"/>.
        /// </summary>
        public Dictionary<Tuple<char, TState>, TState> TransitionFunction { get; protected set; }
        /// <summary>
        /// Функция выходов АА (фактически - инструкция для функции выходов). <br/>
        /// Ставит в соответствие паре (текущий вход, текущее состояние) соответствующее выходное значение из <see cref="OutputAlphabet"/>.
        /// </summary>
        public abstract IDictionary<object, char> OutputFunction { get; protected set; }
        /// <summary>
        /// Текущий входной символ.
        /// </summary>
        public char CurrentInputSymbol { get; protected set; } = '\0';
        /// <summary>
        /// Текущий выходной символ.
        /// </summary>
        public char CurrentOutputSymbol { get; protected set; } = '\0';
        /// <summary>
        /// Текущее состояние АА.
        /// </summary>
        protected TState CurrentState;

        #endregion



        #region Параметры для графического отображения АА

        public event EventHandler TransitionsChanged;

        /// <summary>
        /// <see cref="PictureBox"/> для отрисовки АА.
        /// </summary>
        public PictureBox Container { get; protected set; }



        #region Для AutomatonCreator

        /// <summary>
        /// Связанный с данным АА Объект контроллера <see cref="AutomatonCreator"/>. 
        /// </summary>
        public AutomatonCreator AC { get; protected set; }
        /// <summary>
        /// Список, содержащий описания для входных символов АА.
        /// </summary>
        public List<string> InputsDescription { get; protected set; }
        /// <summary>
        /// Список, содержащий описания для выходных символов АА.
        /// </summary>
        public List<string> OutputsDescription { get; protected set; }

        #endregion



        #region Для отрисовки состояний

        /// <summary>
        /// Карандаш для отрисовки переходов в неактивном состоянии.
        /// </summary>
        public Pen TransitionBlackPen { get; private set; }
        /// <summary>
        /// Карандаш для отрисовки переходов в активном состоянии.
        /// </summary>
        public Pen TransitionLightPen { get; private set; }
        /// <summary>
        /// Объект <see cref="System.Windows.Forms.ToolTip"/> для отображения всплывающих подсказок.
        /// </summary>
        public ToolTip ToolTip { get; protected set; }
        /// <summary>
        /// Объект <see cref="System.Windows.Forms.Timer"/> для задержек отображения всплывающих подсказок.
        /// </summary>
        public System.Windows.Forms.Timer HoverTimer { get; protected set; }
        /// <summary>
        /// Последняя позиция курсора.
        /// </summary>
        public Point LastMousePos { get; protected set; }
        /// <summary>
        /// Состояние АА, над которым находится курсор.
        /// </summary>
        protected TState HoveredState;
        /// <summary>
        /// Состояние АА, которое в данный момент находится в режиме перемещения.
        /// </summary>
        protected TState StateBeingMoved;

        #endregion



        #region Отрисовка связей

        /// <summary>
        /// Список, использующийся для отрисовки переходов между состояниями АА.
        /// </summary>
        public List<AutomatonTransition<TState>> Transitions;
        /// <summary>
        /// Булева карта для верного размещения состояний АА.
        /// </summary>
        public bool[,] StatesSpace { get; protected set; }
        /// <summary>
        /// Булева карта для верной отрисовки переходов АА.
        /// </summary>
        public bool[,] TransitionsSpace { get; protected set; }
        /// <summary>
        /// Кэш-изображение, используемое для отрисовки переходов АА.
        /// </summary>
        public Bitmap TransitionsCache { get; protected set; }
        /// <summary>
        /// Определяет, нужно ли обновить переходы.
        /// </summary>
        public bool TransitionsNeedUpdate { get; protected set; } = true;
        /// <summary>
        /// Объект для грамотной перерисовки переходов.
        /// </summary>
        protected readonly object CacheLock = new object();

        #endregion



        #region Настройки

        /// <summary>
        /// Диаметр круга, который ограничивает область состояния АА.
        /// </summary>
        public int CircleDiameter { get; protected set; } = 50;
        /// <summary>
        /// Ширина границы области состояния АА.
        /// </summary>
        public int BorderWidth { get; protected set; } = 5;
        /// <summary>
        /// Ширина карандаша для активного карандаша.
        /// </summary>
        public float TransitionBlackPenWidth { get; protected set; } = 3.0f;
        /// <summary>
        /// Ширина карандаша для неактивного карандаша.
        /// </summary>
        public float TransitionLightPenWidth { get; protected set; } = 3.0f;
        /// <summary>
        /// Задержка отрисовки каждого шага работы АА в миллисекундах.
        /// </summary>
        public int DrawStepDelay { get; private set; } = 750;
        /// <summary>
        /// Цвет заливки рабочей области.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color ContainerBackColor { get; private set; } = Color.FromArgb(96, 96, 96);
        /// <summary>
        /// Цвет границы активного состояния.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color ActiveBorderColor { get; private set; } = Color.LimeGreen;
        /// <summary>
        /// Цвет границы неактивного состояния.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color InactiveBorderColor { get; private set; } = Color.Black;
        /// <summary>
        /// Цвет границы выбранного состояния.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color HighlightedBorderColor { get; private set; } = Color.DarkGray;
        /// <summary>
        /// Цвет заливки внутреннего состояния.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color InnerStateColor { get; private set; } = Color.LightGray;
        /// <summary>
        /// Цвет активного перехода.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color ActiveTransitionColor { get; private set; } = Color.LimeGreen;
        /// <summary>
        /// Цвет неактивного перехода.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color InactiveTransitionColor { get; private set; } = Color.Black;
        /// <summary>
        /// Нужно ли запретить пересечения переходов при их отрисовке.
        /// </summary>
        public bool ProhibitIntersectingTransitions { get; protected set; } = true;
        /// <summary>
        /// Включает или отключает режим разработчика.
        /// </summary>
        public bool DeveloperMode { get; protected set; } = false;

        #endregion

        #endregion



        /// <summary>
        /// Представляет абстрактный класс АА.
        /// </summary>
        /// <param name="container"><see cref="PictureBox"/> для отрисовки АА.</param>
        /// <param name="aC"><see cref="AutomatonCreator"/> для управления АА.</param>
        protected DFAutomaton(PictureBox container, AutomatonCreator aC)
        {
            // Проверка на допустимые классы для состояния Автомата
            if (!(typeof(TState) == typeof(MealyAutomatonState) || typeof(TState) == typeof(MooreAutomatonState)))
                throw new NotSupportedException("Неподдерживаемый тип состояния Автомата.");


            TransitionBlackPen = CreatePen(Brushes.Black, TransitionBlackPenWidth);
            TransitionLightPen = CreatePen(Brushes.LimeGreen, TransitionLightPenWidth);

            Container = container;
            DrawHelper.SetGraphicsParameters(Container.CreateGraphics());

            InputAlphabet = new List<char>();
            OutputAlphabet = new List<char>();
            StatesAlphabet = new List<TState>();
            TransitionFunction = new Dictionary<Tuple<char, TState>, TState>();
            Transitions = new List<AutomatonTransition<TState>>();
            // OutputFunction реализуется отдельно для каждого типа автомата

            Container.CreateGraphics();
            TransitionsCache = new Bitmap(Container.Width, Container.Height);

            InputsDescription = new List<string>();
            OutputsDescription = new List<string>();

            ToolTip = new ToolTip();
            HoverTimer = new System.Windows.Forms.Timer();
            HoverTimer.Interval = 1000;
            HoverTimer.Tick += HoverTimer_Tick;
            Container.Paint += Container_Paint;
            Container.MouseMove += Container_MouseMove;
            Container.Click += Container_MouseClick;
            Container.MouseDoubleClick += Container_MouseDoubleClick;
            Container.MouseLeave += Container_MouseLeave;

            HoveredState = null;
            StateBeingMoved = null;
            AC = aC;
            AC.Check();
        }

        /// <summary>
        /// Освобождает связанные с АА ресурсы.
        /// </summary>
        public virtual void Dispose()
        {
            // Очистка ресурсов
            InputAlphabet?.Clear();
            OutputAlphabet?.Clear();
            StatesAlphabet?.Clear();
            TransitionFunction?.Clear();
            OutputFunction?.Clear();
            Transitions?.Clear();
            InputsDescription?.Clear();
            OutputsDescription?.Clear();

            // Остановка и освобождение таймера
            if (HoverTimer != null)
            {
                HoverTimer.Stop();
                HoverTimer.Tick -= HoverTimer_Tick;
                HoverTimer.Dispose();
            }

            // Освобождение ToolTip
            ToolTip?.Dispose();

            // Освобождение карандашей
            TransitionBlackPen?.Dispose();
            TransitionLightPen?.Dispose();

            // Освобождение кеша переходов
            if (TransitionsCache != null)
            {
                TransitionsCache.Dispose();
                TransitionsCache = null;
            }

            // Отвязка событий от PictureBox
            if (Container != null)
            {
                Container.Paint -= Container_Paint;
                Container.MouseMove -= Container_MouseMove;
                Container.MouseClick -= Container_MouseClick;
                Container.MouseDoubleClick -= Container_MouseDoubleClick;
                Container.MouseLeave -= Container_MouseLeave;
            }

            // Убрать ссылки на объекты
            InputAlphabet = null;
            OutputAlphabet = null;
            StatesAlphabet = null;
            TransitionFunction = null;
            Transitions = null;
            OutputFunction = null;

            InputsDescription = null;
            OutputsDescription = null;

            Container = null;
            TransitionsCache = null;
        }

        /// <summary>
        /// Освобождает связанные с Автоматом ресурсы.
        /// </summary>
        ~DFAutomaton()
        {
            Dispose();
        }





        #region Сеттеры параметров

        /// <summary>
        /// Устанавливает диаметр круга, отображающего состояние АА.
        /// </summary>
        /// <param name="d">Диаметр.</param>
        public void SetStateDiameter(int d)
        {
            if (d > 60 && d < 30)
                return;
            else
                CircleDiameter = d;

            foreach (dynamic state in StatesAlphabet)
            {
                state.CreateTransitionStartPoints();
            }

            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает ширину границы круга, отображающего состояние АА.
        /// </summary>
        /// <param name="b">Ширина.</param>
        public void SetStateBorder(int b)
        {
            if (b > 5 && b < 2)
                return;
            else
                BorderWidth = b;
            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает временной шаг задержки отрисовки работы.
        /// </summary>
        /// <param name="t">Время в миллисекундах.</param>
        public void SetDrawStepDelay(int t)
        {
            DrawStepDelay = t;
        }

        /// <summary>
        /// Устанавливает цвет активной границы состояния.
        /// </summary>
        /// <param name="c">Цвет активной границы.</param>
        public void SetActiveBorderColor(Color c)
        {
            ActiveBorderColor = c;
            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает цвет неактивной границы состояния.
        /// </summary>
        /// <param name="c">Цвет неактивной границы.</param>
        public void SetInactiveBorderColor(Color c)
        {
            InactiveBorderColor = c;
            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает цвет подсвеченной границы состояния.
        /// </summary>
        /// <param name="c">Цвет подсвеченной границы.</param>
        public void SetHighlightedBorderColor(Color c)
        {
            HighlightedBorderColor = c;
            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает цвет заливки внутренней части состояния.
        /// </summary>
        /// <param name="c">Цвет заливки внутренней части состояния.</param>
        public void SetInnerStateColor(Color c)
        {
            InnerStateColor = c;
            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает цвет активного перехода.
        /// </summary>
        /// <param name="c">Цвет активного перехода.</param>
        public void SetActiveTransitionColor(Color c)
        {
            ActiveTransitionColor = c;
            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает цвет неактивного перехода.
        /// </summary>
        /// <param name="c">Цвет неактивного перехода.</param>
        public void SetInactiveTransitionColor(Color c)
        {
            InactiveTransitionColor = c;
            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает цвет рабочей области.
        /// </summary>
        /// <param name="c">Цвет рабочей области.</param>
        public void SetContainerBackColor(Color c)
        {
            ContainerBackColor = c;
            Container.BackColor = ContainerBackColor;
            OnTransitionsChanged();
        }

        /// <summary>
        /// Обновляет параметры карандаша для неактивных переходов.
        /// </summary>
        public void SetTransitionBlackPen(Color color, float width)
        {
            TransitionBlackPen?.Dispose();
            TransitionBlackPenWidth = width;
            TransitionBlackPen = CreatePen(new SolidBrush(color), width);
            SetInactiveTransitionColor(color);
            OnTransitionsChanged();
        }

        /// <summary>
        /// Обновляет параметры карандаша для активных переходов.
        /// </summary>
        public void SetTransitionLightPen(Color color, float width)
        {
            TransitionLightPen?.Dispose();
            TransitionLightPenWidth = width;
            TransitionLightPen = CreatePen(new SolidBrush(color), width);
            SetActiveTransitionColor(color);
            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает режим отрисовки переходов.
        /// </summary>
        /// <param name="m">Поддерживаются ли пересекающиеся переходы.</param>
        public void SetIntersectionsMode(bool m)
        {
            ProhibitIntersectingTransitions = m;
            OnTransitionsChanged();
        }

        /// <summary>
        /// Устанавливает режим разработчика.
        /// </summary>
        /// <param name="m">Нужно ли установить режим разработчика.</param>
        public void SetDevMode(bool m)
        {
            DeveloperMode = m;
            OnTransitionsChanged();
        }

        #endregion





        #region Логика работы АА

        /// <summary>
        /// Проверяет, готов ли АА к обработке входной последовательности.
        /// </summary>
        /// <returns> <see langword="true"/> , если АА не имеет обрыва связей; иначе <see langword="false"/>.</returns>
        public virtual bool IsReady()
        {
            foreach (var state in StatesAlphabet)
            {
                if (!HasAllTransitions(state))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Проверяет, нет ли обрыва связи у данного состояния.
        /// </summary>
        /// <param name="state">Состояние для проверки.</param>
        /// <returns></returns>
        private bool HasAllTransitions(TState state)
        {
            if (((dynamic)state).Transitions.Count != InputAlphabet.Count)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Выполняет обработку входной последовательности символов с "подсветкой" текущих активных выходов, состояний и т.п.
        /// </summary>
        /// <param name="inputSequence">Входная последовательность символов.</param>
        /// <returns>Возвращает текст (строку), описывающий обработку входной последовательности.</returns>
        public abstract Task<string> ProcessInputSequence(string inputSequence);

        #endregion





        #region Визуализация

        /// <summary>
        /// Добавляет новое состояние.
        /// </summary>
        /// <param name="state">Новое состояние.</param>
        public void AddState(TState state)
        {
            StatesAlphabet.Add(state);

            FillMaps();

            UpdateTransitionsCache();
            OnTransitionsChanged();

            Redraw();
            AC.Check();
        }

        /// <summary>
        /// Удаляет состояние.
        /// </summary>
        /// <param name="state">Состояние для удаления.</param>
        public abstract void DeleteState(TState state);

        #region Отрисовка путей

        /// <summary>
        /// Создаёт карандаш указанной ширины и цвета указанной кисти.
        /// </summary>
        /// <param name="brush">Кисть для передачи цвета.</param>
        /// <param name="width">Ширина карандаша.</param>
        /// <returns>Карандаш с указанными параметрами</returns>
        private Pen CreatePen(Brush brush, float width)
        {
            return new Pen(brush, width)
            {
                EndCap = LineCap.ArrowAnchor,
                CustomEndCap = new AdjustableArrowCap(width, width, true)
            };
        }

        /// <summary>
        /// Отрисовывает состояния в соответствии с необходимыми текущими настройками.
        /// </summary>
        /// <param name="g">Объект для отрисовки.</param>
        public virtual void DrawTransitions(Graphics g)
        {
            if (Transitions == null || Transitions.Count == 0 || TransitionsSpace == null)
                return;

            // РАЗНОЕ
        }

        /// <summary>
        /// Заполняет булевы карты, необходимые для верной отрисовки состояний и переходов.
        /// </summary>
        public void FillMaps()
        {
            if (Container != null && StatesAlphabet != null)
            {
                int width = Container.Width;
                int height = Container.Height;
                StatesSpace = new bool[width, height];
                TransitionsSpace = new bool[width, height];

                // Параметры для кругов и эллипсов
                float stateExlusionRadius = CircleDiameter * 2f;                    // Радиус круга исключения для StatesSpace
                float transitionEllipseHorizontalRadius = CircleDiameter / 1.5f;    // Горизонтальный радиус эллипса для TransitionsSpace
                float transitionEllipseVerticalRadius = CircleDiameter / 1.15f;     // Вертикальный радиус эллипса для TransitionsSpace

                // Инициализация всего пространства с границами
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        // Проверка границ для StatesSpace
                        bool isOutOfStatesBoundary = x <= CircleDiameter * 1.5f ||
                                                      y <= CircleDiameter * 1.5f ||
                                                      x >= Container.Width - CircleDiameter * 1.5f ||
                                                      y >= Container.Height - CircleDiameter * 1.5f;
                        StatesSpace[x, y] = !isOutOfStatesBoundary;

                        // Проверка границ для TransitionsSpace
                        bool isOutOfTransitionsBoundary = x <= CircleDiameter ||
                                                          y <= CircleDiameter ||
                                                          x >= Container.Width - CircleDiameter ||
                                                          y >= Container.Height - CircleDiameter;
                        TransitionsSpace[x, y] = !isOutOfTransitionsBoundary;
                    }
                }


                /// ОБЩЕ_РАЗНОЕ
                // Отмечаем круглые области, занятые состояниями в StatesSpace
                foreach (dynamic state in StatesAlphabet)
                {
                    int centerX = state.StateCenter.X;
                    int centerY = state.StateCenter.Y;

                    int left = centerX - (int)stateExlusionRadius;
                    int top = centerY - (int)stateExlusionRadius;
                    int right = centerX + (int)stateExlusionRadius;
                    int bottom = centerY + (int)stateExlusionRadius;

                    for (int x = Math.Max(0, left); x < Math.Min(width, right); x++)
                    {
                        for (int y = Math.Max(0, top); y < Math.Min(height, bottom); y++)
                        {
                            // Проверка, попадает ли точка внутрь круга
                            float dx = x - centerX;
                            float dy = y - centerY;
                            if (dx * dx + dy * dy <= stateExlusionRadius * stateExlusionRadius)
                            {
                                StatesSpace[x, y] = false;
                            }
                        }
                    }
                }

                // Отмечаем эллиптические области для TransitionsSpace
                foreach (dynamic state in StatesAlphabet)
                {
                    int stateIndex = StatesAlphabet.IndexOf(state);
                    float gold = (float)(stateIndex + 1) / StatesAlphabet.Count;

                    int centerX = state.StateCenter.X;
                    int centerY = state.StateCenter.Y;

                    // Динамическая настройка вертикального радиуса эллипса
                    float dynamicVerticalRadius = transitionEllipseVerticalRadius * ((gold >= 0.65f && gold <= 0.67f) ? 1.15f : 1f) * (1f + gold / 4);

                    int left = centerX - (int)transitionEllipseHorizontalRadius;
                    int top = centerY - (int)dynamicVerticalRadius;
                    int right = centerX + (int)transitionEllipseHorizontalRadius;
                    int bottom = centerY + (int)dynamicVerticalRadius;

                    for (int x = Math.Max(0, left); x < Math.Min(width, right); x++)
                    {
                        for (int y = Math.Max(0, top); y < Math.Min(height, bottom); y++)
                        {
                            // Нормализация координат для эллипса
                            float normalizedX = (x - centerX) / transitionEllipseHorizontalRadius;
                            float normalizedY = (y - centerY) / dynamicVerticalRadius;

                            // Проверка, попадает ли точка внутрь эллипса
                            if (normalizedX * normalizedX + normalizedY * normalizedY <= 1.0f)
                            {
                                TransitionsSpace[x, y] = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обновляет кэш, необходимый для отрисовки переходов.
        /// </summary>
        public void UpdateTransitionsCache()
        {
            lock (CacheLock)
            {
                FillMaps();

                TransitionsCache?.Dispose();
                TransitionsCache = new Bitmap(Container.Width, Container.Height);

                using (Graphics g = Graphics.FromImage(TransitionsCache))
                {
                    DrawHelper.SetGraphicsParameters(g);
                    DrawTransitions(g);
                }

                TransitionsNeedUpdate = false;
            }
        }

        /// <summary>
        /// Происходит при изменении переходов.
        /// </summary>
        public virtual void OnTransitionsChanged()
        {
            lock (CacheLock)
            {
                TransitionsNeedUpdate = true;
            }
            Container.Invalidate();

            TransitionsChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion





        #region Обработчики событий

        /// <summary>
        /// Вызывает перерисовку контейнера
        /// </summary>
        public void Redraw()
        {
            Container.Invalidate();
        }

        /// <summary>
        /// Обработчик события Paint для <see cref="PictureBox"/> Container.
        /// </summary>
        /// <param name="sender">Объект, инициирующий событие.</param>
        /// <param name="e">Аргументы <see cref="PaintEventArgs"/></param>
        private void Container_Paint(object sender, PaintEventArgs e)
        {
            DrawHelper.SetGraphicsParameters(e.Graphics);

            // Отрисовка состояний
            foreach (var state in StatesAlphabet)
            {
                bool highlightBoundary = ((dynamic)state).IsInBoundaryArea(Container.PointToClient(Cursor.Position)) && !(state == StateBeingMoved) && StateBeingMoved == null;
                bool highlightInner = ((dynamic)state).IsInInnerArea(Container.PointToClient(Cursor.Position)) && !(state == StateBeingMoved) && StateBeingMoved == null;

                ((dynamic)state).Draw(e.Graphics, highlightBoundary, highlightInner);

                if (DeveloperMode)
                    DrawHelper.DrawPoints(e.Graphics, ((dynamic)state).TransitionStartPoints, Color.FromArgb(255, 0, 0, 255));
            }

            // Рисуем кэшированные переходы
            lock (CacheLock)
            {
                if (TransitionsCache == null || TransitionsNeedUpdate)
                {
                    UpdateTransitionsCache();
                }

                if (TransitionsCache != null)
                {
                    e.Graphics.DrawImage(TransitionsCache, 0, 0, TransitionsCache.Width, TransitionsCache.Height);
                }
            }

            if (DeveloperMode)
            {
                Bitmap bitmap = new Bitmap(Container.Width, Container.Height);
                if (StatesSpace != null)
                    DrawHelper.DrawCustomRectangle(e.Graphics, bitmap, StatesSpace, Color.FromArgb(128, 0, 0, 255));
                if (TransitionsSpace != null)
                    DrawHelper.DrawCustomRectangle(e.Graphics, bitmap, TransitionsSpace, Color.FromArgb(128, 255, 0, 0));
            }
        }

        /// <summary>
        /// Обработчик события MouseMove для <see cref="PictureBox"/> Container.
        /// </summary>
        /// <param name="sender">Объект, инициирующий событие.</param>
        /// <param name="e">Аргументы <see cref="MouseEventArgs"/></param>
        private void Container_MouseMove(object sender, MouseEventArgs e)
        {
            LastMousePos = e.Location;
            bool cursorChanged = false;

            foreach (var state in StatesAlphabet)
            {
                if (StateBeingMoved != null)
                {
                    int x = e.X, y = e.Y;
                    if (!StatesSpace[x, y])
                        Container.Cursor = Cursors.No;
                    else
                        Container.Cursor = Cursors.Hand;
                    return;
                }

                if (((dynamic)state).IsInBoundaryArea(e.Location) || ((dynamic)state).IsInInnerArea(e.Location))
                {
                    Container.Cursor = Cursors.Hand;
                    cursorChanged = true;

                    // Начало новой логики для Tooltip'ов
                    if (HoveredState != state)
                    {
                        HoveredState = state;
                        HoverTimer.Stop();
                        HoverTimer.Start();
                    }
                    break;
                }
                else
                {
                    Container.Cursor = Cursors.Default;
                }
            }

            if (!cursorChanged)
            {
                HoveredState = null;
                HoverTimer.Stop();
                ToolTip.Hide(Container);
            }

            Redraw();
        }

        /// <summary>
        /// Обработчик события MouseClick для <see cref="PictureBox"/> Container.
        /// </summary>
        /// <param name="sender">Объект, инициирующий событие.</param>
        /// <param name="e">Аргументы <see cref="EventArgs"/></param>
        private void Container_MouseClick(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (StateBeingMoved != null)
            {
                // Получаем позицию клика
                if (me != null && me.Button == MouseButtons.Left && Container.Cursor != Cursors.No)
                {
                    Point newPosition = me.Location;
                    ((dynamic)StateBeingMoved).SetPosition(newPosition);
                    ((dynamic)StateBeingMoved).IsMoving = false;
                    StateBeingMoved = null;

                    // Перерисовываем контейнер
                    OnTransitionsChanged();
                }
            }

            if (me.Button == MouseButtons.Right && StatesAlphabet != null)
            {
                foreach (var state in StatesAlphabet)
                {
                    if (((dynamic)state).IsInBoundaryArea(me.Location))
                    {
                        ((dynamic)state).ShowContextMenu(me.Location);
                        break;
                    }
                }
            }

            FillMaps();
            AC.Check();
        }

        /// <summary>
        /// Обработчик события MouseDoubleClick для <see cref="PictureBox"/> Container.
        /// </summary>
        /// <param name="sender">Объект, инициирующий событие.</param>
        /// <param name="e">Аргументы <see cref="MouseEventArgs"/></param>
        private void Container_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point clickPoint = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                foreach (var state in StatesAlphabet)
                {
                    if (((dynamic)state).IsInInnerArea(clickPoint))
                    {
                        ((dynamic)state).IsMoving = true;
                        StateBeingMoved = state;
                        // ((dynamic)StateBeingMoved).DrawMovingFrame(Container.CreateGraphics());
                        break;
                    }
                }
            }

            AC.Check();
        }

        /// <summary>
        /// Обработчик события MouseLeave для <see cref="PictureBox"/> Container.
        /// </summary>
        /// <param name="sender">Объект, инициирующий событие.</param>
        /// <param name="e">Аргументы <see cref="EventArgs"/></param>
        private void Container_MouseLeave(object sender, EventArgs e)
        {
            HoveredState = null;
            HoverTimer.Stop();
            // ToolTip.Hide(Container);
        }

        /// <summary>
        /// Обработчик события Tick для <see cref="Timer"/> HoverTimer.
        /// </summary>
        /// <param name="sender">Объект, инициирующий событие.</param>
        /// <param name="e">Аргументы <see cref="EventArgs"/></param>
        private void HoverTimer_Tick(object sender, EventArgs e)
        {
            HoverTimer.Stop();
            if (HoveredState != null)
            {
                Point point = new Point(LastMousePos.X + 15, LastMousePos.Y + 15);
                if (((dynamic)HoveredState).IsInInnerArea(LastMousePos))
                {
                    ToolTip.Show($"\tСостояние \"{((dynamic)HoveredState).Name}\": \nОписание: {((dynamic)HoveredState).UserDefinedText}\n{GetStateIODescription(HoveredState)}" +
                        "\n\nДоступные действия: " +
                        "\n\t1) Двойной клик ЛКМ, чтобы начать перенос состояния, \n\t\tдалее одиночный клик ЛКМ в доступной области для завершения переноса." +
                        "\n\t2) Наведитесь на граничную область состояния, \n\t\tдалее одиночный клик ПКМ для вызова контекстного меню."
                        , Container, point);
                }
                else if (((dynamic)HoveredState).IsInBoundaryArea(LastMousePos))
                {
                    ToolTip.Show("Клик ПКМ для вызова контекстного меню.", Container, point);
                }
            }
        }

        /// <summary>
        /// Формирует описание, связанное с состоянием Автомата.
        /// </summary>
        /// <param name="state">Состояние Автомата, для которого нужно сформировать описание.</param>
        /// <returns></returns>
        private string GetStateIODescription(TState state)
        {
            string result = string.Empty;

            if (state != null && state is MooreAutomatonState mooreState)
            {
                result += "Переходы: ";
                foreach (var kvp in mooreState.Transitions)
                {
                    result += $"\n\tв состояние {kvp.Value.Name} по входу '{kvp.Key}';";
                }

                result += $"\nВыходной сигнал: {mooreState.Output}";
            }

            if (state != null && state is MealyAutomatonState mealyState)
            {
                result += "Переходы: ";
                foreach (var kvp in mealyState.Transitions)
                {
                    result += $"\n\tв состояние {kvp.Value.Name} по входу '{kvp.Key}';";
                }

                result += "\nВыходные сигналы: ";
                foreach (var kvp in mealyState.Outputs)
                {
                    result += $"\n\tпо входу \'{kvp.Key}\' выход: \'{kvp.Value}\';";
                }
            }

            return result;
        }

        #endregion

        #endregion





        #region Сохранение и загрузка в файл

        /// <summary>
        /// Заполняет данные для Контроллера АА.
        /// </summary>
        /// <param name="AC">Объект Контроллера АА.</param>
        protected void FillACData(AutomatonCreator AC)
        {
            List<DescribedObject<object>> inputAlphabet = new List<DescribedObject<object>>();
            List<DescribedObject<object>> outputAlphabet = new List<DescribedObject<object>>();

            for (int i = 0; i < InputAlphabet.Count; i++)
            {
                inputAlphabet.Add(new DescribedObject<object>((char)InputAlphabet[i], InputsDescription?[i]));
            }

            for (int i = 0; i < OutputAlphabet.Count; i++)
            {
                outputAlphabet.Add(new DescribedObject<object>((char)OutputAlphabet[i], OutputsDescription?[i]));
            }

            AC.SetDescriptions(inputAlphabet, outputAlphabet);
            AC.Check();
        }

        /// <summary>
        /// Сохраняет АА в JSON-файл.
        /// </summary>
        /// <param name="filePath">Путь для сохранения.</param>
        public abstract void Save(string filePath);

        /// <summary>
        /// Загружает данные АА из JSON-файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="container"><see cref="PictureBox"/> для отображения АА.</param>
        public abstract void Load(string filePath, PictureBox container);

        #endregion
    }
}