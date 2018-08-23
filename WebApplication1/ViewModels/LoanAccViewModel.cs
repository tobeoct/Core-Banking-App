using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class LoanAccViewModel
    {
        public IEnumerable<GLAccount> GlAccounts { get; set; }
        public AccountType LoanAccountType { get; set; }
    }
}