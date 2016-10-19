using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    using WeifenLuo.WinFormsUI.ThemeSolarizedLight;

    /// <summary>
    /// Visual Studio 2012 Light theme.
    /// </summary>
    public class SolarizedLightTheme : ThemeBase
    {
        /// <summary>
        /// Applies the specified theme to the dock panel.
        /// </summary>
        /// <param name="dockPanel">The dock panel.</param>
        public override void Apply(DockPanel dockPanel)
        {
            if (dockPanel == null)
            {
                throw new NullReferenceException("dockPanel");
            }

            Measures.SplitterSize = 6;
            dockPanel.Extender.DockPaneCaptionFactory = new SolarizedLightDockPaneCaptionFactory();
            dockPanel.Extender.AutoHideStripFactory = new SolarizedLightAutoHideStripFactory();
            dockPanel.Extender.AutoHideWindowFactory = new SolarizedLightAutoHideWindowFactory();
            dockPanel.Extender.DockPaneStripFactory = new SolarizedLightDockPaneStripFactory();
            dockPanel.Extender.DockPaneSplitterControlFactory = new SolarizedLightDockPaneSplitterControlFactory();
            dockPanel.Extender.DockWindowSplitterControlFactory = new SolarizedLightDockWindowSplitterControlFactory();
            dockPanel.Extender.DockWindowFactory = new SolarizedLightDockWindowFactory();
            dockPanel.Extender.PaneIndicatorFactory = new SolarizedLightPaneIndicatorFactory();
            dockPanel.Extender.PanelIndicatorFactory = new SolarizedLightPanelIndicatorFactory();
            dockPanel.Extender.DockOutlineFactory = new SolarizedLightDockOutlineFactory();
            dockPanel.Skin = CreateSolarizedLight();
        }

        private class SolarizedLightDockOutlineFactory : DockPanelExtender.IDockOutlineFactory
        {
            public DockOutlineBase CreateDockOutline()
            {
                return new SolarizedLightDockOutline();
            }

            private class SolarizedLightDockOutline : DockOutlineBase
            {
                public SolarizedLightDockOutline()
                {
                    m_dragForm = new DragForm();
                    SetDragForm(Rectangle.Empty);
                    DragForm.BackColor = Color.FromArgb(0xff, 91, 173, 255);
                    DragForm.Opacity = 0.5;
                }

                DragForm m_dragForm;
                private DragForm DragForm
                {
                    get { return m_dragForm; }
                }

                protected override void OnShow()
                {
                    CalculateRegion();
                }

                protected override void OnHide()
                {
					DragForm.Visible = false;
                }

                protected override void OnClose()
                {
                    DragForm.Close();
                }

                private void CalculateRegion()
                {
                    if (SameAsOldValue)
                        return;

                    if (!FloatWindowBounds.IsEmpty)
                        SetOutline(FloatWindowBounds);
                    else if (DockTo is DockPanel)
                        SetOutline(DockTo as DockPanel, Dock, (ContentIndex != 0));
                    else if (DockTo is DockPane)
                        SetOutline(DockTo as DockPane, Dock, ContentIndex);
                    else
                        SetOutline();
                }

                private void SetOutline()
                {
                    SetDragForm(Rectangle.Empty);
                }

                private void SetOutline(Rectangle floatWindowBounds)
                {
                    SetDragForm(floatWindowBounds);
                }

                private void SetOutline(DockPanel dockPanel, DockStyle dock, bool fullPanelEdge)
                {
                    Rectangle rect = fullPanelEdge ? dockPanel.DockArea : dockPanel.DocumentWindowBounds;
                    rect.Location = dockPanel.PointToScreen(rect.Location);
                    if (dock == DockStyle.Top)
                    {
                        int height = dockPanel.GetDockWindowSize(DockState.DockTop);
                        rect = new Rectangle(rect.X, rect.Y, rect.Width, height);
                    }
                    else if (dock == DockStyle.Bottom)
                    {
                        int height = dockPanel.GetDockWindowSize(DockState.DockBottom);
                        rect = new Rectangle(rect.X, rect.Bottom - height, rect.Width, height);
                    }
                    else if (dock == DockStyle.Left)
                    {
                        int width = dockPanel.GetDockWindowSize(DockState.DockLeft);
                        rect = new Rectangle(rect.X, rect.Y, width, rect.Height);
                    }
                    else if (dock == DockStyle.Right)
                    {
                        int width = dockPanel.GetDockWindowSize(DockState.DockRight);
                        rect = new Rectangle(rect.Right - width, rect.Y, width, rect.Height);
                    }
                    else if (dock == DockStyle.Fill)
                    {
                        rect = dockPanel.DocumentWindowBounds;
                        rect.Location = dockPanel.PointToScreen(rect.Location);
                    }

                    SetDragForm(rect);
                }

                private void SetOutline(DockPane pane, DockStyle dock, int contentIndex)
                {
                    if (dock != DockStyle.Fill)
                    {
                        Rectangle rect = pane.DisplayingRectangle;
                        if (dock == DockStyle.Right)
                            rect.X += rect.Width / 2;
                        if (dock == DockStyle.Bottom)
                            rect.Y += rect.Height / 2;
                        if (dock == DockStyle.Left || dock == DockStyle.Right)
                            rect.Width -= rect.Width / 2;
                        if (dock == DockStyle.Top || dock == DockStyle.Bottom)
                            rect.Height -= rect.Height / 2;
                        rect.Location = pane.PointToScreen(rect.Location);

                        SetDragForm(rect);
                    }
                    else if (contentIndex == -1)
                    {
                        Rectangle rect = pane.DisplayingRectangle;
                        rect.Location = pane.PointToScreen(rect.Location);
                        SetDragForm(rect);
                    }
                    else
                    {
                        using (GraphicsPath path = pane.TabStripControl.GetOutline(contentIndex))
                        {
                            RectangleF rectF = path.GetBounds();
                            Rectangle rect = new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
                            using (Matrix matrix = new Matrix(rect, new Point[] { new Point(0, 0), new Point(rect.Width, 0), new Point(0, rect.Height) }))
                            {
                                path.Transform(matrix);
                            }

                            Region region = new Region(path);
                            SetDragForm(rect, region);
                        }
                    }
                }

                private void SetDragForm(Rectangle rect)
                {
				    DragForm.Show( false );
                    DragForm.Bounds = rect;
                    if (rect == Rectangle.Empty)
                    {
                        if (DragForm.Region != null)
                        {
                            DragForm.Region.Dispose();
                        }

                        DragForm.Region = new Region(Rectangle.Empty);
                    }
                    else if (DragForm.Region != null)
                    {
                        DragForm.Region.Dispose();
                        DragForm.Region = null;
                    }
                }

                private void SetDragForm(Rectangle rect, Region region)
                {
				    DragForm.Show( false );
                    DragForm.Bounds = rect;
                    if (DragForm.Region != null)
                    {
                        DragForm.Region.Dispose();
                    }

                    DragForm.Region = region;
                }
            }
        }

        private class SolarizedLightPanelIndicatorFactory : DockPanelExtender.IPanelIndicatorFactory
        {
            public DockPanel.IPanelIndicator CreatePanelIndicator(DockStyle style)
            {
                return new SolarizedLightPanelIndicator(style);
            }

            private class SolarizedLightPanelIndicator : PictureBox, DockPanel.IPanelIndicator
            {
                private static Image _imagePanelLeft = Resources.DockIndicator_PanelLeft;
                private static Image _imagePanelRight = Resources.DockIndicator_PanelRight;
                private static Image _imagePanelTop = Resources.DockIndicator_PanelTop;
                private static Image _imagePanelBottom = Resources.DockIndicator_PanelBottom;
                private static Image _imagePanelFill = Resources.DockIndicator_PanelFill;
                private static Image _imagePanelLeftActive = Resources.DockIndicator_PanelLeft;
                private static Image _imagePanelRightActive = Resources.DockIndicator_PanelRight;
                private static Image _imagePanelTopActive = Resources.DockIndicator_PanelTop;
                private static Image _imagePanelBottomActive = Resources.DockIndicator_PanelBottom;
                private static Image _imagePanelFillActive = Resources.DockIndicator_PanelFill;

                public SolarizedLightPanelIndicator(DockStyle dockStyle)
                {
                    m_dockStyle = dockStyle;
                    SizeMode = PictureBoxSizeMode.AutoSize;
                    Image = ImageInactive;
                }

                private DockStyle m_dockStyle;

                private DockStyle DockStyle
                {
                    get { return m_dockStyle; }
                }

                private DockStyle m_status;

                public DockStyle Status
                {
                    get { return m_status; }
                    set
                    {
                        if (value != DockStyle && value != DockStyle.None)
                            throw new InvalidEnumArgumentException();

                        if (m_status == value)
                            return;

                        m_status = value;
                        IsActivated = (m_status != DockStyle.None);
                    }
                }

                private Image ImageInactive
                {
                    get
                    {
                        if (DockStyle == DockStyle.Left)
                            return _imagePanelLeft;
                        else if (DockStyle == DockStyle.Right)
                            return _imagePanelRight;
                        else if (DockStyle == DockStyle.Top)
                            return _imagePanelTop;
                        else if (DockStyle == DockStyle.Bottom)
                            return _imagePanelBottom;
                        else if (DockStyle == DockStyle.Fill)
                            return _imagePanelFill;
                        else
                            return null;
                    }
                }

                private Image ImageActive
                {
                    get
                    {
                        if (DockStyle == DockStyle.Left)
                            return _imagePanelLeftActive;
                        else if (DockStyle == DockStyle.Right)
                            return _imagePanelRightActive;
                        else if (DockStyle == DockStyle.Top)
                            return _imagePanelTopActive;
                        else if (DockStyle == DockStyle.Bottom)
                            return _imagePanelBottomActive;
                        else if (DockStyle == DockStyle.Fill)
                            return _imagePanelFillActive;
                        else
                            return null;
                    }
                }

                private bool m_isActivated = false;

                private bool IsActivated
                {
                    get { return m_isActivated; }
                    set
                    {
                        m_isActivated = value;
                        Image = IsActivated ? ImageActive : ImageInactive;
                    }
                }

                public DockStyle HitTest(Point pt)
                {
                    return this.Visible && ClientRectangle.Contains(PointToClient(pt)) ? DockStyle : DockStyle.None;
                }
            }
        }

        private class SolarizedLightPaneIndicatorFactory : DockPanelExtender.IPaneIndicatorFactory
        {
            public DockPanel.IPaneIndicator CreatePaneIndicator()
            {
                return new SolarizedLightPaneIndicator();
            }

            private class SolarizedLightPaneIndicator : PictureBox, DockPanel.IPaneIndicator
            {
                private static Bitmap _bitmapPaneDiamond = Resources.Dockindicator_PaneDiamond;
                private static Bitmap _bitmapPaneDiamondLeft = Resources.Dockindicator_PaneDiamond_Fill;
                private static Bitmap _bitmapPaneDiamondRight = Resources.Dockindicator_PaneDiamond_Fill;
                private static Bitmap _bitmapPaneDiamondTop = Resources.Dockindicator_PaneDiamond_Fill;
                private static Bitmap _bitmapPaneDiamondBottom = Resources.Dockindicator_PaneDiamond_Fill;
                private static Bitmap _bitmapPaneDiamondFill = Resources.Dockindicator_PaneDiamond_Fill;
                private static Bitmap _bitmapPaneDiamondHotSpot = Resources.Dockindicator_PaneDiamond_Hotspot;
                private static Bitmap _bitmapPaneDiamondHotSpotIndex = Resources.DockIndicator_PaneDiamond_HotspotIndex;

                private static DockPanel.HotSpotIndex[] _hotSpots = new[]
                    {
                        new DockPanel.HotSpotIndex(1, 0, DockStyle.Top),
                        new DockPanel.HotSpotIndex(0, 1, DockStyle.Left),
                        new DockPanel.HotSpotIndex(1, 1, DockStyle.Fill),
                        new DockPanel.HotSpotIndex(2, 1, DockStyle.Right),
                        new DockPanel.HotSpotIndex(1, 2, DockStyle.Bottom)
                    };

                private GraphicsPath _displayingGraphicsPath = DrawHelper.CalculateGraphicsPathFromBitmap(_bitmapPaneDiamond);

                public SolarizedLightPaneIndicator()
                {
                    SizeMode = PictureBoxSizeMode.AutoSize;
                    Image = _bitmapPaneDiamond;
                    Region = new Region(DisplayingGraphicsPath);
                }

                public GraphicsPath DisplayingGraphicsPath
                {
                    get { return _displayingGraphicsPath; }
                }

                public DockStyle HitTest(Point pt)
                {
                    if (!Visible)
                        return DockStyle.None;

                    pt = PointToClient(pt);
                    if (!ClientRectangle.Contains(pt))
                        return DockStyle.None;

                    for (int i = _hotSpots.GetLowerBound(0); i <= _hotSpots.GetUpperBound(0); i++)
                    {
                        if (_bitmapPaneDiamondHotSpot.GetPixel(pt.X, pt.Y) == _bitmapPaneDiamondHotSpotIndex.GetPixel(_hotSpots[i].X, _hotSpots[i].Y))
                            return _hotSpots[i].DockStyle;
                    }

                    return DockStyle.None;
                }

                private DockStyle m_status = DockStyle.None;

                public DockStyle Status
                {
                    get { return m_status; }
                    set
                    {
                        m_status = value;
                        if (m_status == DockStyle.None)
                            Image = _bitmapPaneDiamond;
                        else if (m_status == DockStyle.Left)
                            Image = _bitmapPaneDiamondLeft;
                        else if (m_status == DockStyle.Right)
                            Image = _bitmapPaneDiamondRight;
                        else if (m_status == DockStyle.Top)
                            Image = _bitmapPaneDiamondTop;
                        else if (m_status == DockStyle.Bottom)
                            Image = _bitmapPaneDiamondBottom;
                        else if (m_status == DockStyle.Fill)
                            Image = _bitmapPaneDiamondFill;
                    }
                }
            }
        }

        private class SolarizedLightAutoHideWindowFactory : DockPanelExtender.IAutoHideWindowFactory
        {
            public DockPanel.AutoHideWindowControl CreateAutoHideWindow(DockPanel panel)
            {
                return new SolarizedLightAutoHideWindowControl(panel);
            }
        }

        private class SolarizedLightDockPaneSplitterControlFactory : DockPanelExtender.IDockPaneSplitterControlFactory
        {
            public DockPane.SplitterControlBase CreateSplitterControl(DockPane pane)
            {
                return new SolarizedLightSplitterControl(pane);
            }
        }

        private class SolarizedLightDockWindowSplitterControlFactory : DockPanelExtender.IDockWindowSplitterControlFactory
        {
            public SplitterBase CreateSplitterControl()
            {
                return new SolarizedLightDockWindow.SolarizedLightDockWindowSplitterControl();
            }
        }

        private class SolarizedLightDockPaneStripFactory : DockPanelExtender.IDockPaneStripFactory
        {
            public DockPaneStripBase CreateDockPaneStrip(DockPane pane)
            {
                return new SolarizedLightDockPaneStrip(pane);
            }
        }

        private class SolarizedLightAutoHideStripFactory : DockPanelExtender.IAutoHideStripFactory
        {
            public AutoHideStripBase CreateAutoHideStrip(DockPanel panel)
            {
                return new SolarizedLightAutoHideStrip(panel);
            }
        }

        private class SolarizedLightDockPaneCaptionFactory : DockPanelExtender.IDockPaneCaptionFactory
        {
            public DockPaneCaptionBase CreateDockPaneCaption(DockPane pane)
            {
                return new SolarizedLightDockPaneCaption(pane);
            }
        }

        private class SolarizedLightDockWindowFactory : DockPanelExtender.IDockWindowFactory
        {
            public DockWindow CreateDockWindow(DockPanel dockPanel, DockState dockState)
            {
                return new SolarizedLightDockWindow(dockPanel, dockState);
            }
        }

        public static DockPanelSkin CreateSolarizedLight()
        {
            // Solarized
            var SolarizedBase03 = Color.FromArgb(unchecked((int)0xFF002B36));
            var SolarizedBase02 = Color.FromArgb(unchecked((int)0xFF073642));
            var SolarizedBase01 = Color.FromArgb(unchecked((int)0xFF586E75));
            var SolarizedBase00 = Color.FromArgb(unchecked((int)0xFF657B83));
            var SolarizedBase0 = Color.FromArgb(unchecked((int)0xFF839496));
            var SolarizedBase1 = Color.FromArgb(unchecked((int)0xFF93A1A1));
            var SolarizedBase2 = Color.FromArgb(unchecked((int)0xFFEEE8D5));
            var SolarizedBase3 = Color.FromArgb(unchecked((int)0xFFFDF6E3));
            var SolarizedYellow  = Color.FromArgb(unchecked((int)0xFFB58900));
            var SolarizedOrange  = Color.FromArgb(unchecked((int)0xFFCB4B16));
            var SolarizedRed     = Color.FromArgb(unchecked((int)0xFFDC322F));
            var SolarizedMagenta = Color.FromArgb(unchecked((int)0xFFD33682)); 
            var SolarizedViolet  = Color.FromArgb(unchecked((int)0xFF6C71C4));
            var SolarizedBlue    = Color.FromArgb(unchecked((int)0xFF268BD2));
            var SolarizedCyan    = Color.FromArgb(unchecked((int)0xFF2AA198));
            var SolarizedGreen   = Color.FromArgb(unchecked((int)0xFF859900));

            var dot = Color.FromArgb(80, 170, 220);
            var lostFocusTab = Color.FromArgb(0xFF, 204, 206, 219);
            var skin = new DockPanelSkin();

            skin.AutoHideStripSkin.DockStripGradient.StartColor = SolarizedBase2;
            skin.AutoHideStripSkin.DockStripGradient.EndColor = SystemColors.ControlLight;
            skin.AutoHideStripSkin.TabGradient.TextColor = SystemColors.ControlDarkDark;

            skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.StartColor = SolarizedBase3;
            skin.DockPaneStripSkin.DocumentGradient.DockStripGradient.EndColor = SolarizedBase3;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.StartColor = SolarizedBase2;
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.EndColor = SolarizedBase2; // temp: same as focus
            skin.DockPaneStripSkin.DocumentGradient.ActiveTabGradient.TextColor = SolarizedBase01;
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.StartColor = SolarizedBase3;
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.EndColor = SolarizedBase2;
            skin.DockPaneStripSkin.DocumentGradient.InactiveTabGradient.TextColor = SolarizedBase00;

            skin.DockPaneStripSkin.ToolWindowGradient.DockStripGradient.StartColor = SolarizedBase3;
            skin.DockPaneStripSkin.ToolWindowGradient.DockStripGradient.EndColor = SolarizedBase3;

            skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.StartColor = SystemColors.ControlLightLight;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.EndColor = SystemColors.ControlLightLight;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveTabGradient.TextColor = SolarizedBase2;

            skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.StartColor = SolarizedBase3;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.EndColor = SolarizedBase3;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveTabGradient.TextColor = SystemColors.GrayText;

            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.StartColor = SolarizedBase2;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.EndColor = dot;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.TextColor = Color.White;

            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.StartColor = SolarizedBase3;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.EndColor = SystemColors.ControlDark;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.TextColor = SystemColors.GrayText;

            return skin;
        }
    }
}