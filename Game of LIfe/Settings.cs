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
    public partial class Settings : Form
    {
        Form1 game;
        public Settings(Form1 game)
        {
            // Create form and initialize values to whatever the current values are
            InitializeComponent();
            this.game = game;
            game.Reset();
            numericUpDown1.Value = game.timer.Interval;
            numericUpDown2.Value = game.universe.GetLength(0);
            numericUpDown3.Value = game.universe.GetLength(1);
            checkBox1.Checked = game.wrapAround;
            this.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Accept changes then close the form
            UpdateData();
            this.Close();
        }

        private void UpdateData()
        {
            // Update game's values to the values input by the user
            game.universe = new bool[(int)numericUpDown2.Value, (int)numericUpDown3.Value];
            game.scratchPad = new bool[(int)numericUpDown2.Value, (int)numericUpDown3.Value];
            game.timer.Interval = (int)numericUpDown1.Value;
            game.wrapAround = checkBox1.Checked;
            game.Redraw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Reset the values to my own defined default settings of 15x15, 100ms intervals, wrap-around boundaries
            game.universe = new bool[15, 15];
            game.scratchPad = new bool[15, 15];
            game.timer.Interval = 100;
            game.wrapAround = true;
            game.Redraw();
            this.Close();
        }
    }
}
