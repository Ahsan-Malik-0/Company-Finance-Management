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
    public partial class CreditList : Form
    {
        static string conString = @"Data Source=DESKTOP-ND2U4SJ\SQLEXPRESS;Initial Catalog=Company;Integrated Security=True";
        SqlConnection conn = new SqlConnection(conString);
        public CreditList()
        {
            InitializeComponent();
        }

        private void CreditList_Load(object sender, EventArgs e)
        {
            showSpendList();
            getSum();

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
        }

        private void showSpendList()
        {
            string creditQuery = @"SELECT Spends.name AS [Name], Spends.item AS [Item], Spends.cost AS [Cost],
                                 FORMAT(DailyRecords.date, 'MM-dd-yyyy') AS Date
                                 FROM Spends
                                 JOIN DailyRecords ON DailyRecords.id = Spends.DailyRecordId
                                 WHERE Spends.status = 'Credit'
                                 ORDER BY DailyRecords.date";
            SqlCommand creditCmd = new SqlCommand(creditQuery, conn);
            SqlDataAdapter creditCmdDataAdapter = new SqlDataAdapter(creditCmd);
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                creditCmdDataAdapter.Fill(dt);
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            dataGridView1.DataSource = dt;
        }

        private void getSum()
        {
            string harisQuery = @"SELECT SUM(cost) FROM Spends WHERE status = 'Credit' AND item LIKE 'plate%'";
            SqlCommand harisCmd = new SqlCommand(harisQuery, conn);

            string bhattiQuery = @"SELECT SUM(cost) FROM Spends WHERE status = 'Credit' AND (item LIKE 'Business Card%' OR item LIKE 'Printing')";
            SqlCommand bhattiCmd = new SqlCommand(bhattiQuery, conn);

            string rashidQuery = @"SELECT SUM(cost) FROM Spends WHERE status = 'Credit' AND (item LIKE 'Flex%' OR item LIKE 'Standies')";
            SqlCommand rashidCmd = new SqlCommand(rashidQuery, conn);

            string totalQuery = @"SELECT SUM(cost) FROM Spends WHERE status = 'Credit'";
            SqlCommand totalCmd = new SqlCommand(totalQuery, conn);


            try
            {
                conn.Open();

                object harisResult = harisCmd.ExecuteScalar();
                harisResult = (int)harisResult;

                object bhattiResult = bhattiCmd.ExecuteScalar();
                bhattiResult = (int)bhattiResult;

                object rashidResult = rashidCmd.ExecuteScalar();
                rashidResult = (int)rashidResult;

                object totalResult = totalCmd.ExecuteScalar();
                totalResult = (int)totalResult;


                conn.Close();

                lblHaris.Text = harisResult.ToString();
                lblBhatti.Text = bhattiResult.ToString();
                lblRashid.Text = rashidResult.ToString();
                lblTotal.Text = totalResult.ToString();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
