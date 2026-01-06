using System;
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
        private Button buttonOrders;
        private ListBox _listBoxUsers;
        private User _selectedUser;

        public ManagerForm(User user)
        {
            _currentUser = user;
            _authService = new AuthService();
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.Text = "Менеджер";
            this.Width = 650;
            this.Height = 450;
            this.StartPosition = FormStartPosition.CenterScreen;

            _labelTitle = new Label();
            _labelTitle.Text = "Добавить пользователя";
            _labelTitle.Left = 20;
            _labelTitle.Top = 10;
            _labelTitle.Width = 200;

            _labelUsername = new Label();
            _labelUsername.Text = "Логин";
            _labelUsername.Left = 20;
            _labelUsername.Top = 40;
            _labelUsername.Width = 100;

            _textBoxUsername = new TextBox();
            _textBoxUsername.Left = 20;
            _textBoxUsername.Top = 60;
            _textBoxUsername.Width = 220;

            _labelPassword = new Label();
            _labelPassword.Text = "Пароль";
            _labelPassword.Left = 20;
            _labelPassword.Top = 95;
            _labelPassword.Width = 100;

            _textBoxPassword = new TextBox();
            _textBoxPassword.Left = 20;
            _textBoxPassword.Top = 115;
            _textBoxPassword.Width = 220;
            _textBoxPassword.PasswordChar = '*';

            _labelFullName = new Label();
            _labelFullName.Text = "ФИО";
            _labelFullName.Left = 20;
            _labelFullName.Top = 150;
            _labelFullName.Width = 100;

            _textBoxFullName = new TextBox();
            _textBoxFullName.Left = 20;
            _textBoxFullName.Top = 170;
            _textBoxFullName.Width = 220;

            _labelRole = new Label();
            _labelRole.Text = "Роль";
            _labelRole.Left = 20;
            _labelRole.Top = 205;
            _labelRole.Width = 100;

            _comboBoxRole = new ComboBox();
            _comboBoxRole.Left = 20;
            _comboBoxRole.Top = 225;
            _comboBoxRole.Width = 220;
            _comboBoxRole.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBoxRole.Items.AddRange(new string[] { "Master", "Seamstress", "Storekeeper", "Manager" });
            _comboBoxRole.SelectedIndex = 0;

            _buttonAdd = new Button();
            _buttonAdd.Text = "Добавить";
            _buttonAdd.Left = 20;
            _buttonAdd.Top = 270;
            _buttonAdd.Width = 105;
            _buttonAdd.Click += ButtonAdd_Click;

            _buttonDelete = new Button();
            _buttonDelete.Text = "Удалить";
            _buttonDelete.Left = 135;
            _buttonDelete.Top = 270;
            _buttonDelete.Width = 105;
            _buttonDelete.Click += ButtonDelete_Click;

            _listBoxUsers = new ListBox();
            _listBoxUsers.Left = 260;
            _listBoxUsers.Top = 20;
            _listBoxUsers.Width = 350;
            _listBoxUsers.Height = 360;
            _listBoxUsers.SelectedIndexChanged += ListBoxUsers_SelectedIndexChanged;

            buttonOrders = new Button();
            buttonOrders.Text = "Заказы";
            buttonOrders.Left = 260;
            buttonOrders.Top = 375;
            buttonOrders.Width = 350;
            buttonOrders.Height = 30;
            buttonOrders.Click += ButtonOrders_Click;

            this.Controls.Add(buttonOrders);
            this.Controls.Add(_labelTitle);
            this.Controls.Add(_labelUsername);
            this.Controls.Add(_textBoxUsername);
            this.Controls.Add(_labelPassword);
            this.Controls.Add(_textBoxPassword);
            this.Controls.Add(_labelFullName);
            this.Controls.Add(_textBoxFullName);
            this.Controls.Add(_labelRole);
            this.Controls.Add(_comboBoxRole);
            this.Controls.Add(_buttonAdd);
            this.Controls.Add(_buttonDelete);
            this.Controls.Add(_listBoxUsers);
        }
        private void ButtonOrders_Click(object sender, EventArgs e)
        {
            OrderForm form = new OrderForm(_currentUser);
            form.ShowDialog();
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            string username = _textBoxUsername.Text.Trim();
            string password = _textBoxPassword.Text;
            string fullName = _textBoxFullName.Text.Trim();
            int roleId = _comboBoxRole.SelectedIndex + 1;

            bool ok = _authService.Register(username, password, fullName, roleId);

            if (ok)
            {
                MessageBox.Show("Пользователь добавлен", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _textBoxUsername.Clear();
                _textBoxPassword.Clear();
                _textBoxFullName.Clear();
                _comboBoxRole.SelectedIndex = 0;
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Ошибка: поля пустые или логин уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для удаления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedUser.Username == _currentUser.Username)
            {
                MessageBox.Show("Нельзя удалить самого себя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool ok = _authService.DeleteUser(_selectedUser.Id);

            if (ok)
            {
                MessageBox.Show("Пользователь удалён", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _selectedUser = null;
                _textBoxUsername.Clear();
                _textBoxPassword.Clear();
                _textBoxFullName.Clear();
                _comboBoxRole.SelectedIndex = 0;
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Ошибка при удалении пользователя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ListBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxUsers.SelectedIndex < 0) return;

            using (var db = new AppDbContext())
            {
                var users = db.Users.ToList();
                _selectedUser = users[_listBoxUsers.SelectedIndex];
            }

            _textBoxUsername.Text = _selectedUser.Username;
            _textBoxFullName.Text = _selectedUser.FullName;
            _comboBoxRole.SelectedIndex = _selectedUser.RoleId - 1;
            _textBoxPassword.Clear();
        }

        private void LoadUsers()
        {
            _listBoxUsers.Items.Clear();
            using (var db = new AppDbContext())
            {
                var users = db.Users.ToList();
                foreach (var u in users)
                {
                    string roleName = db.Roles.FirstOrDefault(r => r.Id == u.RoleId)?.Name ?? "NoRole";
                    _listBoxUsers.Items.Add($"{u.Username} ({roleName})");
                }
            }
        }
    }
}
