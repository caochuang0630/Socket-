using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Client_form
{
    public partial class register : Form
    {
        public register()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            set_register();


        }

        private void set_register()
        {
            Socket socket = Method.Connect("register");

            socket.Send(Encoding.UTF8.GetBytes(String.Format("#SQL-q insert [User] values('{0}','{1}')", this.textBox1.Text, this.textBox2.Text)));

            byte[] readBuff = new byte[1024];
            int count = socket.Receive(readBuff);
            string result = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);

            if (result == "1")
            {
                MessageBox.Show("注册成功");
                this.Close();
            }
            else
            {
                MessageBox.Show("注册失败");
            }

            Method.Disconnect(socket);
        }
    }
}
