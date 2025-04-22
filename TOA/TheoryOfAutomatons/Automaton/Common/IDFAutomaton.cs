using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheoryOfAutomatons.Automaton.Common;
using TheoryOfAutomatons.Automaton;
using TheoryOfAutomatons.Utils.Helpers;
using System.Drawing;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Collections;

namespace TOA.TheoryOfAutomatons.Automaton.Common
{
    internal interface IDFAutomaton : IDisposable
    {
        #region Поля



        #region Параметры для работы

        /// <summary>
        /// Определяет тип Автомата.
        /// </summary>
        AutomatonType Type { get; }
        /// <summary>
        /// Входной алфавит АА.
        /// </summary>
        List<char> InputAlphabet { get; }
        /// <summary>
        /// Выходной алфавит АА.
        /// </summary>
        List<char> OutputAlphabet { get; }
        /// <summary>
        /// Алфавит состояний АА.
        /// </summary>
        List<IAutomatonState> StatesAlphabet { get; }
        /// <summary>
        /// Функция переходов АА (фактически - инструкция для функции переходов)<br/>
        /// Ставит в соответствие паре (вход, состояние) соответствующее новое состояние из <see cref="StatesAlphabet"/>.
        /// </summary>
        Dictionary<Tuple<char, IAutomatonState>, IAutomatonState> TransitionFunction { get; }
        /// <summary>
        /// Функция выходов АА (фактически - инструкция для функции выходов). <br/>
        /// Ставит в соответствие паре (текущий вход, текущее состояние) соответствующее выходное значение из <see cref="OutputAlphabet"/>.
        /// </summary>
        IDictionary<object, char> OutputFunction { get; }
        /// <summary>
        /// Текущий входной символ.
        /// </summary>
        char CurrentInputSymbol { get; }
        /// <summary>
        /// Текущий выходной символ.
        /// </summary>
        char CurrentOutputSymbol { get; }
       
        #endregion



        #region Параметры для графического отображения АА

        event EventHandler TransitionsChanged;

        /// <summary>
        /// <see cref="PictureBox"/> для отрисовки АА.
        /// </summary>
        PictureBox Container { get; }



        #region Для AutomatonCreator

        /// <summary>
        /// Связанный с данным АА Объект контроллера <see cref="AutomatonCreator"/>. 
        /// </summary>
        AutomatonCreator AC { get; }
        /// <summary>
        /// Список, содержащий описания для входных символов АА.
        /// </summary>
        List<string> InputsDescription { get; }
        /// <summary>
        /// Список, содержащий описания для выходных символов АА.
        /// </summary>
        List<string> OutputsDescription { get; }

        #endregion



        #region Отрисовка состояний

        /// <summary>
        /// Карандаш для отрисовки переходов в неактивном состоянии.
        /// </summary>
        Pen TransitionBlackPen { get; }

        /// <summary>
        /// Карандаш для отрисовки переходов в активном состоянии.
        /// </summary>
        Pen TransitionLightPen { get; }

        /// <summary>
        /// Объект <see cref="System.Windows.Forms.ToolTip"/> для отображения всплывающих подсказок.
        /// </summary>
        ToolTip ToolTip { get; }

        /// <summary>
        /// Объект <see cref="System.Windows.Forms.Timer"/> для задержек отображения всплывающих подсказок.
        /// </summary>
        System.Windows.Forms.Timer HoverTimer { get; }

        /// <summary>
        /// Последняя позиция курсора.
        /// </summary>
        Point LastMousePos { get; }

        #endregion



        #region Отрисовка связей
        List<IAutomatonTransition> Transitions { get; }

        /// <summary>
        /// Булева карта для верного размещения состояний АА.
        /// </summary>
        bool[,] StatesSpace { get; }

        /// <summary>
        /// Булева карта для верной отрисовки переходов АА.
        /// </summary>
        bool[,] TransitionsSpace { get; }

        /// <summary>
        /// Кэш-изображение, используемое для отрисовки переходов АА.
        /// </summary>
        Bitmap TransitionsCache { get; }

        /// <summary>
        /// Определяет, нужно ли обновить переходы.
        /// </summary>
        bool TransitionsNeedUpdate { get; }

        #endregion

        #endregion



        #region Настройки

        /// <summary>
        /// Диаметр круга, который ограничивает область состояния АА.
        /// </summary>
        int CircleDiameter { get; }

        /// <summary>
        /// Ширина границы области состояния АА.
        /// </summary>
        int BorderWidth { get; }

        /// <summary>
        /// Ширина карандаша для активного карандаша.
        /// </summary>
        float TransitionBlackPenWidth { get; }

        /// <summary>
        /// Ширина карандаша для неактивного карандаша.
        /// </summary>
        float TransitionLightPenWidth { get; }

        /// <summary>
        /// Задержка отрисовки каждого шага работы АА в миллисекундах.
        /// </summary>
        int DrawStepDelay { get; }

        /// <summary>
        /// Цвет заливки рабочей области.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color ContainerBackColor { get; }

        /// <summary>
        /// Цвет границы активного состояния.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color ActiveBorderColor { get; }

        /// <summary>
        /// Цвет границы неактивного состояния.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color InactiveBorderColor { get; }

        /// <summary>
        /// Цвет границы выбранного состояния.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color HighlightedBorderColor { get; }

