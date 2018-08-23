using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class TellerDto
    {

        [Key]
        public int Id { get; set; }

        [Display(Name = "User")]
        public string UserTellerId { get; set; }
        

        [Display(Name = "Available Till Accounts")]
        public int TillAccountId { get; set; }


        [Display(Name = "Till Account Balance")]
        public long TillAccountBalance { get; set; }
        public bool IsAssigned { get; set; }
    }
}