using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Group_work.Models;

namespace Group_work.Controllers
{
    public class ExpenseController : Controller
    {
        // Mock data for transactions since no DB is connected yet
        private static List<ExpenseTransaction> mockTransactions = new List<ExpenseTransaction>
        {
            new ExpenseTransaction { Id = 1, TransactionId = "302012", Category = "Rent", Code = "10", Price = 120.00m, Status = "Lower", DateAdded = new DateTime(2022, 12, 29) },
            new ExpenseTransaction { Id = 2, TransactionId = "302011", Category = "Shopping", Code = "204", Price = 590.00m, Status = "Higher", DateAdded = new DateTime(2022, 12, 24) },
            new ExpenseTransaction { Id = 3, TransactionId = "302002", Category = "Groceries", Code = "48", Price = 125.00m, Status = "Average", DateAdded = new DateTime(2022, 12, 12) },
            new ExpenseTransaction { Id = 4, TransactionId = "301901", Category = "Others", Code = "401", Price = 348.00m, Status = "Higher", DateAdded = new DateTime(2022, 10, 21) },
            new ExpenseTransaction { Id = 5, TransactionId = "301881", Category = "Others", Code = "432", Price = 234.00m, Status = "Higher", DateAdded = new DateTime(2022, 10, 21) },
            new ExpenseTransaction { Id = 6, TransactionId = "301843", Category = "Others", Code = "0", Price = 760.00m, Status = "Lower", DateAdded = new DateTime(2022, 9, 19) },
            new ExpenseTransaction { Id = 7, TransactionId = "301600", Category = "Groceries", Code = "347", Price = 400.00m, Status = "Higher", DateAdded = new DateTime(2022, 9, 19) },
        };

        // GET: Expense
        public ActionResult Index()
        {
            return View();
        }

        // GET: Expense/Transactions
        public ActionResult Transactions(string filter = "All Expenses", string searchTerm = "", DateTime? fromDate = null, DateTime? toDate = null)
        {
            ViewBag.CurrentFilter = filter;
            var transactions = mockTransactions;

            // Apply filtering based on status
            if (filter != "All Expenses")
            {
                transactions = transactions.Where(t => t.Status == filter).ToList();
            }

            // Apply search term if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                transactions = transactions.Where(t =>
                    t.TransactionId.Contains(searchTerm) ||
                    t.Category.Contains(searchTerm) ||
                    t.Code.Contains(searchTerm)
                ).ToList();
            }

            // Apply date filtering if dates are provided
            if (fromDate.HasValue)
            {
                transactions = transactions.Where(t => t.DateAdded >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                transactions = transactions.Where(t => t.DateAdded <= toDate.Value).ToList();
            }

            return View(transactions);
        }

        // GET: Expense/AddExpense
        public ActionResult AddExpense()
        {
            return View();
        }

        // POST: Expense/AddExpense
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExpense(ExpenseTransaction expense)
        {
            if (ModelState.IsValid)
            {
                // Generate a new ID
                expense.Id = mockTransactions.Count > 0 ? mockTransactions.Max(t => t.Id) + 1 : 1;
                expense.TransactionId = "30" + new Random().Next(1000, 9999).ToString();
                expense.DateAdded = DateTime.Now;

                // Add to our mock data
                mockTransactions.Add(expense);

                return RedirectToAction("Transactions");
            }

            return View(expense);
        }

        // GET: Expense/Export
        public ActionResult Export()
        {
            // In a real application, you would generate a CSV or Excel file here
            // For now, we'll just redirect back to the Transactions page
            TempData["Message"] = "Data exported successfully!";
            return RedirectToAction("Transactions");
        }

        // Dashboard view
        public ActionResult Dashboard()
        {
            return View();
        }

        // Budget view
        public ActionResult Budget()
        {
            return View();
        }

        // Reports view
        public ActionResult Reports()
        {
            return View();
        }

        // Settings view
        public ActionResult Settings()
        {
            return View();
        }
    }
}