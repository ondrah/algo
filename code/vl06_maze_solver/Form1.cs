using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace maze
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Tile[,] playground = null;

        bool flSettingStart;
        bool flSettingFinish;
        bool flMarkBrick;
        bool flMarkWay;

        int[] startCoord = new int[2];
        int[] finishCoord = new int[2] { 16, 16 };

        private static Random rng = new Random();

        /// <summary>
        /// Fisher-Yates list shuffle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private int MaxX => playground.GetLength(0) - 1;
        private int MaxY => playground.GetLength(1) - 1;

        private void walk_generate(int x, int y)
        {
            playground[x, y].Dig();

            List<Tuple<int, int>> directions = new List<Tuple<int, int>>() {
                new Tuple<int, int>(2, 0), new Tuple<int, int>(-2, 0), new Tuple<int, int>(0, 2), new Tuple<int, int>(0, -2) };

            Shuffle(directions);

            foreach (Tuple<int, int> direction in directions)
            {
                int newX = x + direction.Item1;
                int newY = y + direction.Item2;

                if (newX >= 0 && newX <= MaxX
                    && newY >= 0 && newY <= MaxY
                    && playground[newX, newY].IsBrick)
                {
                    playground[(x + newX) / 2, (y + newY) / 2].Dig();
                    walk_generate(newX, newY);
                }
            }
        }

        /// <summary>
        /// Walk through a maze recursively.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>true is the field is a part of path between start and finish</returns>
        private bool walk_solve(int x, int y)
        {
            if (x == finishCoord[0] && y == finishCoord[1])
                return true;

            if (!playground[x, y].IsFreeWay)
                return false;

            // mark this field as speculation (do not go here anymore in recursive walk_solve calls)
            playground[x, y].Speculate();

            List<Tuple<int, int>> directions = new List<Tuple<int, int>>() {
                new Tuple<int, int>(1, 0), // right
                new Tuple<int, int>(-1, 0), // left
                new Tuple<int, int>(0, 1), // down
                new Tuple<int, int>(0, -1) // up
            };

            bool isSolution = false;

            foreach (Tuple<int, int> direction in directions)
            {
                int newX = x + direction.Item1;
                int newY = y + direction.Item2;

                if (newX >= 0 && newX <= MaxX
                    && newY >= 0 && newY <= MaxY
                    && (playground[newX, newY].IsFreeWay
                    || playground[newX, newY].IsSolution))
                {
                    // if the new field is a solution, there is no need to go there
                    if (playground[newX, newY].IsSolution || walk_solve(newX, newY))
                    {
                        isSolution = true;
                    }
                }
            }

            if(isSolution)
            {
                playground[x, y].IsSolution = true;
            }
            else
            {
                playground[x, y].Unspeculate();
            }

            return isSolution;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            playground = new Tile[17, 17];

            for (int y = 0; y <= MaxY; y++)
            {
                for (int x = 0; x <= MaxX; x++)
                {
                    playground[x, y] = new Tile('#');
                }
            }

            walk_generate(0, 0);

            panel1.Invalidate();
        }


        static System.Drawing.Font defaultFont = new System.Drawing.Font("Arial", 16);
        static System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

        const int CELL_X = 20;
        const int CELL_Y = 20;

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Pen myPen = new Pen(Color.Blue, 2);
            Pen myWhitePen = new Pen(Color.White, 2);
            //e.Graphics.DrawLine(myPen, 0, 0, 60, 30);

            if (playground == null)
                return;

            for (int y = 0; y <= MaxY; y++)
            {
                for (int x = 0; x <= MaxX; x++)
                {
                    if (playground[x, y] == null)
                    {
                        e.Graphics.DrawString("?", defaultFont, drawBrush, x * CELL_X, y * CELL_Y);
                        continue;
                    }

                    if (playground[x, y].IsBrick)
                    {
                        e.Graphics.FillRectangle(Brushes.DarkRed, x * CELL_X, y * CELL_Y, CELL_X, CELL_Y);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.White, x * CELL_X, y * CELL_Y, CELL_X, CELL_Y);
                    }

                    if (x == startCoord[0] && y == startCoord[1])
                    {
                        e.Graphics.DrawString("S", defaultFont, drawBrush, x * CELL_X, y * CELL_Y);
                    }

                    if (x == finishCoord[0] && y == finishCoord[1])
                    {
                        e.Graphics.DrawString("F", defaultFont, drawBrush, x * CELL_X, y * CELL_Y);
                    }

                    if (playground[x, y].IsSolution)
                    {
                        e.Graphics.DrawString("*", defaultFont, Brushes.Green, x * CELL_X, y * CELL_Y);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            flSettingFinish = false;
            flSettingStart = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            flSettingStart = false;
            flSettingFinish = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            walk_solve(startCoord[0], startCoord[1]);
            panel1.Invalidate();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Point click = panel1.PointToClient(e.Location);

            int x = e.X / CELL_X, y = e.Y / CELL_Y;

            if (flSettingStart)
            {
                flSettingStart = false;
                startCoord[0] = x;
                startCoord[1] = y;

                panel1.Invalidate();
            }

            if (flSettingFinish)
            {
                flSettingFinish = false;
                finishCoord[0] = x;
                finishCoord[1] = y;

                panel1.Invalidate();
            }

            if (flMarkBrick)
            {
                flMarkWay = false;
                playground[x, y].SetContent('#');
                panel1.Invalidate();
            }

            if (flMarkWay)
            {
                flMarkWay = false;
                playground[x, y].SetContent(' ');
                panel1.Invalidate();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            flMarkBrick = true;
            flMarkWay = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            flMarkWay = true;
            flMarkBrick = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int y = 0; y <= MaxY; y++)
            {
                for (int x = 0; x <= MaxX; x++)
                {
                    if (!playground[x, y].IsBrick)
                    {
                        playground[x, y].Reset();
                    }
                }
            }
            panel1.Invalidate();
        }
    }

    public class Tile
    {
        char content;

        /// <summary>
        /// Is it a brick?
        /// </summary>
        public bool IsBrick => content.Equals('#');

        /// <summary>
        /// Is it a free path?
        /// </summary>
        public bool IsFreeWay => content.Equals(' ');

        /// <summary>
        /// Initialize a new tile with given content.
        /// </summary>
        /// <param name="_type"></param>
        public Tile(char _type)
        {
            content = _type;
        }

        /// <summary>
        /// Dig a way.
        /// </summary>
        public void Dig()
        {
            content = ' ';
        }

        /// <summary>
        /// Mark field as speculation.
        /// </summary>
        public void Speculate()
        {
            content = '?';
        }

        /// <summary>
        /// Restore speculation to a free path.
        /// </summary>
        public void Unspeculate()
        {
            content = ' ';
        }

        /// <summary>
        /// Indicates whether the path field is a possible solution or not.
        /// </summary>
        public bool IsSolution { get; set; }

        /// <summary>
        /// Reset a path field.
        /// </summary>
        public void Reset()
        {
            IsSolution = false;
            content = ' ';
        }

        public void SetContent(char nc)
        {
            content = nc;
        }
    }
}
