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
        int[] finishCoord = new int[2];

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

            foreach(Tuple<int,int> direction in directions)
            {
                int newX = x + direction.Item1;
                int newY = y + direction.Item2;

                if(newX >= 0 && newX <= MaxX
                    && newY >= 0 && newY <= MaxY
                    && playground[newX, newY].IsBrick)
                {
                    playground[(x + newX) / 2, (y + newY) / 2].Dig();
                    walk_generate(newX, newY);
                }
            }
        }

        private int walk_solve(int distance, int x, int y)
        {
            if (x == finishCoord[0] && y == finishCoord[1])
                return 1;

            if (!playground[x, y].IsFreeWay)
                return 0;

            playground[x, y].Speculate();

            List<Tuple<int, int>> directions = new List<Tuple<int, int>>() {
                new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0), new Tuple<int, int>(0, 1), new Tuple<int, int>(0, -1) };

            int ret = 0;

            foreach (Tuple<int, int> direction in directions)
            {
                int newX = x + direction.Item1;
                int newY = y + direction.Item2;

                if (newX >= 0 && newX <= MaxX
                    && newY >= 0 && newY <= MaxY
                    && playground[newX, newY].IsFreeWay)
                {
                    ret += walk_solve(distance + 1, newX, newY);
                }
            }

            if(ret > 0)
            {
                playground[x, y].MarkSolution(distance);
            }
            else
            {
                playground[x, y].Unspeculate();
            }

            return ret;
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

            for(int y = 0; y <= MaxY; y++)
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

                    if(x == startCoord[0] && y == startCoord[1])
                    {
                        e.Graphics.DrawString("S", defaultFont, drawBrush, x * CELL_X, y * CELL_Y);
                    }

                    if(x == finishCoord[0] && y == finishCoord[1])
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
            walk_solve(0, startCoord[0], startCoord[1]);
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

            if(flMarkWay)
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
    }

    public class Tile
    {
        char content;

        public bool IsBrick => content.Equals('#');

        public bool IsFreeWay => content.Equals(' ');

        public Tile(char _type)
        {
            content = _type;
        }

        public void Dig()
        {
            content = ' ';
        }

        public void Speculate()
        {
            content = 's';
        }

        public void Unspeculate()
        {
            content = ' ';
        }

        int solution_distance = -1;

        public void MarkSolution(int distance)
        {
            solution_distance = distance;
        }

        public bool IsSolution => solution_distance > -1;

        public void SetContent(char nc)
        {
            content = nc;
        }
    }
}