        /// <summary>
        /// Цвет заливки внутреннего состояния.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color InnerStateColor { get; }

        /// <summary>
        /// Цвет активного перехода.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color ActiveTransitionColor { get; }

        /// <summary>
        /// Цвет неактивного перехода.
        /// </summary>
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color InactiveTransitionColor { get; }

        /// <summary>
        /// Нужно ли запретить пересечения переходов при их отрисовке.
        /// </summary>
        bool ProhibitIntersectingTransitions { get; }

        /// <summary>
        /// Включает или отключает режим разработчика.
        /// </summary>
        bool DeveloperMode { get; }

        #endregion



        #endregion





        #region Сеттеры параметров

        /// <summary>
        /// Устанавливает диаметр круга, отображающего состояние АА.
        /// </summary>
        /// <param name="d">Диаметр.</param>
        void SetStateDiameter(int d);

        /// <summary>
        /// Устанавливает ширину границы круга, отображающего состояние АА.
        /// </summary>
        /// <param name="b">Ширина.</param>
        void SetStateBorder(int b);

        /// <summary>
        /// Устанавливает временной шаг задержки отрисовки работы.
        /// </summary>
        /// <param name="t">Время в миллисекундах.</param>
        void SetDrawStepDelay(int t);

        /// <summary>
        /// Устанавливает цвет активной границы состояния.
        /// </summary>
        /// <param name="c">Цвет активной границы.</param>
        void SetActiveBorderColor(Color c);

        /// <summary>
        /// Устанавливает цвет неактивной границы состояния.
        /// </summary>
        /// <param name="c">Цвет неактивной границы.</param>
        void SetInactiveBorderColor(Color c);

        /// <summary>
        /// Устанавливает цвет подсвеченной границы состояния.
        /// </summary>
        /// <param name="c">Цвет подсвеченной границы.</param>
        void SetHighlightedBorderColor(Color c);

        /// <summary>
        /// Устанавливает цвет заливки внутренней части состояния.
        /// </summary>
        /// <param name="c">Цвет заливки внутренней части состояния.</param>
        void SetInnerStateColor(Color c);

        /// <summary>
        /// Устанавливает цвет активного перехода.
        /// </summary>
        /// <param name="c">Цвет активного перехода.</param>
        void SetActiveTransitionColor(Color c);

        /// <summary>
        /// Устанавливает цвет неактивного перехода.
        /// </summary>
        /// <param name="c">Цвет неактивного перехода.</param>
        void SetInactiveTransitionColor(Color c);

        /// <summary>
        /// Устанавливает цвет рабочей области.
        /// </summary>
        /// <param name="c">Цвет рабочей области.</param>
        void SetContainerBackColor(Color c);

        /// <summary>
        /// Обновляет параметры карандаша для неактивных переходов.
        /// </summary>
        void SetTransitionBlackPen(Color color, float width);

        /// <summary>
        /// Обновляет параметры карандаша для активных переходов.
        /// </summary>
        void SetTransitionLightPen(Color color, float width);

        /// <summary>
        /// Устанавливает режим отрисовки переходов.
        /// </summary>
        /// <param name="m">Поддерживаются ли пересекающиеся переходы.</param>
        void SetIntersectionsMode(bool m);

        /// <summary>
        /// Устанавливает режим разработчика.
        /// </summary>
        /// <param name="m">Нужно ли установить режим разработчика.</param>
        void SetDevMode(bool m);

        #endregion










        #region Логика работы АА

        /// <summary>
        /// Проверяет, готов ли АА к обработке входной последовательности.
        /// </summary>
        /// <returns> <see langword="true"/> , если АА не имеет обрыва связей; иначе <see langword="false"/>.</returns>
        bool IsReady();
      
        /// <summary>
        /// Выполняет обработку входной последовательности символов с "подсветкой" текущих активных выходов, состояний и т.п.
        /// </summary>
        /// <param name="inputSequence">Входная последовательность символов.</param>
        /// <returns>Возвращает текст (строку), описывающий обработку входной последовательности.</returns>
        Task<string> ProcessInputSequence(string inputSequence);

        #endregion


        #region Визуализация

        /// <summary>
        /// Добавляет новое состояние.
        /// </summary>
        /// <param name="state">Новое состояние.</param>
        void AddState(IAutomatonState state);

        /// <summary>
        /// Удаляет состояние.
        /// </summary>
        /// <param name="state">Состояние для удаления.</param>
        void DeleteState(IAutomatonState state);

        #region Отрисовка путей

        /// <summary>
        /// Заполняет булевы карты, необходимые для верной отрисовки состояний и переходов.
        /// </summary>
        void FillMaps();

        /// <summary>
        /// Обновляет кэш, необходимый для отрисовки переходов.
        /// </summary>
        void UpdateTransitionsCache();

        /// <summary>
        /// Происходит при изменении переходов.
        /// </summary>
        void OnTransitionsChanged();

        /// <summary>
        /// Вызывает перерисовку контейнера
        /// </summary>
        public void Redraw()
        {
            Container.Invalidate();
        }

        #endregion





        #endregion



        void Save(string filePath);

        /// <summary>
        /// Загружает данные АА из JSON-файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="container"><see cref="PictureBox"/> для отображения АА.</param>
        void Load(string filePath, PictureBox container);
    }
}
