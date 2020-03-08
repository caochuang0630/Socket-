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
        private static DBhelper db = new DBhelper();
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
                try
                {
                    count = socket.socket.Receive(readBuff);
                }
                catch (Exception)
                {
                    Console.WriteLine("客户端异常断开连接");
                    disconnect(socket);
                    return;
                }
                

                str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
                //检测客户端是否异常断开
                if (str=="")
                {
                    Console.WriteLine("客户端异常断开连接");
                    disconnect(socket);
                    return;
                }
                Console.WriteLine("[服务器接收]" + str);

                if (str=="#exit")
                {
                    disconnect(socket);
                    return;
                }

                ////检测是否连接
                //if (!IsSocketConnected(socket.socket))
                //{
                //    Console.WriteLine("客户调已经异常退出");

                //    Console.WriteLine(socket.name + " 客户端断开了连接");
                //    Data.list_Socket.Remove(socket);
                //    socket.socket.Close();
                //    return;
                //}

                IsSQL(str, socket);
                IsChat(str, socket.socket);
                IsLogin(str, socket.socket);
            }
        }

        public static void Socket_send(Socket_Thread socket, string text)
        {
            socket.socket.Send(System.Text.Encoding.UTF8.GetBytes(text));
        }

        private static void disconnect(Socket_Thread socket)
        {
            Console.WriteLine(socket.name + " 客户端断开了连接");
            //判断连接类型,2为指定chat连接类型,当chat断开连接时，应该把数据库的status信息更新为off
            if (socket.type==2)
            {
                db.query(String.Format("update [User] set status = 'off' where username = '{0}' ",socket.name)) ;
            }
            Data.list_Socket.Remove(socket);
            socket.socket.Close();
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

            ////命令作用 获取当前服务器的用户名
            //if (text=="#Chat-getusers")
            //{
            //    foreach (Socket_Thread i in Data.list_Socket )
            //    {
            //        connfd.Send(Encoding.UTF8.GetBytes(i.name));
            //        Thread.Sleep(100);
            //    }
            //    Thread.Sleep(100);
            //    connfd.Send(Encoding.UTF8.GetBytes("#End"));
            //}

            if (text == "#Chat-getusers")
            {
                //建立变量，存储用户名
                string[] user_list = new string[Data.list_Socket.Count];
                for (int i = 0; i < Data.list_Socket.Count; i++)
                {
                    user_list[i] = Data.list_Socket[i].name;
                }

                //拼接字符串,并返回给客户端
                connfd.Send(Encoding.UTF8.GetBytes(String.Join("*",user_list))); 
            }

            
        }
        /// <summary>
        /// 判断是否登陆命令
        /// </summary>
        /// <param name="text"></param>
        /// <param name="connfd"></param>
        public static void IsLogin(string text,Socket connfd)
        {
            DBhelper db = new DBhelper();
            //解析命令是否合法
            if (Regex.IsMatch(text,@"^#Login "))
            {
                try
                {
                    //字符串操作出来账号密码
                    string username = text.Substring(7).Split(',')[0];
                    string password = text.Substring(7).Split(',')[1];

                    //验证账号密码
                    if(password == db.GetScalar(String.Format("select password from [User] where username='{0}'", username)))
                    {
                        //判断账号是否已经的登陆
                        string status =  db.GetScalar(String.Format("select status from [User] where username = '{0}'",username));
                        if (status=="on")
                        {
                            //说明账号已经登录无需登录
                            connfd.Send(Encoding.UTF8.GetBytes("#already")) ;
                        }else if (status=="off")
                        {
                            //说明没有登陆允许登录
                            //数据库写入状态--登录
                            db.query(String.Format(" update [User] set status = 'on',last_login_date=getdate() where username = '{0}' ", username));
                            connfd.Send(Encoding.UTF8.GetBytes("#successful"));
                        }
                    }
                    else
                    {
                        connfd.Send(Encoding.UTF8.GetBytes("#fail"));
                    }
                }
                catch (Exception)
                {
                    connfd.Send(Encoding.UTF8.GetBytes("#命令错误"));
                    throw;
                }
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

        /// <summary>
        /// 检查一个Socket是否可连接
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static bool IsSocketConnected(Socket client)
        {
            bool blockingState = client.Blocking;
            try
            {
                byte[] tmp = new byte[1];
                client.Blocking = false;
                client.Send(tmp, 0, 0);
                return true;
            }
            catch (SocketException e)
            {
                // 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的
                if (e.NativeErrorCode.Equals(10035))
                    return false;
                else
                    return true;
            }
            finally
            {
                client.Blocking = blockingState;    // 恢复状态
            }
        }
    }
}
