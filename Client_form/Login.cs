using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_form
{
    public partial class Login : Form
    {
        //检测登陆超时线程
        Thread overtime;
        int count = -1;

        //大厅窗口
        Chat_form cf;

        public Login()
        {
            InitializeComponent();
            //开放线程之间对控件的操作
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!(this.textBox1.Text==""||this.textBox2.Text==""))
            {
                IsLogin(this.textBox1.Text, this.textBox2.Text);
            }
            else
            {
                this.label4.Text = "账号密码不能为空";
            }
            
        }

        /// <summary>
        /// 判断账号密码是否正确
        /// </summary>
        /// <returns></returns>
        private void IsLogin(string user,string password)
        {
            byte[] readBuff = new byte[1024];

            //Socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Connect
            socket.Connect("144.48.7.216", 2222);
            socket.Send(Encoding.UTF8.GetBytes("login"));

            //设置一个线程来检测登录是否超时
            overtime = new Thread(new ParameterizedThreadStart(Is_Overtime));//创建线程
            overtime.Start(socket);

            //这个地方如果连接没有返回字符串说明服务器连接有问题,他就会卡在这里，检测线程就会等待5秒,如果5秒不回复,直接结束程序
            try
            {
                count = socket.Receive(readBuff);
            }
            catch (Exception)
            {
                return;
            }

            //如果数据收到了就结束检测线程
            overtime.Abort();

            socket.Send(Encoding.UTF8.GetBytes(String.Format("#SQL-s select * from [User] where username='{0}' and password='{1}'",user,password)));

            ////设置一个线程来登录
            //login = new Thread(new ParameterizedThreadStart(Recv));//创建线程
            //login.Start(socket);
           
            count = socket.Receive(readBuff);

            string Recv_str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            if (Convert.ToInt32(Recv_str) > 0)
            {
                //登录成功
                //MessageBox.Show("登录成功");
                cf = new Chat_form(new Socket_info(Method.Connect(this.textBox1.Text), this.textBox1.Text));

                cf.Show();

                this.Visible = false;
            }
            else
            {
                //登录失败
                MessageBox.Show("登录失败");
            }
            //Console.WriteLine("服务器返回: " + Recv_str);

            //断开连接
            socket.Send(Encoding.UTF8.GetBytes("#exit"));
            socket.Close();


        }

        private void Is_Overtime(object o)
        {

            Socket socket = (Socket)o;

            for (int i = 0; i < 5; i++)
            {
                //判断count值获取到了没，获取到了就结束进程
                if (count != -1)
                {
                    overtime.Abort();
                    return;
                }
                Thread.Sleep(1000);

            }

            MessageBox.Show("登陆超时!");
            Method.Disconnect(socket);
            Application.Exit();
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            register f = new register();
            f.ShowDialog();
        }
    }
}
