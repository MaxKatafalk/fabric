using System;
using System.Linq;
using System.Windows.Forms;
using fabric.BLL.Services;
using fabric.DAL.Models;

namespace fabric.Forms
{
    public class OrderForm : Form
    {
        private User _currentUser;
        private OrderService _orderService;
        private ListBox _listBoxOrders;
        private TextBox _textBoxOrderNumber;
        private TextBox _textBoxCustomer;
        private DateTimePicker _datePickerOrder;
        private DateTimePicker _datePickerDue;
        private TextBox _textBoxQuantity;
        private TextBox _textBoxNotes;
        private Button _buttonCreate;
        private ComboBox _comboBoxStatus;
        private Button _buttonUpdateStatus;
        private Order _selectedOrder;

        public OrderForm(User user)
        {
            _currentUser = user;
            _orderService = new OrderService();
            InitializeComponent();
            LoadOrders();
        }

        private void InitializeComponent()
        {
            this.Text = "Заказы";
            this.Width = 900;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;

            _listBoxOrders = new ListBox();
            _listBoxOrders.Left = 20;
            _listBoxOrders.Top = 20;
            _listBoxOrders.Width = 520;
            _listBoxOrders.Height = 520;
            _listBoxOrders.SelectedIndexChanged += ListBoxOrders_SelectedIndexChanged;

            Label l1 = new Label();
            l1.Text = "Номер заказа";
            l1.Left = 560;
            l1.Top = 20;
            _textBoxOrderNumber = new TextBox();
            _textBoxOrderNumber.Left = 560;
            _textBoxOrderNumber.Top = 40;
            _textBoxOrderNumber.Width = 300;

            Label l2 = new Label();
            l2.Text = "Клиент";
            l2.Left = 560;
            l2.Top = 80;
            _textBoxCustomer = new TextBox();
            _textBoxCustomer.Left = 560;
            _textBoxCustomer.Top = 100;
            _textBoxCustomer.Width = 300;

            Label l3 = new Label();
            l3.Text = "Дата заказа";
            l3.Left = 560;
            l3.Top = 140;
            _datePickerOrder = new DateTimePicker();
            _datePickerOrder.Left = 560;
            _datePickerOrder.Top = 160;
            _datePickerOrder.Width = 300;

            Label l4 = new Label();
            l4.Text = "Срок (дата)";
            l4.Left = 560;
            l4.Top = 200;
            _datePickerDue = new DateTimePicker();
            _datePickerDue.Left = 560;
            _datePickerDue.Top = 220;
            _datePickerDue.Width = 300;
            _datePickerDue.Format = DateTimePickerFormat.Short;

            Label l5 = new Label();
            l5.Text = "Количество";
            l5.Left = 560;
            l5.Top = 260;
            _textBoxQuantity = new TextBox();
            _textBoxQuantity.Left = 560;
            _textBoxQuantity.Top = 280;
            _textBoxQuantity.Width = 300;

            Label l6 = new Label();
            l6.Text = "Примечание";
            l6.Left = 560;
            l6.Top = 320;
            _textBoxNotes = new TextBox();
            _textBoxNotes.Left = 560;
            _textBoxNotes.Top = 340;
            _textBoxNotes.Width = 300;
            _textBoxNotes.Height = 80;
            _textBoxNotes.Multiline = true;

            _buttonCreate = new Button();
            _buttonCreate.Text = "Создать заказ";
            _buttonCreate.Left = 560;
            _buttonCreate.Top = 440;
            _buttonCreate.Width = 300;
            _buttonCreate.Click += ButtonCreate_Click;

            _comboBoxStatus = new ComboBox();
            _comboBoxStatus.Left = 560;
            _comboBoxStatus.Top = 500;
            _comboBoxStatus.Width = 200;
            _comboBoxStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxStatus.Items.AddRange(new string[] { "Created", "InProgress", "Ready", "Closed" });
            _comboBoxStatus.SelectedIndex = 0;

            _buttonUpdateStatus = new Button();
            _buttonUpdateStatus.Text = "Обновить статус";
            _buttonUpdateStatus.Left = 770;
            _buttonUpdateStatus.Top = 500;
            _buttonUpdateStatus.Width = 90;
            _buttonUpdateStatus.Click += ButtonUpdateStatus_Click;

            this.Controls.Add(_listBoxOrders);
            this.Controls.Add(l1);
            this.Controls.Add(_textBoxOrderNumber);
            this.Controls.Add(l2);
            this.Controls.Add(_textBoxCustomer);
            this.Controls.Add(l3);
            this.Controls.Add(_datePickerOrder);
            this.Controls.Add(l4);
            this.Controls.Add(_datePickerDue);
            this.Controls.Add(l5);
            this.Controls.Add(_textBoxQuantity);
            this.Controls.Add(l6);
            this.Controls.Add(_textBoxNotes);
            this.Controls.Add(_buttonCreate);
            this.Controls.Add(_comboBoxStatus);
            this.Controls.Add(_buttonUpdateStatus);
        }

        private void LoadOrders()
        {
            _listBoxOrders.Items.Clear();
            var list = _orderService.GetAll();
            foreach (var o in list)
            {
                _listBoxOrders.Items.Add($"{o.Id}: {o.OrderNumber} | {o.CustomerName} | {o.Status} | {o.OrderDate.ToShortDateString()} | {o.TotalQuantity}");
            }
        }

        private void ButtonCreate_Click(object sender, EventArgs e)
        {
            string orderNumber = _textBoxOrderNumber.Text.Trim();
            string customer = _textBoxCustomer.Text.Trim();
            if (!int.TryParse(_textBoxQuantity.Text.Trim(), out int qty))
            {
                MessageBox.Show("Неверное количество");
                return;
            }

            DateTime orderDate = _datePickerOrder.Value;
            DateTime? due = _datePickerDue.Value;
            bool ok = _orderService.CreateOrder(orderNumber, customer, orderDate, due, qty, _textBoxNotes.Text.Trim(), _currentUser?.Id);
            if (ok)
            {
                MessageBox.Show("Заказ создан");
                _textBoxOrderNumber.Clear();
                _textBoxCustomer.Clear();
                _textBoxQuantity.Clear();
                _textBoxNotes.Clear();
                LoadOrders();
            }
            else
            {
                MessageBox.Show("Ошибка создания заказа");
            }
        }

        private void ListBoxOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxOrders.SelectedIndex < 0)
            {
                _selectedOrder = null;
                return;
            }

            string item = _listBoxOrders.Items[_listBoxOrders.SelectedIndex].ToString();
            int id = int.Parse(item.Split(':')[0]);
            _selectedOrder = _orderService.GetById(id);
            if (_selectedOrder != null)
            {
                _textBoxOrderNumber.Text = _selectedOrder.OrderNumber;
                _textBoxCustomer.Text = _selectedOrder.CustomerName;
                _datePickerOrder.Value = _selectedOrder.OrderDate;
                _datePickerDue.Value = _selectedOrder.DueDate ?? DateTime.Now;
                _textBoxQuantity.Text = _selectedOrder.TotalQuantity.ToString();
                _textBoxNotes.Text = _selectedOrder.Notes;
                _comboBoxStatus.SelectedIndex = (int)_selectedOrder.Status - 1;
            }
        }

        private void ButtonUpdateStatus_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            int idx = _comboBoxStatus.SelectedIndex;
            OrderStatus status = (OrderStatus)(idx + 1);
            bool ok = _orderService.UpdateStatus(_selectedOrder.Id, status);
            if (ok)
            {
                MessageBox.Show("Статус обновлён");
                LoadOrders();
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении статуса");
            }
        }
    }
}
