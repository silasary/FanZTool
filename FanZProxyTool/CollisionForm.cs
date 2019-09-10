using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        internal static string Resolve(string[] search, string name, string image_id)
        {
            // There's a mess with some of the old Image Packs, due to Jarret using sequential uuids, rather than naturally generating them.
            // While the issues have been cleaned up in the XML, some of the old uuids still show up in old imagepacks.
            // These "bad images" can be safely deleted, as nothing refers to them anymore.
            switch (image_id) 
            {
                case "09291d3f-1889-4c27-9822-e4fe01076164":
                case "09291d3f-1889-4c27-9822-e4fe01076076":
                case "09291d3f-1889-4c27-9822-e4fe01076062":
                    var bad = search.Single(p => p.Contains("e8d510f7-4ad1-4ed8-a85d-3b7c21ce083b"));
                    File.Delete(bad);
                    return search.Single(p => p.Contains("32b3926d-0563-4b05-b8e7-9f2848b500d3"));
                default:
                    // TLDR: If there's an issue, it's probably in e8d510f7-4ad1-4ed8-a85d-3b7c21ce083b.
                    break;
            }
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
