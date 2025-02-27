using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using FontAwesome.Sharp;
using System.Drawing;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace BDO_Boss_Timer
{
    public partial class BossTimerForm : Form
    {
        private WebView2 webView;
        private IconPictureBox lockIcon;
        private IconPictureBox settingsIcon;
        private IconPictureBox moveIcon;
        private bool isDragging = false;
        private Point lastCursor;
        private Point lastForm;
        private bool isLocked = false;
        private Panel topPanel;
        private SettingsForm settingsForm;
        private string currentServer = "mena";
        private TimerSettings currentSettings;
        private Timer positionSaveTimer;

        public BossTimerForm()
        {
            InitializeComponent();

            LoadSettings();

            // Reset opacity to 100 if it's 0
            if (currentSettings.Opacity == 0)
            {
                currentSettings.Opacity = 100;
            }


            SetStartupPosition();
            InitializeUI();
            InitializeWebView();
            SetPositionAgain();
            SetupPositionSaveTimer();
        }

        private void SetupPositionSaveTimer()
        {
            positionSaveTimer = new Timer();
            positionSaveTimer.Interval = 3000;
            positionSaveTimer.Tick += (s, e) => SaveSettings();
            positionSaveTimer.Start();
        }

        private void SetStartupPosition()
        {
            try
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(currentSettings.WindowX, currentSettings.WindowY);
                this.Size = new Size(currentSettings.WindowWidth, currentSettings.WindowHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting startup position: {ex.Message}");
            }
        }

        private void SetPositionAgain()
        {
            try
            {
                this.StartPosition = FormStartPosition.Manual;

                if (currentSettings.WindowX > 0 && currentSettings.WindowY > 0)
                {
                    this.Location = new Point(currentSettings.WindowX, currentSettings.WindowY);
                    this.Size = new Size(currentSettings.WindowWidth, currentSettings.WindowHeight);
                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error re-setting position: {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            currentSettings = SettingsManager.LoadSettings();
            currentServer = currentSettings.Server;
            isLocked = currentSettings.IsLocked;
        }

        private void ApplySettings()
        {
            try
            {
                bool isVisiblePosition = false;
                foreach (Screen screen in Screen.AllScreens)
                {
                    Rectangle adjustedBounds = screen.WorkingArea;
                    adjustedBounds.Inflate(-50, -50);

                    if (adjustedBounds.Contains(new Point(currentSettings.WindowX, currentSettings.WindowY)))
                    {
                        isVisiblePosition = true;
                        break;
                    }
                }

                if (isVisiblePosition && currentSettings.WindowX > 0 && currentSettings.WindowY > 0)
                {
                    this.Location = new Point(currentSettings.WindowX, currentSettings.WindowY);
                }
                else
                {
                    this.StartPosition = FormStartPosition.CenterScreen;
                }

                this.Size = new Size(
                    Math.Max(currentSettings.WindowWidth, 200),
                    Math.Max(currentSettings.WindowHeight, 100));

                this.Opacity = currentSettings.Opacity / 100.0;

                if (lockIcon != null)
                {
                    lockIcon.IconChar = isLocked ? IconChar.Lock : IconChar.LockOpen;
                    if (moveIcon != null)
                    {
                        moveIcon.Enabled = !isLocked;
                        moveIcon.IconColor = isLocked ? Color.Gray : Color.White;
                    }
                }

                if (settingsForm != null && !settingsForm.IsDisposed)
                {
                    settingsForm.UpdateControls(currentSettings);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying settings: {ex.Message}");
            }
        }

        private void SaveSettings()
        {
            try
            {
                if (!this.Visible || this.WindowState == FormWindowState.Minimized)
                    return;

                currentSettings.WindowX = this.Location.X;
                currentSettings.WindowY = this.Location.Y;
                currentSettings.WindowWidth = this.Width;
                currentSettings.WindowHeight = this.Height;
                currentSettings.Opacity = (int)(this.Opacity * 100);
                currentSettings.Server = currentServer;
                currentSettings.IsLocked = isLocked;

                SettingsManager.SaveSettings(currentSettings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private void InitializeUI()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.Opacity = currentSettings.Opacity / 100.0;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            // Create a clickable transparent top area
            topPanel = new Panel();
            topPanel.Height = 20;
            topPanel.Dock = DockStyle.Top;
            topPanel.BackColor = Color.Black;
            this.Controls.Add(topPanel);

            // Close button
            IconPictureBox closeIcon = new IconPictureBox();
            closeIcon.IconChar = IconChar.Times;
            closeIcon.IconSize = 16;
            closeIcon.IconColor = Color.White;
            closeIcon.BackColor = Color.Transparent;
            closeIcon.Size = new Size(16, 16);
            closeIcon.Location = new Point(3, 2);
            closeIcon.Cursor = Cursors.Hand;
            closeIcon.Click += (s, e) => {
                SaveSettings();
                Application.Exit();
            };
            topPanel.Controls.Add(closeIcon);

            // Lock icon
            lockIcon = new IconPictureBox();
            lockIcon.IconChar = isLocked ? IconChar.Lock : IconChar.LockOpen;
            lockIcon.IconSize = 16;
            lockIcon.IconColor = Color.White;
            lockIcon.BackColor = Color.Transparent;
            lockIcon.Size = new Size(16, 16);
            lockIcon.Location = new Point(25, 2);
            lockIcon.Cursor = Cursors.Hand;
            lockIcon.Click += LockIcon_Click;
            topPanel.Controls.Add(lockIcon);

            // Settings icon
            settingsIcon = new IconPictureBox();
            settingsIcon.IconChar = IconChar.Cog;
            settingsIcon.IconSize = 16;
            settingsIcon.IconColor = Color.White;
            settingsIcon.BackColor = Color.Transparent;
            settingsIcon.Size = new Size(16, 16);
            settingsIcon.Location = new Point(47, 2);
            settingsIcon.Cursor = Cursors.Hand;
            settingsIcon.Click += SettingsIcon_Click;
            topPanel.Controls.Add(settingsIcon);

            // Move icon
            moveIcon = new IconPictureBox();
            moveIcon.IconChar = IconChar.Arrows;
            moveIcon.IconSize = 16;
            moveIcon.IconColor = isLocked ? Color.Gray : Color.White;
            moveIcon.BackColor = Color.Transparent;
            moveIcon.Size = new Size(16, 16);
            moveIcon.Location = new Point(69, 2);
            moveIcon.Cursor = Cursors.SizeAll;
            moveIcon.Enabled = !isLocked;
            moveIcon.MouseDown += MoveIcon_MouseDown;
            moveIcon.MouseMove += MoveIcon_MouseMove;
            moveIcon.MouseUp += MoveIcon_MouseUp;
            topPanel.Controls.Add(moveIcon);

            // Resize handle
            SizeGrip sizeGrip = new SizeGrip();
            sizeGrip.Width = 10;
            sizeGrip.Height = 10;
            sizeGrip.Dock = DockStyle.Bottom;
            sizeGrip.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(sizeGrip);

            // Wire up events
            this.Resize += BossTimerForm_Resize;
            this.FormClosing += BossTimerForm_FormClosing;
            this.Shown += BossTimerForm_Shown;
            this.LocationChanged += BossTimerForm_LocationChanged;
            this.Move += BossTimerForm_Move;

            // Create settings form
            settingsForm = new SettingsForm(this, currentSettings);
        }

        private void BossTimerForm_Move(object sender, EventArgs e)
        {
            if (!isDragging)
                SaveSettings();
        }

        private void BossTimerForm_LocationChanged(object sender, EventArgs e)
        {
            if (!isDragging)
                SaveSettings();
        }

        private void BossTimerForm_Shown(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void BossTimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (positionSaveTimer != null)
            {
                positionSaveTimer.Stop();
                positionSaveTimer.Dispose();
            }

            SaveSettings();

            try
            {
                string settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "BDO_Boss_Timer",
                    "settings.xml");

                string xml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<TimerSettings xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <WindowX>{this.Location.X}</WindowX>
  <WindowY>{this.Location.Y}</WindowY>
  <WindowWidth>{this.Width}</WindowWidth>
  <WindowHeight>{this.Height}</WindowHeight>
  <Server>{currentServer}</Server>
  <Opacity>{(int)(this.Opacity * 100)}</Opacity>
  <IsLocked>{isLocked.ToString().ToLower()}</IsLocked>
</TimerSettings>";

                string directory = Path.GetDirectoryName(settingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(settingsPath + ".backup", xml);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing backup settings: {ex.Message}");
            }
        }

        public void ChangeServer(string server)
        {
            currentServer = server;
            if (webView != null && webView.CoreWebView2 != null)
            {
                string url = $"https://garmoth.com/stream-element/boss-timer/{server}/x";
                webView.CoreWebView2.Navigate(url);
            }

            SaveSettings();
        }

        public void ChangeOpacity(double opacity)
        {
            this.Opacity = opacity;
            SaveSettings();
        }

        private void SettingsIcon_Click(object sender, EventArgs e)
        {
            Point location = this.PointToScreen(new Point(this.Width, 0));
            settingsForm.Location = location;

            if (!settingsForm.Visible)
            {
                settingsForm.Show();
            }
            else
            {
                settingsForm.BringToFront();
            }
        }

        private void BossTimerForm_Resize(object sender, EventArgs e)
        {
            if (webView != null)
            {
                webView.Size = new Size(this.Width, this.Height - topPanel.Height);
            }
        }

        private async void InitializeWebView()
        {
            webView = new WebView2();
            webView.Location = new Point(0, topPanel.Height);
            webView.Size = new Size(this.Width, this.Height - topPanel.Height);
            webView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(webView);

            try
            {
                await webView.EnsureCoreWebView2Async(null);

                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                webView.DefaultBackgroundColor = Color.Transparent;

                string url = $"https://garmoth.com/stream-element/boss-timer/{currentServer}/x";
                webView.CoreWebView2.Navigate(url);

                webView.CoreWebView2.NavigationCompleted += async (s, e) =>
                {
                    await Task.Delay(500);
                    await MakeTransparent();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task MakeTransparent()
        {
            string script = @"
            document.body.style.backgroundColor = 'transparent';
            document.documentElement.style.backgroundColor = 'transparent';
            
            var elements = document.querySelectorAll('*');
            for (var i = 0; i < elements.length; i++) {
                var style = window.getComputedStyle(elements[i]);
                if (style.backgroundColor && style.backgroundColor !== 'transparent') {
                    elements[i].style.backgroundColor = 'transparent';
                }
            }";

            await webView.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void LockIcon_Click(object sender, EventArgs e)
        {
            isLocked = !isLocked;
            lockIcon.IconChar = isLocked ? IconChar.Lock : IconChar.LockOpen;
            moveIcon.Enabled = !isLocked;
            moveIcon.IconColor = isLocked ? Color.Gray : Color.White;

            SaveSettings();
        }

        private void MoveIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isLocked && e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursor = Cursor.Position;
                lastForm = this.Location;
            }
        }

        private void MoveIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point delta = new Point(
                    Cursor.Position.X - lastCursor.X,
                    Cursor.Position.Y - lastCursor.Y);

                this.Location = new Point(
                    lastForm.X + delta.X,
                    lastForm.Y + delta.Y);
            }
        }

        private void MoveIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                SaveSettings();
            }
        }
    }
}