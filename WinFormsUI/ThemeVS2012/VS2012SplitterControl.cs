using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal class VS2012SplitterControl : DockPane.SplitterControlBase
    {
        private readonly SolidBrush _splitterBrush;


        public VS2012SplitterControl(DockPane pane)
            : base(pane)
        {
            _splitterBrush = new SolidBrush(pane.DockPanel.Skin.PanelSplitter);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = ClientRectangle;

            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            switch (Alignment)
            {
                case DockAlignment.Right:
                case DockAlignment.Left:
                    {
                        e.Graphics.FillRectangle(_splitterBrush, rect.X, rect.Y, Measures.SplitterSize, rect.Height);
                    }
                    break;
                case DockAlignment.Bottom:
                case DockAlignment.Top:
                    {
                        e.Graphics.FillRectangle(_splitterBrush, rect.X, rect.Y,
                                        rect.Width, Measures.SplitterSize);
                    }
                    break;
            }
        }
    }
}