using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class CurrentAccViewModel
    {
        public CurrentAccountType CurrentAccountType { get; set; }
        public IEnumerable<GLAccount> GlAccounts { get; set; }
    }
}