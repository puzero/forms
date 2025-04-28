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
    public partial class FormManager : Form
    {
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlCommandBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private SqlDataAdapter sqlDataAdapterOrders = null;
        private SqlDataAdapter sqlDataAdapterClients = null;
        private SqlDataAdapter sqlDataAdapterOrders_Contents = null;
        private DataSet dataSet = null;
        private DataSet dataSetOrders = null;
        private DataSet dataSetClients = null;
        private DataSet dataSetOrders_Contents = null;

        private bool newRowAdding = false;
        public FormManager()
        {
            InitializeComponent();
        }
        private void FormManager_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(@"Data Source=DESKTOP-LBILO57\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True");
            sqlConnection.Open();
            LoadData();
            LoadOrdersData();
            LoadClientsData();
            LoadOrders_ContentsData();

        }

        private void ReloadData()
        {
            try
            {
                dataSet.Tables["Products"].Clear();
                sqlDataAdapter.Fill(dataSet, "Products");

                dataGridView1.DataSource = dataSet.Tables["Products"];



                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[4, i] = linkCell;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void LoadData()
        {
            try
            {
                sqlDataAdapter = new SqlDataAdapter("select id_Produckt, title, price, inStock, 'Delete' as [Command] from Products", sqlConnection);
                sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);


                sqlCommandBuilder.GetInsertCommand();
                sqlCommandBuilder.GetUpdateCommand();
                sqlCommandBuilder.GetDeleteCommand();

                dataSet = new DataSet();

                sqlDataAdapter.Fill(dataSet, "Products");

                dataGridView1.DataSource = dataSet.Tables["Products"];



                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[4, i] = linkCell;

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAdding == false)
                {
                    newRowAdding = true;

                    int lastRow = dataGridView1.RowCount - 2;

                    DataGridViewRow row = dataGridView1.Rows[lastRow];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[4, lastRow] = linkCell;

                    row.Cells["Command"].Value = "Insert";

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {

                string task = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();


                if (task == "Delete")
                {
                    if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        int rowIndex = e.RowIndex;

                        dataGridView1.Rows.RemoveAt(rowIndex);
                        dataSet.Tables["Products"].Rows[rowIndex].Delete();
                        sqlDataAdapter.Update(dataSet, "Products");
                    }

                }
                else if (task == "Insert")
                {

                    int rowIndex = dataGridView1.Rows.Count - 2;

                    DataRow row = dataSet.Tables["Products"].NewRow();



                    row["title"] = dataGridView1.Rows[rowIndex].Cells["title"].Value;
                    row["price"] = dataGridView1.Rows[rowIndex].Cells["price"].Value;
                    row["inStock"] = dataGridView1.Rows[rowIndex].Cells["inStock"].Value;

                    dataSet.Tables["Products"].Rows.Add(row);
                    dataSet.Tables["Products"].Rows.RemoveAt(dataSet.Tables["Products"].Rows.Count - 1);
                    dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                    dataGridView1.Rows[e.RowIndex].Cells[4].Value = "Delete";

                    sqlDataAdapter.Update(dataSet, "Products");
                    newRowAdding = false;
                }
                else if (task == "Update")
                {
                    int r = e.RowIndex;

                    dataSet.Tables["Products"].Rows[r]["title"] = dataGridView1.Rows[r].Cells["title"].Value;
                    dataSet.Tables["Products"].Rows[r]["price"] = dataGridView1.Rows[r].Cells["price"].Value;
                    dataSet.Tables["Products"].Rows[r]["inStock"] = dataGridView1.Rows[r].Cells["inStock"].Value;
                    sqlDataAdapter.Update(dataSet, "Products");
                    dataGridView1.Rows[e.RowIndex].Cells[4].Value = "Delete";
                }

                ReloadData();


            }

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAdding == false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;

                    DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[4, rowIndex] = linkCell;

                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private void LoadOrdersData()
        {
            try
            {
                sqlDataAdapterOrders = new SqlDataAdapter("select id_Order ,id_Client, 'Delete' as [Command] from Orders", sqlConnection);
                sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapterOrders);


                sqlCommandBuilder.GetInsertCommand();
                sqlCommandBuilder.GetUpdateCommand();
                sqlCommandBuilder.GetDeleteCommand();

                dataSetOrders = new DataSet();

                sqlDataAdapterOrders.Fill(dataSetOrders, "Orders");

                dataGridViewOrders.DataSource = dataSetOrders.Tables["Orders"];


                for (int i = 0; i < dataGridViewOrders.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewOrders[2, i] = linkCell;

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ReloadOrdersData()
        {
            try
            {
                dataSetOrders.Tables["Orders"].Clear();
                sqlDataAdapterOrders.Fill(dataSetOrders, "Orders");

                dataGridViewOrders.DataSource = dataSetOrders.Tables["Orders"];



                for (int i = 0; i < dataGridViewOrders.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewOrders[2, i] = linkCell;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dataGridViewOrders_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAdding == false)
                {
                    newRowAdding = true;

                    int lastRow = dataGridViewOrders.RowCount - 2;

                    DataGridViewRow row = dataGridViewOrders.Rows[lastRow];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewOrders[2, lastRow] = linkCell;

                    row.Cells["Command"].Value = "Insert";

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridViewOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {

                string task = dataGridViewOrders.Rows[e.RowIndex].Cells[2].Value.ToString();


                if (task == "Delete")
                {
                    if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        int rowIndex = e.RowIndex;

                        dataGridViewOrders.Rows.RemoveAt(rowIndex);
                        dataSetOrders.Tables["Orders"].Rows[rowIndex].Delete();
                        sqlDataAdapterOrders.Update(dataSetOrders, "Orders");
                    }

                }
                else if (task == "Insert")
                {

                    int rowIndex = dataGridViewOrders.Rows.Count - 2;

                    DataRow row = dataSetOrders.Tables["Orders"].NewRow();



                    row["id_Client"] = dataGridViewOrders.Rows[rowIndex].Cells["id_Client"].Value;

                    dataSetOrders.Tables["Orders"].Rows.Add(row);
                    dataSetOrders.Tables["Orders"].Rows.RemoveAt(dataSetOrders.Tables["Orders"].Rows.Count - 1);
                    dataGridViewOrders.Rows.RemoveAt(dataGridViewOrders.Rows.Count - 2);
                    dataGridViewOrders.Rows[e.RowIndex].Cells[2].Value = "Delete";

                    sqlDataAdapterOrders.Update(dataSetOrders, "Orders");
                    newRowAdding = false;
                }
                else if (task == "Update")
                {
                    int r = e.RowIndex;

                    dataSetOrders.Tables["Orders"].Rows[r]["id_Client"] = dataGridViewOrders.Rows[r].Cells["id_Client"].Value;
                    sqlDataAdapterOrders.Update(dataSetOrders, "Orders");
                    dataGridViewOrders.Rows[e.RowIndex].Cells[2].Value = "Delete";
                }

                ReloadOrdersData();


            }

        }

        private void dataGridVieOrders_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAdding == false)
                {
                    int rowIndex = dataGridViewOrders.SelectedCells[0].RowIndex;

                    DataGridViewRow editingRow = dataGridViewOrders.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewOrders[2, rowIndex] = linkCell;

                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        private void ReloadClientsData()
        {
            try
            {
                dataSetClients.Tables["Clients"].Clear();
                sqlDataAdapterClients.Fill(dataSetClients, "Clients");

                dataGridViewClients.DataSource = dataSetClients.Tables["Clients"];



                for (int i = 0; i < dataGridViewClients.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewClients[5, i] = linkCell;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void LoadClientsData()
        {
            try
            {
                sqlDataAdapterClients = new SqlDataAdapter("select  id_Client, name,lastName,tel,address, 'Delete' as [Command] from Clients", sqlConnection);
                sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapterClients);


                sqlCommandBuilder.GetInsertCommand();
                sqlCommandBuilder.GetUpdateCommand();
                sqlCommandBuilder.GetDeleteCommand();

                dataSetClients = new DataSet();

                sqlDataAdapterClients.Fill(dataSetClients, "Clients");

                dataGridViewClients.DataSource = dataSetClients.Tables["Clients"];


                for (int i = 0; i < dataGridViewClients.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewClients[5, i] = linkCell;

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewClients_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAdding == false)
                {
                    newRowAdding = true;

                    int lastRow = dataGridViewClients.RowCount - 2;

                    DataGridViewRow row = dataGridViewClients.Rows[lastRow];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewClients[5, lastRow] = linkCell;

                    row.Cells["Command"].Value = "Insert";

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridViewClients_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {

                string task = dataGridViewClients.Rows[e.RowIndex].Cells[5].Value.ToString();


                if (task == "Delete")
                {
                    if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        int rowIndex = e.RowIndex;

                        dataGridViewClients.Rows.RemoveAt(rowIndex);
                        dataSetClients.Tables["Clients"].Rows[rowIndex].Delete();
                        sqlDataAdapterClients.Update(dataSetClients, "Clients");
                    }

                }
                else if (task == "Insert")
                {

                    int rowIndex = dataGridViewClients.Rows.Count - 2;

                    DataRow row = dataSetClients.Tables["Clients"].NewRow();



                    row["name"] = dataGridViewClients.Rows[rowIndex].Cells["name"].Value;
                    row["lastName"] = dataGridViewClients.Rows[rowIndex].Cells["lastName"].Value;
                    row["tel"] = dataGridViewClients.Rows[rowIndex].Cells["tel"].Value;
                    row["address"] = dataGridViewClients.Rows[rowIndex].Cells["address"].Value;

                    dataSetClients.Tables["Clients"].Rows.Add(row);
                    dataSetClients.Tables["Clients"].Rows.RemoveAt(dataSetClients.Tables["Clients"].Rows.Count - 1);
                    dataGridViewClients.Rows.RemoveAt(dataGridViewClients.Rows.Count - 2);
                    dataGridViewClients.Rows[e.RowIndex].Cells[5].Value = "Delete";

                    sqlDataAdapterClients.Update(dataSetClients, "Clients");
                    newRowAdding = false;
                }
                else if (task == "Update")
                {
                    int r = e.RowIndex;

                    dataSetClients.Tables["Clients"].Rows[r]["name"] = dataGridViewClients.Rows[r].Cells["name"].Value;
                    dataSetClients.Tables["Clients"].Rows[r]["lastName"] = dataGridViewClients.Rows[r].Cells["lastName"].Value;
                    dataSetClients.Tables["Clients"].Rows[r]["tel"] = dataGridViewClients.Rows[r].Cells["tel"].Value;
                    dataSetClients.Tables["Clients"].Rows[r]["address"] = dataGridViewClients.Rows[r].Cells["address"].Value;
                    sqlDataAdapterClients.Update(dataSetClients, "Clients");
                    dataGridViewClients.Rows[e.RowIndex].Cells[5].Value = "Delete";
                }

                ReloadClientsData();


            }

        }

        private void dataGridVieClients_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAdding == false)
                {
                    int rowIndex = dataGridViewClients.SelectedCells[0].RowIndex;

                    DataGridViewRow editingRow = dataGridViewClients.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewClients[5, rowIndex] = linkCell;

                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// 

        private void ReloadOrders_ContentsData()
        {
            try
            {
                dataSetOrders_Contents.Tables["Order_Contents"].Clear();
                sqlDataAdapterOrders_Contents.Fill(dataSetOrders_Contents, "Order_Contents");

                dataGridViewOrders_Contents.DataSource = dataSetOrders_Contents.Tables["Order_Contents"];



                for (int i = 0; i < dataGridViewOrders_Contents.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewOrders_Contents[4, i] = linkCell;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void LoadOrders_ContentsData()
        {
            try
            {
                sqlDataAdapterOrders_Contents = new SqlDataAdapter("select  id, id_Order,id_Produckt,quantity, 'Delete' as [Command] from Order_Contents", sqlConnection);
                sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapterOrders_Contents);


                sqlCommandBuilder.GetInsertCommand();
                sqlCommandBuilder.GetUpdateCommand();
                sqlCommandBuilder.GetDeleteCommand();

                dataSetOrders_Contents = new DataSet();

                sqlDataAdapterOrders_Contents.Fill(dataSetOrders_Contents, "Order_Contents");

                dataGridViewOrders_Contents.DataSource = dataSetOrders_Contents.Tables["Order_Contents"];


                for (int i = 0; i < dataGridViewOrders_Contents.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewOrders_Contents[4, i] = linkCell;

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewOrders_Contents_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (newRowAdding == false)
                {
                    newRowAdding = true;

                    int lastRow = dataGridViewOrders_Contents.RowCount - 2;

                    DataGridViewRow row = dataGridViewOrders_Contents.Rows[lastRow];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewOrders_Contents[4, lastRow] = linkCell;

                    row.Cells["Command"].Value = "Insert";

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridViewOrders_Contents_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {

                string task = dataGridViewOrders_Contents.Rows[e.RowIndex].Cells[4].Value.ToString();


                if (task == "Delete")
                {
                    if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        int rowIndex = e.RowIndex;

                        dataGridViewOrders_Contents.Rows.RemoveAt(rowIndex);
                        dataSetOrders_Contents.Tables["Order_Contents"].Rows[rowIndex].Delete();
                        sqlDataAdapterOrders_Contents.Update(dataSetOrders_Contents, "Order_Contents");
                    }

                }
                else if (task == "Insert")
                {

                    int rowIndex = dataGridViewOrders_Contents.Rows.Count - 2;

                    DataRow row = dataSetOrders_Contents.Tables["Order_Contents"].NewRow();



                    row["id_Order"] = dataGridViewOrders_Contents.Rows[rowIndex].Cells["id_Order"].Value;
                    row["id_Produckt"] = dataGridViewOrders_Contents.Rows[rowIndex].Cells["id_Produckt"].Value;
                    row["quantity"] = dataGridViewOrders_Contents.Rows[rowIndex].Cells["quantity"].Value;


                    dataSetOrders_Contents.Tables["Order_Contents"].Rows.Add(row);
                    dataSetOrders_Contents.Tables["Order_Contents"].Rows.RemoveAt(dataSetOrders_Contents.Tables["Order_Contents"].Rows.Count - 1);
                    dataGridViewOrders_Contents.Rows.RemoveAt(dataGridViewOrders_Contents.Rows.Count - 2);
                    dataGridViewOrders_Contents.Rows[e.RowIndex].Cells[4].Value = "Delete";

                    sqlDataAdapterOrders_Contents.Update(dataSetOrders_Contents, "Order_Contents");
                    newRowAdding = false;
                }
                else if (task == "Update")
                {
                    int r = e.RowIndex;

                    dataSetOrders_Contents.Tables["Order_Contents"].Rows[r]["id_Order"] = dataGridViewOrders_Contents.Rows[r].Cells["id_Order"].Value;
                    dataSetOrders_Contents.Tables["Order_Contents"].Rows[r]["id_Produckt"] = dataGridViewOrders_Contents.Rows[r].Cells["id_Produckt"].Value;
                    dataSetOrders_Contents.Tables["Order_Contents"].Rows[r]["quantity"] = dataGridViewOrders_Contents.Rows[r].Cells["quantity"].Value;
                    sqlDataAdapterOrders_Contents.Update(dataSetOrders_Contents, "Order_Contents");
                    dataGridViewOrders_Contents.Rows[e.RowIndex].Cells[4].Value = "Delete";
                }

                ReloadOrders_ContentsData();


            }

        }

        private void dataGridVieOrders_Contents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAdding == false)
                {
                    int rowIndex = dataGridViewOrders_Contents.SelectedCells[0].RowIndex;

                    DataGridViewRow editingRow = dataGridViewOrders_Contents.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridViewOrders_Contents[4, rowIndex] = linkCell;

                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




    }
}


