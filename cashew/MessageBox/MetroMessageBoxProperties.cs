using System.Diagnostics;
using System.Windows.Forms;

namespace cashew.MessageBox
{
    public class MetroMessageBoxProperties
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MetroMessageBoxControl _owner;

        public MetroMessageBoxProperties(MetroMessageBoxControl owner)
        {
            this._owner = owner;
        }

        public MessageBoxButtons Buttons { get; set; }

        public MessageBoxDefaultButton DefaultButton { get; set; }

        public MessageBoxIcon Icon { get; set; }

        public string Message { get; set; }

        public MetroMessageBoxControl Owner
        {
            get
            {
                return this._owner;
            }
        }

        public string Title { get; set; }
    }
}