using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Game_of_LIfe
{
    public partial class Form1 : Form
    {
        
        // The universe array
        bool[,] universe = new bool[15, 15];
        // Array used for the next generation
        bool[,] scratchPad = new bool[15, 15];

        // boolean used to determine if the board wraps around on itself or if outside neighbors are counted as "dead"
        bool wrapAround = true;

        // boolean used to determine if the cells should be randomized on clear
        bool randomize = true;

        // Seed used for randomization
        bool seeded = false;
        int seed = 1;

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color neighborColor = Color.Red;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Number of living cells
        int livingCells = 0;

        public Form1()
        {
            InitializeComponent();

            // default to outlined cells
            toolStripMenuItem1.Checked = true;

            // default to showing HUD
            toggleHUDToolStripMenuItem.Checked = true;

            try
            {
                LoadSettings();
            }
            catch (FileNotFoundException e)
            {
                // do nothing; default settings are already in place
            }

            // File is read (if it exists) and the values are loaded into memory for initialization
            

            // Set up context strip to have same values as the top menu options
            gridOutlinesToolStripMenuItem.Checked = toolStripMenuItem1.Checked;
            neighborCountToolStripMenuItem.Checked = showNeighborCountToolStripMenuItem.Checked;
            randomizeToolStripMenuItem.Checked = randomize;
            wraparoundToolStripMenuItem.Checked = wrapAround;
            showHUDToolStripMenuItem.Checked = toggleHUDToolStripMenuItem.Checked;

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer paused

            // randomize the newly cleared board if the option is ticked
            if (randomize)
            {
                if (seeded)
                {
                    int persistentSeed = seed;
                    // Iterate through the universe in the y, top to bottom
                    for (int y = 0; y < universe.GetLength(1); y++)
                    {
                        // Iterate through the universe in the x, left to right
                        for (int x = 0; x < universe.GetLength(0); x++)
                        {
                            // Random number is generated with a given seed value, Lehmer RNG algorithm
                            persistentSeed = (48271 * persistentSeed) % Int32.MaxValue;
                            Random rand = new Random(persistentSeed);
                            // 1/3 living, 2/3 dead
                            int randNum = rand.Next() % 3;
                            if (randNum == 0)
                            {
                                universe[x, y] = true;
                                ++livingCells;
                            }
                        }
                    }
                }
                else
                {
                    // Iterate through the universe in the y, top to bottom
                    for (int y = 0; y < universe.GetLength(1); y++)
                    {
                        // Iterate through the universe in the x, left to right
                        for (int x = 0; x < universe.GetLength(0); x++)
                        {
                            Random rand = new Random(Guid.NewGuid().GetHashCode()); // stackoverflow solution that seems to work well
                            int randNum = rand.Next() % 3;
                            if (randNum == 0)
                            {
                                universe[x, y] = true;
                                ++livingCells;
                            }
                        }
                    }
                }
            }
            Redraw();
        }

        // Game logic
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

            // Update HUD Status
            if (showHUDToolStripMenuItem.Checked)
            {
                statusStrip1.Show();
            }
            else
            {
                statusStrip1.Hide();
            }
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            if (cellWidth == 0)
            {
                cellWidth = 1; // minimum size
            }
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);
            if (cellHeight == 0)
            {
                cellHeight = 1;
            }

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // A brush for the neighboring cell count of each cell
            Brush neighborBrush = new SolidBrush(neighborColor);

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

                    // Draw the neighborCount of each cell in the default font in roughly the middle of each cell (presentation rework pending)
                    if (showNeighborCountToolStripMenuItem.Checked)
                    {
                        // minimum font size of 1, which will look *terrible* with lots of numbers but oh well
                        e.Graphics.DrawString(GetNeighbors(x,y).ToString(), new Font(Font.FontFamily, Math.Max(3*cellHeight/5, 1)), neighborBrush, cellRect.X + cellWidth/3, cellRect.Y);
                    }
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
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
            livingCells = 0;

            // randomize the newly cleared board if the option is ticked
            if (randomize)
            {
                if (seeded)
                {
                    int persistentSeed = seed;
                    // Iterate through the universe in the y, top to bottom
                    for (int y = 0; y < universe.GetLength(1); y++)
                    {
                        // Iterate through the universe in the x, left to right
                        for (int x = 0; x < universe.GetLength(0); x++)
                        {
                            // Random number is generated with a given seed value, Lehmer RNG algorithm
                            persistentSeed = (48271 * persistentSeed) % Int32.MaxValue;
                            Random rand = new Random(persistentSeed);
                            // 1/3 living, 2/3 dead
                            int randNum = rand.Next() % 3;
                            if (randNum == 0)
                            {
                                universe[x, y] = true;
                                ++livingCells;
                            }
                        }
                    }
                }
                else
                {
                    // Iterate through the universe in the y, top to bottom
                    for (int y = 0; y < universe.GetLength(1); y++)
                    {
                        // Iterate through the universe in the x, left to right
                        for (int x = 0; x < universe.GetLength(0); x++)
                        {
                            Random rand = new Random(Guid.NewGuid().GetHashCode()); // stackoverflow solution that seems to work well
                            int randNum = rand.Next() % 3;
                            if (randNum == 0)
                            {
                                universe[x, y] = true;
                                ++livingCells;
                            }
                        }
                    }
                }
            }

            Redraw();
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabelCells.Text = "Living cells = " + livingCells.ToString();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                if (cellWidth == 0)
                {
                    cellWidth = 1; // minimum size
                }
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);
                if (cellHeight == 0)
                {
                    cellHeight = 1; // minimum size
                }

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

        public void SaveSettings()
        {
            string[] data = new string[14];
            // Formatting is as follows:
            // int (universe X size)
            // int (universe Y size)
            // int (timer interval)
            // bool (wraparound status)
            // bool (randomization status)
            // bool (seeded status)
            // bool (outline status)
            // bool (neighbor count status)
            // bool (HUD status)
            // int (seed for seeded status)
            // int (ARGB for grid color)
            // int (ARGB for cell color)
            // int (ARGB for neighbor color)
            // int (ARGB for graphicsPanel1.BackColor)

            // add all of the variables mentioned above to the array
            data[0] = getUniverseSizeX().ToString();
            data[1] = getUniverseSizeY().ToString();
            data[2] = timer.Interval.ToString();
            data[3] = wrapAround.ToString();
            data[4] = randomize.ToString();
            data[5] = seeded.ToString();
            data[6] = toolStripMenuItem1.Checked.ToString();
            data[7] = showNeighborCountToolStripMenuItem.Checked.ToString();
            data[8] = toggleHUDToolStripMenuItem.Checked.ToString();
            data[9] = seed.ToString();
            data[10] = gridColor.ToArgb().ToString();
            data[11] = cellColor.ToArgb().ToString();
            data[12] = neighborColor.ToArgb().ToString();
            data[13] = graphicsPanel1.BackColor.ToArgb().ToString();

            // Save!
            System.IO.File.WriteAllLines(Directory.GetCurrentDirectory() + "/settings.txt", data);
        }

        public void LoadSettings()
        {
            // Array to hold saved data to read from a file as strings; parse with Int32.Parse or Boolean.Prase
            string[] savedData = System.IO.File.ReadAllLines(Directory.GetCurrentDirectory() + "/settings.txt");
            // Formatting is as follows:
            // int (universe X size)
            // int (universe Y size)
            // int (timer interval)
            // bool (wraparound status)
            // bool (randomization status)
            // bool (seeded status)
            // bool (outline status)
            // bool (neighbor count status)
            // bool (HUD status)
            // int (seed for seeded status)
            // int (ARGB for grid color)
            // int (ARGB for cell color)
            // int (ARGB for neighbor color)
            // int (ARGB for graphicsPanel1.BackColor)

            // Parse all of the variables mentioned above
            universe = new bool[Int32.Parse(savedData[0]), Int32.Parse(savedData[1])];
            scratchPad = new bool[Int32.Parse(savedData[0]), Int32.Parse(savedData[1])];
            timer.Interval = Int32.Parse(savedData[2]);
            wrapAround = Boolean.Parse(savedData[3]);
            randomize = Boolean.Parse(savedData[4]);
            seeded = Boolean.Parse(savedData[5]);
            toolStripMenuItem1.Checked = Boolean.Parse(savedData[6]);
            showNeighborCountToolStripMenuItem.Checked = Boolean.Parse(savedData[7]);
            toggleHUDToolStripMenuItem.Checked = Boolean.Parse(savedData[8]);
            seed = Int32.Parse(savedData[9]);
            setGridColor(Color.FromArgb(Int32.Parse(savedData[10])));
            setCellColor(Color.FromArgb(Int32.Parse(savedData[11])));
            setNeighborCountColor(Color.FromArgb(Int32.Parse(savedData[12])));
            graphicsPanel1.BackColor = Color.FromArgb(Int32.Parse(savedData[13]));
        }

        public void savePattern()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // List that will be turned into an array for writing
                List<string> pattern = new List<string>();

                // Iterate through the universe in the y, top to bottom
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    string line = "";
                    // Iterate through the universe in the x, left to right
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == true)
                        {
                            line = line + "O";
                        }
                        else
                        {
                            line = line + ".";
                        }
                    }
                    pattern.Add(line);
                }
                System.IO.File.WriteAllLines(saveFileDialog1.FileName, pattern.ToArray());
            }
        }

        public void loadPatternAsUniverse()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Open file as a string array
                string[] pattern = System.IO.File.ReadAllLines(openFileDialog1.FileName);

                // Use variable to track what the largest possible size of the universe is
                int universeSizeX = 0;
                int universeSizeY = 0;

                // use to track how many lines are comments
                int fakeLines = 0;

                for (int i = 0; i < pattern.Length; ++i)
                {
                    if (pattern[i].Length > 0 && (pattern[i][0] == '!' || pattern[i][0] == '#'))
                    {
                        // Comment lines aren't counted for universe size
                        ++fakeLines;
                    }
                    else
                    {
                        if (pattern[i].Length > universeSizeX)
                        {
                            universeSizeX = pattern[i].Length;
                        }
                    }
                }
                universeSizeY = pattern.Length - fakeLines;
                // full universe loading doesn't consider bounding boxes or settings at all
                universe = new bool[universeSizeX, universeSizeY];
                scratchPad = new bool[universeSizeX, universeSizeY];

                for (int y = 0; y < universe.GetLength(1) + fakeLines; y++)
                {
                    // Iterate through the universe in the x, left to right
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (pattern[y].Length == 0 || pattern[y][0] == '#' || pattern[y][0] == '!' || pattern[y][0] == '\n')
                        {
                            // do nothing about comment lines and empty lines
                        }
                        else
                        {
                            if (x > pattern[y].Length - 1 || pattern[y][x] == '\n')
                            {
                                // continue to next loop, no more living cells on that line
                            }
                            else
                            {
                                // O and * represent living cells
                                if (pattern[y][x] == 'O' || pattern[y][x] == '*')
                                {
                                    universe[x, y - fakeLines] = true;
                                }
                                else
                                {
                                    universe[x, y - fakeLines] = false;
                                }
                            }
                        }
                    }
                }

                // redraw required after editing state of universe
                Redraw();
            }
        }

        public void loadPatternIntoUniverse()
        {
            // Open file as a string array
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] pattern = System.IO.File.ReadAllLines(openFileDialog1.FileName);

                // Use variable to track what the largest possible size of the universe is
                int universeSizeX = 0;
                int universeSizeY = 0;

                // use to track how many lines are comments
                int fakeLines = 0;

                for (int i = 0; i < pattern.Length; ++i)
                {
                    if (pattern[i].Length > 0 && (pattern[i][0] == '!' || pattern[i][0] == '#'))
                    {
                        // Comment lines aren't counted for universe size
                        ++fakeLines;
                    }
                    else
                    {
                        if (pattern[i].Length > universeSizeX)
                        {
                            universeSizeX = pattern[i].Length;
                        }
                    }
                }

                // real Y size is the array length minus the number of comment lines
                universeSizeY = pattern.Length - fakeLines;

                if (universeSizeX > universe.GetLength(0) || universeSizeY > universe.GetLength(1))
                {
                    // Don't load an impossible universe, show dialog
                    MessageBox.Show("The universe is not big enough to import this universe. It must be at least " + universeSizeX + " by " + universeSizeY + ".");
                }
                else
                {
                    for (int y = 0; y < universeSizeY + fakeLines; y++)
                    {
                        // Iterate through the universe in the x, left to right
                        for (int x = 0; x < universeSizeX; x++)
                        {
                            if (pattern[y].Length == 0 || pattern[y][0] == '#' || pattern[y][0] == '!' || pattern[y][0] == '\n')
                            {
                                // do nothing about comment lines and empty lines
                            }
                            else
                            {
                                if (x > pattern[y].Length - 1 || pattern[y][x] == '\n')
                                {
                                    // continue to next loop, no more living cells on that line
                                }
                                else
                                {
                                    // O and * represent living cells
                                    if (pattern[y][x] == 'O' || pattern[y][x] == '*')
                                    {
                                        universe[x, y - fakeLines] = true;
                                    }
                                    else
                                    {
                                        universe[x, y - fakeLines] = false;
                                    }
                                }
                            }
                        }
                    }
                }

                // redraw required after editing state of universe
                Redraw();
            }
        }

        // Getters/setters are self-explanatory
        public int getUniverseSizeX()
        {
            return universe.GetLength(0);
        }

        public int getUniverseSizeY()
        {
            return universe.GetLength(1);
        }

        public void setUniverseSize(int x, int y)
        {
            universe = new bool[x, y];
            scratchPad = new bool[x, y];
            Reset();
        }

        public bool getBoundaryBehavior()
        {
            return wrapAround;
        }

        public void setBoundaryBehavior(bool value)
        {
            wrapAround = value;
            wraparoundToolStripMenuItem.Checked = value;
        }

        public bool getRandomization()
        {
            return randomize;
        }

        public void setRandomization(bool value)
        {
            randomize = value;
            randomizeToolStripMenuItem.Checked = value;
        }

        public bool getSeeded()
        {
            return seeded;
        }

        public int getSeed()
        {
            return seed;
        }

        public void setSeeded(bool value)
        {
            seeded = value;
        }

        public void setSeed(int value)
        {
            seed = value;
        }

        public int getTimerInterval()
        {
            return timer.Interval;
        }

        public void setTimerInterval(int value)
        {
            timer.Interval = value;
        }

        public void setBackgroundColor(Color color)
        {
            graphicsPanel1.BackColor = color;
        }

        public void setGridColor(Color color)
        {
            gridColor = color;
        }

        public void setCellColor(Color color)
        {
            cellColor = color;
        }

        public void setNeighborCountColor(Color color)
        {
            neighborColor = color;
        }

        public void setNeighborCount(bool value)
        {
            neighborCountToolStripMenuItem.Checked = value;
            showNeighborCountToolStripMenuItem.Checked = value;
        }

        // GUI elements coded below
        private void startStripButton_Click(object sender, EventArgs e)
        {
            // Enable timer when clicking start button
            timer.Enabled = true;
        }

        private void pauseStripButton_Click(object sender, EventArgs e)
        {
            // Disable timer when clicking pause button
            timer.Enabled = false;
        }

        private void nextStripButton_Click(object sender, EventArgs e)
        {
            // Advance exactly one generation when clicking the next button
            NextGeneration();
            Redraw();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open up the settings menu on click
            Settings settings = new Settings(this);
            settings.Show();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reset the board when clicking the "new" button
            Reset();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Redraw the board after toggling outlines on/off and updating the context menu
            gridOutlinesToolStripMenuItem.Checked = toolStripMenuItem1.Checked;
            Redraw();
        }

        private void showNeighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Redraw the board after toggling and updating the context menu
            neighborCountToolStripMenuItem.Checked = showNeighborCountToolStripMenuItem.Checked;
            Redraw();
        }

        private void toggleHUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the other equivalant control to the same value and toggle the HUD
            showHUDToolStripMenuItem.Checked = toggleHUDToolStripMenuItem.Checked;
            if (toggleHUDToolStripMenuItem.Checked)
            {
                statusStrip1.Show();
            }
            else
            {
                statusStrip1.Hide();
            }
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the color customization screen
            Visuals visuals = new Visuals(this);
            visuals.Show();
        }

        private void gridOutlinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Redraw the board after toggling and updating the view menu
            toolStripMenuItem1.Checked = gridOutlinesToolStripMenuItem.Checked;
            Redraw();
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Redraw the board after toggling and updating the view menu
            showNeighborCountToolStripMenuItem.Checked = neighborCountToolStripMenuItem.Checked;
            Redraw();
        }

        private void wraparoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Redraw the board after toggling
            wrapAround = wraparoundToolStripMenuItem.Checked;
            Redraw();
        }

        private void randomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Redraw the board after toggling
            randomize = randomizeToolStripMenuItem.Checked;
            Redraw();
        }

        private void showHUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the other equivalant control to the same value and toggle the HUD
            toggleHUDToolStripMenuItem.Checked = showHUDToolStripMenuItem.Checked;
            if (showHUDToolStripMenuItem.Checked)
            {
                statusStrip1.Show();
            }
            else
            {
                statusStrip1.Hide();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Plaintext Files (*.cells)|*.cells";
            saveFileDialog1.Title = "Choose a name and location for the pattern...";
            savePattern();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Plaintext Files (*.cells)|*.cells";
            openFileDialog1.Title = "Choose a pattern file...";
            loadPatternAsUniverse();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Plaintext Files (*.cells)|*.cells";
            openFileDialog1.Title = "Choose a pattern file...";
            loadPatternIntoUniverse();
        }
    }
}
