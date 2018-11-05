using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class TSSAccount
    {
        [Key]
        public int Id { get; set; }

        public int GLAccountId { get; set; }

        public GLAccount GLAccount { get; set; }
    }
}