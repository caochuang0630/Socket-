﻿using System;
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
                IsLogin1(this.textBox1.Text, this.textBox2.Text);
            }
            else
            {
                this.label4.Text = "账号密码不能为空";
            }
            
        }

        /// <summary>
        /// 登陆函数
        /// </summary>
        /// <returns></returns>
        private void IsLogin(string user,string password)
        {
            byte[] readBuff = new byte[1024];

            ////Socket
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ////Connect
            //socket.Connect("144.48.7.216", 2222);
            //socket.Send(Encoding.UTF8.GetBytes("#1 login"));

            ////设置一个线程来检测登录是否超时
            //overtime = new Thread(new ParameterizedThreadStart(Is_Overtime));//创建线程
            //overtime.Start(socket);

            ////这个地方如果连接没有返回字符串说明服务器连接有问题,他就会卡在这里，检测线程就会等待5秒,如果5秒不回复,直接结束程序
            //try
            //{
            //    count = socket.Receive(readBuff);
            //}
            //catch (Exception)
            //{
            //    return;
            //}

            ////如果数据收到了就结束检测线程
            //overtime.Abort();
            Socket socket = Method.Connect("#1 Login");

            if (socket==null)
            {
                return;
            }
            socket.Send(Encoding.UTF8.GetBytes(String.Format("#Login {0},{1}",user,password)));

            ////设置一个线程来登录
            //login = new Thread(new ParameterizedThreadStart(Recv));//创建线程
            //login.Start(socket);
           
            //解析状态
            count = socket.Receive(readBuff);
            string Recv_str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            if (Recv_str == "#successful")
            {
                //登录成功
                //MessageBox.Show("登录成功");
                //使用类型2来进行chat连接，名字为用户名
                cf = new Chat_form(new Socket_info(Method.Connect("#2 "+this.textBox1.Text), this.textBox1.Text));

                cf.Show();

                this.Visible = false;
            }
            else if(Recv_str == "#fail")
            {
                //登录失败
                MessageBox.Show("登录失败");
            }else if (Recv_str == "#already")
            {
                //账号已登录
                MessageBox.Show("账号已经登录");
            }
            //Console.WriteLine("服务器返回: " + Recv_str);

            Method.Disconnect(socket);
            ////断开连接
            //socket.Send(Encoding.UTF8.GetBytes("#exit"));
            //socket.Close();


        }

        /// <summary>
        /// 登陆函数1（该方法采用全新的的登陆方式,只用一个socket，采用类型3）
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        private void IsLogin1(string user, string password)
        {
            //使用类型3来进行登录
            Socket connect =  Method.Connect(String.Format("#3 {0},{1}", user, password));

            byte[] readBuff = new byte[1024];
            try
            {
                count = connect.Receive(readBuff);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode==10060)
                {
                    return;
                }
            }
            
            string Recv_str = Encoding.UTF8.GetString(readBuff, 0, count);

            if (Recv_str == "#successful")
            {
                //登录成功
                //MessageBox.Show("登录成功");
                //使用类型2来进行chat连接，名字为用户名
                cf = new Chat_form(new Socket_info(connect, this.textBox1.Text));

                cf.Show();

                this.Visible = false;
            }
            else if (Recv_str == "#fail")
            {
                //登录失败
                MessageBox.Show("登录失败");
                Method.Disconnect(connect);
            }
            else if (Recv_str == "#already")
            {
                //账号已登录
                MessageBox.Show("账号已经登录");
                Method.Disconnect(connect);
            }

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
            //没完善前不打开
            MessageBox.Show("爬");

            //register f = new register();
            //f.ShowDialog();
        }
    }
}
