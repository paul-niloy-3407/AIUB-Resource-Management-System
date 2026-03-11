using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AIUBResourceManagementSystem
{
    public partial class testLogin : Form
    {
        SqlConnection con;
        string connectionString1 = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Niloy Paul\Documents\Resource Management.mdf;Integrated Security=True;Connect Timeout=30";

        // Method to test the connection
        private static bool TestConnection(string connectionString1)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString1))
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // Method to establish the connection
        public void dbcon()
        {
            if (TestConnection(connectionString1))
            {
                con = new SqlConnection(connectionString1);
                con.Open();
            }
        }

        public testLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim(); // Trim to remove unnecessary whitespace
            string password = textBox2.Text.Trim(); // Trim to remove unnecessary whitespace

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dbcon(); 

                string query = "SELECT COUNT(1) FROM Faculty WHERE Username = @Username AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count == 1)
                    {
                        MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
              con.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Ensure a row is selected in the DataGridView
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to transfer.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the FacultyID of the selected row
            string selectedFacultyID = dataGridView2.SelectedRows[0].Cells["FacultyID"].Value.ToString();

            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Niloy Paul\Documents\Resource Management.mdf;Integrated Security=True;Connect Timeout=30"))
                {
                    con.Open();

                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Query to transfer the selected row to ApprovedFaculty
                            string transferQuery = @"
                        INSERT INTO ApprovedFaculty (FacultyID, FullName, EmailAddress, ContactNumber, Department, Designation, DateOfBirth, Gender, Username, Password)
                        SELECT FacultyID, FullName, EmailAddress, ContactNumber, Department, Designation, DateOfBirth, Gender, Username, Password
                        FROM Faculty
                        WHERE FacultyID = @FacultyID";

                            // Query to delete the selected row from Faculty
                            string deleteQuery = "DELETE FROM Faculty WHERE FacultyID = @FacultyID";

                            // Execute transfer query
                            using (SqlCommand transferCmd = new SqlCommand(transferQuery, con, transaction))
                            {
                                transferCmd.Parameters.AddWithValue("@FacultyID", selectedFacultyID);
                                int rowsTransferred = transferCmd.ExecuteNonQuery();

                                if (rowsTransferred > 0)
                                {
                                    // Execute delete query
                                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, con, transaction))
                                    {
                                        deleteCmd.Parameters.AddWithValue("@FacultyID", selectedFacultyID);
                                        deleteCmd.ExecuteNonQuery();
                                    }

                                    // Commit transaction
                                    transaction.Commit();

                                    // Remove the transferred row from DataGridView (this step reflects the database change)
                                    dataGridView2.Rows.RemoveAt(dataGridView2.SelectedRows[0].Index);

                                    // Inform user
                                    MessageBox.Show("Record successfully transferred and removed from the Faculty table.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    // Rollback if no records were transferred
                                    transaction.Rollback();
                                    MessageBox.Show("Selected record not found in the Faculty table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch
                        {
                            // Rollback the transaction in case of any error
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error message if something goes wrong
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            dbcon();
            SqlCommand sq1 = new SqlCommand("select * from Faculty", con);
            SqlCommand sq2 = new SqlCommand("select * from UAFaculty", con);
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            SqlDataReader sdr = sq1.ExecuteReader();
            dt.Load(sdr);
            SqlDataReader sdr2 = sq2.ExecuteReader();
            dt2.Load(sdr2);

            dataGridView1.DataSource = dt;
            dataGridView2.DataSource = dt2;
            con.Close();
        }
    }
}

