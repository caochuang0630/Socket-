using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client_form
{
    public class Socket_info
    {
        public Socket_info(Socket s, string n)
        {
            this.socket = s;
            this.name = n;
        }

        public Socket socket;
        public string name;
    }
}
