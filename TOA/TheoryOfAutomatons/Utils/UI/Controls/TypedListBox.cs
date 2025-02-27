using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using TheoryOfAutomatons.Utils.Containers;
using TheoryOfAutomatons.Utils.UI.Forms.Adders;

namespace TheoryOfAutomatons.Utils.UI.Controls
{
    internal enum SupportedTypes
    {
        Int,
        Char,
        String
    }

    internal partial class TypedListBox : UserControl
    {
        private ListBox listBox;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem addMenuItem;
        private ToolStripMenuItem removeMenuItem;
        private System.Windows.Forms.ToolTip toolTip;

        // Свойство для выбора типа T через перечисление SupportedTypes
        private SupportedTypes supportedType = SupportedTypes.Char;

        [Category("Данные")]
        [Description("Определяет тип данных для хранимых в элементе Value.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SupportedTypes SupportedType
        {
            get { return supportedType; }
            set
            {
                supportedType = value;
                InitializeDataSource();
            }
        }

        // Источник данных
        private List<DescribedObject<object>> dataSource;

        public TypedListBox()
        {
            InitializeComponent();
            InitializeListBox();
            InitializeContextMenu();
            InitializeDataSource();
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = "ToString";
        }

        private void InitializeContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            addMenuItem = new ToolStripMenuItem("Добавить");
            removeMenuItem = new ToolStripMenuItem("Удалить");

            addMenuItem.Click += AddMenuItem_Click;
            removeMenuItem.Click += RemoveMenuItem_Click;

            contextMenu.Items.AddRange(new ToolStripItem[] { addMenuItem, removeMenuItem });
            listBox.ContextMenuStrip = contextMenu;
        }

        private void InitializeDataSource()
        {
            // Инициализируем источник данных в зависимости от выбранного типа
            dataSource = new List<DescribedObject<object>>();
            listBox.DataSource = dataSource;
            listBox.DisplayMember = "ToString";
            listBox.ClearSelected();
        }

        private void AddMenuItem_Click(object sender, EventArgs e)
        {
            using (AddItemForm form = new AddItemForm(supportedType))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    DescribedObject<object> newItem = new DescribedObject<object>(form.Value, form.Description);
                    dataSource.Add(newItem);
                    RefreshListBox();
                    OnItemAdded(new ItemAddedEventArgs(newItem));
                }
            }
        }

        private void RemoveMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem is DescribedObject<object> selectedItem)
            {
                var result = MessageBox.Show("Вы действительно хотите удалить выбранный элемент?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    dataSource.Remove(selectedItem);
                    RefreshListBox();
                    OnItemRemoved(new ItemRemovedEventArgs(selectedItem));
                }
            }
        }

        public void RefreshListBox()
        {
            listBox.DataSource = null;
            listBox.DataSource = dataSource;
            listBox.DisplayMember = "ToString";
            listBox.ClearSelected();
        }

        // Метод для получения данных
        public List<DescribedObject<object>> GetData()
        {
            return dataSource;
        }

        // Метод для установки данных
        public void SetData(List<DescribedObject<object>> data)
        {
            dataSource = new List<DescribedObject<object>>();
            dataSource.Capacity = data.Count;
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                dataSource.Add(item);
                OnItemAdded(new ItemAddedEventArgs(item));
            }
            RefreshListBox();
        }




        #region События

        public event EventHandler<ItemAddedEventArgs> ItemAdded;
        public event EventHandler<ItemRemovedEventArgs> ItemRemoved;

        protected virtual void OnItemAdded(ItemAddedEventArgs e)
        {
            ItemAdded?.Invoke(this, e);
            listBox.ClearSelected();
        }
        protected virtual void OnItemRemoved(ItemRemovedEventArgs e)
        {
            ItemRemoved?.Invoke(this, e);
            listBox.ClearSelected();
        }

        // Класс для аргументов события добавления элемента
        public class ItemAddedEventArgs : EventArgs
        {
            public DescribedObject<object> AddedItem { get; }

            public ItemAddedEventArgs(DescribedObject<object> addedItem)
            {
                AddedItem = addedItem;
            }
        }

        // Класс для аргументов события удаления элемента
        public class ItemRemovedEventArgs : EventArgs
        {
            public DescribedObject<object> RemovedItem { get; }

            public ItemRemovedEventArgs(DescribedObject<object> removedItem)
            {
                RemovedItem = removedItem;
            }
        }
        #endregion



        #region Код, автоматически созданный конструктором компонентов

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

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listBox = new System.Windows.Forms.ListBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.ForeColor = System.Drawing.Color.Gainsboro;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(120, 96);
            this.listBox.TabIndex = 0;
            this.toolTip.SetToolTip(this.listBox, "Нажмите правой кнопкой мыши для вызова меню.\r\n\r\n\"Добавить\" - добавляет новую запи" +
        "сь.\r\n\"Удалить\" - удаляет выбранную запись. Для выбора записи нажмите по ней лево" +
        "й кнопкой мыши.\r\n");
            // 
            // TypedListBox
            // 
            this.Controls.Add(this.listBox);
            this.Name = "TypedListBox";
            this.Size = new System.Drawing.Size(120, 96);
            this.toolTip.SetToolTip(this, "Нажмите правой кнопкой мыши для вызова меню.\r\n\r\n\"Добавить\" - добавляет новую запи" +
        "сь.\r\n\"Удалить\" - удаляет выбранную запись. Для выбора записи нажмите по ней лево" +
        "й кнопкой мыши.");
            this.ResumeLayout(false);

        }
        #endregion
    }
}