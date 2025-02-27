using System;
using System.Drawing;
using System.Windows.Forms;

namespace BDO_Boss_Timer
{
    public partial class SettingsForm : Form
    {
        private BossTimerForm parentForm;
        private TrackBar opacityTrackBar;
        private Label opacityValueLabel;
        private ComboBox serverComboBox;

        public SettingsForm(BossTimerForm parent, TimerSettings settings)
        {
            parentForm = parent;
            InitializeUI();
            UpdateControls(settings);
        }

        // Method to update UI controls based on settings
        public void UpdateControls(TimerSettings settings)
        {
            if (settings == null) return;

            // Update server selection
            if (!string.IsNullOrEmpty(settings.Server))
            {
                int index = serverComboBox.FindStringExact(settings.Server.ToLower());
                if (index >= 0)
                {
                    serverComboBox.SelectedIndex = index;
                }
            }

            // Update opacity
            if (settings.Opacity >= opacityTrackBar.Minimum && settings.Opacity <= opacityTrackBar.Maximum)
            {
                opacityTrackBar.Value = settings.Opacity;
                opacityValueLabel.Text = $"{settings.Opacity}%";
            }
        }

        private void InitializeUI()
        {
            // Form settings
            this.Text = "Settings";
            this.Size = new Size(180, 160);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 8F);

            int padding = 10;
            int controlTop = padding;
            int controlWidth = this.ClientSize.Width - (padding * 2);

            // Server selection
            Label serverLabel = new Label();
            serverLabel.Text = "Server:";
            serverLabel.Location = new Point(padding, controlTop);
            serverLabel.AutoSize = true;
            this.Controls.Add(serverLabel);

            controlTop += serverLabel.Height + 2;

            serverComboBox = new ComboBox();
            serverComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            serverComboBox.Items.AddRange(new string[] { "EU", "NA", "RU", "JP", "KR", "TW", "SA", "MENA", "ASIA" });
            serverComboBox.Location = new Point(padding, controlTop);
            serverComboBox.Width = controlWidth;
            serverComboBox.BackColor = Color.FromArgb(50, 50, 50);
            serverComboBox.ForeColor = Color.White;
            serverComboBox.FlatStyle = FlatStyle.Flat;
            serverComboBox.SelectedIndex = 0; // Default to mena
            serverComboBox.SelectedIndexChanged += ServerComboBox_SelectedIndexChanged;
            this.Controls.Add(serverComboBox);

            controlTop += serverComboBox.Height + padding;

            // Opacity control
            Label opacityHeaderLabel = new Label();
            opacityHeaderLabel.Text = "Opacity:";
            opacityHeaderLabel.Location = new Point(padding, controlTop);
            opacityHeaderLabel.AutoSize = true;
            this.Controls.Add(opacityHeaderLabel);

            opacityValueLabel = new Label();
            opacityValueLabel.Text = "90%";
            opacityValueLabel.TextAlign = ContentAlignment.MiddleRight;
            opacityValueLabel.Location = new Point(this.ClientSize.Width - 40, controlTop);
            opacityValueLabel.Size = new Size(30, opacityHeaderLabel.Height);
            this.Controls.Add(opacityValueLabel);

            controlTop += opacityHeaderLabel.Height + 2;

            opacityTrackBar = new TrackBar();
            opacityTrackBar.Minimum = 20;
            opacityTrackBar.Maximum = 100;
            opacityTrackBar.Value = 90;
            opacityTrackBar.TickFrequency = 20;
            opacityTrackBar.Location = new Point(padding, controlTop);
            opacityTrackBar.Width = controlWidth;
            opacityTrackBar.ValueChanged += OpacityTrackBar_ValueChanged;
            this.Controls.Add(opacityTrackBar);

            controlTop += opacityTrackBar.Height + padding;

            // Close button
            Button closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.Size = new Size(80, 23);
            closeButton.Location = new Point(this.ClientSize.Width - 80 - padding, controlTop);
            closeButton.BackColor = Color.FromArgb(60, 60, 60);
            closeButton.ForeColor = Color.White;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Hide();
            this.Controls.Add(closeButton);

            // Form size based on controls
            this.ClientSize = new Size(this.ClientSize.Width, controlTop + closeButton.Height + padding);

            // Handle form closing to hide instead of close
            this.FormClosing += (s, e) => {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    this.Hide();
                }
            };
        }

        private void ServerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedServer = serverComboBox.SelectedItem.ToString();
            parentForm.ChangeServer(selectedServer);
        }

        private void OpacityTrackBar_ValueChanged(object sender, EventArgs e)
        {
            double opacityValue = opacityTrackBar.Value / 100.0;
            opacityValueLabel.Text = $"{opacityTrackBar.Value}%";
            parentForm.ChangeOpacity(opacityValue);
        }
    }
}