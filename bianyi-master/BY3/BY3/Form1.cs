using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace BY3
{
    public partial class Form1 : Form
    {
        CFFX c;
        public Form1()
        {
            InitializeComponent();
            c = new CFFX();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            int index = textBox1.GetFirstCharIndexOfCurrentLine();
            int line = textBox1.GetLineFromCharIndex(index) + 1;
            int col = textBox1.SelectionStart - index + 1;
            label3.Text = line + " 行，" + col + " 列";
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int index = textBox1.GetFirstCharIndexOfCurrentLine();
            int line = textBox1.GetLineFromCharIndex(index) + 1;
            int col = textBox1.SelectionStart - index + 1;
            label3.Text = line + " 行，" + col + " 列";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String phrase = "";
            String error = "";
            //词法分析
            c.fenxi(textBox1.Text, out phrase, out error);
            textBox2.Text = phrase;  //词法分析结果     
            textBox4.Text = "词法分析错误:\r\n";
            textBox4.Text += error;  //出现的错误
            //Console.WriteLine(phrase);
            //句法分析
            JFFX j = new JFFX();
            String sen = "";
            j.fenxi(phrase, out sen, out error);
            textBox3.Text = sen;
            textBox4.Text += "\r\n语法分析错误:\r\n";
            textBox4.Text += error;

            //中间代码
            textBox5.Text = YYFX.yuyi;
            textBox5.Text += (YYFX.line+1) + "\tend\r\n";//输出end 终结语义分析

            //符号表
            this.dataGridView1.Rows.Clear();
            Hashtable fuhaobiao = YYFX.fuhaoBiao;
            foreach (DictionaryEntry d in fuhaobiao)
            {
                FuHao fuhao = (FuHao)d.Value;
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = fuhao.name;
                this.dataGridView1.Rows[index].Cells[1].Value = fuhao.width;
                this.dataGridView1.Rows[index].Cells[2].Value = fuhao.offset;
                this.dataGridView1.Rows[index].Cells[3].Value = fuhao.leixin;
            }
            dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
        }
    }
}
