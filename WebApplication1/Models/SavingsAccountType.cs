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
        public float CreditInterestRate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public float MinimumBalance { get; set; }

        public int InterestExpenseGlAccountId { get; set; }

        public int GlAccountId { get; set; }

        [Display(Name = "Interest Expense GL Account")]

        public GLAccount GlAccount { get; set; }

    }
}