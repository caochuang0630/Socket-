using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Client_form
{
    class Method
    {
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
            return socket;
        }

        
    }
}
