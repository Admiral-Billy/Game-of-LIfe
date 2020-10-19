using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_LIfe
{
    public partial class Visuals : Form
    {
        Form1 game;
        public Visuals(Form1 game)
        {
            InitializeComponent();
            this.game = game;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();
            colorDialog1.ShowDialog();
            game.setBackgroundColor(colorDialog1.Color);
            game.Redraw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog2 = new ColorDialog();
            colorDialog2.ShowDialog();
            game.setGridColor(colorDialog2.Color);
            game.Redraw();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog3 = new ColorDialog();
            colorDialog3.ShowDialog();
            game.setCellColor(colorDialog3.Color);
            game.Redraw();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog4 = new ColorDialog();
            colorDialog4.ShowDialog();
            game.setNeighborCountColor(colorDialog4.Color);
            game.Redraw();
        }
    }
}
