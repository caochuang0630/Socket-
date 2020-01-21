using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace server
{
    class DBhelper
    { 
        SqlConnection sqlcon;

        public DBhelper()
        {
            sqlcon = new SqlConnection("Data Source=192.168.2.182;Initial Catalog=login;User ID=root");
        }

        public int query(string sql)
        {
            SqlCommand sqlcmd = new SqlCommand(sql,sqlcon);

            sqlcon.Open();
            int result = sqlcmd.ExecuteNonQuery();
            sqlcon.Close();

            return result;
        }

        
    }
}
