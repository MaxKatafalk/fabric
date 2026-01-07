using System;
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
        private ComboBox _comboBoxSeamstress;
        private TextBox _textBoxDescription;
        private TextBox _textBoxQuantity;
        private Button _buttonCreateTask;
        private Order _selectedOrder;

        public MasterForm(User user)
        {
            _currentUser = user;
            _orderService = new OrderService();
            _taskService = new ProductionTaskService();
            InitializeComponent();
            LoadOrders();
            LoadSeamstresses();
        }

        private void InitializeComponent()
        {
            this.Text = "Мастер";
            this.Width = 900;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;

            _listBoxOrders = new ListBox();
            _listBoxOrders.Left = 20;
            _listBoxOrders.Top = 20;
            _listBoxOrders.Width = 420;
            _listBoxOrders.Height = 520;
            _listBoxOrders.SelectedIndexChanged += ListBoxOrders_SelectedIndexChanged;

            _listBoxTasks = new ListBox();
            _listBoxTasks.Left = 460;
            _listBoxTasks.Top = 20;
            _listBoxTasks.Width = 400;
            _listBoxTasks.Height = 260;

            _comboBoxSeamstress = new ComboBox();
            _comboBoxSeamstress.Left = 460;
            _comboBoxSeamstress.Top = 300;
            _comboBoxSeamstress.Width = 240;
            _comboBoxSeamstress.DropDownStyle = ComboBoxStyle.DropDownList;

            _textBoxDescription = new TextBox();
            _textBoxDescription.Left = 460;
            _textBoxDescription.Top = 340;
            _textBoxDescription.Width = 400;
            _textBoxDescription.Height = 60;
            _textBoxDescription.Multiline = true;
            _textBoxDescription.PlaceholderText = "Описание задания";

            _textBoxQuantity = new TextBox();
            _textBoxQuantity.Left = 460;
            _textBoxQuantity.Top = 410;
            _textBoxQuantity.Width = 120;
            _textBoxQuantity.PlaceholderText = "Количество";

            _buttonCreateTask = new Button();
            _buttonCreateTask.Text = "Создать задание";
            _buttonCreateTask.Left = 460;
            _buttonCreateTask.Top = 450;
            _buttonCreateTask.Width = 200;
            _buttonCreateTask.Click += ButtonCreateTask_Click;

            this.Controls.Add(_listBoxOrders);
            this.Controls.Add(_listBoxTasks);
            this.Controls.Add(_comboBoxSeamstress);
            this.Controls.Add(_textBoxDescription);
            this.Controls.Add(_textBoxQuantity);
            this.Controls.Add(_buttonCreateTask);
        }

        private void LoadOrders()
        {
            _listBoxOrders.Items.Clear();
            var list = _orderService.GetAll();
            foreach (var o in list)
            {
                _listBoxOrders.Items.Add($"{o.Id}: {o.OrderNumber} | {o.CustomerName} | {o.Status}");
            }
        }

        private void LoadSeamstresses()
        {
            _comboBoxSeamstress.Items.Clear();
            using (var db = new AppDbContext())
            {
                var users = db.Users.Where(u => u.RoleId == 2 && u.IsActive).ToList();
                foreach (var u in users)
                {
                    _comboBoxSeamstress.Items.Add(new ComboboxItem { Text = $"{u.Username} ({u.FullName})", Value = u.Id });
                }
            }
            if (_comboBoxSeamstress.Items.Count > 0) _comboBoxSeamstress.SelectedIndex = 0;
        }

        private void ListBoxOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxOrders.SelectedIndex < 0)
            {
                _selectedOrder = null;
                _listBoxTasks.Items.Clear();
                return;
            }

            string item = _listBoxOrders.Items[_listBoxOrders.SelectedIndex].ToString();
            int id = int.Parse(item.Split(':')[0]);
            _selectedOrder = _orderService.GetById(id);
            LoadTasksForOrder();
        }

        private void LoadTasksForOrder()
        {
            _listBoxTasks.Items.Clear();
            if (_selectedOrder == null) return;
            var tasks = _taskService.GetTasksByOrder(_selectedOrder.Id);
            foreach (var t in tasks)
            {
                _listBoxTasks.Items.Add($"{t.Id}: ToUser:{t.AssignedToUserId} | {t.Description} | {t.QuantityCompleted}/{t.QuantityAssigned} | {t.Status}");
            }
        }

        private void ButtonCreateTask_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            if (_comboBoxSeamstress.SelectedItem == null)
            {
                MessageBox.Show("Выберите швею");
                return;
            }

            if (!int.TryParse(_textBoxQuantity.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Неверное количество");
                return;
            }

            var selected = _comboBoxSeamstress.SelectedItem as ComboboxItem;
            int seamId = (int)selected.Value;
            bool ok = _taskService.CreateTask(_selectedOrder.Id, _textBoxDescription.Text.Trim(), qty, seamId, _currentUser.Id);
            if (ok)
            {
                MessageBox.Show("Задание создано");
                _textBoxDescription.Clear();
                _textBoxQuantity.Clear();
                LoadTasksForOrder();
            }
            else
            {
                MessageBox.Show("Ошибка создания задания");
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
