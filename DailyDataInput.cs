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
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Company
{
    public partial class DailyDataInput : Form
    {
        public DailyDataInput()
        {
            InitializeComponent();
        }
        private void DailyDataInput_Load(object sender, EventArgs e)
        {
            GetMonth();
            rbDebit.Checked = true;
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = "dd-MM-yyyy";
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

        static string conString = @"Data Source=DESKTOP-ND2U4SJ\SQLEXPRESS;Initial Catalog=Company;Integrated Security=True";
        SqlConnection conn = new SqlConnection(conString);



        // Add Spend
        private void btnSpendAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Get Values
                string customerName = tbSpendName.Text;
                string itemName = tbSpendItem.Text;
                int itemCost = Convert.ToInt32(tbSpendCost.Text);
                string status = rbDebit.Checked ? "Debit" : "Credit";

                addSpend(customerName, itemName, itemCost, status);

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

        public void addSpend(string customerName, string itemName, int itemCost, string status)
        {
            // Create a new SubItem object
            Spend newItem = new Spend
            {
                name = customerName,
                item = itemName,
                cost = itemCost,
                status = status
            };

            // Add the new item to the linked list
            spend.AddLast(newItem);

            // Add the new item to the ListBox
            if (newItem.status == "Debit")
            {
                DebitList.Items.Add($"{newItem.cost} - {newItem.item} : {newItem.name}");
            }
            else
            {
                CreditList.Items.Add($"{newItem.cost} - {newItem.item} : {newItem.name}");
            }   
        }

        // Call Credit List Form
        private void btnCreToDeb_Click(object sender, EventArgs e)
        {
            CreditListInput creditList = new CreditListInput(this);
            creditList.ShowDialog();
        }

        // Clear Spend
        private void btnSpendClear_Click(object sender, EventArgs e)
        {
            // Check if an item is selected in the Debit List
            if (DebitList.SelectedItem != null)
            {
                // Get the selected item's text
                string selectedItemText = DebitList.SelectedItem.ToString();

                // Parse the name, cost and item from the selected item's text
                string[] parts = selectedItemText.Split('-', ':');
                if (parts.Length == 3)
                {
                    try
                    {
                        int selectedCost = int.Parse(parts[0].Trim());
                        string selectedItem = parts[1].Trim();
                        string selectedName = parts[2].Trim();

                        // Find the corresponding subItem in the LinkedList
                        Spend itemToRemove = null;
                        foreach (var item in spend)
                        {
                            if (item.cost == selectedCost && item.name == selectedName && item.item == selectedItem)
                            {
                                itemToRemove = item;
                                break;
                            }
                        }

                        // Remove the item from the LinkedList and ListBox if found
                        if (itemToRemove != null)
                        {
                            spend.Remove(itemToRemove); // Remove from LinkedList
                            DebitList.Items.Remove(DebitList.SelectedItem); // Remove from ListBox
                            MessageBox.Show("Item successfully deleted.");
                        }
                        else
                        {
                            MessageBox.Show("Item not found in the linked list.");
                        }
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

            // Check if an item is selected in the Credit List
            else if (CreditList.SelectedItem != null)
            {
                // Get the selected item's text
                string selectedItemText = CreditList.SelectedItem.ToString();

                // Parse the name, cost and item from the selected item's text
                string[] parts = selectedItemText.Split('-', ':');
                if (parts.Length == 3)
                {
                    try
                    {
                        int selectedCost = int.Parse(parts[0].Trim());
                        string selectedItem = parts[1].Trim();
                        string selectedName = parts[2].Trim();

                        // Find the corresponding subItem in the LinkedList
                        Spend itemToRemove = null;
                        foreach (var item in spend)
                        {
                            if (item.cost == selectedCost && item.name == selectedName && item.item == selectedItem)
                            {
                                itemToRemove = item;
                                break;
                            }
                        }

                        // Remove the item from the LinkedList and ListBox if found
                        if (itemToRemove != null)
                        {
                            spend.Remove(itemToRemove); // Remove from LinkedList
                            CreditList.Items.Remove(CreditList.SelectedItem); // Remove from ListBox
                            MessageBox.Show("Item successfully deleted.");
                        }
                        else
                        {
                            MessageBox.Show("Item not found in the linked list.");
                        }
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
        // Credit To Debit
       


        // Add Recive
        private void btnReciveAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string customerName = tbReciveName.Text;
                string itemName = tbReciveItem.Text;
                int itemCost = Convert.ToInt32(tbReciveCost.Text);

                // Create a new SubItem object
                Item newItem = new Item
                {
                    name = customerName,
                    item = itemName,
                    cost = itemCost
                };

                // Add the new item to the linked list
                recive.AddLast(newItem);

                // Add the new item to the ListBox
                ReciveList.Items.Add($"{newItem.cost} - {newItem.item} : {newItem.name}");

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
        // Remove Recive
        private void btnReciveClear_Click(object sender, EventArgs e)
        {
            // Check if an item is selected in the ListBox
            if (ReciveList.SelectedItem != null)
            {
                // Get the selected item's text
                string selectedItemText = ReciveList.SelectedItem.ToString();

                // Parse the cost and name from the selected item's text
                string[] parts = selectedItemText.Split('-', ':');
                if (parts.Length == 3)
                {
                    try
                    {
                        int selectedCost = int.Parse(parts[0].Trim());
                        string selectedItem = parts[1].Trim();
                        string selectedName = parts[2].Trim();

                        // Find the corresponding SubItem in the LinkedList
                        Item itemToRemove = null;
                        foreach (var item in recive)
                        {
                            if (item.cost == selectedCost && item.name == selectedName && item.item == selectedItem)
                            {
                                itemToRemove = item;
                                break;
                            }
                        }

                        // Remove the item from the LinkedList and ListBox if found
                        if (itemToRemove != null)
                        {
                            recive.Remove(itemToRemove); // Remove from LinkedList
                            ReciveList.Items.Remove(ReciveList.SelectedItem); // Remove from ListBox
                            MessageBox.Show("Recive Item successfully deleted.");
                        }
                        else
                        {
                            MessageBox.Show("Item not found in the linked list.");
                        }
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


        // Add Other
        private void btnAddOther_Click(object sender, EventArgs e)
        {
            try
            {
                string customerName = tbOtherName.Text;
                string itemName = tbOtherItem.Text;
                int itemCost = Convert.ToInt32(tbOtherCost.Text);

                Item item = new Item
                {
                    name = customerName,
                    item = itemName,
                    cost = itemCost

                };

                // Add to the linked list
                other.AddLast(item);

                // Display item in ListBox
                OtherList.Items.Add($"{item.cost} - {item.item} : {item.name}");

                // Clear input fields for convenience
                tbOtherName.Clear();
                tbOtherItem.Clear();
                tbOtherCost.Clear();
                tbOtherName.Focus();
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid inputs for the name and cost.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Remove Other
        private void btnClearOther_Click(object sender, EventArgs e)
        {
            // Check if an item is selected in the Other List
            if (OtherList.SelectedItem != null)
            {
                // Get the selected item's text
                string selectedItemText = OtherList.SelectedItem.ToString();

                // Parse the name, item and cost from the selected item's text
                string[] parts = selectedItemText.Split('-', ':');
                if (parts.Length == 3)
                {
                    try
                    {
                        int selectedCost = int.Parse(parts[0].Trim());
                        string selectedItem = parts[1].Trim();
                        string selectedName = parts[2].Trim();

                        // Find the corresponding SubItem in the LinkedList
                        Item itemToRemove = null;
                        foreach (var item in other)
                        {
                            if (item.cost == selectedCost && item.name == selectedName && item.item == selectedItem)
                            {
                                itemToRemove = item;
                                break;
                            }
                        }

                        // Remove the item from the LinkedList and ListBox if found
                        if (itemToRemove != null)
                        {
                            other.Remove(itemToRemove); // Remove from LinkedList
                            OtherList.Items.Remove(OtherList.SelectedItem); // Remove from ListBox
                            MessageBox.Show("Item successfully deleted.");
                        }
                        else
                        {
                            MessageBox.Show("Item not found in the linked list.");
                        }
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



        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (tbNetWorth.Text == "")
            {
                MessageBox.Show("Networth can't be empty");
            }
            else
            {
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

                try
                {
                    conn.Open();

                    // Retrive Previous Networth
                    string netWorhtQuery = @"SELECT TOP 1(netWorth) 
                                            FROM DailyRecords 
                                            ORDER BY id DESC;";
                    SqlCommand netWorhtCmd = new SqlCommand(netWorhtQuery, conn);
                    object result = netWorhtCmd.ExecuteScalar();
                    int PreviousNetWorth = Convert.ToInt32(result);

                    // Calculating Difference
                    int netWorthDifference = PreviousNetWorth + totalRecive;
                    netWorthDifference -= totalSpend;
                    netWorthDifference -= totalOther;
                    netWorthDifference -= netWorthFromUser;


                    // Insert Daily Record
                    string dailyRecordquery = @"INSERT INTO DailyRecords (netWorth, differ, date, month) 
                                                VALUES (@netWorth, @differ, @date, @month);
                                                SELECT SCOPE_IDENTITY();";
                    SqlCommand dailyRecordcmd = new SqlCommand(dailyRecordquery, conn);
                    dailyRecordcmd.Parameters.AddWithValue("@netWorth", netWorthFromUser);
                    dailyRecordcmd.Parameters.AddWithValue("@differ", netWorthDifference);
                    dailyRecordcmd.Parameters.AddWithValue("@date", getDate);
                    dailyRecordcmd.Parameters.AddWithValue("@month", month);
                    int dailyRecordId = Convert.ToInt32(dailyRecordcmd.ExecuteScalar());

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
                        spendQueryCmd.Parameters.AddWithValue("@DailyRecordId", dailyRecordId);
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
                        reciveQueryCmd.Parameters.AddWithValue("@DailyRecordId", dailyRecordId);
                        reciveQueryCmd.ExecuteNonQuery();
                    }

                    // Insert Other Data
                    string otherQuery = @"INSERT INTO Others (name, item, cost, DailyRecordId) 
                                            VALUES (@name, @item, @cost, @DailyRecordId)";
                    foreach (Item item in other)
                    {
                        SqlCommand otherQueryCmd = new SqlCommand(otherQuery, conn);
                        otherQueryCmd.Parameters.AddWithValue("@name", item.name);
                        otherQueryCmd.Parameters.AddWithValue("@item", item.item);
                        otherQueryCmd.Parameters.AddWithValue("@cost", item.cost);
                        otherQueryCmd.Parameters.AddWithValue("@DailyRecordId", dailyRecordId);
                        otherQueryCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Record Added Successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                    this.Close();
                }
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


