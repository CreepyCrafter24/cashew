using System.Windows.Forms;

namespace cashew.MessageBox
{
    public class MetroMessageBoxProperties
    {
        public MetroMessageBoxProperties(MetroMessageBoxControl owner) => Owner = owner;

        public MessageBoxButtons Buttons { get; set; }

        public MessageBoxDefaultButton DefaultButton { get; set; }

        public MessageBoxIcon Icon { get; set; }

        public string Message { get; set; }

        public MetroMessageBoxControl Owner { get; }

        public string Title { get; set; }
    }
}