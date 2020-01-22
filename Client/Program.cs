using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Method;

namespace Client
{
    class Program
    {
        static int count=-1;
        static Thread overtime;

        static void Main(string[] args)
        {
                send();

        }

        static void send()
        {
            //接收服务器的byte
            byte[] readBuff = new byte[1024];


            //Socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Connect
            socket.Connect("144.48.7.216", 2222);
            Console.WriteLine("请输入登录用户名");
            socket.Send(System.Text.Encoding.UTF8.GetBytes(Console.ReadLine()));

            //创建监听返回
            overtime = new Thread(new ParameterizedThreadStart(Is_Overtime));//创建线程
            overtime.Start(socket);                                               //启动线程

            try
            {
                count = socket.Receive(readBuff);
            }
            catch (Exception)
            {
                return;
            }

            overtime.Abort();

            string Recv_str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);

            Console.WriteLine("连接成功！");

            Thread thread = new Thread(new ParameterizedThreadStart(Recv));//创建线程
            thread.Start(socket);

            while (true)
            {
                //Send
                Console.WriteLine("输入你要发送的消息,输入e断开");
                string result = Console.ReadLine();

                if (result=="e")
                {
                    thread.Abort();
                    break;
                }
                string send_text = result;
                byte[] send_bytes = System.Text.Encoding.UTF8.GetBytes(send_text);
                socket.Send(send_bytes);

               
            }

            //Close
            socket.Send(System.Text.Encoding.UTF8.GetBytes("#exit"));
            socket.Close();
        }

        static void Recv(object s)
        {
            Socket socket = (Socket)s;
            byte[] readBuff = new byte[1024];
            //Recv

            while (true)
            {
                
                int count = socket.Receive(readBuff);
                string Recv_str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
                Console.WriteLine("服务器返回: " + Recv_str);
            }

        }

        static void Is_Overtime(object o)
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

            Console.WriteLine("登陆超时!");
            Method.Class1.Disconnect(socket);

        }


    }
}
