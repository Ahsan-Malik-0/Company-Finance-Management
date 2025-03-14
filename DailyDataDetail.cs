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
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            DataSet dataSet = new DataSet();

            //MessageBox.Show("Set the date first");
            DateTime FromDate = dateTimePicker1.Value;
            DateTime Todate = dateTimePicker2.Value;

            // Operations On DailyRecords
            string dailyRecordsQuery = @"SELECT netWorth, differ, FORMAT(date, 'MM-dd-yyyy') AS [Date]
                                        FROM DailyRecords 
                                        WHERE date BETWEEN @from AND @to
                                        ORDER BY date";
            SqlCommand dailyRecordsCmd = new SqlCommand(dailyRecordsQuery, conn);
            dailyRecordsCmd.Parameters.AddWithValue("@from", FromDate);
            dailyRecordsCmd.Parameters.AddWithValue("@to", Todate);
            SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter(dailyRecordsCmd);

            // Operations on Spends
            string spendsQuery = @"SELECT Spends.name AS [Name], Spends.item AS [Item], Spends.cost AS [Cost], Spends.status AS [Status],
                           FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS [Date]
                            FROM Spends
                            JOIN DailyRecords ON DailyRecords.id = Spends.DailyRecordId
                            WHERE DailyRecords.date BETWEEN @from AND @to
                            ORDER BY DailyRecords.date;";

            SqlCommand spendsCmd = new SqlCommand(spendsQuery, conn);

            spendsCmd.Parameters.AddWithValue("@from", FromDate);
            spendsCmd.Parameters.AddWithValue("@to", Todate);
            SqlDataAdapter sqlDataAdapter2 = new SqlDataAdapter(spendsCmd);

            // Operations on Recives
            string reciivesQuery = @"SELECT Recives.name AS [Name], Recives.item AS [Item], Recives.cost AS [Cost],
                            FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS [Date]
                            FROM Recives
                            JOIN DailyRecords ON DailyRecords.id = Recives.DailyRecordId
                            WHERE DailyRecords.date BETWEEN @from AND @to
                            ORDER BY DailyRecords.date;";

            SqlCommand reciivesCmd = new SqlCommand(reciivesQuery, conn);

            reciivesCmd.Parameters.AddWithValue("@from", FromDate);
            reciivesCmd.Parameters.AddWithValue("@to", Todate);
            SqlDataAdapter sqlDataAdapter3 = new SqlDataAdapter(reciivesCmd);

            // Operations on Others
            string othersQuery = @"SELECT Others.name AS [Name], Others.item AS [Item], Others.cost AS [Cost],
                            FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS [Date]
                            FROM Others
                            JOIN DailyRecords ON DailyRecords.id = Others.DailyRecordId
                            WHERE DailyRecords.date BETWEEN @from AND @to
                            ORDER BY DailyRecords.date;";

            SqlCommand othersCmd = new SqlCommand(othersQuery, conn);

            othersCmd.Parameters.AddWithValue("@from", FromDate);
            othersCmd.Parameters.AddWithValue("@to", Todate);
            SqlDataAdapter sqlDataAdapter4 = new SqlDataAdapter(othersCmd);

            try
            {
                conn.Open();

                sqlDataAdapter1.Fill(dataSet, "DailyRecords");
                sqlDataAdapter2.Fill(dataSet, "Spends");
                sqlDataAdapter3.Fill(dataSet, "Recives");
                sqlDataAdapter4.Fill(dataSet, "Others");

                conn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            dgvNetWorth.DataSource = dataSet.Tables["DailyRecords"];
            dgvSpends.DataSource = dataSet.Tables["Spends"];
            dgvRecives.DataSource = dataSet.Tables["Recives"];
            dgvOthers.DataSource = dataSet.Tables["Others"];

            dgvNetWorth.AllowUserToAddRows = false;
            dgvSpends.AllowUserToAddRows = false;
            dgvRecives.AllowUserToAddRows = false;
            dgvOthers.AllowUserToAddRows = false;
        }
    }
} 