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

        private Order _selectedOrder;

        public MasterForm(User user)
        {
            _currentUser = user;
            _orderService = new OrderService();
            _taskService = new ProductionTaskService();

            InitializeComponent();
            LoadOrders();

            AppEvents.OrderStatusChanged += OnOrderStatusChanged;
            this.FormClosed += (s, e) => AppEvents.OrderStatusChanged -= OnOrderStatusChanged;

            LoadSeamstresses();
            LoadMaterials();
        }

        private void InitializeComponent()
        {
            this.Text = "Мастер";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;

            _listBoxOrders = new ListBox
            {
                Left = 20,
                Top = 20,
                Width = 420,
                Height = 300
            };
            _listBoxOrders.SelectedIndexChanged += ListBoxOrders_SelectedIndexChanged;

            Label lblNotes = new Label
            {
                Text = "Примечание администратора:",
                Left = 20,
                Top = 330,
                Width = 300
            };

            _textBoxOrderNotes = new TextBox
            {
                Left = 20,
                Top = 350,
                Width = 420,
                Height = 120,
                Multiline = true,
                ReadOnly = true
            };

            _listBoxTasks = new ListBox
            {
                Left = 460,
                Top = 20,
                Width = 500,
                Height = 260
            };

            _comboBoxSeamstress = new ComboBox
            {
                Left = 460,
                Top = 300,
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _comboBoxMaterial = new ComboBox
            {
                Left = 460,
                Top = 340,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _textBoxQtyPerUnit = new TextBox
            {
                Left = 670,
                Top = 340,
                Width = 120,
                PlaceholderText = "Материал/ед."
            };

            _textBoxDescription = new TextBox
            {
                Left = 460,
                Top = 380,
                Width = 500,
                Height = 60,
                Multiline = true,
                PlaceholderText = "Описание задания"
            };

            _textBoxQuantity = new TextBox
            {
                Left = 460,
                Top = 450,
                Width = 120,
                PlaceholderText = "Количество"
            };

            _buttonCreateTask = new Button
            {
                Text = "Создать задание",
                Left = 460,
                Top = 490,
                Width = 200
            };
            _buttonCreateTask.Click += ButtonCreateTask_Click;

            this.Controls.AddRange(new Control[]
            {
                _listBoxOrders,
                lblNotes,
                _textBoxOrderNotes,
                _listBoxTasks,
                _comboBoxSeamstress,
                _comboBoxMaterial,
                _textBoxQtyPerUnit,
                _textBoxDescription,
                _textBoxQuantity,
                _buttonCreateTask
            });
        }

        private void LoadOrders()
        {
            _listBoxOrders.Items.Clear();
            foreach (var o in _orderService.GetAll())
            {
                _listBoxOrders.Items.Add($"{o.Id}: {o.OrderNumber} | {o.CustomerName} | {o.Status}");
            }
        }

        private void OnOrderStatusChanged()
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(OnOrderStatusChanged));
                return;
            }
            LoadOrders();
            if (_selectedOrder != null)
            {
                _selectedOrder = _orderService.GetById(_selectedOrder.Id);
                _textBoxOrderNotes.Text = _selectedOrder?.Notes ?? "";
                LoadTasksForOrder();
            }
        }



        private void LoadSeamstresses()
        {
            _comboBoxSeamstress.Items.Clear();
            using (var db = new AppDbContext())
            {
                foreach (var u in db.Users.Where(u => u.RoleId == 2 && u.IsActive))
                {
                    _comboBoxSeamstress.Items.Add(new ComboboxItem
                    {
                        Text = $"{u.Username} ({u.FullName})",
                        Value = u.Id
                    });
                }
            }
            if (_comboBoxSeamstress.Items.Count > 0)
                _comboBoxSeamstress.SelectedIndex = 0;
        }

        private void LoadMaterials()
        {
            _comboBoxMaterial.Items.Clear();
            using (var db = new AppDbContext())
            {
                foreach (var m in db.Materials)
                {
                    _comboBoxMaterial.Items.Add(new ComboboxItem
                    {
                        Text = $"{m.Name} ({m.Quantity} {m.Unit})",
                        Value = m.Id
                    });
                }
            }
            if (_comboBoxMaterial.Items.Count > 0)
                _comboBoxMaterial.SelectedIndex = 0;
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

            int id = int.Parse(_listBoxOrders.SelectedItem.ToString().Split(':')[0]);
            _selectedOrder = _orderService.GetById(id);

            _textBoxOrderNotes.Text = _selectedOrder?.Notes;
            LoadTasksForOrder();
        }

        private void LoadTasksForOrder()
        {
            _listBoxTasks.Items.Clear();
            if (_selectedOrder == null) return;

            foreach (var t in _taskService.GetTasksByOrder(_selectedOrder.Id))
            {
                _listBoxTasks.Items.Add(
                    $"{t.Id}: {t.Description} | {t.QuantityCompleted}/{t.QuantityAssigned} | {t.Status}"
                );
            }
        }

        private void ButtonCreateTask_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            if (!int.TryParse(_textBoxQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Неверное количество");
                return;
            }

            var seam = _comboBoxSeamstress.SelectedItem as ComboboxItem;

            int? materialId = null;
            decimal qtyPerUnit = 0;

            if (_comboBoxMaterial.SelectedItem is ComboboxItem mat)
            {
                materialId = (int)mat.Value;
                if (!decimal.TryParse(_textBoxQtyPerUnit.Text, out qtyPerUnit))
                {
                    MessageBox.Show("Неверно указано количество материала");
                    return;
                }
            }

            bool ok = _taskService.CreateTask(
                _selectedOrder.Id,
                _textBoxDescription.Text,
                qty,
                (int)seam.Value,
                _currentUser.Id,
                materialId,
                qtyPerUnit
            );

            if (ok)
            {
                MessageBox.Show("Задание создано");
                _textBoxDescription.Clear();
                _textBoxQuantity.Clear();
                _textBoxQtyPerUnit.Clear();
                LoadTasksForOrder();
                LoadOrders();
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
