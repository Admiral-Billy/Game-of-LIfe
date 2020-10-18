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
            // Timer interval box
            numericUpDown1.Value = game.getTimerInterval();

            // Universe X by Y size
            numericUpDown2.Value = game.getUniverseSizeX();
            numericUpDown3.Value = game.getUniverseSizeY();

            // Wrap-around checkbox
            checkBox1.Checked = game.getBoundaryBehavior();

            // Randomization checkbox
            checkBox2.Checked = game.getRandomization();

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
            game.setUniverseSize((int)numericUpDown2.Value, (int)numericUpDown3.Value);
            game.setTimerInterval((int)numericUpDown1.Value);
            game.setBoundaryBehavior(checkBox1.Checked);
            game.setRandomization(checkBox2.Checked);
            game.Reset();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Reset the values to my own defined default settings of 15x15, 100ms intervals, wrap-around boundaries
            game.setUniverseSize(15, 15);
            game.setTimerInterval(100);
            game.setBoundaryBehavior(true);
            game.setRandomization(true);
            game.Reset();
            this.Close();
        }
    }
}
