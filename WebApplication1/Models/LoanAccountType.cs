using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LoanAccountType
    {
        [Key]
        [Display(Name = "Name")]
        public string Id { get; set; }

        [Required]
        public float DebitInterestRate { get; set; }

        public int InterestIncomeGLAccountId { get; set; }

        [Display(Name = "Interest Income GL Account")]
        public int GlAccountId { get; set; }

        public GLAccount GlAccount { get; set; }
    }
}