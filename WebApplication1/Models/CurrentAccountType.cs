using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CurrentAccountType
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

        [Display(Name = "Interest Expense GL Account")]
        public int GLAccountId { get; set; }

        
        public GLAccount GlAccount { get; set; }

        [Display(Name = "Interest Expense GL Account")]
        public int InterestExpenseGLAccountId { get; set; }

        [Display(Name = "Commission on Turn-Over")]
        public int COT { get; set; }

        [Display(Name = "COT Income GL Account")]
        public int COTIncomeGLAccountId { get; set; }

        
    }
}