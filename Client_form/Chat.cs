using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace Client_form
{
    public partial class Chat : Form
    {
        Socket_info chat_socket;



        //监听线程
        Thread Recv_thread;

        //判断发送是否成功
        bool Is_send = false;

        public Chat(string name, Socket_info s)
        {
            InitializeComponent();
            this.Text = name;
            this.chat_socket = s;

            //开始监听
            Recv_thread = new Thread(new ParameterizedThreadStart(Recv));//创建线程
            Recv_thread.Start(chat_socket);
        }

        //发送按钮
        private void button1_Click(object sender, EventArgs e)
        {
            ///先判断是不是空消息
            if (this.textBox1.Text!="")
            {
                chat_socket.socket.Send(Encoding.UTF8.GetBytes(String.Format("#Chat {0} {1}",this.Text,this.textBox1.Text)));

                for (int i = 0; i < 3; i++)
                {
                    //判断是否发送成功
                    if (Is_send==true)
                    {
                        this.listBox1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss" + " 我："));
                        this.listBox1.Items.Add(this.textBox1.Text);
                        this.textBox1.Text = "";
                        return;
                    }
                    Thread.Sleep(1000);
                }

                MessageBox.Show("发送超时！");
                
            }
            else
            {
                MessageBox.Show("消息不能为空");
            }
        }

        /// <summary>
        /// 监听线程
        /// </summary>
        /// <param name="s"></param>
        private void Recv(object s)
        {
            Socket_info socket = (Socket_info)s;

            byte[] readBuff = new byte[1024];
            int count;
            string name = "";
            while (true)
            {
                count = socket.socket.Receive(readBuff);
                name = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);

                if (name=="#发送成功")
                {
                    Is_send = true;
                    continue;
                }

                this.listBox1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss" + " " + this.Text+":"));
                this.listBox1.Items.Add(name);
            }
        }

        //窗口关闭前关闭监听线程
        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Recv_thread.Abort();
        }
    }
}
