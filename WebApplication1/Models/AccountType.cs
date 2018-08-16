using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class AccountType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Display(Name = "Credit Interest Rate")]
        public float? CreditInterestRate { get; set; }

        [Display(Name = "Debit Interest Rate")]
        public float? DebitInterestRate { get; set; }

        [Display(Name = "Minimum Balance")]
        [DataType(DataType.Currency)]
        public float? MinimumBalance { get; set; }

        [Display(Name = "Interest Expense GL Account")]
        public int? InterestExpenseGLAccountId { get; set; }

        public GLAccount InterestExpenseGLAccount { get; set; }

        [Display(Name = "Interest Income GL Account")]
        public int? InterestIncomeGLAccountId { get; set; }

        public GLAccount InterestIncomeGLAccount { get; set; }

        [Display(Name = "Interest Expense GL Account")]
        public int? COTIncomeGLAccountId { get; set; }

        public GLAccount COTIncomeGLAccount { get; set; }

        [Display(Name = "Commission on Turn Over")]
        public float? COT { get; set; }
    }
}