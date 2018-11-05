using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ATMTerminal
    {
       [Key]
       public int Id { get; set; }

        public string Name { get; set; }

        public string TerminalID { get; set; }
        public string Location { get; set; }
        
    }
}