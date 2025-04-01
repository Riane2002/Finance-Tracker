using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExpenseTracker
{
    public partial class ExpenseReport : System.Web.UI.Page
    {
        // Connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["FinanceTrackerConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    // Set default values for filters
                    txtStartDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                    txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                    // Load the expense report
                    LoadExpenseReport();

                    // Set the username (for demo purposes, replace with actual user authentication)
                    lblUser.Text = "Demo User";
                }
                catch (Exception ex)
                {
                    lblError.Text = "Error loading page: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }

       private void LoadExpenseReport()
{
    try
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT TransactionID, Username, Date, Category, Amount, Description FROM Expenses WHERE 1=1";
            
            // Search by either username or date
            if (!string.IsNullOrEmpty(txtSearchKey.Text))
            {
                // Try to parse as date first
                if (DateTime.TryParse(txtSearchKey.Text, out DateTime searchDate))
                {
                    query += " AND CONVERT(date, Date) = @SearchDate";
                }
                else
                {
                    query += " AND Username LIKE @Username";
                }
            }
            
            // Date range filters
            if (!string.IsNullOrEmpty(txtStartDate.Text) && DateTime.TryParse(txtStartDate.Text, out DateTime startDate))
            {
                query += " AND Date >= @StartDate";
            }
            if (!string.IsNullOrEmpty(txtEndDate.Text) && DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
            {
                query += " AND Date <= @EndDate";
            }
            
            // Category filter
            if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
            {
                query += " AND Category = @Category";
            }

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                // Search parameter
                if (!string.IsNullOrEmpty(txtSearchKey.Text))
                {
                    if (DateTime.TryParse(txtSearchKey.Text, out DateTime searchDate))
                    {
                        cmd.Parameters.AddWithValue("@SearchDate", searchDate.Date);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Username", "%" + txtSearchKey.Text + "%");
                    }
                }
                
                // Date range parameters
                if (!string.IsNullOrEmpty(txtStartDate.Text) && DateTime.TryParse(txtStartDate.Text, out DateTime startDate))
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                }
                if (!string.IsNullOrEmpty(txtEndDate.Text) && DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
                {
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                }
                
                // Category parameter
                if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
                {
                    cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
                }

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvExpenseReport.DataSource = dt;
                gvExpenseReport.DataBind();

                if (dt.Rows.Count == 0)
                {
                    lblError.Text = "No expenses found matching your criteria.";
                    lblError.Visible = true;
                }
                else
                {
                    lblError.Visible = false;
                }
            }
        }
    }
    catch (Exception ex)
    {
        lblError.Text = "Error loading expense report: " + ex.Message;
        lblError.Visible = true;
    }
}
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!DateTime.TryParse(txtDate.Text, out DateTime date) || !decimal.TryParse(txtAmount.Text, out decimal amount))
                {
                    lblError.Text = "Invalid date or amount format.";
                    lblError.Visible = true;
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd;
                    if (string.IsNullOrEmpty(hfTransactionID.Value))
                    {
                        // Insert new expense
                        cmd = new SqlCommand("INSERT INTO Expenses (Username, Date, Category, Amount, Description) VALUES (@Username, @Date, @Category, @Amount, @Description); SELECT SCOPE_IDENTITY();", con);
                    }
                    else
                    {
                        // Update existing expense
                        cmd = new SqlCommand("UPDATE Expenses SET Date = @Date, Category = @Category, Amount = @Amount, Description = @Description WHERE TransactionID = @TransactionID", con);
                        cmd.Parameters.AddWithValue("@TransactionID", Convert.ToInt32(hfTransactionID.Value));
                    }

                    cmd.Parameters.AddWithValue("@Username", lblUser.Text); // Replace with actual user authentication
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@Category", ddlFormCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text);

                    con.Open();
                    if (string.IsNullOrEmpty(hfTransactionID.Value))
                    {
                        hfTransactionID.Value = cmd.ExecuteScalar().ToString();
                    }
                    else
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // Clear the form and reload the grid
                ClearForm();
                LoadExpenseReport();
            }
            catch (Exception ex)
            {
                lblError.Text = "Error saving expense: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadExpenseReport();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtStartDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlCategory.SelectedIndex = 0;
            LoadExpenseReport();
        }

        protected void btnDownloadCSV_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Include Username in the query
                    string query = @"SELECT Username, TransactionID, Date, Category, Amount, Description 
                           FROM Expenses 
                           ORDER BY Date DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();

                    // Add headers including Username
                    sb.AppendLine("Username,TransactionID,Date,Category,Amount,Description");

                    foreach (DataRow row in dt.Rows)
                    {
                        sb.AppendLine(
                            $"\"{row["Username"].ToString().Replace("\"", "\"\"")}\"," +
                            $"{row["TransactionID"]}," +
                            $"{Convert.ToDateTime(row["Date"]).ToString("MM/dd/yyyy")}," +
                            $"\"{row["Category"].ToString().Replace("\"", "\"\"")}\"," +
                            $"{row["Amount"]}," +
                            $"\"{row["Description"].ToString().Replace("\"", "\"\"")}\""
                        );
                    }

                    Response.Clear();
                    Response.ContentType = "text/csv";
                    Response.AddHeader("Content-Disposition", "attachment;filename=AllExpenses.csv");
                    Response.Write(sb.ToString());
                    Response.End();
                }
                else
                {
                    lblError.Text = "No expenses found to export.";
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error downloading CSV: " + ex.Message;
                lblError.Visible = true;
            }
        }  // ...



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadExpenseReport();
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearchKey.Text = string.Empty;
            LoadExpenseReport();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Implement logout logic (e.g., clear session and redirect to login page)
            Response.Redirect("Login.aspx");
        }

        protected void gvExpenseReport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                int transactionId = Convert.ToInt32(gvExpenseReport.DataKeys[rowIndex].Value);

                if (e.CommandName == "EditExpense")
                {
                    DataRow row = GetExpenseById(transactionId);
                    if (row != null)
                    {
                        hfTransactionID.Value = transactionId.ToString();
                        txtDate.Text = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                        ddlFormCategory.SelectedValue = row["Category"].ToString();
                        txtAmount.Text = row["Amount"].ToString();
                        txtDescription.Text = row["Description"].ToString();
                        lblFormTitle.Text = "Edit Expense";
                    }
                }
                else if (e.CommandName == "DeleteExpense")
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Expenses WHERE TransactionID = @TransactionID", con))
                        {
                            cmd.Parameters.AddWithValue("@TransactionID", transactionId);
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadExpenseReport();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error processing command: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private DataRow GetExpenseById(int transactionId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Expenses WHERE TransactionID = @TransactionID", con))
                    {
                        cmd.Parameters.AddWithValue("@TransactionID", transactionId);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error retrieving expense: " + ex.Message;
                lblError.Visible = true;
                return null;
            }
        }

        private void ClearForm()
        {
            hfTransactionID.Value = string.Empty;
            txtDate.Text = string.Empty;
            ddlFormCategory.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
            txtDescription.Text = string.Empty;
            lblFormTitle.Text = "Add New Expense";
            lblError.Visible = false;
        }
    }
}