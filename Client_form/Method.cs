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
    class Method
    {

        static byte[] readBuff = new byte[1024];
        /// <summary>
        /// 连接服务器方法
        /// </summary>
        /// <param name="con_name">连接用户名</param>
        public static Socket Connect(string con_name)
        {
            //Socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //设置超时
            socket.ReceiveTimeout = 5000;
            socket.SendTimeout = 5000;

            //Connect
            socket.Connect("144.48.7.216", 2222);
            socket.Send(Encoding.UTF8.GetBytes(con_name));

            int count;
            try
            {
                count = socket.Receive(readBuff);
            }
            catch (Exception)
            {
                MessageBox.Show("登陆超时,请检查网络");
                return null;
            }
            
            string Recv_str = Encoding.UTF8.GetString(readBuff, 0, count);
            if (Recv_str=="#True")
            {
                socket.Send(Encoding.UTF8.GetBytes("#hello"));
                return socket;
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// 断开一个socket的连接
        /// </summary>
        public static void Disconnect(Socket s)
        {
            try
            {
                s.Send(Encoding.UTF8.GetBytes("#exit"));
            }
            catch (Exception)
            {
                
            }
            
            s.Close();
        }

        public delegate void SetListBoxCallback(ListBox listbox, string text); //定义委托

        public static void add_info(ListBox listbox,string text)
        {
            if (listbox.InvokeRequired)
            {
                SetListBoxCallback s = new SetListBoxCallback(add_info);
                listbox.Invoke(s, listbox, text);
            }
            else
            {
                listbox.Items.Add(text);
            }
        }

        /// <summary>
        /// 设置窗体居中
        /// </summary>
        /// <param name="form"></param>
        public static void set_position_center(Form form)
        {
            //获取桌面宽高
            int desktop_width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            int desktop_height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;

            //获取程序宽高
            int form_width = form.Width;
            int form_height = form.Height;

            //设置居中
            
        }
    }
}
