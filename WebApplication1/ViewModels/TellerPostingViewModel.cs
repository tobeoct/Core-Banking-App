using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class TellerPostingViewModel
    {
        public TellerPosting TellerPosting { get; set; }
        public IEnumerable<CustomerAccount> CustomerAccounts { get; set; }
        public IEnumerable<string> PostingType { get; set; }
    }
}