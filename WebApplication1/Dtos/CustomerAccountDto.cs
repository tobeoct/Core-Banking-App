using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace WebApplication1.Dtos
{
    public class CustomerAccountDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsClosed { get; set; }


        public int AccountNumber { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
       

        [Display(Name = "Branch")]
        public int BranchId { get; set; }


        [Display(Name = "Customer Account Type")]
        public int AccountTypeId { get; set; }

       

        public int? LoanDetailsId { get; set; }
        
    }
}