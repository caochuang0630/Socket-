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
        public Socket_Thread(Socket s,string n)
        {
            this.socket = s;
            this.name = n;
        }

        public Socket socket;
        public string name;
    }
}
