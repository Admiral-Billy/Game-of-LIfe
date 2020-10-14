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
    public partial class Form1 : Form
    {
        // The universe array
        public bool[,] universe = new bool[15, 15];
        // Array used for the next generation
        public bool[,] scratchPad = new bool[15, 15];

        // boolean used to determine if the board wraps around on itself or if outside neighbors are counted as "dead"
        public bool wrapAround = true;

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        public Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Number of living cells
        int livingCells = 0;

        public Form1()
        {
            InitializeComponent();

            // default to outlined cells
            toolStripMenuItem1.Checked = true;

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer paused
        }

        private int GetNeighbors(int x, int y)
        {
            int neighbors = 0;

            if (wrapAround)
            {
                int boundaryXmin = x - 1;
                int boundaryXmax = x + 1;
                int boundaryYmin = y - 1;
                int boundaryYmax = y + 1;

                // check if bounds need to loop
                if (boundaryXmin < 0)
                {
                    boundaryXmin = universe.GetLength(0) - 1;
                }
                if (boundaryXmax > universe.GetLength(0) - 1)
                {
                    boundaryXmax = 0;
                }
                if (boundaryYmin < 0)
                {
                    boundaryYmin = universe.GetLength(1) - 1;
                }
                if (boundaryYmax > universe.GetLength(1) - 1)
                {
                    boundaryYmax = 0;
                }

                // top left
                if (universe[boundaryXmin, boundaryYmax] == true)
                {
                    ++neighbors;
                }
                // left
                if (universe[boundaryXmin, y] == true)
                {
                    ++neighbors;
                }
                // bottom left
                if (universe[boundaryXmin, boundaryYmin] == true)
                {
                    ++neighbors;
                }
                // top
                if (universe[x, boundaryYmax] == true)
                {
                    ++neighbors;
                }
                // bottom
                if (universe[x, boundaryYmin] == true)
                {
                    ++neighbors;
                }
                // top right
                if (universe[boundaryXmax, boundaryYmax] == true)
                {
                    ++neighbors;
                }
                // right
                if (universe[boundaryXmax, y] == true)
                {
                    ++neighbors;
                }
                // bottom right
                if (universe[boundaryXmax, boundaryYmin] == true)
                {
                    ++neighbors;
                }
            }
            else // uses a modified version of the previous code, a simplified version probably exists but this works for now
            {
                int boundaryXmin = x - 1;
                int boundaryXmax = x + 1;
                int boundaryYmin = y - 1;
                int boundaryYmax = y + 1;

                // check if bounds need to loop, but instead labels these bounds as "inappropriate"
                if (boundaryXmin < 0)
                {
                    boundaryXmin = -1;
                }
                if (boundaryXmax > universe.GetLength(0) - 1)
                {
                    boundaryXmax = -1;
                }
                if (boundaryYmin < 0)
                {
                    boundaryYmin = -1;
                }
                if (boundaryYmax > universe.GetLength(1) - 1)
                {
                    boundaryYmax = -1;
                }

                // top left
                if (boundaryXmin != -1 && boundaryYmax != -1 && universe[boundaryXmin, boundaryYmax] == true)
                {
                    ++neighbors;
                }
                // left
                if (boundaryXmin != -1 && universe[boundaryXmin, y] == true)
                {
                    ++neighbors;
                }
                // bottom left
                if (boundaryXmin != -1 && boundaryYmin != -1 && universe[boundaryXmin, boundaryYmin] == true)
                {
                    ++neighbors;
                }
                // top
                if (boundaryYmax != -1 && universe[x, boundaryYmax] == true)
                {
                    ++neighbors;
                }
                // bottom
                if (boundaryYmin != -1 && universe[x, boundaryYmin] == true)
                {
                    ++neighbors;
                }
                // top right
                if (boundaryXmax != -1 && boundaryYmax != -1 && universe[boundaryXmax, boundaryYmax] == true)
                {
                    ++neighbors;
                }
                // right
                if (boundaryXmax != -1 && universe[boundaryXmax, y] == true)
                {
                    ++neighbors;
                }
                // bottom right
                if (boundaryXmax != -1 && boundaryYmin != -1 && universe[boundaryXmax, boundaryYmin] == true)
                {
                    ++neighbors;
                }
            }

            return neighbors;
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // Reset cell count for recalculation
            livingCells = 0;
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neighbors = GetNeighbors(x, y);
                    if (neighbors < 2 || neighbors > 3)
                    {
                        scratchPad[x, y] = false;
                    }
                    else if (neighbors == 3 && universe[x,y] == false)
                    {
                        scratchPad[x, y] = true;
                    }
                    else
                    {
                        scratchPad[x, y] = universe[x, y];
                    }
                }
            }

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = scratchPad[x, y];
                    if (scratchPad[x, y] == true)
                    {
                        ++livingCells;
                    }
                }
            }

            // clear scratchPad
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    scratchPad[x, y] = false;
                }
            }

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabelCells.Text = "Living cells = " + livingCells.ToString();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            // progress cell state by one generation then redraw
            NextGeneration();
            Redraw();
        }

        public void Redraw()
        {
            // Tell Windows you need to repaint
            graphicsPanel1.Invalidate();
            // Update cell count
            toolStripStatusLabelCells.Text = "Living cells = " + livingCells.ToString();
        }

        public void Reset()
        {
            // clear scratchPad and universe
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    scratchPad[x, y] = false;
                    universe[x, y] = false;
                }
            }

            // reset program state to "new", by setting the generation count to 0, the timer to be disabled, and redrawing
            timer.Enabled = false;
            generations = 0;
            Redraw();
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabelCells.Text = "Living cells = " + livingCells.ToString();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    if (toolStripMenuItem1.Checked)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state if not clicking in empty space
                try
                {
                    if (universe[x, y] == true)
                    {
                        --livingCells;
                    }
                    else
                    {
                        ++livingCells;
                    }
                    universe[x, y] = !universe[x, y];
                }
                catch (IndexOutOfRangeException exception)
                {
                    // silently do nothing if user clicks out of the grid
                }

                // Update visuals
                Redraw();
            }
        }

        private void startStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void pauseStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void nextStripButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(this);
            settings.Show();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Redraw();
        }
    }
}
