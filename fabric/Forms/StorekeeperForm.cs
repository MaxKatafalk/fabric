using System;
using System.Drawing;
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
        private Material _selectedMaterial;
        private Panel _panelLeft;
        private Panel _panelRight;
        private Label _labelTitle;
        private Label _labelStock;
        private Label _labelAddTitle;
        private Label _labelReceiveTitle;
        private Label _labelMaterialInfo;

        public StorekeeperForm(User user)
        {
            _currentUser = user;
            _materialService = new MaterialService();
            InitializeComponent();
            LoadMaterials();
        }

        private void InitializeComponent()
        {
            this.Text = "Кладовщик - " + _currentUser.FullName;
            this.Width = 900;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Левая панель - список материалов
            _panelLeft = new Panel();
            _panelLeft.Left = 20;
            _panelLeft.Top = 20;
            _panelLeft.Width = 500;
            _panelLeft.Height = 600;
            _panelLeft.BackColor = Color.White;
            _panelLeft.BorderStyle = BorderStyle.FixedSingle;
            _panelLeft.Padding = new Padding(15);

            _labelTitle = new Label();
            _labelTitle.Text = "СКЛАД МАТЕРИАЛОВ";
            _labelTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            _labelTitle.ForeColor = Color.FromArgb(41, 128, 185);
            _labelTitle.AutoSize = true;
            _labelTitle.Top = 10;
            _labelTitle.Left = 15;

            _buttonRefresh = new Button();
            _buttonRefresh.Text = "⟳ Обновить";
            _buttonRefresh.Left = 350;
            _buttonRefresh.Top = 10;
            _buttonRefresh.Width = 120;
            _buttonRefresh.Height = 30;
            _buttonRefresh.Font = new Font("Arial", 9);
            _buttonRefresh.BackColor = Color.FromArgb(52, 152, 219);
            _buttonRefresh.ForeColor = Color.White;
            _buttonRefresh.FlatStyle = FlatStyle.Flat;
            _buttonRefresh.FlatAppearance.BorderSize = 0;
            _buttonRefresh.Cursor = Cursors.Hand;
            _buttonRefresh.Click += (object s, EventArgs e) => LoadMaterials();

            _listBoxMaterials = new ListBox();
            _listBoxMaterials.Left = 15;
            _listBoxMaterials.Top = 50;
            _listBoxMaterials.Width = 470;
            _listBoxMaterials.Height = 480;
            _listBoxMaterials.Font = new Font("Arial", 9);
            _listBoxMaterials.BorderStyle = BorderStyle.FixedSingle;
            _listBoxMaterials.BackColor = Color.FromArgb(250, 250, 250);
            _listBoxMaterials.SelectionMode = SelectionMode.One;
            _listBoxMaterials.SelectedIndexChanged += ListBoxMaterials_SelectedIndexChanged;

            _labelStock = new Label();
            _labelStock.Text = "Материалов на складе: 0";
            _labelStock.Font = new Font("Arial", 9);
            _labelStock.ForeColor = Color.FromArgb(127, 140, 141);
            _labelStock.AutoSize = true;
            _labelStock.Top = 540;
            _labelStock.Left = 15;

            // Правая панель - управление
            _panelRight = new Panel();
            _panelRight.Left = 540;
            _panelRight.Top = 20;
            _panelRight.Width = 340;
            _panelRight.Height = 600;
            _panelRight.BackColor = Color.White;
            _panelRight.BorderStyle = BorderStyle.FixedSingle;
            _panelRight.Padding = new Padding(15);

            _labelAddTitle = new Label();
            _labelAddTitle.Text = "ДОБАВЛЕНИЕ МАТЕРИАЛА";
            _labelAddTitle.Font = new Font("Arial", 12, FontStyle.Bold);
            _labelAddTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _labelAddTitle.AutoSize = true;
            _labelAddTitle.Top = 10;
            _labelAddTitle.Left = 15;

            int yPos = 50;

            Label labelName = new Label();
            labelName.Text = "Название:";
            labelName.Font = new Font("Arial", 9);
            labelName.ForeColor = Color.FromArgb(52, 73, 94);
            labelName.AutoSize = true;
            labelName.Top = yPos;
            labelName.Left = 15;

            _textBoxName = new TextBox();
            _textBoxName.Left = 15;
            _textBoxName.Top = yPos + 25;
            _textBoxName.Width = 310;
            _textBoxName.Height = 30;
            _textBoxName.Font = new Font("Arial", 9);
            _textBoxName.PlaceholderText = "Например: Хлопковая ткань";
            _textBoxName.BackColor = Color.White;
            _textBoxName.BorderStyle = BorderStyle.FixedSingle;

            yPos += 70;

            Label labelUnit = new Label();
            labelUnit.Text = "Единица измерения:";
            labelUnit.Font = new Font("Arial", 9);
            labelUnit.ForeColor = Color.FromArgb(52, 73, 94);
            labelUnit.AutoSize = true;
            labelUnit.Top = yPos;
            labelUnit.Left = 15;

            _textBoxUnit = new TextBox();
            _textBoxUnit.Left = 15;
            _textBoxUnit.Top = yPos + 25;
            _textBoxUnit.Width = 140;
            _textBoxUnit.Height = 30;
            _textBoxUnit.Font = new Font("Arial", 9);
            _textBoxUnit.PlaceholderText = "м/шт";
            _textBoxUnit.BackColor = Color.White;
            _textBoxUnit.BorderStyle = BorderStyle.FixedSingle;

            Label labelInitialQty = new Label();
            labelInitialQty.Text = "Начальное количество:";
            labelInitialQty.Font = new Font("Arial", 9);
            labelInitialQty.ForeColor = Color.FromArgb(52, 73, 94);
            labelInitialQty.AutoSize = true;
            labelInitialQty.Top = yPos;
            labelInitialQty.Left = 170;

            _textBoxQuantity = new TextBox();
            _textBoxQuantity.Left = 170;
            _textBoxQuantity.Top = yPos + 25;
            _textBoxQuantity.Width = 155;
            _textBoxQuantity.Height = 30;
            _textBoxQuantity.Font = new Font("Arial", 9);
            _textBoxQuantity.PlaceholderText = "0.00";
            _textBoxQuantity.BackColor = Color.White;
            _textBoxQuantity.BorderStyle = BorderStyle.FixedSingle;

            yPos += 70;

            _buttonAddMaterial = new Button();
            _buttonAddMaterial.Text = "➕ ДОБАВИТЬ МАТЕРИАЛ";
            _buttonAddMaterial.Left = 15;
            _buttonAddMaterial.Top = yPos;
            _buttonAddMaterial.Width = 310;
            _buttonAddMaterial.Height = 40;
            _buttonAddMaterial.Font = new Font("Arial", 10, FontStyle.Bold);
            _buttonAddMaterial.BackColor = Color.FromArgb(46, 204, 113);
            _buttonAddMaterial.ForeColor = Color.White;
            _buttonAddMaterial.FlatStyle = FlatStyle.Flat;
            _buttonAddMaterial.FlatAppearance.BorderSize = 0;
            _buttonAddMaterial.Cursor = Cursors.Hand;
            _buttonAddMaterial.Click += ButtonAddMaterial_Click;

            yPos += 80;

            _labelReceiveTitle = new Label();
            _labelReceiveTitle.Text = "ПРИЁМКА НА СКЛАД";
            _labelReceiveTitle.Font = new Font("Arial", 12, FontStyle.Bold);
            _labelReceiveTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _labelReceiveTitle.AutoSize = true;
            _labelReceiveTitle.Top = yPos;
            _labelReceiveTitle.Left = 15;

            yPos += 35;

            _labelMaterialInfo = new Label();
            _labelMaterialInfo.Text = "Выберите материал из списка";
            _labelMaterialInfo.Font = new Font("Arial", 9);
            _labelMaterialInfo.ForeColor = Color.FromArgb(127, 140, 141);
            _labelMaterialInfo.AutoSize = false;
            _labelMaterialInfo.Width = 310;
            _labelMaterialInfo.Height = 40;
            _labelMaterialInfo.Top = yPos;
            _labelMaterialInfo.Left = 15;
            _labelMaterialInfo.TextAlign = ContentAlignment.MiddleLeft;

            yPos += 50;

            Label labelReceiveQty = new Label();
            labelReceiveQty.Text = "Количество для приёмки:";
            labelReceiveQty.Font = new Font("Arial", 9);
            labelReceiveQty.ForeColor = Color.FromArgb(52, 73, 94);
            labelReceiveQty.AutoSize = true;
            labelReceiveQty.Top = yPos;
            labelReceiveQty.Left = 15;

            _numericQuantity = new NumericUpDown();
            _numericQuantity.Left = 15;
            _numericQuantity.Top = yPos + 25;
            _numericQuantity.Width = 140;
            _numericQuantity.Height = 30;
            _numericQuantity.Font = new Font("Arial", 9);
            _numericQuantity.Minimum = 0;
            _numericQuantity.Maximum = 1000000;
            _numericQuantity.DecimalPlaces = 2;
            _numericQuantity.BackColor = Color.White;
            _numericQuantity.BorderStyle = BorderStyle.FixedSingle;
            _numericQuantity.Enabled = false;

            _buttonReceive = new Button();
            _buttonReceive.Text = "📦 ПРИНЯТЬ";
            _buttonReceive.Left = 170;
            _buttonReceive.Top = yPos + 25;
            _buttonReceive.Width = 155;
            _buttonReceive.Height = 30;
            _buttonReceive.Font = new Font("Arial", 9, FontStyle.Bold);
            _buttonReceive.BackColor = Color.FromArgb(52, 152, 219);
            _buttonReceive.ForeColor = Color.White;
            _buttonReceive.FlatStyle = FlatStyle.Flat;
            _buttonReceive.FlatAppearance.BorderSize = 0;
            _buttonReceive.Cursor = Cursors.Hand;
            _buttonReceive.Enabled = false;
            _buttonReceive.Click += ButtonReceive_Click;

            // Добавление элементов на панели
            _panelLeft.Controls.Add(_labelTitle);
            _panelLeft.Controls.Add(_buttonRefresh);
            _panelLeft.Controls.Add(_listBoxMaterials);
            _panelLeft.Controls.Add(_labelStock);

            _panelRight.Controls.Add(_labelAddTitle);
            _panelRight.Controls.Add(labelName);
            _panelRight.Controls.Add(_textBoxName);
            _panelRight.Controls.Add(labelUnit);
            _panelRight.Controls.Add(_textBoxUnit);
            _panelRight.Controls.Add(labelInitialQty);
            _panelRight.Controls.Add(_textBoxQuantity);
            _panelRight.Controls.Add(_buttonAddMaterial);
            _panelRight.Controls.Add(_labelReceiveTitle);
            _panelRight.Controls.Add(_labelMaterialInfo);
            _panelRight.Controls.Add(labelReceiveQty);
            _panelRight.Controls.Add(_numericQuantity);
            _panelRight.Controls.Add(_buttonReceive);

            // Добавление панелей на форму
            this.Controls.Add(_panelLeft);
            this.Controls.Add(_panelRight);

            // Обработка клавиш
            this.KeyPreview = true;
            this.KeyDown += (object s, KeyEventArgs e) =>
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadMaterials();
                }
            };
        }

        private void LoadMaterials()
        {
            _listBoxMaterials.Items.Clear();
            _selectedMaterial = null;
            _buttonReceive.Enabled = false;
            _numericQuantity.Enabled = false;
            _labelMaterialInfo.Text = "Выберите материал из списка";
            _labelMaterialInfo.ForeColor = Color.FromArgb(127, 140, 141);

            System.Collections.Generic.List<Material> list = _materialService.GetAll();

            _labelStock.Text = "Материалов на складе: " + list.Count.ToString();

            if (list.Count == 0)
            {
                _listBoxMaterials.Items.Add("Нет материалов на складе");
                return;
            }

            foreach (Material m in list)
            {
                string status = "";
                if (m.Quantity <= 0)
                    status = " [НЕТ В НАЛИЧИИ]";
                else if (m.Quantity < 10)
                    status = " [МАЛО]";

                _listBoxMaterials.Items.Add($"[{m.Id}] {m.Name} — {m.Quantity} {m.Unit}{status}");
            }
        }

        private void ButtonAddMaterial_Click(object sender, EventArgs e)
        {
            string name = _textBoxName.Text.Trim();
            string unit = _textBoxUnit.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название материала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBoxName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(unit))
            {
                MessageBox.Show("Введите единицу измерения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBoxUnit.Focus();
                return;
            }

            if (!decimal.TryParse(_textBoxQuantity.Text.Trim(), out decimal qty) || qty < 0)
            {
                MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBoxQuantity.Focus();
                return;
            }

            bool ok = _materialService.AddMaterial(name, qty, unit);
            if (ok)
            {
                MessageBox.Show("Материал успешно добавлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _textBoxName.Clear();
                _textBoxUnit.Clear();
                _textBoxQuantity.Clear();
                LoadMaterials();
            }
            else
            {
                MessageBox.Show("Ошибка добавления материала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListBoxMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxMaterials.SelectedIndex < 0)
            {
                _selectedMaterial = null;
                _buttonReceive.Enabled = false;
                _numericQuantity.Enabled = false;
                _labelMaterialInfo.Text = "Выберите материал из списка";
                _labelMaterialInfo.ForeColor = Color.FromArgb(127, 140, 141);
                return;
            }

            string item = _listBoxMaterials.Items[_listBoxMaterials.SelectedIndex].ToString();
            if (item.Contains("[") && item.Contains("]"))
            {
                try
                {
                    int start = item.IndexOf('[') + 1;
                    int end = item.IndexOf(']');
                    int id = int.Parse(item.Substring(start, end - start));
                    _selectedMaterial = _materialService.GetById(id);

                    if (_selectedMaterial != null)
                    {
                        _labelMaterialInfo.Text = $"{_selectedMaterial.Name}\n" +
                                                $"На складе: {_selectedMaterial.Quantity} {_selectedMaterial.Unit}";
                        _labelMaterialInfo.ForeColor = Color.FromArgb(44, 62, 80);

                        _buttonReceive.Enabled = true;
                        _numericQuantity.Enabled = true;
                        _numericQuantity.Value = 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выборе материала: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ButtonReceive_Click(object sender, EventArgs e)
        {
            if (_selectedMaterial == null)
            {
                MessageBox.Show("Выберите материал", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal qty = _numericQuantity.Value;
            if (qty <= 0)
            {
                MessageBox.Show("Введите положительное количество", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _numericQuantity.Focus();
                return;
            }

            try
            {
                MaterialTransactionService transactionService = new MaterialTransactionService();
                bool success = transactionService.RecordTransaction(_selectedMaterial.Id, qty, _currentUser.Id, null);

                if (success)
                {
                    MessageBox.Show($"Принято {qty} {_selectedMaterial.Unit} материала \"{_selectedMaterial.Name}\"", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _numericQuantity.Value = 0;
                    LoadMaterials();
                }
                else
                {
                    MessageBox.Show("Ошибка при приёмке материала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}