using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CustomerAccountType
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name = "Savings Account")]
        public string SavingsAccountTypeId { get; set; }
        public SavingsAccountType SavingsAccountType { get; set; }

        [Display(Name = "Current Account")]
        public string CurrentAccountTypeId { get; set; }
        public CurrentAccountType CurrentAccountType { get; set; }

        [Display(Name = "Loan Account")]
        public string LoanAccountTypeId { get; set; }
        public LoanAccountType LoanAccountType { get; set; }
    }
}