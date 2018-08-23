using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class TellerViewModel
    {
        public IEnumerable<GLAccount> TillAccounts { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }
        public Teller Teller { get; set; }
    }
}