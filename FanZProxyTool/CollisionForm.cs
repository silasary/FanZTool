using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FanZProxyTool
{
    public partial class CollisionForm : Form
    {
        string chosen = null;

        private CollisionForm()
        {
            InitializeComponent();
        }

        internal static string Resolve(string[] search, string name)
        {
            using (var form = new CollisionForm())
            {
                form.Text = $"Which of these looks like '{name}'?";
                foreach (var item in search)
                {

                    PictureBox image = new PictureBox()
                    {
                        ImageLocation = item,
                        SizeMode = PictureBoxSizeMode.AutoSize
                    };
                    image.Click += form.Image_Click;
                    form.flowLayoutPanel1.Controls.Add(image);
                }
                form.WindowState = FormWindowState.Maximized;
                form.ShowDialog();
                return form.chosen;
            }
        }

        private  void Image_Click(object sender, EventArgs e)
        {
            chosen = (sender as PictureBox).ImageLocation;
            this.Close();
        }
    }
}
