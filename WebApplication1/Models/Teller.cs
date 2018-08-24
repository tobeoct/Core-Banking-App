using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Teller
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "User")]
        public string  UserTellerId { get; set; }

        public ApplicationUser UserTeller { get; set; }

        [Display(Name = "Available Till Accounts")]
        public int TillAccountId { get; set; }

        public GLAccount TillAccount { get; set; }

        [Display(Name = "Till Account Balance")]
        public float TillAccountBalance { get; set; }
        public bool IsAssigned { get; set; }
    }
}