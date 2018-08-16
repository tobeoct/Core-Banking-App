using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LoanDetails
    {
        
        [Key]
        public int Id { get; set; }

        [Required]
        public string Terms { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Loan Amount")]
        public float LoanAmount { get; set; }
    }
}