using System;
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

        public SeamstressForm(User user)
        {
            _currentUser = user;
            _taskService = new ProductionTaskService();
            InitializeComponent();
            LoadMyTasks();
        }

        private void InitializeComponent()
        {
            this.Text = "Швея";
            this.Width = 700;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            _listBoxTasks = new ListBox();
            _listBoxTasks.Left = 20;
            _listBoxTasks.Top = 20;
            _listBoxTasks.Width = 640;
            _listBoxTasks.Height = 360;
            _listBoxTasks.SelectedIndexChanged += ListBoxTasks_SelectedIndexChanged;

            _numericDone = new NumericUpDown();
            _numericDone.Left = 20;
            _numericDone.Top = 400;
            _numericDone.Width = 120;
            _numericDone.Minimum = 1;
            _numericDone.Maximum = 1000000;

            _buttonMarkDone = new Button();
            _buttonMarkDone.Text = "Отметить выполнение";
            _buttonMarkDone.Left = 160;
            _buttonMarkDone.Top = 400;
            _buttonMarkDone.Width = 160;
            _buttonMarkDone.Click += ButtonMarkDone_Click;

            _buttonRefresh = new Button();
            _buttonRefresh.Text = "Обновить";
            _buttonRefresh.Left = 340;
            _buttonRefresh.Top = 400;
            _buttonRefresh.Width = 120;
            _buttonRefresh.Click += (s, e) => LoadMyTasks();

            this.Controls.Add(_listBoxTasks);
            this.Controls.Add(_numericDone);
            this.Controls.Add(_buttonMarkDone);
            this.Controls.Add(_buttonRefresh);
        }

        private void LoadMyTasks()
        {
            _listBoxTasks.Items.Clear();
            var list = _taskService.GetTasksByAssignedUser(_currentUser.Id);
            foreach (var t in list)
            {
                _listBoxTasks.Items.Add($"{t.Id}: Order:{t.OrderId} | {t.Description} | {t.QuantityCompleted}/{t.QuantityAssigned} | {t.Status}");
            }
        }

        private void ListBoxTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBoxTasks.SelectedIndex < 0)
            {
                _selectedTask = null;
                return;
            }

            string item = _listBoxTasks.Items[_listBoxTasks.SelectedIndex].ToString();
            int id = int.Parse(item.Split(':')[0]);
            _selectedTask = _taskService.GetById(id);
            if (_selectedTask != null)
            {
                int remaining = _selectedTask.QuantityAssigned - _selectedTask.QuantityCompleted;
                _numericDone.Maximum = Math.Max(1, remaining);
            }
        }

        private void ButtonMarkDone_Click(object sender, EventArgs e)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Выберите задание");
                return;
            }

            int qty = (int)_numericDone.Value;
            bool ok = _taskService.UpdateProgress(_selectedTask.Id, qty, _currentUser.Id);
            if (ok)
            {
                MessageBox.Show("Отмечено");
                LoadMyTasks();
                fabric.AppEvents.RaiseOrderStatusChanged();
            }
            else
            {
                MessageBox.Show("Ошибка при отметке выполнения. Возможно, не хватает материала на складе.");
            }
        }
    }
}
