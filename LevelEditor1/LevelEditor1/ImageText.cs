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
    public partial class ImageText : UserControl
    {
        public int Index { get; set; }

        public ImageText(Bitmap image, string imageDescription, int index)
        {
            InitializeComponent();
            ImagePB.Image = image;
            ImageLabel.Text = imageDescription;
            Index = index;
        }
    }
}
