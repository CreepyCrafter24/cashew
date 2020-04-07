using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Controls;

namespace cashew.MessageBox
{
    public class MetroMessageBoxControl : Form
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Color _defaultColor = Color.FromArgb(57, 179, 215);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Color _errorColor = Color.FromArgb(210, 50, 45);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Color _question = Color.FromArgb(71, 164, 71);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Color _warningColor = Color.FromArgb(237, 156, 40);

        private Label _messageLabel;
        private MetroButton _metroButton1;
        private MetroButton _metroButton2;
        private MetroButton _metroButton3;
        private Panel _pnlBottom;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _success = Color.FromArgb(71, 164, 71);

        private Label _titleLabel;
        private TableLayoutPanel _tlpBody;

        private IContainer components;

        public MetroMessageBoxControl()
        {
            InitializeComponent();
            Properties = new MetroMessageBoxProperties(this);
            StylizeButton(_metroButton1);
            StylizeButton(_metroButton2);
            StylizeButton(_metroButton3);
            _metroButton1.Click += button_Click;
            _metroButton2.Click += button_Click;
            _metroButton3.Click += button_Click;
        }

        public Panel Body { get; private set; }

        public MetroMessageBoxProperties Properties { get; }

        public DialogResult Result { get; private set; }

        public void ArrangeApperance()
        {
            _titleLabel.Text = Properties.Title;
            _messageLabel.Text = Properties.Message;
            switch (Properties.Icon)
            {
                case MessageBoxIcon.Hand:
                    Body.BackColor = _errorColor;
                    break;
                case MessageBoxIcon.Exclamation:
                    Body.BackColor = _warningColor;
                    break;
            }

            switch (Properties.Buttons)
            {
                case MessageBoxButtons.OK:
                    EnableButton(_metroButton1);
                    _metroButton1.Text = "Ok";
                    _metroButton1.Location = _metroButton3.Location;
                    _metroButton1.Tag = DialogResult.OK;
                    EnableButton(_metroButton2, false);
                    EnableButton(_metroButton3, false);
                    break;
                case MessageBoxButtons.OKCancel:
                    EnableButton(_metroButton1);
                    _metroButton1.Text = "Ok";
                    _metroButton1.Location = _metroButton2.Location;
                    _metroButton1.Tag = DialogResult.OK;
                    EnableButton(_metroButton2);
                    _metroButton2.Text = "Cancel";
                    _metroButton2.Location = _metroButton3.Location;
                    _metroButton2.Tag = DialogResult.Cancel;
                    EnableButton(_metroButton3, false);
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    EnableButton(_metroButton1);
                    _metroButton1.Text = "Abort";
                    _metroButton1.Tag = DialogResult.Abort;
                    EnableButton(_metroButton2);
                    _metroButton2.Text = "Retry";
                    _metroButton2.Tag = DialogResult.Retry;
                    EnableButton(_metroButton3);
                    _metroButton3.Text = "Ignore";
                    _metroButton3.Tag = DialogResult.Ignore;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    EnableButton(_metroButton1);
                    _metroButton1.Text = "Yes";
                    _metroButton1.Tag = DialogResult.Yes;
                    EnableButton(_metroButton2);
                    _metroButton2.Text = "No";
                    _metroButton2.Tag = DialogResult.No;
                    EnableButton(_metroButton3);
                    _metroButton3.Text = "Cancel";
                    _metroButton3.Tag = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.YesNo:
                    EnableButton(_metroButton1);
                    _metroButton1.Text = "Yes";
                    _metroButton1.Location = _metroButton2.Location;
                    _metroButton1.Tag = DialogResult.Yes;
                    EnableButton(_metroButton2);
                    _metroButton2.Text = "No";
                    _metroButton2.Location = _metroButton3.Location;
                    _metroButton2.Tag = DialogResult.No;
                    EnableButton(_metroButton3, false);
                    break;
                case MessageBoxButtons.RetryCancel:
                    EnableButton(_metroButton1);
                    _metroButton1.Text = "Retry";
                    _metroButton1.Location = _metroButton2.Location;
                    _metroButton1.Tag = DialogResult.Retry;
                    EnableButton(_metroButton2);
                    _metroButton2.Text = "Cancel";
                    _metroButton2.Location = _metroButton3.Location;
                    _metroButton2.Tag = DialogResult.Cancel;
                    EnableButton(_metroButton3, false);
                    break;
            }

            switch (Properties.Icon)
            {
                case MessageBoxIcon.Hand:
                    Body.BackColor = _errorColor;
                    break;
                case MessageBoxIcon.Question:
                    Body.BackColor = _question;
                    break;
                case MessageBoxIcon.Exclamation:
                    Body.BackColor = _warningColor;
                    break;
                case MessageBoxIcon.Asterisk:
                    Body.BackColor = _defaultColor;
                    break;
                default:
                    Body.BackColor = Color.DarkGray;
                    break;
            }
        }

        private void EnableButton(MetroButton button)
        {
            EnableButton(button, true);
        }

        private void EnableButton(MetroButton button, bool enabled)
        {
            button.Enabled = enabled;
            button.Visible = enabled;
        }

        public void SetDefaultButton()
        {
            switch (Properties.DefaultButton)
            {
                case MessageBoxDefaultButton.Button1:
                    if (_metroButton1 == null || !_metroButton1.Enabled)
                        break;
                    _metroButton1.Focus();
                    break;
                case MessageBoxDefaultButton.Button2:
                    if (_metroButton2 == null || !_metroButton2.Enabled)
                        break;
                    _metroButton2.Focus();
                    break;
                case MessageBoxDefaultButton.Button3:
                    if (_metroButton3 == null || !_metroButton3.Enabled)
                        break;
                    _metroButton3.Focus();
                    break;
            }
        }

