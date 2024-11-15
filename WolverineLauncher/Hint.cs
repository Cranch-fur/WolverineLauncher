using System.Drawing;
using System.Windows.Forms;

namespace WolverineLauncher
{
    public partial class Hint : Form
    {
        public Hint()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;
        }

        public void UpdateImage(Image newImage)
        {
            pictureBox_Outline.BackgroundImage = newImage;
        }
    }
}
