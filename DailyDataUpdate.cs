using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Company
{
    public partial class DailyDataUpdate : Form
    {
        int DailyDataId = 0;
        static string conString = @"Data Source=DESKTOP-ND2U4SJ\SQLEXPRESS;Initial Catalog=Company;Integrated Security=True";
        SqlConnection conn = new SqlConnection(conString);
        DataSet dataSet = new DataSet();

        public DailyDataUpdate(int dailyDataId)
        {
            InitializeComponent();
            DailyDataId = dailyDataId;
        }

        private void DailyDataUpdate_Load(object sender, EventArgs e)
        {
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = "dd-MM-yyyy";
            dataSet.Tables.Clear();
            // Operations on Spend
            string spendQuery = @"SELECT * FROM Spends where dailyRecordId = @dailyDataId";
            SqlCommand spendCmd = new SqlCommand(spendQuery, conn);
            spendCmd.Parameters.AddWithValue("dailyDataid", DailyDataId);
            SqlDataAdapter spendCmdDataAdapter = new SqlDataAdapter(spendCmd);
            try
            {
                conn.Open();
                spendCmdDataAdapter.Fill(dataSet, "spendTable");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            foreach (DataRow dr in dataSet.Tables["spendTable"].Rows)
            {
                string name = dr["name"].ToString();
                string item = dr["item"].ToString();
                int cost = Convert.ToInt32(dr["cost"]);
                if (dr["status"].ToString() == "Debit")
                {
                    DebitList.Items.Add($"{cost} - {item} : {name}");
                }
                else if (dr["status"].ToString() == "Credit")
                {
                    CreditList.Items.Add($"{cost} - {item} : {name}");
                }
            }

            // Operations on Recive
            string reciveQuery = @"SELECT * FROM Recives where dailyRecordId = @dailyDataId";
            SqlCommand reciveCmd = new SqlCommand(reciveQuery, conn);
            reciveCmd.Parameters.AddWithValue("dailyDataid", DailyDataId);
            SqlDataAdapter reciveCmdDataAdapter = new SqlDataAdapter(reciveCmd);
            try
            {
                conn.Open();
                reciveCmdDataAdapter.Fill(dataSet, "reciveTable");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            foreach (DataRow dr in dataSet.Tables["reciveTable"].Rows)
            {
                string name = dr["name"].ToString();
                string item = dr["item"].ToString();
                int cost = Convert.ToInt32(dr["cost"]);
                ReciveList.Items.Add($"{cost} - {item} : {name}");

            }

            // Operations on Other
            string otherQuery = @"SELECT * FROM Others where dailyRecordId = @dailyDataId";
            SqlCommand otherCmd = new SqlCommand(otherQuery, conn);
            otherCmd.Parameters.AddWithValue("dailyDataid", DailyDataId);
            SqlDataAdapter otherCmdDataAdapter = new SqlDataAdapter(otherCmd);
            try
            {
                conn.Open();
                otherCmdDataAdapter.Fill(dataSet, "OtherTable");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            foreach (DataRow dr in dataSet.Tables["OtherTable"].Rows)
            {
                string name = dr["name"].ToString();
                string item = dr["item"].ToString();
                int cost = Convert.ToInt32(dr["cost"]);
                OtherList.Items.Add($"{cost} - {item} : {name}");

            }

            //Operation on NetWorth/Date
            string DailyRecordQuery = @"SELECT * FROM DailyRecords where id = @dailyDataId";
            SqlCommand DailyRecordCmd = new SqlCommand(DailyRecordQuery, conn);
            DailyRecordCmd.Parameters.AddWithValue("dailyDataid", DailyDataId);
            SqlDataAdapter DailyRecordCmdDataAdapter = new SqlDataAdapter(DailyRecordCmd);
            try
            {
                conn.Open();
                DailyRecordCmdDataAdapter.Fill(dataSet, "dailyData");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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

            if (dataSet.Tables["dailyData"].Rows.Count > 0)
            {
                DataRow dr = dataSet.Tables["dailyData"].Rows[0]; // Get the first row
                tbNetWorth.Text = dr["netWorth"].ToString(); // Set the netWorth value
                DateTime date = (DateTime)dr["date"];
                dateTimePicker.Value = date;
            }
            else
            {
                MessageBox.Show("No data found for the given ID.");
            }

        }

        private void btnSpendAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string customerName = tbSpendName.Text;
                string itemName = tbSpendItem.Text;
                int itemCost = Convert.ToInt32(tbSpendCost.Text);
                string creditStatus = rbDebit.Checked ? "Debit" : "Credit";

                // Add the new item to the ListBox
                if (creditStatus == "Debit")
                {
                    DebitList.Items.Add($"{itemCost} - {itemName} : {customerName}");
                }
                else
                {
                    CreditList.Items.Add($"{itemCost} - {itemName} : {customerName}");
                }

                // Clear input fields for convenience
                tbSpendName.Clear();
                tbSpendItem.Clear();
                tbSpendCost.Clear();
                tbSpendName.Focus();
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid inputs for the name and cost.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSpendClear_Click(object sender, EventArgs e)
        {
            // Check if an item is selected in the ListBox
            if (DebitList.SelectedItem != null)
            {
                DebitList.Items.Remove(DebitList.SelectedItem); // Remove from ListBox
            }
            else if (CreditList.SelectedItem != null)
            {
                CreditList.Items.Remove(CreditList.SelectedItem); // Remove from ListBox
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }

        private void btnCreToDeb_Click(object sender, EventArgs e)
        {
            if (CreditList.SelectedItem != null)
            {
                // Get the selected item's text
                string selectedItemText = CreditList.SelectedItem.ToString();

                // Parse the name, cost and item from the selected item's text
                string[] parts = selectedItemText.Split('-', ':');
                if (parts.Length == 3)
                {
                    try
                    {
                        // Remove the item from the LinkedList and ListBox if found
                        DebitList.Items.Add(CreditList.SelectedItem);
                        CreditList.Items.Remove(CreditList.SelectedItem); // Remove from ListBox
                        MessageBox.Show("Item Added to Debit List.");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid item format in the ListBox.");
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }

        private void btnReciveAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string customerName = tbReciveName.Text;
                string itemName = tbReciveItem.Text;
                int itemCost = Convert.ToInt32(tbReciveCost.Text);
                ReciveList.Items.Add($"{itemCost} - {itemName} : {customerName}");

                // Clear input fields for convenience
                tbReciveName.Clear();
                tbReciveItem.Clear();
                tbReciveCost.Clear();
                tbReciveName.Focus();
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid inputs for the name and cost.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnReciveClear_Click(object sender, EventArgs e)
        {
            // Check if an item is selected in the ListBox
            if (ReciveList.SelectedItem != null)
            {
                ReciveList.Items.Remove(ReciveList.SelectedItem); // Remove from ListBox
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }

        private void btnAddOther_Click(object sender, EventArgs e)
        {
            try
            {
                string customerName = tbOtherName.Text;
                string itemName = tbOtherItem.Text;
                int itemCost = Convert.ToInt32(tbOtherCost.Text);
                OtherList.Items.Add($"{itemCost} - {itemName} : {customerName}");

                // Clear input fields for convenience
                tbOtherName.Clear();
                tbOtherItem.Clear();
                tbOtherCost.Clear();
                tbSpendName.Focus();
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid inputs for the name and cost.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClearOther_Click(object sender, EventArgs e)
        {
            // Check if an item is selected in the ListBox
            if (OtherList.SelectedItem != null)
            {
                OtherList.Items.Remove(OtherList.SelectedItem); // Remove from ListBox
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }

        class Item
        {
            public string name { get; set; }
            public string item { get; set; }
            public int cost { get; set; }
        }
        class Spend : Item
        {
            public string status { get; set; }
        }

        LinkedList<Spend> spend = new LinkedList<Spend>();
        LinkedList<Item> recive = new LinkedList<Item>();
        LinkedList<Item> other = new LinkedList<Item>();

        public void listBoxToLinkedList()
        {
            try
            {
                // Operation on Debit
                if (DebitList.Items.Count > 0)
                {
                    foreach (string items in DebitList.Items)
                    {
                        string selectedItem = items;
                        string status = "Debit";
                        string[] product = selectedItem.Split('-', ':');
                        int cost = Convert.ToInt32(product[0].Trim());
                        string item = product[1].Trim();
                        string name = product[2].Trim();

                        //MessageBox.Show(name + ", " + cost + ", " + credit);

                        Spend newItem = new Spend
                        {
                            name = name,
                            item = item,
                            cost = cost,
                            status = status
                        };
                        spend.AddLast(newItem);
                    }
                }
                // OPeration on Credit
                if (CreditList.Items.Count > 0)
                {

                    foreach (string items in CreditList.Items)
                    {
                        string selectedItem = items;
                        string status = "Credit";
                        string[] product = selectedItem.Split('-', ':');
                        int cost = Convert.ToInt32(product[0].Trim());
                        string item = product[1].Trim();
                        string name = product[2].Trim();

                        //MessageBox.Show(name + ", " + cost + ", " + credit);

                        Spend newItem = new Spend
                        {
                            name = name,
                            item = item,
                            cost = cost,
                            status = status
                        };
                        spend.AddLast(newItem);
                    }
                }


                // Operation on Recive
                if (ReciveList.Items.Count > 0)
                {

                    foreach (string items in ReciveList.Items)
                    {
                        string[] product = items.Split('-', ':');
                        int cost = Convert.ToInt32(product[0].Trim());
                        string item = product[1].Trim();
                        string name = product[2].Trim();

                        //MessageBox.Show(name + ", " + cost);

                        Item newItem = new Item
                        {
                            name = name,
                            item = item,
                            cost = cost,
                        };
                        recive.AddLast(newItem);
                    }
                }

                // Operation on Other
                if (OtherList.Items.Count > 0)
                {

                    foreach (string items in OtherList.Items)
                    {
                        string[] product = items.Split('-', ':');
                        int cost = Convert.ToInt32(product[0].Trim());
                        string item = product[1].Trim();
                        string name = product[2].Trim();

                        //MessageBox.Show(name + ", " + cost);

                        Item newItem = new Item
                        {
                            name = name,
                            item = item,
                            cost = cost,
                        };
                        other.AddLast(newItem);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void deleteFromDataBase()
        {
            // DELETE DATA
            try
            {
                conn.Open();

                // Delete Spend Data
                string deleteSpendQuery = @"DELETE FROM Spends WHERE DailyRecordId = @DailyRecordId";
                SqlCommand deleteSpendCmd = new SqlCommand(deleteSpendQuery, conn);
                deleteSpendCmd.Parameters.AddWithValue("@DailyRecordId", DailyDataId);
                deleteSpendCmd.ExecuteNonQuery();

                // Delete Recieve Data
                string deleteReciveQuery = @"DELETE FROM Recives WHERE DailyRecordId = @DailyRecordId";
                SqlCommand deleteReciveCmd = new SqlCommand(deleteReciveQuery, conn);
                deleteReciveCmd.Parameters.AddWithValue("@DailyRecordId", DailyDataId);
                deleteReciveCmd.ExecuteNonQuery();

                // Delete Other Data
                string deleteOtherQuery = @"DELETE FROM Others WHERE DailyRecordId = @DailyRecordId";
                SqlCommand deleteOtherCmd = new SqlCommand(deleteOtherQuery, conn);
                deleteOtherCmd.Parameters.AddWithValue("@DailyRecordId", DailyDataId);
                deleteOtherCmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            deleteFromDataBase();
            listBoxToLinkedList();

            // Operation On NetWorth / Differ
            int netWorthFromUser = Convert.ToInt32(tbNetWorth.Text);

            int totalSpend = 0;
            int totalRecive = 0;
            int totalOther = 0;
            int month = Convert.ToInt16(tbMonth.Text);
            DateTime getDate = dateTimePicker.Value;

            foreach (Spend item in spend)
            {
                if (item.status == "Debit")
                {
                    totalSpend += item.cost;
                }
            }

            foreach (Item item in recive)
            {
                totalRecive += item.cost;
            }

            foreach (Item item in other)
            {
                totalOther += item.cost;
            }


            // Database Connectivity
            try
            {
                conn.Open();
                // Retrive Previous Networth
                string netWorhtQuery = @"SELECT netWorth 
                                        FROM DailyRecords 
                                        WHERE id = (SELECT MAX(id) FROM DailyRecords WHERE id < @DailyRecordId);";
                SqlCommand netWorhtCmd = new SqlCommand(netWorhtQuery, conn);
                netWorhtCmd.Parameters.AddWithValue("@DailyRecordId", DailyDataId);
                object result = netWorhtCmd.ExecuteScalar();
                int PreviousNetWorth = Convert.ToInt32(result);

                // Calculating Difference
                int netWorthDifference = PreviousNetWorth + totalRecive;
                netWorthDifference -= totalSpend;
                netWorthDifference -= totalOther;
                netWorthDifference -= netWorthFromUser;

                // Update Networth
                string updateNetWortQuery = @"UPDATE DailyRecords
                                                SET netWorth = @netWorth, differ = @differ, date = @date, month = @month
                                                WHERE id = @DailyRecordId";
                SqlCommand updateNetWorthCmd = new SqlCommand(@updateNetWortQuery, conn);
                updateNetWorthCmd.Parameters.AddWithValue("@netWorth", netWorthFromUser);
                updateNetWorthCmd.Parameters.AddWithValue("@differ", netWorthDifference);
                updateNetWorthCmd.Parameters.AddWithValue("@date", getDate);
                updateNetWorthCmd.Parameters.AddWithValue("@month", month);
                updateNetWorthCmd.Parameters.AddWithValue("@DailyRecordId", DailyDataId);
                updateNetWorthCmd.ExecuteNonQuery();

                // Insert Spend Data
                string spendQuery = @"INSERT INTO Spends (name, item, cost, status, DailyRecordId) 
                                            VALUES (@name, @item, @cost, @status, @DailyRecordId)";
                foreach (Spend item in spend)
                {
                    SqlCommand spendQueryCmd = new SqlCommand(spendQuery, conn);
                    spendQueryCmd.Parameters.AddWithValue("@name", item.name);
                    spendQueryCmd.Parameters.AddWithValue("@item", item.item);
                    spendQueryCmd.Parameters.AddWithValue("@cost", item.cost);
                    spendQueryCmd.Parameters.AddWithValue("@status", item.status);
                    spendQueryCmd.Parameters.AddWithValue("@DailyRecordId", DailyDataId);
                    spendQueryCmd.ExecuteNonQuery();
                }

                // Insert Recive Data
                string reciveQuery = @"INSERT INTO Recives (name, item, cost, DailyRecordId) 
                                                VALUES (@name, @item, @cost, @DailyRecordId)";
                foreach (Item item in recive)
                {
                    SqlCommand reciveQueryCmd = new SqlCommand(reciveQuery, conn);
                    reciveQueryCmd.Parameters.AddWithValue("@name", item.name);
                    reciveQueryCmd.Parameters.AddWithValue("@item", item.item);
                    reciveQueryCmd.Parameters.AddWithValue("@cost", item.cost);
                    reciveQueryCmd.Parameters.AddWithValue("@DailyRecordId", DailyDataId);
                    reciveQueryCmd.ExecuteNonQuery();
                }

                // Insert Recive Data
                string otherQuery = @"INSERT INTO Others (name, item, cost, DailyRecordId) 
                                                VALUES (@name, @item, @cost, @DailyRecordId)";
                foreach (Item item in other)
                {
                    SqlCommand otherQueryCmd = new SqlCommand(otherQuery, conn);
                    otherQueryCmd.Parameters.AddWithValue("@name", item.name);
                    otherQueryCmd.Parameters.AddWithValue("@item", item.item);
                    otherQueryCmd.Parameters.AddWithValue("@cost", item.cost);
                    otherQueryCmd.Parameters.AddWithValue("@DailyRecordId", DailyDataId);
                    otherQueryCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Record Update Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
                this.Close();
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DebitList.Items.Clear();
            CreditList.Items.Clear();
            ReciveList.Items.Clear();
            OtherList.Items.Clear();

            spend.Clear();
            recive.Clear();
            other.Clear();

            tbNetWorth.Clear();

            dateTimePicker.Value = DateTime.Now;
        }
    }
}
