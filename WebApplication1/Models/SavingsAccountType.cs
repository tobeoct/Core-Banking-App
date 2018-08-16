using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class SavingsAccountType
    {
        [Key]
        [Display(Name = "Name")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Credit Interest Rate")]
        public float CreditInterestRate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Minimum Balance")]
        public float MinimumBalance { get; set; }

        public int InterestExpenseGlAccountId { get; set; }

        [Display(Name = "Interest Expense GL Account")]
        public int GLAccountId { get; set; }

        public GLAccount GLAccount { get; set; }

    }
}