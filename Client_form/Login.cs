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
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsLogin(this.textBox1.Text,this.textBox2.Text);
        }

        /// <summary>
        /// 判断账号密码是否正确
        /// </summary>
        /// <returns></returns>
        private void IsLogin(string user,string password)
        {
            //Socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Connect
            socket.Connect("144.48.7.216", 2222);
            socket.Send(Encoding.UTF8.GetBytes("login"));

            socket.Send(Encoding.UTF8.GetBytes(String.Format("#SQL-s select * from [User] where username='{0}' and password='{1}'",user,password)));

            byte[] readBuff = new byte[1024];
            int count = socket.Receive(readBuff);
            string Recv_str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            if (Convert.ToInt32(Recv_str) > 0)
            {
                //登录成功
                //MessageBox.Show("登录成功");
                Chat_form f = new Chat_form(Method.Connect(this.textBox1.Text));
                f.Show();
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


    }
}
