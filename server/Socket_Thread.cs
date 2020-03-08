using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace server
{
    class Socket_Thread
    {
        public Socket_Thread(Socket s,string n,int t)
        {
            this.socket = s;
            this.name = n;
            this.type = t;
        }

        public Socket socket;//类的socket
        public string name;//socket连接名字
        public int type;//连接类型
    }
}
