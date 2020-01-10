using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text += "1";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text += "2";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Length > 0)
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
            }
        }

        private void button5_Click(object sender, EventArgs ea)
        {
            string expression = textBox1.Text;

            tbLog.AppendText($"input: \"{expression}\"\r\n");

            Expression exp = new Expression(expression);

            try
            {
                double result = exp.GetResult();
                tbLog.AppendText($"output: {result.ToString()}\r\n");

                string rpn = exp.GetRPN();
                tbLog.AppendText($"RPN: {rpn}\r\n");
            }
            catch (Exception e)
            {
                tbLog.AppendText($"invalid expression: {e.Message}\r\n");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text += "3";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox1.Text += "4";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox1.Text += "5";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Text += "6";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            textBox1.Text += "7";
        }

        private void button14_Click(object sender, EventArgs e)
        {
            textBox1.Text += "8";
        }

        private void button15_Click(object sender, EventArgs e)
        {
            textBox1.Text += "9";
        }

        private void button16_Click(object sender, EventArgs e)
        {
            textBox1.Text += "0";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text += "+";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Text += "-";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Text += "*";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text += "/";
        }

        private void button17_Click(object sender, EventArgs e)
        {
            textBox1.Text += "(";
        }

        private void button18_Click(object sender, EventArgs e)
        {
            textBox1.Text += ")";
        }
    }
}
