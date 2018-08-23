using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CustomerAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsClosed { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Account Balance")]
        public float AccountBalance { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        public Branch Branch { get; set; }

        [Display(Name = "Customer Account Type")]
        public int AccountTypeId { get; set; }

        public AccountType AccountType { get; set; }

        public int? LoanDetailsId { get; set; }
        public LoanDetails LoanDetails { get; set; }
    }
}