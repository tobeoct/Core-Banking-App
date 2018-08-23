using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class LoanDetailsDto
    {

        [Key]
        public int Id { get; set; }

        [Display(Name = "Linked Customer Account Bank")]
        public string LinkedCustomerAccountBank { get; set; }

        [Display(Name = "Linked Customer Account Number")]
        public long LinkedCustomerAccountNumber { get; set; }

        public int customerAccountId { get; set; }

        public long BVN { get; set; }

        [Required]
        [Display(Name = "Terms")]
        public int TermsId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Loan Amount")]
        public float LoanAmount { get; set; }
    }
}