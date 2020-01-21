using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Method
{
    public class Class1
    {
        public static List<byte[]> 拆分Byte数组(byte[] bytes)
        {
            List<byte[]> list_bytes = new List<byte[]>();

            //判断长度，不够拆分直接返回
            if (bytes.Length<=1024)
            {
                list_bytes.Add(bytes);
                return list_bytes;
            }

            //取得拆分后的list长度
            int length;
            if (bytes.Length % 1024 == 0)
            {
                length = bytes.Length / 1024;
            }
            else
            {
                length = (bytes.Length / 1024) + 1;
            }

            for (int i = 0; i < bytes.Length; i+=1024)
            {
                byte[] small_byte = new byte[1024];
                try
                {
                    Array.Copy(bytes, i, small_byte, 0, small_byte.Length);
                }
                catch (Exception)
                {
                    Array.Copy(bytes, i, small_byte, 0, bytes.Length-((length-1)*1024));
                }
                list_bytes.Add(small_byte);
            }
            return list_bytes;
        }

        public static void 发送(Socket socket,string text)
        {
            List<byte[]> send = 拆分Byte数组(System.Text.Encoding.Default.GetBytes("hello"));
            socket.Send(System.Text.Encoding.Default.GetBytes("sendlength:" + send.Count));

            for (int i = 0; i < send.Count; i++)
            {
                socket.Send(send[i]);
            }
        }

        public static void 接收(Socket listenfd,List<byte[]> bytes)
        {
            Socket connfd = listenfd.Accept();
            byte[] readBuff = new byte[1024];
            int count = connfd.Receive(readBuff);
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);

            int length;
            if (Regex.IsMatch(str, @"^sendlength:"))
            {
                length = Convert.ToInt32( str.Substring(11));
            }
        }

        /// <summary>
        /// 根据字符串和字符找出这个字符出现在字符串第几次的索引
        /// </summary>
        /// <param name="find_text">查找字符串</param>
        /// <param name="target">查找字符</param>
        /// <param name="index">查找第几次</param>
        /// <returns></returns>
        public static int index_char(string find_text,char target,int times)
        {
            int num = 0;

            char[] str = find_text.ToCharArray();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i]==target)
                {
                    num++;
                    if (num== times)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
