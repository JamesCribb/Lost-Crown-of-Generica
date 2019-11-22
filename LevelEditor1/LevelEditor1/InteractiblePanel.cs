using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor1
{
    public partial class InteractiblePanel : UserControl
    {
        public InteractiblePanel()
        {
            InitializeComponent();
        }

        public Button GetSaveButton()
        {
            return SaveButton;
        }

        public RichTextBox GetInteractionTextRTB()
        {
            return InteractionTextRTB;
        }

        public Label GetFileNameLabel()
        {
            return FileNameLabel;
        }
    }
}
