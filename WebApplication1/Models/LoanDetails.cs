using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LoanDetails
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }



        [Display(Name = "Customer Linked Account ")]
        public int LinkedCustomerAccountId { get; set; }

        [Required]
        [Display(Name = "Terms")]
        public int TermsId { get; set; }

        public Terms Terms { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Loan Amount")]
        public float LoanAmount { get; set; }
    }
}