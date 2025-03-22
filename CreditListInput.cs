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
    public partial class CreditListInput: Form
    {
        static string conString = @"Data Source=DESKTOP-ND2U4SJ\SQLEXPRESS;Initial Catalog=Company;Integrated Security=True";
        SqlConnection conn = new SqlConnection(conString);
        private DailyDataInput mainForm;
        public CreditListInput(DailyDataInput dailyDataInput)
        {
            InitializeComponent();
            this.mainForm = dailyDataInput; // Assign reference
        }

        private void CreditList_Load(object sender, EventArgs e)
        {
            showSpendList();

            DataGridViewButtonColumn paid = new DataGridViewButtonColumn();
            paid.Text = "Paid";
            paid.HeaderText = "Paid";
            paid.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Insert(5, paid);


            DataGridViewButtonColumn delete = new DataGridViewButtonColumn();
            delete.Text = "Delete";
            delete.HeaderText = "Delete";
            delete.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Insert(6, delete);



            

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoGenerateColumns = false; // Prevent auto-generation of columns
        }

        private void showSpendList()
        {
            string creditQuery = @"SELECT Spends.id AS [ID], Spends.name AS [Name], Spends.item AS [Item], Spends.cost AS [Cost],
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

            int totalCredit = 0;
            // Total Credit
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                totalCredit += Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
            }
            lbTotal.Text = totalCredit.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (e.ColumnIndex == 5)
            {
                DialogResult ds = MessageBox.Show("Are you sure you have paid this item", "Item Status", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

                if (ds == DialogResult.Yes)
                {
                    string name = dataGridView1.Rows[rowIndex].Cells[1].Value.ToString();
                    string item = dataGridView1.Rows[rowIndex].Cells[2].Value.ToString();
                    int price = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells[3].Value);
                    string status = "Debit";

                    //MessageBox.Show($"{name} {item} {price}");
                    mainForm.addSpend(name, item, price, status);
                    //this.Close();
                }
            }
            
            else if(e.ColumnIndex == 6)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells[0].Value);
                DialogResult ds = MessageBox.Show("Are you sure you want to delete this Item", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if(ds == DialogResult.Yes)
                {
                    string query = @"DELETE FROM Spends WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                    showSpendList();
                }
            }
        }
    }
}
