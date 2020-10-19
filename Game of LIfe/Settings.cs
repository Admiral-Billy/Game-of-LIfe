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

            // Seeded checkbox and the value of the seed
            checkBox3.Checked = game.getSeeded();
            numericUpDown4.Value = game.getSeed();
            numericUpDown4.Enabled = checkBox3.Checked; // disable the box if the checkbox is unchecked

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
            game.setSeeded(checkBox3.Checked);
            game.setSeed((int)numericUpDown4.Value);
            game.Reset();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Reset the values to my own defined default settings of 15x15, 100ms intervals, wrap-around boundaries, and purely randomized
            game.setUniverseSize(15, 15);
            game.setTimerInterval(100);
            game.setBoundaryBehavior(true);
            game.setRandomization(true);
            game.setSeeded(false);
            game.setSeed(1);
            game.Reset();
            this.Close();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            // grey out the seed selector when the seeded checkbox is unchecked
            numericUpDown4.Enabled = checkBox3.Checked;
        }
    }
}
