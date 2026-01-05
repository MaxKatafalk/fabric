using System;
using System.Windows.Forms;
using fabric.BLL.Services;
using fabric.DAL.Models;
using fabric.Forms;

namespace fabric
{
    public partial class LoginForm : Form
    {
        private TextBox _textBoxUsername;
        private TextBox _textBoxPassword;
        private Button _buttonLogin;
        private Label _labelUsername;
        private Label _labelPassword;
        private AuthService _authService;

        public LoginForm()
        {
            _authService = new AuthService();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Вход";
            this.Width = 300;
            this.Height = 220;
            this.StartPosition = FormStartPosition.CenterScreen;

            _labelUsername = new Label();
            _labelUsername.Text = "Логин";
            _labelUsername.Left = 20;
            _labelUsername.Top = 20;

            _textBoxUsername = new TextBox();
            _textBoxUsername.Left = 20;
            _textBoxUsername.Top = 40;
            _textBoxUsername.Width = 240;

            _labelPassword = new Label();
            _labelPassword.Text = "Пароль";
            _labelPassword.Left = 20;
            _labelPassword.Top = 75;

            _textBoxPassword = new TextBox();
            _textBoxPassword.Left = 20;
            _textBoxPassword.Top = 95;
            _textBoxPassword.Width = 240;
            _textBoxPassword.PasswordChar = '*';

            _buttonLogin = new Button();
            _buttonLogin.Text = "Войти";
            _buttonLogin.Left = 20;
            _buttonLogin.Top = 135;
            _buttonLogin.Width = 240;
            _buttonLogin.Click += ButtonLogin_Click;

            this.Controls.Add(_labelUsername);
            this.Controls.Add(_textBoxUsername);
            this.Controls.Add(_labelPassword);
            this.Controls.Add(_textBoxPassword);
            this.Controls.Add(_buttonLogin);
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string username = _textBoxUsername.Text;
            string password = _textBoxPassword.Text;

            User user = _authService.Authenticate(username, password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }

            OpenFormByRole(user);
        }

        private void OpenFormByRole(User user)
        {
            Form form = null;

            if (user.RoleId == 1)
            {
                form = new MasterForm(user);
            }
            else if (user.RoleId == 2)
            {
                form = new SeamstressForm(user);
            }
            else if (user.RoleId == 3)
            {
                form = new StorekeeperForm(user);
            }
            else if (user.RoleId == 4)
            {
                form = new ManagerForm(user);
            }

            if (form != null)
            {
                this.Hide();
                form.FormClosed += (s, e) => this.Show();
                form.Show();
            }
        }
    }
}
