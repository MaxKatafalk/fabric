using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using fabric.BLL.Services;
using fabric.DAL;
using fabric.DAL.Models;
using fabric.Forms;

namespace fabric.Forms
{
    public class ManagerForm : Form
    {
        private User _currentUser;
        private AuthService _authService;

        // UI Elements
        private Panel _panelLeft;
        private Panel _panelRight;
        private Label _labelTitle;
        private Label _labelUsername;
        private Label _labelPassword;
        private Label _labelFullName;
        private Label _labelRole;
        private TextBox _textBoxUsername;
        private TextBox _textBoxPassword;
        private TextBox _textBoxFullName;
        private ComboBox _comboBoxRole;
        private Button _buttonAdd;
        private Button _buttonDelete;
        private Button _buttonOrders;
        private ListBox _listBoxUsers;
        private User _selectedUser;
        private Label _labelUserCount;

        public ManagerForm(User user)
        {
            _currentUser = user;
            _authService = new AuthService();

            InitializeForm();
            LoadUsers();
        }

        private void InitializeForm()
        {
            // Основные настройки 
            this.Text = $"Менеджер - {_currentUser.FullName}";
            this.Width = 850;
            this.Height = 550;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Панели для разделения
            _panelLeft = new Panel();
            _panelLeft.Left = 20;
            _panelLeft.Top = 20;
            _panelLeft.Width = 350;
            _panelLeft.Height = 480;
            _panelLeft.BackColor = Color.White;
            _panelLeft.BorderStyle = BorderStyle.FixedSingle;

            _panelRight = new Panel();
            _panelRight.Left = 390;
            _panelRight.Top = 20;
            _panelRight.Width = 430;
            _panelRight.Height = 480;
            _panelRight.BackColor = Color.White;
            _panelRight.BorderStyle = BorderStyle.FixedSingle;

            // Заголовок левой панели
            _labelTitle = new Label();
            _labelTitle.Text = "Добавить пользователя";
            _labelTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _labelTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _labelTitle.AutoSize = true;
            _labelTitle.Top = 20;
            _labelTitle.Left = 20;

            // Поля ввода
            int yPos = 60;

            _labelUsername = CreateLabel("Логин", yPos);
            _textBoxUsername = CreateTextBox(yPos + 25, "Введите логин");

            yPos += 70;
            _labelPassword = CreateLabel("Пароль", yPos);
            _textBoxPassword = CreatePasswordBox(yPos + 25);

            yPos += 70;
            _labelFullName = CreateLabel("ФИО", yPos);
            _textBoxFullName = CreateTextBox(yPos + 25, "Введите ФИО");

            yPos += 70;
            _labelRole = CreateLabel("Роль", yPos);
            _comboBoxRole = new ComboBox();
            _comboBoxRole.Left = 20;
            _comboBoxRole.Top = yPos + 25;
            _comboBoxRole.Width = 310;
            _comboBoxRole.Height = 32;
            _comboBoxRole.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxRole.Font = new Font("Segoe UI", 10);
            _comboBoxRole.Items.AddRange(new string[] { "Мастер", "Швея", "Кладовщик", "Менеджер" });
            _comboBoxRole.SelectedIndex = 0;
            _comboBoxRole.BackColor = Color.White;

            // Кнопки - опущены ниже и кнопка Удалить выровнена по правому краю
            _buttonAdd = CreateButton("Добавить", 20, 370, Color.FromArgb(46, 204, 113));
            _buttonAdd.Click += ButtonAdd_Click;

            _buttonDelete = CreateButton("Удалить", 190, 370, Color.FromArgb(231, 76, 60));
            _buttonDelete.Click += ButtonDelete_Click;

            // Правая панель 
            Label listTitle = new Label();
            listTitle.Text = "Список пользователей";
            listTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            listTitle.ForeColor = Color.FromArgb(44, 62, 80);
            listTitle.AutoSize = true;
            listTitle.Top = 20;
            listTitle.Left = 20;

            _listBoxUsers = new ListBox();
            _listBoxUsers.Left = 20;
            _listBoxUsers.Top = 60;
            _listBoxUsers.Width = 390;
            _listBoxUsers.Height = 340;
            _listBoxUsers.Font = new Font("Segoe UI", 10);
            _listBoxUsers.BorderStyle = BorderStyle.FixedSingle;
            _listBoxUsers.SelectedIndexChanged += ListBoxUsers_SelectedIndexChanged;

            _labelUserCount = new Label();
            _labelUserCount.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            _labelUserCount.ForeColor = Color.FromArgb(127, 140, 141);
            _labelUserCount.AutoSize = true;
            _labelUserCount.Top = 410;
            _labelUserCount.Left = 20;

            // Кнопка заказов
            _buttonOrders = CreateButton("Управление заказами", 20, 430, Color.FromArgb(52, 152, 219));
            _buttonOrders.Width = 390;
            _buttonOrders.Click += ButtonOrders_Click;

            // Добавление элементов на панели
            _panelLeft.Controls.Add(_labelTitle);
            _panelLeft.Controls.Add(_labelUsername);
            _panelLeft.Controls.Add(_textBoxUsername);
            _panelLeft.Controls.Add(_labelPassword);
            _panelLeft.Controls.Add(_textBoxPassword);
            _panelLeft.Controls.Add(_labelFullName);
            _panelLeft.Controls.Add(_textBoxFullName);
            _panelLeft.Controls.Add(_labelRole);
            _panelLeft.Controls.Add(_comboBoxRole);
            _panelLeft.Controls.Add(_buttonAdd);
            _panelLeft.Controls.Add(_buttonDelete);

            _panelRight.Controls.Add(listTitle);
            _panelRight.Controls.Add(_listBoxUsers);
            _panelRight.Controls.Add(_labelUserCount);
            _panelRight.Controls.Add(_buttonOrders);

            // Добавление панелей на форму
            this.Controls.Add(_panelLeft);
            this.Controls.Add(_panelRight);

            // Обработка клавиш
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F5) LoadUsers();
                if (e.KeyCode == Keys.Escape && _selectedUser != null)
                {
                    ClearSelection();
                }
            };
        }

        private Label CreateLabel(string text, int top)
        {
            var label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", 10);
            label.ForeColor = Color.FromArgb(52, 73, 94);
            label.AutoSize = true;
            label.Left = 20;
            label.Top = top;
            return label;
        }

        private TextBox CreateTextBox(int top, string placeholder)
        {
            var textBox = new TextBox();
            textBox.Left = 20;
            textBox.Top = top;
            textBox.Width = 310;
            textBox.Height = 32;
            textBox.Font = new Font("Segoe UI", 10);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Color.White;

            // Placeholder effect
            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = SystemColors.WindowText;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };

            if (textBox.Text == "")
            {
                textBox.Text = placeholder;
                textBox.ForeColor = Color.Gray;
            }

            return textBox;
        }

        private TextBox CreatePasswordBox(int top)
        {
            var textBox = CreateTextBox(top, "Введите пароль");
            textBox.PasswordChar = '•';
            return textBox;
        }

        private Button CreateButton(string text, int left, int top, Color backColor)
        {
            var button = new Button();
            button.Text = text;
            button.Left = left;
            button.Top = top;
            button.Width = 140;
            button.Height = 38;
            button.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            // Hover effects
            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(
                Math.Max(backColor.R - 20, 0),
                Math.Max(backColor.G - 20, 0),
                Math.Max(backColor.B - 20, 0));
            button.MouseLeave += (s, e) => button.BackColor = backColor;

            return button;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            string username = _textBoxUsername.Text.Trim();
            string password = _textBoxPassword.Text;
            string fullName = _textBoxFullName.Text.Trim();
            int roleId = _comboBoxRole.SelectedIndex + 1;

            if (username == "Введите логин" || string.IsNullOrWhiteSpace(username))
            {
                ShowError(_textBoxUsername, "Введите логин");
                return;
            }

            if (password == "Введите пароль" || string.IsNullOrWhiteSpace(password))
            {
                ShowError(_textBoxPassword, "Введите пароль");
                return;
            }

            if (fullName == "Введите ФИО" || string.IsNullOrWhiteSpace(fullName))
            {
                ShowError(_textBoxFullName, "Введите ФИО");
                return;
            }

            bool ok = _authService.Register(username, password, fullName, roleId);

            if (ok)
            {
                MessageBox.Show($"Пользователь '{username}' успешно добавлен",
                    "Успешно",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ResetForm();
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Ошибка: поля пустые или логин уже существует",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для удаления",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_selectedUser.Username == _currentUser.Username)
            {
                MessageBox.Show("Нельзя удалить самого себя",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя '{_selectedUser.Username}'?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            bool ok = _authService.DeleteUser(_selectedUser.Id);

            if (ok)
            {
                MessageBox.Show("Пользователь успешно удалён",
                    "Успешно",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearSelection();
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Ошибка при удалении пользователя",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void ButtonOrders_Click(object sender, EventArgs e)
        {
            OrderForm form = new OrderForm(_currentUser);
            form.ShowDialog();
        }

        private void ListBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxUsers.SelectedIndex < 0)
            {
                ClearSelection();
                return;
            }

            using (var db = new AppDbContext())
            {
                var users = db.Users.ToList();
                _selectedUser = users[_listBoxUsers.SelectedIndex];
            }

            _textBoxUsername.Text = _selectedUser.Username;
            _textBoxFullName.Text = _selectedUser.FullName;
            _comboBoxRole.SelectedIndex = _selectedUser.RoleId - 1;
            _textBoxPassword.Text = "";

            _listBoxUsers.BackColor = Color.White;
        }

        private void LoadUsers()
        {
            _listBoxUsers.Items.Clear();
            using (var db = new AppDbContext())
            {
                var users = db.Users.ToList();
                foreach (var u in users)
                {
                    string roleName = GetRussianRoleName(u.RoleId); // Используем русские названия
                    string status = u.IsActive ? "✓" : "✗";
                    _listBoxUsers.Items.Add($"{status} {u.Username} ({u.FullName}) - {roleName}");
                }
                _labelUserCount.Text = $"Всего пользователей: {users.Count}";
            }
        }

        // Новый метод для получения русских названий ролей
        private string GetRussianRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "Мастер",
                2 => "Швея",
                3 => "Кладовщик",
                4 => "Менеджер",
                _ => "Не определена"
            };
        }

        private void ShowError(Control control, string message)
        {
            control.BackColor = Color.FromArgb(255, 230, 230);
            control.Focus();
            MessageBox.Show(message, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ResetForm()
        {
            _textBoxUsername.Text = "Введите логин";
            _textBoxUsername.ForeColor = Color.Gray;
            _textBoxUsername.BackColor = Color.White;

            _textBoxPassword.Text = "Введите пароль";
            _textBoxPassword.ForeColor = Color.Gray;
            _textBoxPassword.BackColor = Color.White;

            _textBoxFullName.Text = "Введите ФИО";
            _textBoxFullName.ForeColor = Color.Gray;
            _textBoxFullName.BackColor = Color.White;

            _comboBoxRole.SelectedIndex = 0;
        }

        private void ClearSelection()
        {
            _selectedUser = null;
            ResetForm();
            _listBoxUsers.SelectedIndex = -1;
            _listBoxUsers.BackColor = Color.White;
        }
    }
}