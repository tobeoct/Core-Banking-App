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


        public int LinkedCustomerAccountId { get; set; }

        public int CustomerAccountId { get; set; }

        [Required]
        [Display(Name = "Terms")]
        public int TermsId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Loan Amount")]
        public float LoanAmount { get; set; }
        public float? InterestRate { get; set; }
        public float InterestReceivable { get; set; }
        public float InterestIncome { get; set; }
        public float PrincipalOverdue { get; set; }
        public float InterestInSuspense { get; set; }
        public float InterestOverdue { get; set; }
        public float CustomerLoan { get; set; }
    }
}