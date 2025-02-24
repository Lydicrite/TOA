using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheoryOfAutomatons.Utils.Containers
{
    /// <summary>
    /// Представляет приоритетную очередь, которая хранит элементы с ассоциированными приоритетами.
    /// <br></br>Элементы с более высоким приоритетом обрабатываются первыми.
    /// </summary>
    /// <typeparam name="TElement">Тип элементов, хранящихся в очереди.</typeparam>
    /// <typeparam name="TPriority">Тип приоритетов элементов.</typeparam>
    internal class PriorityQueue<TElement, TPriority> : IEnumerable<TElement>
    {
        /// <summary>
        /// Хранит элементы очереди вместе с их приоритетами в виде списка.
        /// </summary>
        private List<(TElement Element, TPriority Priority)> _heap;

        /// <summary>
        /// Компаратор для сравнения приоритетов элементов.
        /// </summary>
        private readonly IComparer<TPriority> _comparer;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PriorityQueue{TElement, TPriority}"/> с компаратором по умолчанию.
        /// </summary>
        public PriorityQueue() : this(null) { }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PriorityQueue{TElement, TPriority}"/> с заданным компаратором.
        /// </summary>
        /// <param name="comparer">Компаратор для сравнения приоритетов. Если null, используется компаратор по умолчанию.</param>
        public PriorityQueue(IComparer<TPriority> comparer)
        {
            _heap = new List<(TElement, TPriority)>();
            _comparer = comparer ?? Comparer<TPriority>.Default;
        }

        /// <summary>
        /// Получает количество элементов в очереди.
        /// </summary>
        public int Count => _heap.Count;

        /// <summary>
        /// Добавляет элемент с указанным приоритетом в очередь.
        /// </summary>
        /// <param name="element">Элемент для добавления.</param>
        /// <param name="priority">Приоритет элемента.</param>
        public void Enqueue(TElement element, TPriority priority)
        {
            _heap.Add((element, priority));
            HeapifyUp(_heap.Count - 1);
        }

        /// <summary>
        /// Удаляет и возвращает элемент с наивысшим приоритетом.
        /// </summary>
        /// <returns>Элемент с наивысшим приоритетом.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если очередь пуста.</exception>
        public TElement Dequeue()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Очередь пуста.");

            var root = _heap[0].Element;
            var last = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);
            if (_heap.Count > 0)
            {
                _heap[0] = last;
                HeapifyDown(0);
            }
            return root;
        }

        /// <summary>
        /// Возвращает элемент с наивысшим приоритетом без его удаления из очереди.
        /// </summary>
        /// <returns>Элемент с наивысшим приоритетом.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если очередь пуста.</exception>
        public TElement Peek()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Очередь пуста.");

            return _heap[0].Element;
        }

        /// <summary>
        /// Проверяет, содержит ли очередь указанный элемент.
        /// </summary>
        /// <param name="element">Элемент для поиска.</param>
        /// <returns>True, если элемент содержится в очереди; иначе False.</returns>
        public bool Contains(TElement element)
        {
            foreach (var item in _heap)
            {
                if (EqualityComparer<TElement>.Default.Equals(item.Element, element))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Очищает все элементы из очереди.
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
        }

        /// <summary>
        /// Изменяет приоритет указанного элемента.
        /// Если элемент существует, его приоритет обновляется и куча перестраивается.
        /// </summary>
        /// <param name="element">Элемент, приоритет которого необходимо изменить.</param>
        /// <param name="newPriority">Новый приоритет элемента.</param>
        /// <returns>True, если приоритет успешно изменен; иначе False.</returns>
        public bool ChangePriority(TElement element, TPriority newPriority)
        {
            for (int i = 0; i < _heap.Count; i++)
            {
                if (EqualityComparer<TElement>.Default.Equals(_heap[i].Element, element))
                {
                    var oldPriority = _heap[i].Priority;
                    _heap[i] = (element, newPriority);
                    int comparison = _comparer.Compare(newPriority, oldPriority);
                    if (comparison < 0)
                    {
                        HeapifyUp(i);
                    }
                    else if (comparison > 0)
                    {
                        HeapifyDown(i);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Перестраивает кучу вверх от указанного индекса для сохранения свойства кучи.
        /// </summary>
        /// <param name="index">Индекс элемента, с которого начинается перестройка.</param>
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_comparer.Compare(_heap[index].Priority, _heap[parent].Priority) < 0)
                {
                    Swap(index, parent);
                    index = parent;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Перестраивает кучу вниз от указанного индекса для сохранения свойства кучи.
        /// </summary>
        /// <param name="index">Индекс элемента, с которого начинается перестройка.</param>
        private void HeapifyDown(int index)
        {
            int lastIndex = _heap.Count - 1;
            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;

                if (leftChild <= lastIndex && _comparer.Compare(_heap[leftChild].Priority, _heap[smallest].Priority) < 0)
                {
                    smallest = leftChild;
                }

                if (rightChild <= lastIndex && _comparer.Compare(_heap[rightChild].Priority, _heap[smallest].Priority) < 0)
                {
                    smallest = rightChild;
                }

                if (smallest != index)
                {
                    Swap(index, smallest);
                    index = smallest;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Обменивает элементы в куче по указанным индексам.
        /// </summary>
        /// <param name="i">Индекс первого элемента.</param>
        /// <param name="j">Индекс второго элемента.</param>
        private void Swap(int i, int j)
        {
            var temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;
        }

        /// <summary>
        /// Возвращает перечислитель, который можно использовать для перебора элементов очереди.
        /// </summary>
        /// <returns>Перечислитель элементов очереди.</returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            foreach (var item in _heap)
            {
                yield return item.Element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
