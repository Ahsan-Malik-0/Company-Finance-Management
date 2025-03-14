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
    public partial class DailyDataSmallTables : Form
    {
        int DailyDataId = 0;
        int ColumnIndex = 0;
        static string conString = @"Data Source=DESKTOP-ND2U4SJ\SQLEXPRESS;Initial Catalog=Company;Integrated Security=True";
        SqlConnection conn = new SqlConnection(conString);
        public DailyDataSmallTables(int dailyDataId, int columnIndex)
        {
            InitializeComponent();
            DailyDataId = dailyDataId;
            ColumnIndex = columnIndex;
        }

        private void DailyDataSmallTables_Load(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            DataTable dt = new DataTable();
            switch (ColumnIndex)
            {
                case 2:
                    string reciveQuery = @"SELECT name AS [Name], item AS [Item], cost AS [Cost] FROM Recives where dailyRecordId = @dailyDataId";
                    SqlCommand reciveCmd = new SqlCommand(reciveQuery, conn);
                    reciveCmd.Parameters.AddWithValue("dailyDataid", DailyDataId);
                    SqlDataAdapter reciveCmdDataAdapter = new SqlDataAdapter(reciveCmd);
                    try
                    {
                        conn.Open();
                        reciveCmdDataAdapter.Fill(dt);
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    dataGridView1.DataSource = dt;
                    break;


                case 3:
                    string spendQuery = @"SELECT name AS [Name], item AS [Item], cost AS [Cost], status AS [status] FROM Spends where dailyRecordId = @dailyDataId";
                    SqlCommand spendCmd = new SqlCommand(spendQuery, conn);
                    spendCmd.Parameters.AddWithValue("dailyDataid", DailyDataId);
                    SqlDataAdapter spendCmdDataAdapter = new SqlDataAdapter(spendCmd);
                    try
                    {
                        conn.Open();
                        spendCmdDataAdapter.Fill(dt);
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    dataGridView1.DataSource = dt;
                    break;


                case 4:
                    string otherQuery = @"SELECT name AS [Name], item AS [Item], cost AS [Cost] FROM Others where dailyRecordId = @dailyDataId";
                    SqlCommand otherCmd = new SqlCommand(otherQuery, conn);
                    otherCmd.Parameters.AddWithValue("dailyDataid", DailyDataId);
                    SqlDataAdapter otherCmdDataAdapter = new SqlDataAdapter(otherCmd);
                    try
                    {
                        conn.Open();
                        otherCmdDataAdapter.Fill(dt);
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    dataGridView1.DataSource = dt;
                    break;
            }
        }
    }
}
