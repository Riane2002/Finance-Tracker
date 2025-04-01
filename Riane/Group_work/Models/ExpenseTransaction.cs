using System;
using System.ComponentModel.DataAnnotations;

namespace Group_work.Models
{
    public class ExpenseTransaction
    {
        public int Id { get; set; }

        [Display(Name = "Transaction ID")]
        public string TransactionId { get; set; }

        [Required]
        public string Category { get; set; }

        public string Code { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string Status { get; set; }

        [Display(Name = "Date Added")]
        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }
    }
}