using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Sockets;
using Method;
using System.Data;
using System.Threading;
using System.Net;
using System.IO;

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
                    Data.list_Socket.Remove(socket);
                    socket.socket.Close();
                    return;
                }

                IsSQL(str, socket);
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
        public static void IsSQL(string text, Socket_Thread connfd)
        {
            DBhelper db = new DBhelper();
            if (Regex.IsMatch(text,@"^#SQL"))
            {
                //微信推送
                HttpGet(String.Format("用户：【{0}】尝试访问SQL接口",connfd.name),text.Substring(4));

                //判断是增删改，还是查
                if (Regex.IsMatch(text,@"^#SQL-q "))
                {
                    try
                    {
                        //增删改返回影响行数
                        string sql = text.Substring(7);
                        int result = db.query(sql);

                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(result.ToString());
                        connfd.socket.Send(bytes);
                    }
                    catch (Exception)
                    {
                        connfd.socket.Send(Encoding.UTF8.GetBytes("命令错误"));
                    }
                }else if (Regex.IsMatch(text,@"^#SQL-s "))
                {
                    try
                    {
                        //查返回查询到了几行
                        string sql = text.Substring(7);
                        DataTable result = db.GetTable(sql);

                        connfd.socket.Send(Encoding.UTF8.GetBytes(result.Rows.Count.ToString()));
                    }
                    catch (Exception)
                    {
                        connfd.socket.Send(Encoding.UTF8.GetBytes("命令错误"));
                    }
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

                    try
                    {
                        //根据查找找到相应socket来发送
                        List<Socket_Thread> result = Data.list_Socket.Where(x => x.name == name).ToList();
                        result[0].socket.Send(System.Text.Encoding.UTF8.GetBytes(send_text));
                        connfd.Send(System.Text.Encoding.UTF8.GetBytes("#发送成功"));
                    }
                    catch (Exception)
                    {
                        connfd.Send(Encoding.UTF8.GetBytes("#用户名不存在,或者用户已经下线"));
                    }
                    
                }
                catch (Exception)
                {
                    connfd.Send(Encoding.UTF8.GetBytes("#命令错误"));
                }
                
            }

            //命令作用 获取当前服务器的用户名
            if (text=="#Chat-getusers")
            {
                foreach (Socket_Thread i in Data.list_Socket )
                {
                    connfd.Send(Encoding.UTF8.GetBytes(i.name));
                    Thread.Sleep(100);
                }
                Thread.Sleep(100);
                connfd.Send(Encoding.UTF8.GetBytes("#End"));
            }
        }


        /// <summary>
        /// 微信推送接口
        /// </summary>
        /// <param name="url">你想微信推送的消息</param>
        /// <returns></returns>
        public static string HttpGet(string text,string desp)
        {
            //微信推送get请求到方糖
            string url = String.Format("https://sc.ftqq.com/SCU12102Td2a63a7ac77a8f1e4f60668da76df1b159c2a2c6a516f.send?text={0}&desp={1}", text,desp);

            WebRequest myWebRequest = WebRequest.Create(url);
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream ReceiveStream = myWebResponse.GetResponseStream();
            string responseStr = "";
            if (ReceiveStream != null)
            {
                StreamReader reader = new StreamReader(ReceiveStream, Encoding.UTF8);
                responseStr = reader.ReadToEnd();
                reader.Close();
            }
            myWebResponse.Close();
            return responseStr;

        }
    }
}
