using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using fabric.BLL.Services;
using fabric.DAL.Models;

namespace fabric.Forms
{
    public class SeamstressForm : Form
    {
        private User _currentUser;
        private ProductionTaskService _taskService;
        private ListBox _listBoxTasks;
        private NumericUpDown _numericDone;
        private Button _buttonMarkDone;
        private Button _buttonRefresh;
        private ProductionTask _selectedTask;
        private Panel _panelMain;
        private Label _labelTitle;
        private Label _labelCount;
        private Panel _panelDetails;
        private Label _labelSelectedInfo;

        public SeamstressForm(User user)
        {
            _currentUser = user;
            _taskService = new ProductionTaskService();
            InitializeComponent();
            LoadMyTasks();
        }

        private void InitializeComponent()
        {
            this.Text = "Швея - " + _currentUser.FullName;
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Основная панель
            _panelMain = new Panel();
            _panelMain.Left = 10;
            _panelMain.Top = 10;
            _panelMain.Width = 770;
            _panelMain.Height = 550;
            _panelMain.BackColor = Color.White;
            _panelMain.BorderStyle = BorderStyle.FixedSingle;
            _panelMain.Padding = new Padding(15);

            // Заголовок
            _labelTitle = new Label();
            _labelTitle.Text = "МОИ ЗАДАНИЯ";
            _labelTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            _labelTitle.ForeColor = Color.FromArgb(41, 128, 185);
            _labelTitle.AutoSize = true;
            _labelTitle.Top = 10;
            _labelTitle.Left = 15;

            // Счетчик заданий
            _labelCount = new Label();
            _labelCount.Text = "Заданий: 0";
            _labelCount.Font = new Font("Arial", 9);
            _labelCount.ForeColor = Color.FromArgb(127, 140, 141);
            _labelCount.AutoSize = true;
            _labelCount.Top = 15;
            _labelCount.Left = 650;

            // Список заданий
            _listBoxTasks = new ListBox();
            _listBoxTasks.Left = 15;
            _listBoxTasks.Top = 50;
            _listBoxTasks.Width = 740;
            _listBoxTasks.Height = 320;
            _listBoxTasks.Font = new Font("Arial", 9);
            _listBoxTasks.BorderStyle = BorderStyle.FixedSingle;
            _listBoxTasks.BackColor = Color.FromArgb(250, 250, 250);
            _listBoxTasks.SelectionMode = SelectionMode.One;
            _listBoxTasks.SelectedIndexChanged += ListBoxTasks_SelectedIndexChanged;

            // Панель с информацией о выбранном задании
            _panelDetails = new Panel();
            _panelDetails.Left = 15;
            _panelDetails.Top = 380;
            _panelDetails.Width = 740;
            _panelDetails.Height = 80;
            _panelDetails.BackColor = Color.FromArgb(248, 249, 250);
            _panelDetails.BorderStyle = BorderStyle.FixedSingle;
            _panelDetails.Padding = new Padding(10);

            _labelSelectedInfo = new Label();
            _labelSelectedInfo.Text = "Выберите задание для просмотра деталей";
            _labelSelectedInfo.Font = new Font("Arial", 9);
            _labelSelectedInfo.ForeColor = Color.FromArgb(127, 140, 141);
            _labelSelectedInfo.AutoSize = false;
            _labelSelectedInfo.Width = 720;
            _labelSelectedInfo.Height = 60;
            _labelSelectedInfo.TextAlign = ContentAlignment.MiddleCenter;

            // Поле для ввода количества
            Label labelQuantity = new Label();
            labelQuantity.Text = "Выполнено изделий:";
            labelQuantity.Font = new Font("Arial", 9);
            labelQuantity.ForeColor = Color.FromArgb(52, 73, 94);
            labelQuantity.AutoSize = true;
            labelQuantity.Top = 470;
            labelQuantity.Left = 15;

            _numericDone = new NumericUpDown();
            _numericDone.Left = 140;
            _numericDone.Top = 467;
            _numericDone.Width = 120;
            _numericDone.Height = 26;
            _numericDone.Font = new Font("Arial", 9);
            _numericDone.Minimum = 1;
            _numericDone.Maximum = 1000000;
            _numericDone.BackColor = Color.White;
            _numericDone.BorderStyle = BorderStyle.FixedSingle;

            // Кнопка отметки выполнения
            _buttonMarkDone = new Button();
            _buttonMarkDone.Text = "✓ Отметить выполнение";
            _buttonMarkDone.Left = 280;
            _buttonMarkDone.Top = 467;
            _buttonMarkDone.Width = 180;
            _buttonMarkDone.Height = 30;
            _buttonMarkDone.Font = new Font("Arial", 9, FontStyle.Bold);
            _buttonMarkDone.BackColor = Color.FromArgb(46, 204, 113);
            _buttonMarkDone.ForeColor = Color.White;
            _buttonMarkDone.FlatStyle = FlatStyle.Flat;
            _buttonMarkDone.FlatAppearance.BorderSize = 0;
            _buttonMarkDone.Cursor = Cursors.Hand;
            _buttonMarkDone.Click += ButtonMarkDone_Click;
            _buttonMarkDone.Enabled = false;

            // Кнопка обновления
            _buttonRefresh = new Button();
            _buttonRefresh.Text = "⟳ Обновить список";
            _buttonRefresh.Left = 480;
            _buttonRefresh.Top = 467;
            _buttonRefresh.Width = 150;
            _buttonRefresh.Height = 30;
            _buttonRefresh.Font = new Font("Arial", 9);
            _buttonRefresh.BackColor = Color.FromArgb(52, 152, 219);
            _buttonRefresh.ForeColor = Color.White;
            _buttonRefresh.FlatStyle = FlatStyle.Flat;
            _buttonRefresh.FlatAppearance.BorderSize = 0;
            _buttonRefresh.Cursor = Cursors.Hand;
            _buttonRefresh.Click += (object s, EventArgs e) => LoadMyTasks();

            // Добавление элементов на панель
            _panelDetails.Controls.Add(_labelSelectedInfo);

            _panelMain.Controls.Add(_labelTitle);
            _panelMain.Controls.Add(_labelCount);
            _panelMain.Controls.Add(_listBoxTasks);
            _panelMain.Controls.Add(_panelDetails);
            _panelMain.Controls.Add(labelQuantity);
            _panelMain.Controls.Add(_numericDone);
            _panelMain.Controls.Add(_buttonMarkDone);
            _panelMain.Controls.Add(_buttonRefresh);

            // Добавление панели на форму
            this.Controls.Add(_panelMain);

            // Обработка клавиш
            this.KeyPreview = true;
            this.KeyDown += (object s, KeyEventArgs e) =>
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadMyTasks();
                }
            };
        }

        private void LoadMyTasks()
        {
            _listBoxTasks.Items.Clear();
            System.Collections.Generic.List<ProductionTask> list = _taskService.GetTasksByAssignedUser(_currentUser.Id);

            _labelCount.Text = "Заданий: " + list.Count.ToString();

            if (list.Count == 0)
            {
                _listBoxTasks.Items.Add("Нет назначенных заданий");
                _buttonMarkDone.Enabled = false;
                return;
            }

            foreach (ProductionTask t in list)
            {
                string status = t.Status.ToString();
                string statusRu = GetTaskStatusRussian(status);
                string progress = t.QuantityCompleted.ToString() + "/" + t.QuantityAssigned.ToString();
                _listBoxTasks.Items.Add($"[{t.Id}] Заказ #{t.OrderId} | {t.Description} | Выполнено: {progress} | {statusRu}");
            }
        }

        private string GetTaskStatusRussian(string status)
        {
            status = status.ToLower();

            if (status.Contains("pending") || status.Contains("ожидан"))
                return "Ожидание";
            else if (status.Contains("assigned") || status.Contains("назначен") )
                return "Назначен";
            else if (status.Contains("progress") || status.Contains("работе") || status.Contains("выполнен"))
                return "В работе";
            else if (status.Contains("completed") || status.Contains("завершен"))
                return "Завершено";
            else
                return status;
        }

        private void ListBoxTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxTasks.SelectedIndex < 0)
            {
                _selectedTask = null;
                _buttonMarkDone.Enabled = false;
                _labelSelectedInfo.Text = "Выберите задание для просмотра деталей";
                _labelSelectedInfo.ForeColor = Color.FromArgb(127, 140, 141);
                return;
            }

            string item = _listBoxTasks.Items[_listBoxTasks.SelectedIndex].ToString();
            if (item.Contains("[") && item.Contains("]"))
            {
                try
                {
                    int start = item.IndexOf('[') + 1;
                    int end = item.IndexOf(']');
                    int id = int.Parse(item.Substring(start, end - start));
                    _selectedTask = _taskService.GetById(id);

                    if (_selectedTask != null)
                    {
                        int remaining = _selectedTask.QuantityAssigned - _selectedTask.QuantityCompleted;
                        _numericDone.Maximum = Math.Max(1, remaining);
                        _numericDone.Value = 1;

                        string status = _selectedTask.Status.ToString();
                        string statusRu = GetTaskStatusRussian(status);

                        _labelSelectedInfo.Text = $"Задание #{_selectedTask.Id} | Заказ #{_selectedTask.OrderId}\n" +
                                                $"Описание: {_selectedTask.Description}\n" +
                                                $"Прогресс: {_selectedTask.QuantityCompleted}/{_selectedTask.QuantityAssigned} | Статус: {statusRu}";
                        _labelSelectedInfo.ForeColor = Color.FromArgb(44, 62, 80);

                        // Активируем кнопку только если задание еще не завершено
                        _buttonMarkDone.Enabled = (_selectedTask.QuantityCompleted < _selectedTask.QuantityAssigned);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выборе задания: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ButtonMarkDone_Click(object sender, EventArgs e)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Выберите задание", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int qty = (int)_numericDone.Value;
            bool ok = _taskService.UpdateProgress(_selectedTask.Id, qty, _currentUser.Id);
            if (ok)
            {
                MessageBox.Show("Выполнение успешно отмечено", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMyTasks();
                fabric.AppEvents.RaiseOrderStatusChanged();
            }
            else
            {
                MessageBox.Show("Ошибка при отметке выполнения. Возможно, не хватает материала на складе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}