        private void button_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void button_MouseEnter(object sender, EventArgs e)
        {
            StylizeButton((MetroButton) sender, true);
        }

        private void button_MouseLeave(object sender, EventArgs e)
        {
            StylizeButton((MetroButton) sender);
        }

        private void StylizeButton(MetroButton button)
        {
            StylizeButton(button, false);
        }

        private void StylizeButton(MetroButton button, bool hovered)
        {
            button.Cursor = Cursors.Hand;
            button.MouseClick -= button_MouseClick;
            button.MouseClick += button_MouseClick;
            button.MouseEnter -= button_MouseEnter;
            button.MouseEnter += button_MouseEnter;
            button.MouseLeave -= button_MouseLeave;
            button.MouseLeave += button_MouseLeave;
        }

        private void button_Click(object sender, EventArgs e)
        {
            MetroButton metroButton = (MetroButton) sender;
            if (!metroButton.Enabled)
                return;
            Result = (DialogResult) metroButton.Tag;
            Hide();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Body = new Panel();
            _tlpBody = new TableLayoutPanel();
            _messageLabel = new Label();
            _titleLabel = new Label();
            _metroButton1 = new MetroButton();
            _metroButton3 = new MetroButton();
            _metroButton2 = new MetroButton();
            _pnlBottom = new Panel();
            Body.SuspendLayout();
            _tlpBody.SuspendLayout();
            _pnlBottom.SuspendLayout();
            SuspendLayout();
            Body.BackColor = Color.DarkGray;
            Body.Controls.Add(_tlpBody);
            Body.Dock = DockStyle.Fill;
            Body.Location = new Point(0, 0);
            Body.Margin = new Padding(0);
            Body.Name = "Body";
            Body.Size = new Size(804, 211);
            Body.TabIndex = 2;
            _tlpBody.ColumnCount = 3;
            _tlpBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
            _tlpBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80f));
            _tlpBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
            _tlpBody.Controls.Add(_messageLabel, 1, 2);
            _tlpBody.Controls.Add(_titleLabel, 1, 1);
            _tlpBody.Controls.Add(_pnlBottom, 1, 3);
            _tlpBody.Dock = DockStyle.Fill;
            _tlpBody.Location = new Point(0, 0);
            _tlpBody.Name = "_tlpBody";
            _tlpBody.RowCount = 4;
            _tlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 5f));
            _tlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 25f));
            _tlpBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            _tlpBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            _tlpBody.Size = new Size(804, 211);
            _tlpBody.TabIndex = 6;
            _messageLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _messageLabel.BackColor = Color.Transparent;
            _messageLabel.ForeColor = Color.White;
            _messageLabel.Location = new Point(83, 30);
            _messageLabel.Margin = new Padding(3, 0, 0, 0);
            _messageLabel.Name = "_messageLabel";
            _messageLabel.Size = new Size(640, 141);
            _messageLabel.TabIndex = 0;
            _messageLabel.Text = "message here";
            _titleLabel.AutoSize = true;
            _titleLabel.BackColor = Color.Transparent;
            _titleLabel.Font = new Font("Segoe UI Semibold", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _titleLabel.ForeColor = Color.WhiteSmoke;
            _titleLabel.Location = new Point(80, 5);
            _titleLabel.Margin = new Padding(0);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new Size(125, 25);
            _titleLabel.TabIndex = 1;
            _titleLabel.Text = "message title";
            _metroButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _metroButton1.BackColor = Color.ForestGreen;
            _metroButton1.Location = new Point(357, 1);
            _metroButton1.Name = "_metroButton1";
            _metroButton1.Size = new Size(90, 26);
            _metroButton1.TabIndex = 3;
            _metroButton1.Text = "button 1";
            _metroButton3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _metroButton3.Location = new Point(553, 1);
            _metroButton3.Name = "_metroButton3";
            _metroButton3.Size = new Size(90, 26);
            _metroButton3.TabIndex = 5;
            _metroButton3.Text = "button 3";
            _metroButton2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _metroButton2.Location = new Point(455, 1);
            _metroButton2.Name = "_metroButton2";
            _metroButton2.Size = new Size(90, 26);
            _metroButton2.TabIndex = 4;
            _metroButton2.Text = "button 2";
            _pnlBottom.BackColor = Color.Transparent;
            _pnlBottom.Controls.Add(_metroButton2);
            _pnlBottom.Controls.Add(_metroButton1);
            _pnlBottom.Controls.Add(_metroButton3);
            _pnlBottom.Dock = DockStyle.Fill;
            _pnlBottom.Location = new Point(80, 171);
            _pnlBottom.Margin = new Padding(0);
            _pnlBottom.Name = "_pnlBottom";
            _pnlBottom.Size = new Size(643, 40);
            _pnlBottom.TabIndex = 2;
            AutoScaleDimensions = new SizeF(8f, 21f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 211);
            ControlBox = false;
            Controls.Add(Body);
            Font = new Font("Segoe UI Light", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 5, 4, 5);
            Name = nameof(MetroMessageBoxControl);
            StartPosition = FormStartPosition.Manual;
            Body.ResumeLayout(false);
            _tlpBody.ResumeLayout(false);
            _tlpBody.PerformLayout();
            _pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}