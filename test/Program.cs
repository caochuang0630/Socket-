using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Method;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpGet("https://sc.ftqq.com/SCU12102Td2a63a7ac77a8f1e4f60668da76df1b159c2a2c6a516f.send?text=nihao");
        }

        /// <summary>
        /// 发起一个HTTP请求（以GET方式）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static  string HttpGet(string text)
        {
            string url = String.Format("https://sc.ftqq.com/SCU12102Td2a63a7ac77a8f1e4f60668da76df1b159c2a2c6a516f.send?text={0}",text);

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
