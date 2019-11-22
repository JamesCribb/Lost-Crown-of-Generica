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
    public partial class ZoneTriggerPanel : UserControl
    {
        public ZoneTriggerPanel()
        {
            InitializeComponent();
        }

        public Button GetSaveButton()
        {
            return SaveButton;
        }

        public TextBox GetZoneTB()
        {
            return ZoneTB;
        }

        public NumericUpDown GetLevelXNUD()
        {
            return LevelXNUD;
        }

        public NumericUpDown GetLevelYNUD()
        {
            return LevelYNUD;
        }

        public NumericUpDown GetPlayerXNUD()
        {
            return PlayerXNUD;
        }

        public NumericUpDown GetPlayerYNUD()
        {
            return PlayerYNUD;
        }
    }
}
