using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_form
{
    public partial class Chat1 : Form
    {
        Socket_info chat_socket;


        //监听线程
        Thread Recv_thread;

        //判断发送是否成功
        bool Is_send = false;

        public Chat1(string name, Socket_info s)
        {
            InitializeComponent();
            this.Text = name;
            this.chat_socket = s;

            //开始监听
            Recv_thread = new Thread(new ParameterizedThreadStart(Recv));//创建线程
            Recv_thread.Start(chat_socket);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///先判断是不是空消息
            if (this.textBox1.Text != "")
            {
                chat_socket.socket.Send(Encoding.UTF8.GetBytes(String.Format("#Chat {0} {1}", this.Text, this.textBox1.Text)));

                for (int i = 0; i < 3; i++)
                {
                    //判断是否发送成功
                    if (Is_send == true)
                    {
                        this.chatControl1.add(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss") + " 我：", this.textBox1.Text);
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

        private delegate void Thread_control(string title,string text);

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
                try
                {
                    count = socket.socket.Receive(readBuff);
                    name = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);

                    if (name == "#发送成功" || name == "")
                    {
                        Is_send = true;
                        continue;
                    }
                    //测试语句
                    //MessageBox.Show("消息");

                    Thread_control thread_control = new Thread_control(this.chatControl1.add);
                    this.Invoke(thread_control, DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss") + " " + this.Text + ":", name);

                }
                catch (System.Net.Sockets.SocketException e)
                {
                    if (e.ErrorCode==10060)
                    {
                        continue;
                    }
                    
                }
            }
        }

        //窗口关闭前关闭监听线程
        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Recv_thread.Abort();
        }

        //按下回车键键发送消息
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(null, null);

            }
        }
    }
}
