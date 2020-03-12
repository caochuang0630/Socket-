using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client_form;

namespace controls_test
{
    public partial class Form1 : Form
    {
        chatControl c;
        public Form1()
        {
            InitializeComponent();
            c = new chatControl();
            this.splitContainer1.Panel1.Controls.Add(c);
            c.Dock = DockStyle.Fill;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            c.add(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss") + " " + this.Text + ":", "啊啊啊啊啊啊啊啊啊啊啊aaaaaaaaaaaaaa啊啊啊啊啊啊啊");
        }

        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{
            
        //    if (this.textBox1.TextLength >10)
        //    {
        //        this.textBox1.TextChanged -= new EventHandler(textBox1_TextChanged);
        //        string text = this.textBox1.Text;
        //        text =  text.Replace("\r\n", "");

        //        int lines;
        //        if (this.textBox1.TextLength%10==0)
        //        {
        //            lines = text.Length / 10;
        //        }
        //        else
        //        {
        //            lines = text.Length / 10 + 1;
        //        }
        //        for (int i = 1; i < lines; i++)
        //        {
        //            text = text.Insert(i*10+(i-1)*2, "\r\n");
        //        }
        //        this.textBox1.Text = text;

        //        this.textBox1.TextChanged += new EventHandler(textBox1_TextChanged);
        //    }
        //}
    }
}
