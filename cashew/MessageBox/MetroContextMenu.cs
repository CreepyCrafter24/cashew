using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace MetroFramework.Controls
{
    public class MetroContextMenu : ContextMenuStrip, IMetroControl
    {
        private MetroColorStyle metroStyle;
        private MetroStyleManager metroStyleManager;
        private MetroThemeStyle metroTheme;

        public MetroContextMenu(IContainer Container)
        {
            Container?.Add(this);
        }

        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseCustomBackColor { get; set; }

        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseCustomForeColor { get; set; }

        [DefaultValue(false)]
        [Category("Metro Appearance")]
        public bool UseStyleColors { get; set; }

        [Category("Metro Behaviour")]
        [Browsable(false)]
        [DefaultValue(false)]
        public bool UseSelectable
        {
            get => GetStyle(ControlStyles.Selectable);
            set => SetStyle(ControlStyles.Selectable, value);
        }

        [Category("Metro Appearance")]
        [DefaultValue(MetroColorStyle.Blue)]
        public MetroColorStyle Style
        {
            get
            {
                if (DesignMode || metroStyle != MetroColorStyle.Blue)
                    return metroStyle;
                if (StyleManager != null && metroStyle == MetroColorStyle.Blue)
                    return StyleManager.Style;
                return StyleManager == null && metroStyle == MetroColorStyle.Blue ? MetroColorStyle.Blue : metroStyle;
            }
            set => metroStyle = value;
        }

        [Category("Metro Appearance")]
        [DefaultValue(MetroThemeStyle.Light)]
        public MetroThemeStyle Theme
        {
            get
            {
                if (DesignMode || metroTheme != MetroThemeStyle.Light)
                    return metroTheme;
                if (StyleManager != null && metroTheme == MetroThemeStyle.Light)
                    return StyleManager.Theme;
                return StyleManager == null && metroTheme == MetroThemeStyle.Light ? MetroThemeStyle.Light : metroTheme;
            }
            set => metroTheme = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public MetroStyleManager StyleManager
        {
            get => metroStyleManager;
            set
            {
                metroStyleManager = value;
                settheme();
            }
        }

        private void settheme()
        {
            BackColor = MetroPaint.BackColor.Form(Theme);
            ForeColor = MetroPaint.ForeColor.Button.Normal(Theme);
            Renderer = new MetroCTXRenderer(Theme, Style);
        }

        private class MetroCTXRenderer : ToolStripProfessionalRenderer
        {
            private readonly MetroThemeStyle _theme;

            public MetroCTXRenderer(MetroThemeStyle Theme, MetroColorStyle Style)
                : base(new contextcolors(Theme, Style)) =>
                _theme = Theme;

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = MetroPaint.ForeColor.Button.Normal(_theme);
                base.OnRenderItemText(e);
            }
        }

        private class contextcolors : ProfessionalColorTable
        {
            private readonly MetroColorStyle _style = MetroColorStyle.Blue;
            private readonly MetroThemeStyle _theme = MetroThemeStyle.Light;

            public contextcolors(MetroThemeStyle Theme, MetroColorStyle Style)
            {
                _theme = Theme;
                _style = Style;
            }

            public override Color MenuItemSelected => MetroPaint.GetStyleColor(_style);

            public override Color MenuBorder => MetroPaint.BackColor.Form(_theme);

            public override Color ToolStripBorder => MetroPaint.GetStyleColor(_style);

            public override Color MenuItemBorder => MetroPaint.GetStyleColor(_style);

            public override Color ToolStripDropDownBackground => MetroPaint.BackColor.Form(_theme);

            public override Color ImageMarginGradientBegin => MetroPaint.BackColor.Form(_theme);

            public override Color ImageMarginGradientMiddle => MetroPaint.BackColor.Form(_theme);

            public override Color ImageMarginGradientEnd => MetroPaint.BackColor.Form(_theme);
        }
    }
}