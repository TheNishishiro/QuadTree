using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuadTree
{
    public partial class Form1 : Form
    {
        QuadTree<Point> qt;

        public Form1()
        {
            InitializeComponent();

            pictureBox1.Width = 800;
            pictureBox1.Height = 600;

            pictureBox1.BackColor = Color.Black;

            qt = new QuadTree<Point>(new QuadRectangle<Point>(0, 0, pictureBox1.Width, pictureBox1.Height), 4);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            qt.Draw(e);

            e.Graphics.DrawRectangle(new Pen(Color.Green, 1), 100, 100, 300, 300);

            List<QuadTreeElement<Point>> points = qt.Query(new QuadRectangle<Point>(100, 100, 300, 300));
            foreach(QuadTreeElement<Point> p in points)
            {

                e.Graphics.DrawEllipse(
                    new Pen(Color.Red, 1),
                    p.userData.X - 1, p.userData.Y - 1, 2, 2
                    );
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            qt.Insert(new QuadTreeElement<Point>(coordinates.X, coordinates.Y, coordinates));
            Refresh();
        }
    }
}
