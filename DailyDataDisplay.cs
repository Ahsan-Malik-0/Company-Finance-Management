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
    public partial class DailyDataDisplay : Form
    {
        public DailyDataDisplay()
        {
            InitializeComponent();
        }
        static string conString = @"Data Source=DESKTOP-ND2U4SJ\SQLEXPRESS;Initial Catalog=Company;Integrated Security=True";
        SqlConnection conn = new SqlConnection(conString);

        private void DailyDataDisplay_Load(object sender, EventArgs e)
        {

            GetMonth();
            DisplayData();

            DataGridViewButtonColumn recive = new DataGridViewButtonColumn();
            recive.Text = "Recive";
            recive.HeaderText = "Recive";
            recive.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Insert(2, recive);

            DataGridViewButtonColumn spend = new DataGridViewButtonColumn();
            spend.Text = "Spend";
            spend.HeaderText = "Spend";
            spend.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Insert(3, spend);

            DataGridViewButtonColumn other = new DataGridViewButtonColumn();
            other.Text = "Other";
            other.HeaderText = "Other";
            other.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Insert(4, other);

            DataGridViewButtonColumn update = new DataGridViewButtonColumn();
            update.Text = "Update";
            update.HeaderText = "Update";
            update.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Insert(6, update);

            DataGridViewButtonColumn delete = new DataGridViewButtonColumn();
            delete.Text = "Delete";
            delete.HeaderText = "Delete";
            delete.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Insert(7, delete);

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoGenerateColumns = false; // Prevent auto-generation of columns
        }
        public void GetMonth()
        {
            // Get Month From Last Record Entry
            int month = 0;
            string query = @"SELECT TOP 1 month
                            FROM DailyRecords
                            ORDER BY id DESC;";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                object result = sqlCommand.ExecuteScalar();
                month = Convert.ToInt16(result);
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            tbMonth.Text = month.ToString();
        }

        public void DisplayData()
        {
            int month = Convert.ToInt16(tbMonth.Text);
            string displayQuery = @"SELECT DailyRecords.id, 
                                CAST(DailyRecords.netWorth AS VARCHAR) + '/' + CAST(DailyRecords.differ AS VARCHAR) AS NetWorths,
                                FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS Date
                                FROM DailyRecords
                                WHERE month = @month
                                ORDER BY date";

            SqlCommand displayCmd = new SqlCommand(displayQuery, conn);
            displayCmd.Parameters.AddWithValue("@month", month);
            DataTable dt = new DataTable();

            try
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(displayCmd);
                da.Fill(dt);
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            dataGridView1.DataSource = dt;

            int totalSpends = 0;
            int totalRecives = 0;
            int totalOthers = 0;
            string totalSpendsQuery = @"SELECT SUM(Spends.cost) AS Cost
                                    FROM Spends
                                    JOIN DailyRecords ON DailyRecords.id = Spends.DailyRecordId
                                    where DailyRecords.month = @month AND Spends.status = 'Debit'";

            string totalRecivesQuery = @"SELECT SUM(Recives.cost) AS Cost
                                    FROM Recives
                                    JOIN DailyRecords ON DailyRecords.id = Recives.DailyRecordId
                                    where DailyRecords.month = @month";

            string totalOthersQuery = @"SELECT SUM(Others.cost) AS Cost
                                    FROM Others
                                    JOIN DailyRecords ON DailyRecords.id = Others.DailyRecordId
                                    where DailyRecords.month = @month";
            try
            {
                conn.Open();

                SqlCommand totalSpendCmd = new SqlCommand(totalSpendsQuery, conn);
                totalSpendCmd.Parameters.AddWithValue("@month", month);
                object totalSpendResult = totalSpendCmd.ExecuteScalar();
                totalSpends = (int)totalSpendResult;

                SqlCommand totalReciveCmd = new SqlCommand(totalRecivesQuery, conn);
                totalReciveCmd.Parameters.AddWithValue("@month", month);
                object totalReciveResult = totalReciveCmd.ExecuteScalar();
                totalRecives = (int)totalReciveResult;

                SqlCommand totalOtherCmd = new SqlCommand(totalOthersQuery, conn);
                totalOtherCmd.Parameters.AddWithValue("@month", month);
                object totalOtherResult = totalOtherCmd.ExecuteScalar();
                totalOthers = (int)totalOtherResult;

                conn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            lblTotalSpend.Text = totalSpends.ToString();
            lblTotalRecive.Text = totalRecives.ToString();
            lblComProfit.Text = (totalRecives - totalSpends).ToString();
            lblNetProfit.Text = (totalRecives - (totalSpends + totalOthers)).ToString();



        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DailyDataInput dailyDataInput = new DailyDataInput();
            dailyDataInput.ShowDialog();
            DisplayData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int dailyRecordId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);

            if (e.RowIndex >= 0 && e.ColumnIndex == 7) // Ensure the row index is valid and the column index is for the "Delete" button
            {
                DialogResult dr = MessageBox.Show("Are you sure, you want to delete this Record", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {

                    string query = @"DELETE FROM DailyRecords WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", dailyRecordId);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Record Deleted Successfully");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }

                    DisplayData(); // Refresh the DataGridView after deletion
                }
            }

            if(e.RowIndex >= 0 && e.ColumnIndex == 6)
            {
                DailyDataUpdate dailyDataUpdate = new DailyDataUpdate(dailyRecordId);
                dailyDataUpdate.ShowDialog();
                DisplayData();
            }

            if (e.RowIndex >= 0 && (e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4))
            {
                DailyDataSmallTables dailyDataSmallTables = new DailyDataSmallTables(dailyRecordId, e.ColumnIndex);
                dailyDataSmallTables.ShowDialog();
                DisplayData();
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            DailyDataDetail dailyDataDetail = new DailyDataDetail();
            dailyDataDetail.ShowDialog();
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            DisplayData();
        }
    }
}
