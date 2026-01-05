using System;
using System.Windows.Forms;
using fabric.DAL.Models;

namespace fabric.Forms
{
    public class StorekeeperForm : Form
    {
        private Label labelHeader;
        private User _user;

        public StorekeeperForm(User user)
        {
            _user = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.labelHeader = new System.Windows.Forms.Label();
            this.SuspendLayout();
            this.labelHeader.AutoSize = true;
            this.labelHeader.Location = new System.Drawing.Point(24, 22);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(120, 13);
            this.labelHeader.Text = "Storekeeper Panel";
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.labelHeader);
            this.Name = "StorekeeperForm";
            this.Text = "Storekeeper";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
