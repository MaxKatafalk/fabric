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
        private TextBox _textBoxOrderNotes;
        private ComboBox _comboBoxSeamstress;
        private ComboBox _comboBoxMaterial;
        private TextBox _textBoxDescription;
        private TextBox _textBoxQuantity;
        private TextBox _textBoxQtyPerUnit;
        private Button _buttonCreateTask;
        private Button _buttonRefresh;
        private Order _selectedOrder;

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
            this.Text = "Мастер";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;

            _listBoxOrders = new ListBox();
            _listBoxOrders.Left = 20;
            _listBoxOrders.Top = 20;
            _listBoxOrders.Width = 420;
            _listBoxOrders.Height = 300;
            _listBoxOrders.SelectedIndexChanged += ListBoxOrders_SelectedIndexChanged;

            Label lblNotes = new Label();
            lblNotes.Text = "Примечание администратора:";
            lblNotes.Left = 20;
            lblNotes.Top = 330;
            lblNotes.Width = 300;

            _textBoxOrderNotes = new TextBox();
            _textBoxOrderNotes.Left = 20;
            _textBoxOrderNotes.Top = 350;
            _textBoxOrderNotes.Width = 420;
            _textBoxOrderNotes.Height = 120;
            _textBoxOrderNotes.Multiline = true;
            _textBoxOrderNotes.ReadOnly = true;

            _buttonRefresh = new Button();
            _buttonRefresh.Text = "Обновить";
            _buttonRefresh.Left = 20;
            _buttonRefresh.Top = 480;
            _buttonRefresh.Width = 120;
            _buttonRefresh.Click += (s, e) =>
            {
                LoadOrders();
                if (_selectedOrder != null)
                {
                    _selectedOrder = _orderService.GetById(_selectedOrder.Id);
                    LoadTasksForOrder();
                    _textBoxOrderNotes.Text = _selectedOrder?.Notes ?? "";
                }
            };

            _listBoxTasks = new ListBox();
            _listBoxTasks.Left = 460;
            _listBoxTasks.Top = 20;
            _listBoxTasks.Width = 500;
            _listBoxTasks.Height = 260;

            _comboBoxSeamstress = new ComboBox();
            _comboBoxSeamstress.Left = 460;
            _comboBoxSeamstress.Top = 300;
            _comboBoxSeamstress.Width = 300;
            _comboBoxSeamstress.DropDownStyle = ComboBoxStyle.DropDownList;

            _comboBoxMaterial = new ComboBox();
            _comboBoxMaterial.Left = 460;
            _comboBoxMaterial.Top = 340;
            _comboBoxMaterial.Width = 200;
            _comboBoxMaterial.DropDownStyle = ComboBoxStyle.DropDownList;

            _textBoxQtyPerUnit = new TextBox();
            _textBoxQtyPerUnit.Left = 670;
            _textBoxQtyPerUnit.Top = 340;
            _textBoxQtyPerUnit.Width = 120;
            _textBoxQtyPerUnit.PlaceholderText = "Материал/ед.";

            _textBoxDescription = new TextBox();
            _textBoxDescription.Left = 460;
            _textBoxDescription.Top = 380;
            _textBoxDescription.Width = 500;
            _textBoxDescription.Height = 60;
            _textBoxDescription.Multiline = true;
            _textBoxDescription.PlaceholderText = "Описание задания";

            _textBoxQuantity = new TextBox();
            _textBoxQuantity.Left = 460;
            _textBoxQuantity.Top = 450;
            _textBoxQuantity.Width = 120;
            _textBoxQuantity.PlaceholderText = "Количество";

            _buttonCreateTask = new Button();
            _buttonCreateTask.Text = "Создать задание";
            _buttonCreateTask.Left = 460;
            _buttonCreateTask.Top = 490;
            _buttonCreateTask.Width = 200;
            _buttonCreateTask.Click += ButtonCreateTask_Click;

            this.Controls.Add(_listBoxOrders);
            this.Controls.Add(lblNotes);
            this.Controls.Add(_textBoxOrderNotes);
            this.Controls.Add(_buttonRefresh);
            this.Controls.Add(_listBoxTasks);
            this.Controls.Add(_comboBoxSeamstress);
            this.Controls.Add(_comboBoxMaterial);
            this.Controls.Add(_textBoxQtyPerUnit);
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

        private void LoadMaterials()
        {
            _comboBoxMaterial.Items.Clear();
            using (var db = new AppDbContext())
            {
                var mats = db.Materials.ToList();
                foreach (var m in mats)
                {
                    _comboBoxMaterial.Items.Add(new ComboboxItem { Text = $"{m.Name} ({m.Quantity} {m.Unit})", Value = m.Id });
                }
            }
            if (_comboBoxMaterial.Items.Count > 0) _comboBoxMaterial.SelectedIndex = 0;
        }

        private void ListBoxOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxOrders.SelectedIndex < 0)
            {
                _selectedOrder = null;
                _listBoxTasks.Items.Clear();
                _textBoxOrderNotes.Clear();
                return;
            }

            string item = _listBoxOrders.Items[_listBoxOrders.SelectedIndex].ToString();
            int id = int.Parse(item.Split(':')[0]);
            _selectedOrder = _orderService.GetById(id);
            _textBoxOrderNotes.Text = _selectedOrder?.Notes ?? "";
            LoadTasksForOrder();
        }

        private void LoadTasksForOrder()
        {
            _listBoxTasks.Items.Clear();
            if (_selectedOrder == null) return;
            var tasks = _taskService.GetTasksByOrder(_selectedOrder.Id);
            foreach (var t in tasks)
            {
                string materialInfo = t.MaterialId.HasValue ? $"Mat:{t.MaterialId} x{t.QuantityPerUnit}" : "";
                _listBoxTasks.Items.Add($"{t.Id}: ToUser:{t.AssignedToUserId} | {t.Description} | {t.QuantityCompleted}/{t.QuantityAssigned} | {t.Status} {materialInfo}");
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

            int? materialId = null;
            decimal qtyPerUnit = 0m;
            if (_comboBoxMaterial.SelectedItem != null)
            {
                var matItem = _comboBoxMaterial.SelectedItem as ComboboxItem;
                materialId = (int)matItem.Value;
                if (!decimal.TryParse(_textBoxQtyPerUnit.Text.Trim(), out qtyPerUnit))
                {
                    MessageBox.Show("Неверно указано количество материала на единицу");
                    return;
                }
            }

            var selected = _comboBoxSeamstress.SelectedItem as ComboboxItem;
            int seamId = (int)selected.Value;
            bool ok = _task_service_create(_selectedOrder.Id, _textBoxDescription.Text.Trim(), qty, seamId, _currentUser.Id, materialId, qtyPerUnit);
            if (ok)
            {
                MessageBox.Show("Задание создано");
                _textBoxDescription.Clear();
                _textBoxQuantity.Clear();
                _textBoxQtyPerUnit.Clear();
                LoadTasksForOrder();
                LoadOrders();
            }
            else
            {
                MessageBox.Show("Ошибка создания задания");
            }
        }

        private bool _task_service_create(int orderId, string description, int qty, int assignedTo, int assignedBy, int? materialId, decimal qtyPerUnit)
        {
            return _taskService.CreateTask(orderId, description, qty, assignedTo, assignedBy, materialId, qtyPerUnit);
        }

        private class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }
    }
}
