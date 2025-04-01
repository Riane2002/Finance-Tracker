<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpenseReport.aspx.cs" Inherits="ExpenseTracker.ExpenseReport" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Expense Report - eXPENSE Tracker</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: #f8f9fa;
        }
        .navbar-brand {
            font-weight: bold;
        }
        .card {
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        .table th {
            background-color: #007bff;
            color: white;
        }
        .table td {
            vertical-align: middle;
        }
        .filter-section, .form-section, .search-section {
            margin-bottom: 20px;
        }
        .invalid-feedback {
            display: none;
            color: #dc3545;
            font-size: 0.875em;
            margin-top: 0.25rem;
        }
        .is-invalid ~ .invalid-feedback {
            display: block;
        }
        .expense-section-title {
            color: #b83280;
            font-size: 18px;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Navigation Bar -->
        <nav class="navbar navbar-expand-lg navbar-light bg-light">
            <a class="navbar-brand" href="#">eXPENSE Tracker</a>
            <div class="collapse navbar-collapse">
                <ul class="navbar-nav mr-auto">
                    <li class="nav-item">
                        <a class="nav-link" href="#">Dashboard</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#">Transactions</a>
                    </li>
                    <li class="nav-item active">
                        <a class="nav-link" href="#">Reports <span class="sr-only">(current)</span></a>
                    </li>
                </ul>
                <span class="navbar-text">
                    Welcome, <asp:Label ID="lblUser" runat="server" Text="Guest"></asp:Label>
                    <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="btn btn-link" OnClick="btnLogout_Click" CausesValidation="false" />
                </span>
            </div>
        </nav>

        <!-- Main Content -->
        <div class="container mt-4">
            <div class="card">
                <div class="card-header">
                    <h2 class="card-title expense-section-title">EXPENSE REPORT</h2>
                </div>
                <div class="card-body">
                    <!-- Error Message -->
                    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False" CssClass="mb-3"></asp:Label>

                    <!-- Search Section -->
                    <div class="search-section">
                        <div class="row">
                            <div class="col-md-4">
                                <label for="txtSearchKey">Search by Date:</label>
                                <asp:TextBox ID="txtSearchKey" runat="server" TextMode="Date" CssClass="form-control" placeholder="MM/DD/YYYY"></asp:TextBox>
                            </div>
                            <div class="col-md-8 align-self-end">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                                <asp:Button ID="btnClearSearch" runat="server" Text="Clear Search" CssClass="btn btn-secondary ml-2" OnClick="btnClearSearch_Click" CausesValidation="false" />
                            </div>
                        </div>
                    </div>

                    <!-- Form for Adding/Editing Expenses -->
                    <div class="form-section">
                        <h4><asp:Label ID="lblFormTitle" runat="server" Text="Add New Expense"></asp:Label></h4>
                        <asp:HiddenField ID="hfTransactionID" runat="server" />
                        <div class="row">
                            <div class="col-md-3">
                                <label for="txtDate">Date:</label>
                                <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" placeholder="MM/DD/YYYY" required="required"></asp:TextBox>
                                <div class="invalid-feedback">Please select a valid date.</div>
                            </div>
                            <div class="col-md-3">
                                <label for="ddlFormCategory">Category:</label>
                                <asp:DropDownList ID="ddlFormCategory" runat="server" CssClass="form-control" required="required">
                                    <asp:ListItem Text="" Value="" disabled="disabled" Selected="True">Select a category</asp:ListItem>
                                    <asp:ListItem Text="Travel" Value="Travel"></asp:ListItem>
                                    <asp:ListItem Text="Food" Value="Food"></asp:ListItem>
                                    <asp:ListItem Text="Office Supplies" Value="Office Supplies"></asp:ListItem>
                                    <asp:ListItem Text="Entertainment" Value="Entertainment"></asp:ListItem>
                                </asp:DropDownList>
                                <div class="invalid-feedback">Please select a category.</div>
                            </div>
                            <div class="col-md-3">
                                <label for="txtAmount">Amount:</label>
                                <asp:TextBox ID="txtAmount" runat="server" TextMode="Number" step="0.01" CssClass="form-control" placeholder="0.00" required="required" min="0.01"></asp:TextBox>
                                <div class="invalid-feedback">Please enter a positive amount.</div>
                            </div>
                            <div class="col-md-3">
                                <label for="txtDescription">Description:</label>
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" placeholder="Description"></asp:TextBox>
                                <div class="invalid-feedback">Description is required.</div>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-md-12">
                                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" OnClientClick="return validateForm();" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary ml-2" OnClick="btnCancel_Click" CausesValidation="false" />
                            </div>
                        </div>
                    </div>

                    <!-- Filter Section -->
                    <div class="filter-section">
                        <div class="row">
                            <div class="col-md-4">
                                <label for="txtStartDate">Start Date:</label>
                                <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control" placeholder="MM/DD/YYYY"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="txtEndDate">End Date:</label>
                                <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control" placeholder="MM/DD/YYYY"></asp:TextBox>
                            </div>
                            <div class="col-md-4">
                                <label for="ddlCategory">Category:</label>
                                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="All" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Travel" Value="Travel"></asp:ListItem>
                                    <asp:ListItem Text="Food" Value="Food"></asp:ListItem>
                                    <asp:ListItem Text="Office Supplies" Value="Office Supplies"></asp:ListItem>
                                    <asp:ListItem Text="Entertainment" Value="Entertainment"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-md-12">
                                <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary" OnClick="btnFilter_Click" />
                                <asp:Button ID="btnClear" runat="server" Text="Clear Filters" CssClass="btn btn-secondary ml-2" OnClick="btnClear_Click" CausesValidation="false" />
                                <asp:Button ID="btnDownloadCSV" runat="server" Text="Download CSV" CssClass="btn btn-success ml-2" OnClick="btnDownloadCSV_Click" />
                            </div>
                        </div>
                    </div>

                    <!-- GridView -->
                    <asp:GridView ID="gvExpenseReport" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover" OnRowCommand="gvExpenseReport_RowCommand" DataKeyNames="TransactionID">
                        <Columns>
                            <asp:BoundField DataField="Username" HeaderText="Username" />
                            <asp:BoundField DataField="TransactionID" HeaderText="Transaction ID" />
                            <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                            <asp:BoundField DataField="Category" HeaderText="Category" />
                            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="Description" HeaderText="Description" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditExpense" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-sm btn-warning" />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteExpense" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to delete this expense?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center">No expense records found.</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>

    <!-- Bootstrap JS (for navbar functionality) -->
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.5.2/dist/js/bootstrap.bundle.min.js"></script>
    <!-- Client-side validation script -->
    <script>
        function validateForm() {
            var isValid = true;
            var txtDate = document.getElementById('<%= txtDate.ClientID %>');
            var ddlCategory = document.getElementById('<%= ddlFormCategory.ClientID %>');
            var txtAmount = document.getElementById('<%= txtAmount.ClientID %>');
            var txtDescription = document.getElementById('<%= txtDescription.ClientID %>');

            // Reset validation state
            txtDate.classList.remove('is-invalid');
            ddlCategory.classList.remove('is-invalid');
            txtAmount.classList.remove('is-invalid');
            txtDescription.classList.remove('is-invalid');

            // Validate Date
            if (!txtDate.value) {
                txtDate.classList.add('is-invalid');
                isValid = false;
            }

            // Validate Category
            if (!ddlCategory.value) {
                ddlCategory.classList.add('is-invalid');
                isValid = false;
            }

            // Validate Amount
            if (!txtAmount.value || parseFloat(txtAmount.value) <= 0) {
                txtAmount.classList.add('is-invalid');
                isValid = false;
            }

            // Description is optional, so no validation required
            // If you want to make it required again, uncomment the following:
            /*
            if (!txtDescription.value.trim()) {
                txtDescription.classList.add('is-invalid');
                isValid = false;
            }
            */

            return isValid;
        }
    </script>
</body>
</html>