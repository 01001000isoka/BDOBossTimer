using System;
using System.Drawing;
using System.Windows.Forms;

namespace BDO_Boss_Timer
{
    public partial class SizeGrip : UserControl
    {
        private const int WM_NCHITTEST = 0x0084;
        private const int HTBOTTOMRIGHT = 17;

        public SizeGrip()
        {
            InitializeComponent();

            this.Width = 10;
            this.Height = 10;
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Cursor = Cursors.SizeNWSE;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw diagonal resize lines
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                for (int i = 0; i < 2; i++)
                {
                    int offset = 3 * i + 2;
                    e.Graphics.FillRectangle(brush, this.Width - offset, this.Height - 2, 1, 1);
                    e.Graphics.FillRectangle(brush, this.Width - 2, this.Height - offset, 1, 1);
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTBOTTOMRIGHT;
                return;
            }
            base.WndProc(ref m);
        }
    }
}