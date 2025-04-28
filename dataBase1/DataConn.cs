using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataBase1
{
    public class DataConn
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=DESKTOP-LBILO57\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True");

        public void openConnection() {
            sqlConnection.Open();
        }
        public void closeConnection()
        {
         sqlConnection.Close();
        }
        public SqlConnection getConnection()
        {
            return sqlConnection;
        }

    }
}
