using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using fabric.BLL.Services;
using fabric.DAL;
using fabric.DAL.Models;

namespace fabric.Forms
{
    public class MasterForm : Form
    {
        private User _currentUser;
        private OrderService _orderService;
        private ProductionTaskService _taskService;
        private ListBox _listBoxOrders;
        private ListBox _listBoxTasks;
        private TextBox _textBoxOrderNotes;
        private ComboBox _comboBoxSeamstress;
        private ComboBox _comboBoxMaterial;
        private TextBox _textBoxDescription;
        private TextBox _textBoxQuantity;
        private TextBox _textBoxQtyPerUnit;
        private Button _buttonCreateTask;
        private Button _buttonRefresh;
        private Order _selectedOrder;
        private ProductionTask _selectedTask;
        private Panel _panelLeft;
        private Panel _panelRight;
        private Panel _panelTaskDetails;
        private Label _labelOrderTitle;
        private Label _labelTaskTitle;
        private Label _labelNotes;

        public MasterForm(User user)
        {
            _currentUser = user;
            _orderService = new OrderService();
            _taskService = new ProductionTaskService();
            InitializeComponent();
            LoadOrders();
            LoadSeamstresses();
            LoadMaterials();
        }

        private void InitializeComponent()
        {
            // Основные настройки формы
            this.Text = $"Мастер - {_currentUser.FullName}";
            this.Width = 1100;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Левая панель - Заказы
            _panelLeft = new Panel();
            _panelLeft.Left = 20;
            _panelLeft.Top = 20;
            _panelLeft.Width = 400;
            _panelLeft.Height = 640;
            _panelLeft.BackColor = Color.White;
            _panelLeft.BorderStyle = BorderStyle.FixedSingle;
            _panelLeft.Padding = new Padding(10);

            // Заголовок левой панели
            _labelOrderTitle = new Label();
            _labelOrderTitle.Text = "СПИСОК ЗАКАЗОВ";
            _labelOrderTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _labelOrderTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _labelOrderTitle.AutoSize = true;
            _labelOrderTitle.Top = 15;
            _labelOrderTitle.Left = 15;

            // Кнопка обновления
            _buttonRefresh = new Button();
            _buttonRefresh.Text = "⟳ Обновить";
            _buttonRefresh.Left = 250;
            _buttonRefresh.Top = 15;
            _buttonRefresh.Width = 120;
            _buttonRefresh.Height = 30;
            _buttonRefresh.Font = new Font("Segoe UI", 9);
            _buttonRefresh.BackColor = Color.FromArgb(52, 152, 219);
            _buttonRefresh.ForeColor = Color.White;
            _buttonRefresh.FlatStyle = FlatStyle.Flat;
            _buttonRefresh.FlatAppearance.BorderSize = 0;
            _buttonRefresh.Cursor = Cursors.Hand;
            _buttonRefresh.Click += (s, e) =>
            {
                LoadOrders();
                if (_selectedOrder != null)
                {
                    _selectedOrder = _orderService.GetById(_selectedOrder.Id);
                    LoadTasksForOrder();
                    _textBoxOrderNotes.Text = _selectedOrder?.Notes ?? "";
                }
                if (_selectedTask != null)
                {
                    _selectedTask = GetTaskByIdFromDb(_selectedTask.Id);
                }
            };

            // Список заказов
            _listBoxOrders = new ListBox();
            _listBoxOrders.Left = 15;
            _listBoxOrders.Top = 60;
            _listBoxOrders.Width = 370;
            _listBoxOrders.Height = 280;
            _listBoxOrders.Font = new Font("Segoe UI", 9);
            _listBoxOrders.BorderStyle = BorderStyle.FixedSingle;
            _listBoxOrders.SelectionMode = SelectionMode.One;
            _listBoxOrders.SelectedIndexChanged += ListBoxOrders_SelectedIndexChanged;

            // Метка примечаний
            _labelNotes = new Label();
            _labelNotes.Text = "Примечание к заказу:";
            _labelNotes.Left = 15;
            _labelNotes.Top = 350;
            _labelNotes.Width = 200;
            _labelNotes.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _labelNotes.ForeColor = Color.FromArgb(52, 73, 94);

            // Поле примечаний
            _textBoxOrderNotes = new TextBox();
            _textBoxOrderNotes.Left = 15;
            _textBoxOrderNotes.Top = 380;
            _textBoxOrderNotes.Width = 370;
            _textBoxOrderNotes.Height = 120;
            _textBoxOrderNotes.Multiline = true;
            _textBoxOrderNotes.ReadOnly = true;
            _textBoxOrderNotes.Font = new Font("Segoe UI", 9);
            _textBoxOrderNotes.BackColor = Color.FromArgb(248, 249, 250);
            _textBoxOrderNotes.BorderStyle = BorderStyle.FixedSingle;
            _textBoxOrderNotes.ScrollBars = ScrollBars.Vertical;

            // Центральная панель - Задания
            _panelRight = new Panel();
            _panelRight.Left = 440;
            _panelRight.Top = 20;
            _panelRight.Width = 300;
            _panelRight.Height = 640;
            _panelRight.BackColor = Color.White;
            _panelRight.BorderStyle = BorderStyle.FixedSingle;
            _panelRight.Padding = new Padding(10);

            // Заголовок центральной панели
            _labelTaskTitle = new Label();
            _labelTaskTitle.Text = "ЗАДАНИЯ ПО ЗАКАЗУ";
            _labelTaskTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _labelTaskTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _labelTaskTitle.AutoSize = true;
            _labelTaskTitle.Top = 15;
            _labelTaskTitle.Left = 15;

            // Список заданий
            _listBoxTasks = new ListBox();
            _listBoxTasks.Left = 15;
            _listBoxTasks.Top = 60;
            _listBoxTasks.Width = 270;
            _listBoxTasks.Height = 540;
            _listBoxTasks.Font = new Font("Segoe UI", 9);
            _listBoxTasks.BorderStyle = BorderStyle.FixedSingle;
            _listBoxTasks.SelectionMode = SelectionMode.One;
            _listBoxTasks.SelectedIndexChanged += ListBoxTasks_SelectedIndexChanged;

            // Правая панель - Создание задания
            _panelTaskDetails = new Panel();
            _panelTaskDetails.Left = 760;
            _panelTaskDetails.Top = 20;
            _panelTaskDetails.Width = 320;
            _panelTaskDetails.Height = 640;
            _panelTaskDetails.BackColor = Color.White;
            _panelTaskDetails.BorderStyle = BorderStyle.FixedSingle;
            _panelTaskDetails.Padding = new Padding(10);

            // Заголовок правой панели
            Label labelCreateTask = new Label();
            labelCreateTask.Text = "СОЗДАНИЕ ЗАДАНИЯ";
            labelCreateTask.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            labelCreateTask.ForeColor = Color.FromArgb(44, 62, 80);
            labelCreateTask.AutoSize = true;
            labelCreateTask.Top = 15;
            labelCreateTask.Left = 15;

            // Элементы для создания задания
            int yPos = 60;

            // Швея
            Label labelSeamstress = new Label();
            labelSeamstress.Text = "Швея:";
            labelSeamstress.Font = new Font("Segoe UI", 10);
            labelSeamstress.ForeColor = Color.FromArgb(52, 73, 94);
            labelSeamstress.AutoSize = true;
            labelSeamstress.Left = 15;
            labelSeamstress.Top = yPos;

            _comboBoxSeamstress = new ComboBox();
            _comboBoxSeamstress.Left = 15;
            _comboBoxSeamstress.Top = yPos + 25;
            _comboBoxSeamstress.Width = 290;
            _comboBoxSeamstress.Height = 32;
            _comboBoxSeamstress.Font = new Font("Segoe UI", 9);
            _comboBoxSeamstress.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxSeamstress.BackColor = Color.White;
            _comboBoxSeamstress.FlatStyle = FlatStyle.Flat;

            yPos += 70;

            // Материал
            Label labelMaterial = new Label();
            labelMaterial.Text = "Материал (опционально):";
            labelMaterial.Font = new Font("Segoe UI", 10);
            labelMaterial.ForeColor = Color.FromArgb(52, 73, 94);
            labelMaterial.AutoSize = true;
            labelMaterial.Left = 15;
            labelMaterial.Top = yPos;

            _comboBoxMaterial = new ComboBox();
            _comboBoxMaterial.Left = 15;
            _comboBoxMaterial.Top = yPos + 25;
            _comboBoxMaterial.Width = 180;
            _comboBoxMaterial.Height = 32;
            _comboBoxMaterial.Font = new Font("Segoe UI", 9);
            _comboBoxMaterial.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxMaterial.BackColor = Color.White;
            _comboBoxMaterial.FlatStyle = FlatStyle.Flat;

            _textBoxQtyPerUnit = new TextBox();
            _textBoxQtyPerUnit.Left = 205;
            _textBoxQtyPerUnit.Top = yPos + 25;
            _textBoxQtyPerUnit.Width = 100;
            _textBoxQtyPerUnit.Height = 32;
            _textBoxQtyPerUnit.Font = new Font("Segoe UI", 9);
            _textBoxQtyPerUnit.PlaceholderText = "Кол-во на ед.";
            _textBoxQtyPerUnit.BackColor = Color.White;
            _textBoxQtyPerUnit.BorderStyle = BorderStyle.FixedSingle;

            yPos += 70;

            // Описание
            Label labelDescription = new Label();
            labelDescription.Text = "Описание задания:";
            labelDescription.Font = new Font("Segoe UI", 10);
            labelDescription.ForeColor = Color.FromArgb(52, 73, 94);
            labelDescription.AutoSize = true;
            labelDescription.Left = 15;
            labelDescription.Top = yPos;

            _textBoxDescription = new TextBox();
            _textBoxDescription.Left = 15;
            _textBoxDescription.Top = yPos + 25;
            _textBoxDescription.Width = 290;
            _textBoxDescription.Height = 80;
            _textBoxDescription.Multiline = true;
            _textBoxDescription.Font = new Font("Segoe UI", 9);
            _textBoxDescription.PlaceholderText = "Введите описание задания...";
            _textBoxDescription.BackColor = Color.White;
            _textBoxDescription.BorderStyle = BorderStyle.FixedSingle;
            _textBoxDescription.ScrollBars = ScrollBars.Vertical;

            yPos += 120;

            // Количество
            Label labelQuantity = new Label();
            labelQuantity.Text = "Количество изделий:";
            labelQuantity.Font = new Font("Segoe UI", 10);
            labelQuantity.ForeColor = Color.FromArgb(52, 73, 94);
            labelQuantity.AutoSize = true;
            labelQuantity.Left = 15;
            labelQuantity.Top = yPos;

            _textBoxQuantity = new TextBox();
            _textBoxQuantity.Left = 15;
            _textBoxQuantity.Top = yPos + 25;
            _textBoxQuantity.Width = 140;
            _textBoxQuantity.Height = 32;
            _textBoxQuantity.Font = new Font("Segoe UI", 9);
            _textBoxQuantity.PlaceholderText = "Введите число";
            _textBoxQuantity.BackColor = Color.White;
            _textBoxQuantity.BorderStyle = BorderStyle.FixedSingle;

            yPos += 80;

            // Кнопка создания задания
            _buttonCreateTask = new Button();
            _buttonCreateTask.Text = "СОЗДАТЬ ЗАДАНИЕ";
            _buttonCreateTask.Left = 15;
            _buttonCreateTask.Top = yPos;
            _buttonCreateTask.Width = 290;
            _buttonCreateTask.Height = 45;
            _buttonCreateTask.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            _buttonCreateTask.BackColor = Color.FromArgb(46, 204, 113);
            _buttonCreateTask.ForeColor = Color.White;
            _buttonCreateTask.FlatStyle = FlatStyle.Flat;
            _buttonCreateTask.FlatAppearance.BorderSize = 0;
            _buttonCreateTask.Cursor = Cursors.Hand;
            _buttonCreateTask.Click += ButtonCreateTask_Click;

            // Эффекты при наведении
            _buttonCreateTask.MouseEnter += (s, e) => _buttonCreateTask.BackColor = Color.FromArgb(39, 174, 96);
            _buttonCreateTask.MouseLeave += (s, e) => _buttonCreateTask.BackColor = Color.FromArgb(46, 204, 113);

            // Добавление элементов на панели
            _panelLeft.Controls.Add(_labelOrderTitle);
            _panelLeft.Controls.Add(_buttonRefresh);
            _panelLeft.Controls.Add(_listBoxOrders);
            _panelLeft.Controls.Add(_labelNotes);
            _panelLeft.Controls.Add(_textBoxOrderNotes);

            _panelRight.Controls.Add(_labelTaskTitle);
            _panelRight.Controls.Add(_listBoxTasks);

            _panelTaskDetails.Controls.Add(labelCreateTask);
            _panelTaskDetails.Controls.Add(labelSeamstress);
            _panelTaskDetails.Controls.Add(_comboBoxSeamstress);
            _panelTaskDetails.Controls.Add(labelMaterial);
            _panelTaskDetails.Controls.Add(_comboBoxMaterial);
            _panelTaskDetails.Controls.Add(_textBoxQtyPerUnit);
            _panelTaskDetails.Controls.Add(labelDescription);
            _panelTaskDetails.Controls.Add(_textBoxDescription);
            _panelTaskDetails.Controls.Add(labelQuantity);
            _panelTaskDetails.Controls.Add(_textBoxQuantity);
            _panelTaskDetails.Controls.Add(_buttonCreateTask);

            // Добавление панелей на форму
            this.Controls.Add(_panelLeft);
            this.Controls.Add(_panelRight);
            this.Controls.Add(_panelTaskDetails);

            // Обработка клавиш
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadOrders();
                    LoadSeamstresses();
                    LoadMaterials();
                }
            };
        }

        private void LoadOrders()
        {
            _listBoxOrders.Items.Clear();
            var list = _orderService.GetAll();

            if (list == null || !list.Any())
            {
                _listBoxOrders.Items.Add("Нет доступных заказов");
                return;
            }

            foreach (var o in list)
            {
                string status = GetOrderStatusRussian(o.Status.ToString());
                _listBoxOrders.Items.Add($"[{o.Id}] {o.OrderNumber} | {o.CustomerName} | {status}");
            }
        }

        private string GetOrderStatusRussian(string status)
        {
            // Универсальная конвертация статусов
            status = status.ToLower();

            if (status.Contains("pending") || status.Contains("ожидан"))
                return "В ожидании";
            else if (status.Contains("assigned") || status.Contains("назначен"))
                return "Назначен";
            else if (status.Contains("progress") || status.Contains("работе") || status.Contains("выполнен"))
                return "В работе";
            else if (status.Contains("completed") || status.Contains("завершен"))
                return "Завершен";
            else
                return status;
        }

        private string GetTaskStatusRussian(string status)
        {
            // Универсальная конвертация статусов
            status = status.ToLower();

            if (status.Contains("pending") || status.Contains("ожидан"))
                return "В ожидании";
            else if (status.Contains("assigned") || status.Contains("назначен"))
                return "Назначен";
            else if (status.Contains("progress") || status.Contains("работе") || status.Contains("выполнен"))
                return "В работе";
            else if (status.Contains("completed") || status.Contains("завершен"))
                return "Завершено";
            else
                return status;
        }

        private void LoadSeamstresses()
        {
            _comboBoxSeamstress.Items.Clear();
            using (var db = new AppDbContext())
            {
                var users = db.Users.Where(u => u.RoleId == 2 && u.IsActive).ToList();
                foreach (var u in users)
                {
                    _comboBoxSeamstress.Items.Add(new ComboboxItem { Text = $"{u.FullName} ({u.Username})", Value = u.Id });
                }
            }
            if (_comboBoxSeamstress.Items.Count > 0) _comboBoxSeamstress.SelectedIndex = 0;
        }

        private void LoadMaterials()
        {
            _comboBoxMaterial.Items.Clear();
            _comboBoxMaterial.Items.Add(new ComboboxItem { Text = "Без материала", Value = 0 });

            using (var db = new AppDbContext())
            {
                var mats = db.Materials.Where(m => m.Quantity > 0).ToList();
                foreach (var m in mats)
                {
                    _comboBoxMaterial.Items.Add(new ComboboxItem { Text = $"{m.Name} ({m.Quantity} {m.Unit})", Value = m.Id });
                }
            }
            if (_comboBoxMaterial.Items.Count > 0) _comboBoxMaterial.SelectedIndex = 0;
        }

        private void ListBoxOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxOrders.SelectedIndex < 0 || string.IsNullOrEmpty(_listBoxOrders.SelectedItem.ToString()))
            {
                _selectedOrder = null;
                _listBoxTasks.Items.Clear();
                _textBoxOrderNotes.Clear();
                _buttonCreateTask.Enabled = false;
                return;
            }

            string item = _listBoxOrders.Items[_listBoxOrders.SelectedIndex].ToString();

            if (item.Contains("[") && item.Contains("]"))
            {
                try
                {
                    int start = item.IndexOf('[') + 1;
                    int end = item.IndexOf(']');
                    int id = int.Parse(item.Substring(start, end - start));
                    _selectedOrder = _orderService.GetById(id);
                    _textBoxOrderNotes.Text = _selectedOrder?.Notes ?? "";
                    LoadTasksForOrder();
                    _buttonCreateTask.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выборе заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ListBoxTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxTasks.SelectedIndex < 0 || _selectedOrder == null || string.IsNullOrEmpty(_listBoxTasks.SelectedItem.ToString()))
            {
                _selectedTask = null;
                return;
            }

            string item = _listBoxTasks.Items[_listBoxTasks.SelectedIndex].ToString();

            if (item.Contains("[") && item.Contains("]") && !item.Contains("==="))
            {
                try
                {
                    int start = item.IndexOf('[') + 1;
                    int end = item.IndexOf(']');
                    int taskId = int.Parse(item.Substring(start, end - start));
                    _selectedTask = GetTaskByIdFromDb(taskId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выборе задания: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private ProductionTask GetTaskByIdFromDb(int taskId)
        {
            using (var db = new AppDbContext())
            {
                return db.ProductionTasks.FirstOrDefault(t => t.Id == taskId);
            }
        }

        private void LoadTasksForOrder()
        {
            _listBoxTasks.Items.Clear();
            _selectedTask = null;

            if (_selectedOrder == null) return;

            var tasks = _taskService.GetTasksByOrder(_selectedOrder.Id);

            if (tasks == null || !tasks.Any())
            {
                _listBoxTasks.Items.Add("Нет заданий по этому заказу");
                return;
            }

            foreach (var t in tasks)
            {
                string assignedTo = GetUserName(t.AssignedToUserId);
                string status = GetTaskStatusRussian(t.Status.ToString());

                string progress = $"{t.QuantityCompleted}/{t.QuantityAssigned}";
                _listBoxTasks.Items.Add($"[{t.Id}] {t.Description} | Швея: {assignedTo} | {progress} шт. | {status}");
            }
        }

        private string GetUserName(int userId)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                return user?.FullName ?? $"ID:{userId}";
            }
        }

        private void ButtonCreateTask_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ из списка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_comboBoxSeamstress.SelectedItem == null)
            {
                MessageBox.Show("Выберите швею", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_textBoxDescription.Text))
            {
                MessageBox.Show("Введите описание задания", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBoxDescription.Focus();
                return;
            }

            if (!int.TryParse(_textBoxQuantity.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Введите корректное количество изделий", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBoxQuantity.Focus();
                return;
            }

            int? materialId = null;
            decimal qtyPerUnit = 0m;

            var materialItem = _comboBoxMaterial.SelectedItem as ComboboxItem;
            if (materialItem != null && (int)materialItem.Value != 0)
            {
                materialId = (int)materialItem.Value;
                if (!decimal.TryParse(_textBoxQtyPerUnit.Text.Trim(), out qtyPerUnit) || qtyPerUnit <= 0)
                {
                    MessageBox.Show("Введите корректное количество материала на единицу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _textBoxQtyPerUnit.Focus();
                    return;
                }
            }

            var seamstressItem = _comboBoxSeamstress.SelectedItem as ComboboxItem;
            int seamstressId = (int)seamstressItem.Value;

            bool ok = _taskService.CreateTask(
                _selectedOrder.Id,
                _textBoxDescription.Text.Trim(),
                qty,
                seamstressId,
                _currentUser.Id,
                materialId,
                qtyPerUnit
            );

            if (ok)
            {
                MessageBox.Show("Задание успешно создано", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Сброс полей
                _textBoxDescription.Clear();
                _textBoxQuantity.Clear();
                _textBoxQtyPerUnit.Clear();
                if (_comboBoxMaterial.Items.Count > 0) _comboBoxMaterial.SelectedIndex = 0;

                // Обновление списков
                LoadTasksForOrder();
                LoadOrders();
            }
            else
            {
                MessageBox.Show("Ошибка при создании задания. Проверьте данные и повторите попытку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }
    }
}