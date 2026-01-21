using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using fabric.BLL.Services;
using fabric.DAL;
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
        private TextBox _textBoxNotes;
        private Button _buttonCreate;
        private Button _buttonUpdate;
        private Button _buttonDelete;
        private ComboBox _comboBoxStatus;
        private Button _buttonRefresh;
        private Order _selectedOrder;
        private Panel _panelLeft;
        private Panel _panelRight;
        private Label _labelTitle;
        private Label _labelOrderCount;

        // Регулярные выражения для валидации
        private Regex _numbersOnlyRegex = new Regex(@"^\d+$");
        private Regex _lettersOnlyRegex = new Regex(@"^[а-яА-ЯёЁa-zA-Z\s\-]+$");

        // Сообщения об ошибках
        private string _orderNumberErrorMessage = "Номер заказа должен содержать только цифры";
        private string _customerNameErrorMessage = "Имя клиента должно содержать только буквы, пробелы и дефисы";

        public OrderForm(User user)
        {
            _currentUser = user;
            _orderService = new OrderService();
            InitializeComponent();
            LoadOrders();

            AppEvents.OrderStatusChanged += OnOrderStatusChanged;
            this.FormClosed += (s, e) => AppEvents.OrderStatusChanged -= OnOrderStatusChanged;
        }

        private void InitializeComponent()
        {
            // Основные настройки формы
            this.Text = $"Управление заказами - {_currentUser.FullName}";
            this.Width = 950;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Левая панель - Список заказов
            _panelLeft = new Panel();
            _panelLeft.Left = 20;
            _panelLeft.Top = 20;
            _panelLeft.Width = 400;
            _panelLeft.Height = 590;
            _panelLeft.BackColor = Color.White;
            _panelLeft.BorderStyle = BorderStyle.FixedSingle;
            _panelLeft.Padding = new Padding(10);

            // Заголовок левой панели
            _labelTitle = new Label();
            _labelTitle.Text = "СПИСОК ЗАКАЗОВ";
            _labelTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            _labelTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _labelTitle.AutoSize = true;
            _labelTitle.Top = 15;
            _labelTitle.Left = 15;

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
            _buttonRefresh.Click += (s, e) => LoadOrders();

            // Список заказов
            _listBoxOrders = new ListBox();
            _listBoxOrders.Left = 15;
            _listBoxOrders.Top = 60;
            _listBoxOrders.Width = 370;
            _listBoxOrders.Height = 450;
            _listBoxOrders.Font = new Font("Segoe UI", 10);
            _listBoxOrders.BorderStyle = BorderStyle.FixedSingle;
            _listBoxOrders.SelectionMode = SelectionMode.One;
            _listBoxOrders.SelectedIndexChanged += ListBoxOrders_SelectedIndexChanged;

            // Счетчик заказов
            _labelOrderCount = new Label();
            _labelOrderCount.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            _labelOrderCount.ForeColor = Color.FromArgb(127, 140, 141);
            _labelOrderCount.AutoSize = true;
            _labelOrderCount.Top = 520;
            _labelOrderCount.Left = 15;

            // Правая панель - Детали заказа
            _panelRight = new Panel();
            _panelRight.Left = 440;
            _panelRight.Top = 20;
            _panelRight.Width = 490;
            _panelRight.Height = 590;
            _panelRight.BackColor = Color.White;
            _panelRight.BorderStyle = BorderStyle.FixedSingle;
            _panelRight.Padding = new Padding(15);

            // Заголовок правой панели
            Label labelDetails = new Label();
            labelDetails.Text = "ДЕТАЛИ ЗАКАЗА";
            labelDetails.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            labelDetails.ForeColor = Color.FromArgb(44, 62, 80);
            labelDetails.AutoSize = true;
            labelDetails.Top = 15;
            labelDetails.Left = 15;

            // Поля формы
            int yPos = 60;
            int contentWidth = 460;
            int labelWidth = 140;

            // Номер заказа
            Label l1 = CreateLabel("Номер заказа:", yPos, labelWidth);
            _textBoxOrderNumber = CreateTextBox(yPos + 25, contentWidth - 30, "Введите номер заказа (только цифры)");
            _textBoxOrderNumber.TextChanged += TextBoxOrderNumber_TextChanged;
            _textBoxOrderNumber.Leave += TextBoxOrderNumber_Leave;

            yPos += 70;

            // Клиент
            Label l2 = CreateLabel("Клиент:", yPos, labelWidth);
            _textBoxCustomer = CreateTextBox(yPos + 25, contentWidth - 30, "Введите имя клиента (только буквы)");
            _textBoxCustomer.TextChanged += TextBoxCustomer_TextChanged;
            _textBoxCustomer.Leave += TextBoxCustomer_Leave;

            yPos += 70;

            // Дата заказа
            Label l3 = CreateLabel("Дата заказа:", yPos, labelWidth);
            _datePickerOrder = new DateTimePicker();
            _datePickerOrder.Left = 15;
            _datePickerOrder.Top = yPos + 25;
            _datePickerOrder.Width = contentWidth - 30;
            _datePickerOrder.Height = 32;
            _datePickerOrder.Font = new Font("Segoe UI", 10);
            _datePickerOrder.Format = DateTimePickerFormat.Short;
            _datePickerOrder.CalendarMonthBackground = Color.White;
            _datePickerOrder.CalendarTitleBackColor = Color.FromArgb(52, 152, 219);

            yPos += 70;

            // Срок выполнения
            Label l4 = CreateLabel("Срок выполнения:", yPos, labelWidth);
            _datePickerDue = new DateTimePicker();
            _datePickerDue.Left = 15;
            _datePickerDue.Top = yPos + 25;
            _datePickerDue.Width = contentWidth - 30;
            _datePickerDue.Height = 32;
            _datePickerDue.Font = new Font("Segoe UI", 10);
            _datePickerDue.Format = DateTimePickerFormat.Short;
            _datePickerDue.CalendarMonthBackground = Color.White;
            _datePickerDue.CalendarTitleBackColor = Color.FromArgb(52, 152, 219);

            yPos += 70;

            // Примечание
            Label l6 = CreateLabel("Примечание:", yPos, labelWidth);
            _textBoxNotes = new TextBox();
            _textBoxNotes.Left = 15;
            _textBoxNotes.Top = yPos + 25;
            _textBoxNotes.Width = contentWidth - 30;
            _textBoxNotes.Height = 100;
            _textBoxNotes.Multiline = true;
            _textBoxNotes.Font = new Font("Segoe UI", 10);
            _textBoxNotes.BackColor = Color.White;
            _textBoxNotes.BorderStyle = BorderStyle.FixedSingle;
            _textBoxNotes.ScrollBars = ScrollBars.Vertical;
            _textBoxNotes.PlaceholderText = "Введите примечание к заказу...";

            yPos += 130;

            // Статус
            Label l7 = CreateLabel("Статус заказа:", yPos, labelWidth);
            _comboBoxStatus = new ComboBox();
            _comboBoxStatus.Left = 15;
            _comboBoxStatus.Top = yPos + 25;
            _comboBoxStatus.Width = 180;
            _comboBoxStatus.Height = 32;
            _comboBoxStatus.Font = new Font("Segoe UI", 10);
            _comboBoxStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxStatus.BackColor = Color.White;
            _comboBoxStatus.FlatStyle = FlatStyle.Flat;

            // Заполняем статусы
            _comboBoxStatus.Items.AddRange(new string[] { "В ожидании", "В работе", "Завершен" });

            yPos += 70;

            // Кнопки действий
            int buttonWidth = 180;
            int buttonHeight = 40;
            int buttonSpacing = 15;

            // Положение для первой строки кнопок
            int firstRowY = yPos;

            // Кнопка создания (слева)
            _buttonCreate = CreateButton("СОЗДАТЬ ЗАКАЗ", 15, firstRowY, buttonWidth, Color.FromArgb(46, 204, 113));
            _buttonCreate.Click += ButtonCreate_Click;

            // Кнопка сохранения (справа)
            int saveButtonLeft = contentWidth - 30 - buttonWidth;
            _buttonUpdate = CreateButton("СОХРАНИТЬ", saveButtonLeft, firstRowY, buttonWidth, Color.FromArgb(155, 89, 182));
            _buttonUpdate.Click += ButtonUpdate_Click;
            _buttonUpdate.Enabled = false;

            // Вторая строка - кнопка удаления
            int secondRowY = firstRowY + buttonHeight + 20;
            int deleteButtonWidth = buttonWidth * 2 + buttonSpacing;
            int deleteButtonLeft = 15;

            _buttonDelete = CreateButton("УДАЛИТЬ ЗАКАЗ", deleteButtonLeft, secondRowY, deleteButtonWidth, Color.FromArgb(231, 76, 60));
            _buttonDelete.Click += ButtonDelete_Click;
            _buttonDelete.Enabled = false;

            // Добавление элементов на панели
            _panelLeft.Controls.Add(_labelTitle);
            _panelLeft.Controls.Add(_buttonRefresh);
            _panelLeft.Controls.Add(_listBoxOrders);
            _panelLeft.Controls.Add(_labelOrderCount);

            _panelRight.Controls.Add(labelDetails);
            _panelRight.Controls.Add(l1);
            _panelRight.Controls.Add(_textBoxOrderNumber);
            _panelRight.Controls.Add(l2);
            _panelRight.Controls.Add(_textBoxCustomer);
            _panelRight.Controls.Add(l3);
            _panelRight.Controls.Add(_datePickerOrder);
            _panelRight.Controls.Add(l4);
            _panelRight.Controls.Add(_datePickerDue);
            _panelRight.Controls.Add(l6);
            _panelRight.Controls.Add(_textBoxNotes);
            _panelRight.Controls.Add(l7);
            _panelRight.Controls.Add(_comboBoxStatus);
            _panelRight.Controls.Add(_buttonCreate);
            _panelRight.Controls.Add(_buttonUpdate);
            _panelRight.Controls.Add(_buttonDelete);

            // Добавление панелей на форму
            this.Controls.Add(_panelLeft);
            this.Controls.Add(_panelRight);

            // Обработка клавиш
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F5) LoadOrders();
                if (e.KeyCode == Keys.Escape && _selectedOrder != null)
                {
                    ClearSelection();
                }
            };
        }

        private Label CreateLabel(string text, int top, int width)
        {
            var label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(52, 73, 94);
            label.AutoSize = false;
            label.Width = width;
            label.Height = 25;
            label.Left = 15;
            label.Top = top;
            return label;
        }

        private TextBox CreateTextBox(int top, int width, string placeholder)
        {
            var textBox = new TextBox();
            textBox.Left = 15;
            textBox.Top = top;
            textBox.Width = width;
            textBox.Height = 32;
            textBox.Font = new Font("Segoe UI", 10);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Color.White;
            textBox.PlaceholderText = placeholder;

            // Эффекты при фокусе
            textBox.Enter += (s, e) => textBox.BackColor = Color.FromArgb(240, 248, 255);
            textBox.Leave += (s, e) => textBox.BackColor = Color.White;

            return textBox;
        }

        private Button CreateButton(string text, int left, int top, int width, Color backColor)
        {
            var button = new Button();
            button.Text = text;
            button.Left = left;
            button.Top = top;
            button.Width = width;
            button.Height = 40;
            button.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            // Эффекты при наведении
            button.MouseEnter += (s, e) => button.BackColor = DarkenColor(backColor, 20);
            button.MouseLeave += (s, e) => button.BackColor = backColor;

            return button;
        }

        private Color DarkenColor(Color color, int amount)
        {
            return Color.FromArgb(
                Math.Max(color.R - amount, 0),
                Math.Max(color.G - amount, 0),
                Math.Max(color.B - amount, 0)
            );
        }

        // ВАЛИДАЦИЯ ВВОДА

        private void TextBoxOrderNumber_TextChanged(object sender, EventArgs e)
        {
            ValidateOrderNumber();
        }

        private void TextBoxOrderNumber_Leave(object sender, EventArgs e)
        {
            ValidateOrderNumber(true);
        }

        private void TextBoxCustomer_TextChanged(object sender, EventArgs e)
        {
            ValidateCustomerName();
        }

        private void TextBoxCustomer_Leave(object sender, EventArgs e)
        {
            ValidateCustomerName(true);
        }

        private bool ValidateOrderNumber(bool showMessage = false)
        {
            string text = _textBoxOrderNumber.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                _textBoxOrderNumber.BackColor = Color.White;
                return false;
            }

            if (_numbersOnlyRegex.IsMatch(text))
            {
                _textBoxOrderNumber.BackColor = Color.FromArgb(230, 255, 230); // Зеленый фон при успешной валидации
                return true;
            }
            else
            {
                _textBoxOrderNumber.BackColor = Color.FromArgb(255, 230, 230); // Красный фон при ошибке
                if (showMessage)
                {
                    ShowError(_textBoxOrderNumber, _orderNumberErrorMessage);
                }
                return false;
            }
        }

        private bool ValidateCustomerName(bool showMessage = false)
        {
            string text = _textBoxCustomer.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                _textBoxCustomer.BackColor = Color.White;
                return false;
            }

            if (_lettersOnlyRegex.IsMatch(text))
            {
                _textBoxCustomer.BackColor = Color.FromArgb(230, 255, 230); // Зеленый фон при успешной валидации
                return true;
            }
            else
            {
                _textBoxCustomer.BackColor = Color.FromArgb(255, 230, 230); // Красный фон при ошибке
                if (showMessage)
                {
                    ShowError(_textBoxCustomer, _customerNameErrorMessage);
                }
                return false;
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // Проверяем номер заказа
            if (!ValidateOrderNumber(true))
            {
                isValid = false;
            }

            // Проверяем имя клиента
            if (!ValidateCustomerName(true))
            {
                isValid = false;
            }

            // Проверяем примечание
            if (string.IsNullOrWhiteSpace(_textBoxNotes.Text))
            {
                ShowError(_textBoxNotes, "Введите примечание к заказу");
                isValid = false;
            }
            else
            {
                _textBoxNotes.BackColor = Color.White;
            }

            return isValid;
        }

        private void LoadOrders()
        {
            _listBoxOrders.Items.Clear();
            var list = _orderService.GetAll();

            if (list == null || !list.Any())
            {
                _listBoxOrders.Items.Add("Нет доступных заказов");
                _labelOrderCount.Text = "Всего заказов: 0";
                return;
            }

            // Простое отображение без использования конкретных значений enum
            foreach (var o in list)
            {
                string status = GetStatusRussian(o.Status.ToString());
                _listBoxOrders.Items.Add($"[{o.Id}] {o.OrderNumber} | {o.CustomerName} | {status} | {o.OrderDate:dd.MM.yyyy}");
            }

            _labelOrderCount.Text = $"Всего заказов: {list.Count}";
        }

        private string GetStatusRussian(string status)
        {
            // Универсальная конвертация статусов
            status = status.ToLower();

            if (status.Contains("pending") || status.Contains("ожидан"))
                return "В ожидании";
            else if (status.Contains("progress") || status.Contains("работе") || status.Contains("выполнен"))
                return "В работе";
            else if (status.Contains("completed") || status.Contains("завершен"))
                return "Завершен";
            else
                return status;
        }

        private OrderStatus GetStatusFromRussian(string russianStatus)
        {
            // Получаем все значения enum OrderStatus
            var statusValues = Enum.GetValues(typeof(OrderStatus));

            // Ищем подходящее значение
            foreach (OrderStatus status in statusValues)
            {
                string statusStr = status.ToString().ToLower();

                if (russianStatus == "В ожидании" &&
                    (statusStr.Contains("pending") || statusStr.Contains("ожидан")))
                    return status;
                else if (russianStatus == "В работе" &&
                        (statusStr.Contains("progress") || statusStr.Contains("работе")))
                    return status;
                else if (russianStatus == "Завершен" &&
                        (statusStr.Contains("completed") || statusStr.Contains("завершен")))
                    return status;
            }

            // Если не нашли, возвращаем первое значение
            return (OrderStatus)statusValues.GetValue(0);
        }

        private void OnOrderStatusChanged()
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(LoadOrders));
            }
            else
            {
                LoadOrders();
            }
        }

        private void ButtonCreate_Click(object sender, EventArgs e)
        {
            // Проверяем валидность формы перед созданием
            if (!ValidateForm())
            {
                return;
            }

            string orderNumber = _textBoxOrderNumber.Text.Trim();
            string customer = _textBoxCustomer.Text.Trim();
            string notes = _textBoxNotes.Text.Trim();

            DateTime orderDate = _datePickerOrder.Value;
            DateTime? due = _datePickerDue.Value;

            bool ok = _orderService.CreateOrder(orderNumber, customer, orderDate, due, notes, _currentUser?.Id);
            if (ok)
            {
                MessageBox.Show("Заказ успешно создан", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadOrders();
            }
            else
            {
                MessageBox.Show("Ошибка при создании заказа. Проверьте введенные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для обновления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем валидность формы перед обновлением
            if (!ValidateForm())
            {
                return;
            }

            string orderNumber = _textBoxOrderNumber.Text.Trim();
            string customer = _textBoxCustomer.Text.Trim();
            string notes = _textBoxNotes.Text.Trim();

            // Обновляем данные заказа через базу данных напрямую
            using (var db = new AppDbContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == _selectedOrder.Id);
                if (order != null)
                {
                    order.OrderNumber = orderNumber;
                    order.CustomerName = customer;
                    order.OrderDate = _datePickerOrder.Value;
                    order.DueDate = _datePickerDue.Value;
                    order.Notes = notes;

                    // Обновляем статус, если выбран в комбобоксе
                    if (_comboBoxStatus.SelectedItem != null)
                    {
                        string selectedStatus = _comboBoxStatus.SelectedItem.ToString();
                        order.Status = GetStatusFromRussian(selectedStatus);
                    }

                    db.SaveChanges();

                    MessageBox.Show("Заказ успешно обновлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadOrders();
                    AppEvents.RaiseOrderStatusChanged(); // Уведомляем другие формы об изменении
                }
                else
                {
                    MessageBox.Show("Заказ не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для удаления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить заказ №{_selectedOrder.OrderNumber}?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            // Удаляем заказ через базу данных напрямую
            using (var db = new AppDbContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == _selectedOrder.Id);
                if (order != null)
                {
                    // Проверяем, нет ли связанных заданий
                    var tasks = db.ProductionTasks.Where(t => t.OrderId == order.Id).ToList();
                    if (tasks.Any())
                    {
                        var taskResult = MessageBox.Show(
                            $"У этого заказа есть {tasks.Count} связанных заданий. Удалить их вместе с заказом?",
                            "Подтверждение удаления",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (taskResult == DialogResult.Yes)
                        {
                            db.ProductionTasks.RemoveRange(tasks);
                        }
                        else
                        {
                            MessageBox.Show("Удаление отменено. Сначала удалите связанные задания.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    db.Orders.Remove(order);
                    db.SaveChanges();

                    MessageBox.Show("Заказ успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearSelection();
                    LoadOrders();
                }
                else
                {
                    MessageBox.Show("Заказ не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ListBoxOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxOrders.SelectedIndex < 0 || string.IsNullOrEmpty(_listBoxOrders.SelectedItem.ToString()))
            {
                ClearSelection();
                return;
            }

            string item = _listBoxOrders.Items[_listBoxOrders.SelectedIndex].ToString();

            if (!item.Contains("[") || !item.Contains("]"))
            {
                ClearSelection();
                return;
            }

            try
            {
                int start = item.IndexOf('[') + 1;
                int end = item.IndexOf(']');
                int id = int.Parse(item.Substring(start, end - start));
                _selectedOrder = _orderService.GetById(id);

                if (_selectedOrder != null)
                {
                    _textBoxOrderNumber.Text = _selectedOrder.OrderNumber;
                    _textBoxCustomer.Text = _selectedOrder.CustomerName;
                    _datePickerOrder.Value = _selectedOrder.OrderDate;
                    _datePickerDue.Value = _selectedOrder.DueDate ?? DateTime.Now;
                    _textBoxNotes.Text = _selectedOrder.Notes;

                    // Устанавливаем статус
                    string statusRussian = GetStatusRussian(_selectedOrder.Status.ToString());
                    _comboBoxStatus.SelectedItem = statusRussian;

                    // Включаем кнопки редактирования и удаления
                    _buttonUpdate.Enabled = true;
                    _buttonDelete.Enabled = true;
                    _buttonCreate.Enabled = false;

                    // Валидируем данные после загрузки
                    ValidateOrderNumber();
                    ValidateCustomerName();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выборе заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearSelection();
            }
        }

        private void ShowError(Control control, string message)
        {
            control.BackColor = Color.FromArgb(255, 230, 230);
            control.Focus();
            MessageBox.Show(message, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ClearForm()
        {
            _textBoxOrderNumber.Clear();
            _textBoxCustomer.Clear();
            _textBoxNotes.Clear();
            _datePickerOrder.Value = DateTime.Now;
            _datePickerDue.Value = DateTime.Now.AddDays(7);
            _comboBoxStatus.SelectedIndex = 0;

            // Сбрасываем цвета
            _textBoxOrderNumber.BackColor = Color.White;
            _textBoxCustomer.BackColor = Color.White;
            _textBoxNotes.BackColor = Color.White;
        }

        private void ClearSelection()
        {
            _selectedOrder = null;
            ClearForm();
            _listBoxOrders.SelectedIndex = -1;

            // Настраиваем кнопки
            _buttonCreate.Enabled = true;
            _buttonUpdate.Enabled = false;
            _buttonDelete.Enabled = false;
        }
    }
}