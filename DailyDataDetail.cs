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

namespace Company
{
    public partial class DailyDataDetail: Form
    {
        static string conString = @"Data Source=DESKTOP-ND2U4SJ\SQLEXPRESS;Initial Catalog=Company;Integrated Security=True";
        SqlConnection conn = new SqlConnection(conString);

        public DailyDataDetail()
        {
            InitializeComponent();
        }

        private void DailyDataDetail_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd-MM-yyyy";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "dd-MM-yyyy";

            dgvNetWorth.AllowUserToAddRows = false;
            dgvSpends.AllowUserToAddRows = false;
            dgvRecives.AllowUserToAddRows = false;
            dgvOthers.AllowUserToAddRows = false;

            rbName.Checked = true;
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Get selected dates from DateTimePickers
            DateTime fromDate = dateTimePicker1.Value;
            DateTime toDate = dateTimePicker2.Value;

            // Create a dataset to hold results
            DataSet dataSet = new DataSet();

            // Define SQL queries
            string dailyRecordsQuery = @"
        SELECT netWorth, differ, FORMAT(date, 'MM-dd-yyyy') AS [Date]
        FROM DailyRecords 
        WHERE date BETWEEN @from AND @to
        ORDER BY date";

            string spendsQuery = @"
        SELECT Spends.name AS [Name], Spends.item AS [Item], Spends.cost AS [Cost], Spends.status AS [Status],
               FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS [Date]
        FROM Spends
        JOIN DailyRecords ON DailyRecords.id = Spends.DailyRecordId
        WHERE DailyRecords.date BETWEEN @from AND @to
        ORDER BY DailyRecords.date;";

            string reciivesQuery = @"
        SELECT Recives.name AS [Name], Recives.item AS [Item], Recives.cost AS [Cost],
               FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS [Date]
        FROM Recives
        JOIN DailyRecords ON DailyRecords.id = Recives.DailyRecordId
        WHERE DailyRecords.date BETWEEN @from AND @to
        ORDER BY DailyRecords.date;";

            string othersQuery = @"
        SELECT Others.name AS [Name], Others.item AS [Item], Others.cost AS [Cost],
               FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS [Date]
        FROM Others
        JOIN DailyRecords ON DailyRecords.id = Others.DailyRecordId
        WHERE DailyRecords.date BETWEEN @from AND @to
        ORDER BY DailyRecords.date;";

            try
            {
                conn.Open();

                // Load data into dataset
                LoadTable(dailyRecordsQuery, "DailyRecords", fromDate, toDate, dataSet);
                LoadTable(spendsQuery, "Spends", fromDate, toDate, dataSet);
                LoadTable(reciivesQuery, "Recives", fromDate, toDate, dataSet);
                LoadTable(othersQuery, "Others", fromDate, toDate, dataSet);

                conn.Close();

                // Bind data to DataGridViews
                dgvNetWorth.DataSource = dataSet.Tables["DailyRecords"];
                dgvSpends.DataSource = dataSet.Tables["Spends"];
                dgvRecives.DataSource = dataSet.Tables["Recives"];
                dgvOthers.DataSource = dataSet.Tables["Others"];

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Function to load data into dataset
        private void LoadTable(string query, string tableName, DateTime fromDate, DateTime toDate, DataSet dataSet)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@from", fromDate);
                cmd.Parameters.AddWithValue("@to", toDate);

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dataSet, tableName);
                }
            }
        }

        private void tbSerch_TextChanged(object sender, EventArgs e)
        {
            string word = tbSerch.Text.Trim(); // Get text from the textbox
            string category = rbName.Checked ? "name" : "item"; // Choose search category

            // Create a proper SQL query with dynamic column name using string interpolation
            string spendsQuery = $@"
                                SELECT Spends.name AS [Name], Spends.item AS [Item], Spends.cost AS [Cost], Spends.status AS [Status],
                                FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS [Date]
                                FROM Spends
                                JOIN DailyRecords ON DailyRecords.id = Spends.DailyRecordId
                                WHERE Spends.{category} LIKE @word
                                ORDER BY DailyRecords.date;";

            string reciivesQuery = $@"
                                SELECT Recives.name AS [Name], Recives.item AS [Item], Recives.cost AS [Cost],
                                FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS [Date]
                                FROM Recives
                                JOIN DailyRecords ON DailyRecords.id = Recives.DailyRecordId
                                WHERE Recives.{category} LIKE @word
                                ORDER BY DailyRecords.date;";

            string othersQuery = $@"
                                SELECT Others.name AS [Name], Others.item AS [Item], Others.cost AS [Cost],
                                FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS [Date]
                                FROM Others
                                JOIN DailyRecords ON DailyRecords.id = Others.DailyRecordId
                                WHERE Others.{category} LIKE @word
                                ORDER BY DailyRecords.date;";

            try
            {
                conn.Open();

                DataSet dataSet = new DataSet(); // Ensure DataSet is initialized

                using (SqlCommand spendsCmd = new SqlCommand(spendsQuery, conn))
                {
                    spendsCmd.Parameters.AddWithValue("@word", word + "%");
                    SqlDataAdapter sqlDataAdapter2 = new SqlDataAdapter(spendsCmd);
                    sqlDataAdapter2.Fill(dataSet, "searchSpends");
                }

                using (SqlCommand reciivesCmd = new SqlCommand(reciivesQuery, conn))
                {
                    reciivesCmd.Parameters.AddWithValue("@word", word + "%");
                    SqlDataAdapter sqlDataAdapter3 = new SqlDataAdapter(reciivesCmd);
                    sqlDataAdapter3.Fill(dataSet, "searchRecives");
                }

                using (SqlCommand othersCmd = new SqlCommand(othersQuery, conn))
                {
                    othersCmd.Parameters.AddWithValue("@word", word + "%");
                    SqlDataAdapter sqlDataAdapter4 = new SqlDataAdapter(othersCmd);
                    sqlDataAdapter4.Fill(dataSet, "searchOthers");
                }

                conn.Close();

                // Bind data to DataGridViews
                dgvSpends.DataSource = dataSet.Tables["searchSpends"];
                dgvRecives.DataSource = dataSet.Tables["searchRecives"];
                dgvOthers.DataSource = dataSet.Tables["searchOthers"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
} 