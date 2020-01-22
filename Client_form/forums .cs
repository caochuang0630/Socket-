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
    public partial class Chat_form : Form
    {
        //用与服务器建立真正的连接的socket
        Socket_info Chat_socket;
        //准备连接取用户名
        List<string> users = new List<string>();
        //监听线程
        Thread Recv_thread;

        public Chat_form(Socket_info s)
        {
            InitializeComponent();
            this.Chat_socket = s;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Refresh_user();
        }

        private void Refresh_user()
        {
            //清空数据
            this.listBox1.Items.Clear();

            string login_name = "getuser";
            Socket socket = Method.Connect(login_name);

            //开始监听
            //Recv_thread = new Thread(new ParameterizedThreadStart(Recv));//创建线程
            //Recv_thread.Start(socket);

            socket.Send(Encoding.UTF8.GetBytes("#Chat-getusers"));

            byte[] readBuff = new byte[1024];
            int count;
            string name = "";
            while (true)
            {
                count = socket.Receive(readBuff);
                name = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);

                users.Add(name);
                //判断结束
                if (name == "#End")
                {
                    socket.Send(System.Text.Encoding.UTF8.GetBytes("#exit"));
                    socket.Close();
                    break;
                }
            }

            //Thread.Sleep(3000);

            //吧取回来的数据remove掉一个上面用“取用户名”登录查询的这个name
            users.Remove(login_name);
            users.Remove("#End");
            users.Remove(Chat_socket.name);

            //this.listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度 

            for (int i = 0; i < users.Count; i++)   //添加数据 
            {
                this.listBox1.Items.Add(users[i]);
            }

            //this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            users.Clear();

            
        }

        private  void Recv(object s)
        {
            Socket socket = (Socket)s;

            byte[] readBuff = new byte[1024];
            int count;
            string name = "";
            while (true)
            {
                count = socket.Receive(readBuff);
                name = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);

                users.Add(name);
                //判断结束
                if (name == "#End")
                {
                    socket.Send(System.Text.Encoding.UTF8.GetBytes("#exit"));
                    socket.Close();
                    Recv_thread.Abort();
                    break;
                }
            }
        }

        private void Chat_form_Load(object sender, EventArgs e)
        {
            
        }

        //窗体关闭
        private void Chat_form_FormClosing(object sender, FormClosingEventArgs e)
        {

            //断开连接
            Method.Disconnect(Chat_socket.socket);

            Application.Exit();
        }

        //选中一个用户聊天弹出
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Chat f = new Chat(listBox1.Text, Chat_socket);
            f.Show();
        }
    }
}
