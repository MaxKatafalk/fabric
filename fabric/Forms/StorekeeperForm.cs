using System;
using System.Linq;
using System.Windows.Forms;
using fabric.BLL.Services;
using fabric.DAL;
using fabric.DAL.Models;

namespace fabric.Forms
{
    public class StorekeeperForm : Form
    {
        private User _currentUser;
        private MaterialService _materialService;

        private ListBox _listBoxMaterials;
        private Button _buttonRefresh;
        private TextBox _textBoxName;
        private TextBox _textBoxUnit;
        private TextBox _textBoxQuantity;
        private Button _buttonAddMaterial;
        private NumericUpDown _numericQuantity;
        private Button _buttonReceive;
        private Button _buttonHistory;
        private ListBox _listBoxHistory;
        private Material _selectedMaterial;

        public StorekeeperForm(User user)
        {
            _currentUser = user;
            _materialService = new MaterialService();
            InitializeComponent();
            LoadMaterials();
        }



        private void InitializeComponent()
        {
            this.Text = "Кладовщик";
            this.Width = 1000;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;

            _listBoxMaterials = new ListBox();
            _listBoxMaterials.Left = 20;
            _listBoxMaterials.Top = 20;
            _listBoxMaterials.Width = 420;
            _listBoxMaterials.Height = 400;
            _listBoxMaterials.SelectedIndexChanged += ListBoxMaterials_SelectedIndexChanged;

            _buttonRefresh = new Button();
            _buttonRefresh.Left = 20;
            _buttonRefresh.Top = 430;
            _buttonRefresh.Width = 120;
            _buttonRefresh.Text = "Обновить";
            _buttonRefresh.Click += (s, e) => LoadMaterials();

            _textBoxName = new TextBox();
            _textBoxName.Left = 460;
            _textBoxName.Top = 40;
            _textBoxName.Width = 280;
            _textBoxName.PlaceholderText = "Название";

            _textBoxUnit = new TextBox();
            _textBoxUnit.Left = 460;
            _textBoxUnit.Top = 80;
            _textBoxUnit.Width = 280;
            _textBoxUnit.PlaceholderText = "Ед. изм. (м/шт)";

            _textBoxQuantity = new TextBox();
            _textBoxQuantity.Left = 460;
            _textBoxQuantity.Top = 120;
            _textBoxQuantity.Width = 280;
            _textBoxQuantity.PlaceholderText = "Количество (для добавления)";

            _buttonAddMaterial = new Button();
            _buttonAddMaterial.Left = 460;
            _buttonAddMaterial.Top = 160;
            _buttonAddMaterial.Width = 280;
            _buttonAddMaterial.Text = "Добавить материал";
            _buttonAddMaterial.Click += ButtonAddMaterial_Click;

            _numericQuantity = new NumericUpDown();
            _numericQuantity.Left = 460;
            _numericQuantity.Top = 220;
            _numericQuantity.Width = 120;
            _numericQuantity.Minimum = 0;
            _numericQuantity.Maximum = 1000000;
            _numericQuantity.DecimalPlaces = 2;

            _buttonReceive = new Button();
            _buttonReceive.Left = 600;
            _buttonReceive.Top = 220;
            _buttonReceive.Width = 140;
            _buttonReceive.Text = "Принять";
            _buttonReceive.Click += ButtonReceive_Click;

            _buttonHistory = new Button();
            _buttonHistory.Left = 460;
            _buttonHistory.Top = 300;
            _buttonHistory.Width = 280;
            _buttonHistory.Text = "История";
            _buttonHistory.Click += ButtonHistory_Click;

            _listBoxHistory = new ListBox();
            _listBoxHistory.Left = 760;
            _listBoxHistory.Top = 20;
            _listBoxHistory.Width = 420;
            _listBoxHistory.Height = 520;

            this.Controls.Add(_listBoxMaterials);
            this.Controls.Add(_buttonRefresh);
            this.Controls.Add(_textBoxName);
            this.Controls.Add(_textBoxUnit);
            this.Controls.Add(_textBoxQuantity);
            this.Controls.Add(_buttonAddMaterial);
            this.Controls.Add(_numericQuantity);
            this.Controls.Add(_buttonReceive);
            this.Controls.Add(_buttonHistory);
            this.Controls.Add(_listBoxHistory);
        }

        private void LoadMaterials()
        {
            _listBoxMaterials.Items.Clear();
            var list = _materialService.GetAll();
            foreach (var m in list)
            {
                _listBoxMaterials.Items.Add($"{m.Id}: {m.Name} — {m.Quantity} {m.Unit}");
            }
            _listBoxHistory.Items.Clear();
            _selectedMaterial = null;
        }

        private void ButtonAddMaterial_Click(object sender, EventArgs e)
        {
            string name = _textBoxName.Text.Trim();
            string unit = _textBoxUnit.Text.Trim();
            if (!decimal.TryParse(_textBoxQuantity.Text.Trim(), out decimal qty))
            {
                MessageBox.Show("Неверное количество");
                return;
            }

            bool ok = _materialService.AddMaterial(name, qty, unit);
            if (ok)
            {
                MessageBox.Show("Материал добавлен");
                _textBoxName.Clear();
                _textBoxUnit.Clear();
                _textBoxQuantity.Clear();
                LoadMaterials();
            }
            else
            {
                MessageBox.Show("Ошибка добавления материала");
            }
        }

        private void ListBoxMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxMaterials.SelectedIndex < 0)
            {
                _selectedMaterial = null;
                _listBoxHistory.Items.Clear();
                return;
            }

            string item = _listBoxMaterials.Items[_listBoxMaterials.SelectedIndex].ToString();
            int id = int.Parse(item.Split(':')[0]);
            _selectedMaterial = _materialService.GetById(id);
            _listBoxHistory.Items.Clear();
        }

        private void ButtonReceive_Click(object sender, EventArgs e)
        {
            if (_selectedMaterial == null)
            {
                MessageBox.Show("Выберите материал");
                return;
            }

            decimal qty = _numericQuantity.Value;
            if (qty <= 0)
            {
                MessageBox.Show("Введите положительное количество");
                return;
            }

            try
            {
                var transactionService = new MaterialTransactionService();
                bool success = transactionService.RecordTransaction(_selectedMaterial.Id, qty, _currentUser.Id, null);

                if (success)
                {
                    MessageBox.Show($"Принято {qty} {_selectedMaterial.Unit} материала \"{_selectedMaterial.Name}\"");
                    _numericQuantity.Value = 0;
                    LoadMaterials();
                }
                else
                {
                    MessageBox.Show("Ошибка при приемке материала");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ButtonHistory_Click(object sender, EventArgs e)
        {
            if (_selectedMaterial == null)
            {
                MessageBox.Show("Выберите материал");
                return;
            }

            _listBoxHistory.Items.Clear();
        }
    }
}
