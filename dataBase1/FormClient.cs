using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using GroupBox = System.Windows.Forms.GroupBox;

namespace dataBase1
{
    public partial class FormClient : Form
    {
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlCommandBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;


        public FormClient()
        {
            InitializeComponent();
            InitializeComboBoxes();
            InitializeDatabaseComponents();
        }

        private void LoadData(DataSet dataSet)
        {
            string placeholderImage = @"C:\Users\Максим\Pictures\none.jpg";

            foreach (DataRow row in dataSet.Tables["Products"].Rows)
            {
                Panel productPanel = new Panel
                {
                    Width = 520,
                    Height = 100,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(10)
                };

                // Изображение товара
                PictureBox pictureBox = new PictureBox
                {
                    Width = 80,
                    Height = 80,
                    Location = new Point(10, 10),
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                if (row["Izo"] != DBNull.Value && row["Izo"] != null)
                {
                    try
                    {
                        byte[] imageData = (byte[])row["Izo"];
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            pictureBox.Image = Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                        pictureBox.Image = Image.FromFile(placeholderImage);
                    }
                }
                else
                {
                    pictureBox.Image = Image.FromFile(placeholderImage);
                }


                // Название товара
                Label nameLabel = new Label
                {
                    Text = row["title"].ToString(),
                    Location = new Point(110, 20),
                    Width = 180,
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };

                // Производитель
                Label producerLabel = new Label
                {
                    Text = $"Количество: {row["inStock"]}",
                    Location = new Point(110, 50),
                    Width = 190,
                    AutoSize = true
                };

                // Цена
                Label priceLabel = new Label
                {
                    Text = $"Цена: {Convert.ToDecimal(row["price"]):N2} руб.",
                    Location = new Point(350, 20),
                    Width = 180,
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };

                productPanel.Controls.Add(pictureBox);
                productPanel.Controls.Add(nameLabel);
                productPanel.Controls.Add(producerLabel);
                productPanel.Controls.Add(priceLabel);

                flowLayoutPanel1.Controls.Add(productPanel);
            }
        }

        private void FormClient_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(@"Data Source=DESKTOP-LBILO57\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True");
            sqlConnection.Open();

            sqlDataAdapter = new SqlDataAdapter("select *, 'Delete' as [Command] from Products", sqlConnection);
            sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);


            sqlCommandBuilder.GetInsertCommand();
            sqlCommandBuilder.GetUpdateCommand();
            sqlCommandBuilder.GetDeleteCommand();

            dataSet = new DataSet();

            sqlDataAdapter.Fill(dataSet, "Products");

            LoadData(dataSet);
        }
        

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void Search()
        {
            this.flowLayoutPanel1.Controls.Clear();
 
            sqlConnection = new SqlConnection(@"Data Source=DESKTOP-LBILO57\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True");
            sqlConnection.Open();

            sqlDataAdapter = new SqlDataAdapter($"select * from Products where concat (title,price,inStock) like '%" + textBoxSearch.Text + "%' ", sqlConnection);
            sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
            dataSet = new DataSet();

            sqlDataAdapter.Fill(dataSet, "Products");

            LoadData(dataSet);
        }
        private void InitializeDatabaseComponents()
        {
            sqlConnection = new SqlConnection(@"Data Source=DESKTOP-LBILO57\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True");
            sqlConnection.Open();

            sqlDataAdapter = new SqlDataAdapter($"select * from Products", sqlConnection);
            sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
            dataSet = new DataSet();

            sqlDataAdapter.Fill(dataSet, "Products");
        }

        private void LoadProducts(string filter = "", string sort = "")
        {
            try
            {
                string query = "SELECT * FROM Products where concat (title,price,inStock) like '%" + textBoxSearch.Text + "%' ";

                // Добавляем фильтр, если он задан
                if (!string.IsNullOrEmpty(filter))
                {
                    query += $" AND {filter}";
                }

                // Добавляем сортировку, если она задана
                if (!string.IsNullOrEmpty(sort))
                {
                    query += $" ORDER BY {sort}";
                }
                this.flowLayoutPanel1.Controls.Clear();
                sqlConnection = new SqlConnection(@"Data Source=DESKTOP-LBILO57\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True");
                sqlConnection.Open();
                sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
                dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "Products");

                LoadData(dataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComboBoxes()
        {
            // Инициализация ComboBox для фильтрации
            comboBoxFilter.Items.AddRange(new string[]
            {
                "Без фильтра",
                "Цена < 100",
                "Цена от 100 до 500",
                "Цена > 500",
                "Наличие на складе > 10",
                "Наличие на складе = 0"
            });
            comboBoxFilter.SelectedIndex = 0;

            // Инициализация ComboBox для сортировки
            comboBoxSort.Items.AddRange(new string[]
            {
                "Без сортировки",
                "По названию (А-Я)",
                "По названию (Я-А)",
                "По цене (по возрастанию)",
                "По цене (по убыванию)",
                "По количеству (по возрастанию)",
                "По количеству (по убыванию)"
            });
            comboBoxSort.SelectedIndex = 0;
        }

        private string GetFilterCondition()
        {
            switch (comboBoxFilter.SelectedItem.ToString())
            {
                case "Цена < 100": return "price < 100";
                case "Цена от 100 до 500": return "price BETWEEN 100 AND 500";
                case "Цена > 500": return "price > 500";
                case "Наличие на складе > 10": return "inStock > 10";
                case "Наличие на складе = 0": return "inStock = 0";
                default: return "";
            }
        }

        private string GetSortCondition()
        {
            switch (comboBoxSort.SelectedItem.ToString())
            {
                case "По названию (А-Я)": return "title ASC";
                case "По названию (Я-А)": return "title DESC";
                case "По цене (по возрастанию)": return "price ASC";
                case "По цене (по убыванию)": return "price DESC";
                case "По количеству (по возрастанию)": return "inStock ASC";
                case "По количеству (по убыванию)": return "inStock DESC";
                default: return "";
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string filter = GetFilterCondition();
            string sort = GetSortCondition();
            LoadProducts(filter, sort);
        }

        private void textBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            Search();
        }
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filter = GetFilterCondition();

            LoadProducts(filter);
        }
        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filter = GetFilterCondition();
            string sort = GetSortCondition();
            LoadProducts(filter, sort);
        }

    }
}
