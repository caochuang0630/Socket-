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

            //Connect
            socket.Connect("144.48.7.216", 2222);
            socket.Send(Encoding.UTF8.GetBytes(con_name));
            int count = socket.Receive(readBuff);
            return socket;
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
    }
}
