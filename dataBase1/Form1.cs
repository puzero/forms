using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dataBase1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = textBoxPasssword.Text;
            string sql = $"Select role from Users where role = '{login}' and password = '{password}'";
            DataConn dataConn = new DataConn();
            dataConn.openConnection();
            SqlCommand command = new SqlCommand(sql,dataConn.getConnection());
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.HasRows) {
                MessageBox.Show("неверный логин или пароль");
                textBoxLogin.Text = "";
                textBoxPasssword.Text = "";
            }
            while (reader.Read())
            {
                if (reader.GetString(0) == "admin")
                {
                    FormAdmin admin = new FormAdmin();
                    this.Hide();
                    admin.ShowDialog();
                    this.Close();
                }
                else if (reader.GetString(0) == "manager")
                {
                    FormManager manager = new FormManager();
                    this.Hide();
                    manager.ShowDialog();
                    this.Close();
                }
                else if (reader.GetString(0) == "user")
                {
                    FormClient client = new FormClient();
                    this.Hide();
                    client.ShowDialog();
                    this.Close();
                }
            }
            reader.Close();
            dataConn.closeConnection();
        }
    }
}
