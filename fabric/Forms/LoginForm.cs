using System;
using System.Drawing;
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
        private Label _labelTitle;
        private PictureBox _pictureBoxIcon;
        private LinkLabel _linkLabelForgot;
        private AuthService _authService;
        private Panel _panelDivider;

        public LoginForm()
        {
            _authService = new AuthService();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Основные настройки 
            this.Text = "Система управления производством";
            this.Width = 380;
            this.Height = 460;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Заголовок 
            _labelTitle = new Label();
            _labelTitle.Text = "ВХОД В СИСТЕМУ";
            _labelTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            _labelTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _labelTitle.AutoSize = true;

            // Иконка
            _pictureBoxIcon = new PictureBox();
            _pictureBoxIcon.Size = new Size(70, 70);
            _pictureBoxIcon.BackColor = Color.Transparent;

            var bmp = new Bitmap(_pictureBoxIcon.Width, _pictureBoxIcon.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = new Rectangle(5, 5, bmp.Width - 10, bmp.Height - 10);
                g.FillEllipse(new SolidBrush(Color.FromArgb(52, 152, 219)), rect);

                var userRect = new Rectangle(15, 15, bmp.Width - 30, bmp.Height - 40);
                g.FillEllipse(Brushes.White, userRect);

                var bodyRect = new Rectangle(10, 35, bmp.Width - 20, bmp.Height - 45);
                g.FillEllipse(Brushes.White, bodyRect);
            }
            _pictureBoxIcon.Image = bmp;

            // Метка логина
            _labelUsername = new Label();
            _labelUsername.Text = "Логин";
            _labelUsername.Font = new Font("Segoe UI", 10);
            _labelUsername.ForeColor = Color.FromArgb(52, 73, 94);
            _labelUsername.AutoSize = true;

            // Поле ввода логина
            _textBoxUsername = new TextBox();
            _textBoxUsername.Width = 300;
            _textBoxUsername.Height = 35;
            _textBoxUsername.Font = new Font("Segoe UI", 10);
            _textBoxUsername.BorderStyle = BorderStyle.FixedSingle;
            _textBoxUsername.BackColor = Color.White;
            _textBoxUsername.Enter += (s, e) => _textBoxUsername.BackColor = Color.FromArgb(240, 248, 255);
            _textBoxUsername.Leave += (s, e) => _textBoxUsername.BackColor = Color.White;
            _textBoxUsername.Padding = new Padding(5);

            // Метка пароля
            _labelPassword = new Label();
            _labelPassword.Text = "Пароль";
            _labelPassword.Font = new Font("Segoe UI", 10);
            _labelPassword.ForeColor = Color.FromArgb(52, 73, 94);
            _labelPassword.AutoSize = true;

            // Поле ввода пароля
            _textBoxPassword = new TextBox();
            _textBoxPassword.Width = 300;
            _textBoxPassword.Height = 35;
            _textBoxPassword.Font = new Font("Segoe UI", 10);
            _textBoxPassword.BorderStyle = BorderStyle.FixedSingle;
            _textBoxPassword.BackColor = Color.White;
            _textBoxPassword.PasswordChar = '•';
            _textBoxPassword.Enter += (s, e) => _textBoxPassword.BackColor = Color.FromArgb(240, 248, 255);
            _textBoxPassword.Leave += (s, e) => _textBoxPassword.BackColor = Color.White;
            _textBoxPassword.Padding = new Padding(5);

            // Кнопка входа
            _buttonLogin = new Button();
            _buttonLogin.Text = "ВОЙТИ";
            _buttonLogin.Width = 300;
            _buttonLogin.Height = 42;
            _buttonLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            _buttonLogin.BackColor = Color.FromArgb(52, 152, 219);
            _buttonLogin.ForeColor = Color.White;
            _buttonLogin.FlatStyle = FlatStyle.Flat;
            _buttonLogin.FlatAppearance.BorderSize = 0;
            _buttonLogin.Cursor = Cursors.Hand;
            _buttonLogin.Click += ButtonLogin_Click;

            // Эффекты при наведении на кнопку
            _buttonLogin.MouseEnter += (s, e) =>
            {
                _buttonLogin.BackColor = Color.FromArgb(41, 128, 185);
                _buttonLogin.ForeColor = Color.White;
            };
            _buttonLogin.MouseLeave += (s, e) =>
            {
                _buttonLogin.BackColor = Color.FromArgb(52, 152, 219);
                _buttonLogin.ForeColor = Color.White;
            };
            _buttonLogin.MouseDown += (s, e) =>
            {
                _buttonLogin.BackColor = Color.FromArgb(32, 102, 148);
            };
            _buttonLogin.MouseUp += (s, e) =>
            {
                _buttonLogin.BackColor = Color.FromArgb(41, 128, 185);
            };

            // Разделитель
            _panelDivider = new Panel();
            _panelDivider.Width = 300;
            _panelDivider.Height = 1;
            _panelDivider.BackColor = Color.FromArgb(220, 220, 220);

            // "Забыли пароль?"
            _linkLabelForgot = new LinkLabel();
            _linkLabelForgot.Text = "Забыли пароль?";
            _linkLabelForgot.Font = new Font("Segoe UI", 9);
            _linkLabelForgot.AutoSize = true;
            _linkLabelForgot.LinkColor = Color.FromArgb(52, 152, 219);
            _linkLabelForgot.VisitedLinkColor = Color.FromArgb(52, 152, 219);
            _linkLabelForgot.ActiveLinkColor = Color.FromArgb(41, 128, 185);
            _linkLabelForgot.Cursor = Cursors.Hand;
            _linkLabelForgot.Click += (s, e) =>
            {
                MessageBox.Show("Обратитесь к системному администратору для восстановления пароля.",
                    "Восстановление пароля",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            // Добавление элементов на форму
            this.Controls.Add(_labelTitle);
            this.Controls.Add(_pictureBoxIcon);
            this.Controls.Add(_labelUsername);
            this.Controls.Add(_textBoxUsername);
            this.Controls.Add(_labelPassword);
            this.Controls.Add(_textBoxPassword);
            this.Controls.Add(_buttonLogin);
            this.Controls.Add(_panelDivider);
            this.Controls.Add(_linkLabelForgot);

            // Обработчик изменения размера формы
            this.Resize += (s, e) => RepositionControls();

            // Первоначальное позиционирование
            RepositionControls();

            this.AcceptButton = _buttonLogin;

            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            };
        }

        private void RepositionControls()
        {
            int formWidth = this.ClientSize.Width;
            int centerX = formWidth / 2;

            // Заголовок
            _labelTitle.Left = centerX - (_labelTitle.Width / 2);
            _labelTitle.Top = 20;

            // Иконка
            _pictureBoxIcon.Left = centerX - (_pictureBoxIcon.Width / 2);
            _pictureBoxIcon.Top = 60;

            // Метка логина (левое выравнивание относительно текстового поля)
            _labelUsername.Left = centerX - (_textBoxUsername.Width / 2);
            _labelUsername.Top = 150;

            // Поле ввода логина
            _textBoxUsername.Left = centerX - (_textBoxUsername.Width / 2);
            _textBoxUsername.Top = 175;

            // Метка пароля (левое выравнивание относительно текстового поля)
            _labelPassword.Left = centerX - (_textBoxPassword.Width / 2);
            _labelPassword.Top = 220;

            // Поле ввода пароля
            _textBoxPassword.Left = centerX - (_textBoxPassword.Width / 2);
            _textBoxPassword.Top = 245;

            // Кнопка входа
            _buttonLogin.Left = centerX - (_buttonLogin.Width / 2);
            _buttonLogin.Top = 310;

            // Разделитель
            _panelDivider.Left = centerX - (_panelDivider.Width / 2);
            _panelDivider.Top = 370;

            // "Забыли пароль?"
            _linkLabelForgot.Left = centerX - (_linkLabelForgot.Width / 2);
            _linkLabelForgot.Top = 380;
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string username = _textBoxUsername.Text;
            string password = _textBoxPassword.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                _textBoxUsername.BackColor = Color.FromArgb(255, 230, 230);
                _textBoxUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                _textBoxPassword.BackColor = Color.FromArgb(255, 230, 230);
                _textBoxPassword.Focus();
                return;
            }

            _textBoxUsername.BackColor = Color.White;
            _textBoxPassword.BackColor = Color.White;

            User user = _authService.Authenticate(username, password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль. Проверьте введенные данные и повторите попытку.",
                    "Ошибка авторизации",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                _textBoxPassword.SelectAll();
                _textBoxPassword.Focus();
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