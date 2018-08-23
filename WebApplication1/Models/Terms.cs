using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Terms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Tenure { get; set; }

//        [Display(Name = "Interest Rate")]
//        public float InterestRate { get; set; }
//
//        [Display(Name = "Loan Amount")]
//        public float LoanAmount { get; set; }
        [Display(Name = "Payment Rate")]
        public float PaymentRate { get; set; }
        public string Collateral { get; set; }
    }
}