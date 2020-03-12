using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_form
{
    public partial class chatControl : UserControl
    {
        public int height = 0;
        //文本字体大小
        private int textsize = 12;
        //标题字体大小
        private int titlesize = 9;

        public chatControl()
        {
            InitializeComponent();
            //this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
        }

        /// <summary>
        /// 动态添加文本信息
        /// </summary>
        /// <param name="text"></param>
        public void add(string title ,string text)
        {
            //添加时间
            Label L_title = new Label();
            L_title.Text = title;
            L_title.Name = "t";
            L_title.BorderStyle = BorderStyle.None;
            L_title.Font = new Font("微软雅黑", titlesize);
            L_title.Width = L_title.Text.Length * textsize;
            //L_message.Width = this.Width;
            L_title.Margin = new Padding(0, 0, 0, 0);
            L_title.Location = new Point(0, height);
            height += L_title.Height;
            this.panel1.Controls.Add(L_title);

            //添加消息

            TextBox L_message = new TextBox();

            L_message.Text = text;
            settextbox(L_message,20);

            L_message.Name = "t";
            L_message.Multiline = true;

            //L_message.Width = (int)this.CreateGraphics().MeasureString(L_message.Text,L_message.Font).Width;
            L_message.Width = 300;
           
            L_message.Height = L_message.Height * L_message.Text.Split('\r').Length;
            L_message.BorderStyle = BorderStyle.None;
            L_message.Font = new Font("微软雅黑", textsize);
            //L_message.Width = this.Width;
            L_message.Margin = new Padding(0, 0, 0, 0);
            L_message.Location = new Point(0,height);
            height += L_message.Height;
            this.panel1.Controls.Add(L_message);




            //
            //TextBox t_date = new TextBox();
            //t_date.Width = this.panel1.Width;
            ////t_date.Location = new Point(0,height);
            ////t_date.Name = "t";
            //t_date.ReadOnly = true;
            //t_date.BorderStyle = BorderStyle.None;
            //t_date.Font = new Font("微软雅黑", 12);
            //////t_date.Width = this.Width;
            //t_date.Margin = new Padding(0, 0, 0, 0);
            //t_date.Text = text;
            ////this.flowLayoutPanel1.Controls.Add(t_date);
            //t_date.Dock = DockStyle.Top;

            //this.flowLayoutPanel1.Controls.Add(t_date);

        }

        ////textbox根据父容器改变而改变
        //private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        //{
        //    foreach (Control i in this.flowLayoutPanel1.Controls)
        //    {
        //        i.Width = this.flowLayoutPanel1.Width;
        //    }
        //}

        /// <summary>
        /// 格式化textbox，强制让他一行显示几个字
        /// </summary>
        /// <param name="t"></param>
        /// <param name="maxlines"></param>
        private void settextbox(TextBox t,int maxlines)
        {
            if (t.TextLength > maxlines)
            {
                //this.textBox1.TextChanged -= new EventHandler(textBox1_TextChanged);
                string text = t.Text;
                text = text.Replace("\r\n", "");

                int lines;
                if (text.Length % maxlines == 0)
                {
                    lines = text.Length / maxlines;
                }
                else
                {
                    lines = text.Length / maxlines + 1;
                }
                for (int i = 1; i < lines; i++)
                {
                    text = text.Insert(i * maxlines + (i - 1) * 2, "\r\n");
                }
                t.Text = text;

                //this.textBox1.TextChanged += new EventHandler(textBox1_TextChanged);
            }
        }
    }
}
