using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Sockets;
using Method;


namespace server
{
    class IsMethod
    {
        /// <summary>
        /// 监听socket送来的数据
        /// </summary>
        /// <param name="socket"></param>
        public static void Socket_Thread_listen(object s)
        {
            Socket_Thread socket = (Socket_Thread)s;

            byte[] readBuff = new byte[1024];
            int count;
            string str;
            while (true)
            {
                count = socket.socket.Receive(readBuff);
                str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
                Console.WriteLine("[服务器接收]" + str);

                if (str=="#exit")
                {
                    Console.WriteLine(socket.name+" 客户端断开了连接");
                    return;
                }

                IsSQL(str, socket.socket);
                IsChat(str, socket.socket);
            }
        }

        public static void Socket_send(Socket_Thread socket, string text)
        {
            socket.socket.Send(System.Text.Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// 判断是不是sql语句
        /// </summary>
        /// <param name="text"></param>
        /// <param name="connfd"></param>
        public static void IsSQL(string text,Socket connfd)
        {
            DBhelper db = new DBhelper();
            if (Regex.IsMatch(text,@"^#SQL "))
            {
                try
                {
                    string sql = text.Substring(5);
                    int result = db.query(sql);

                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes("影响" + result + "行");
                    connfd.Send(bytes);
                }
                catch (Exception)
                {
                    connfd.Send(Encoding.UTF8.GetBytes("命令错误"));
                }
                
            }
        }

        /// <summary>
        /// 判断是不是聊天语句
        /// </summary>
        /// <param name="text"></param>
        /// <param name="connfd"></param>
        public static void IsChat(string text, Socket connfd)
        {
            //命令格式 #Chat 用户id 发送的内容
            if(Regex.IsMatch(text,@"^#Chat "))
            {
                try
                {
                    string name = text.Substring(Method.Class1.index_char(text, ' ', 1) + 1, (Method.Class1.index_char(text, ' ', 2)) - (Method.Class1.index_char(text, ' ', 1)) - 1);
                    string send_text = text.Substring(Method.Class1.index_char(text, ' ', 2) + 1);

                    //根据查找找到相应socket来发送
                    List<Socket_Thread> result = Data.list_Socket.Where(x => x.name == name).ToList();
                    result[0].socket.Send(System.Text.Encoding.UTF8.GetBytes(send_text));
                    connfd.Send(System.Text.Encoding.UTF8.GetBytes("发送成功"));
                }
                catch (Exception)
                {
                    connfd.Send(Encoding.UTF8.GetBytes("命令错误"));
                }
                
            }

            //命令作用 获取当前服务器的用户名
            if (text=="#Chat-getusers")
            {
                foreach (Socket_Thread i in Data.list_Socket )
                {
                    connfd.Send(Encoding.UTF8.GetBytes(i.name));
                }
                
            }
        }
    }
}
