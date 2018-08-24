using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class CustomerAccountViewModel
    {

        public CustomerAccount CustomerAccount { get; set; }
        public IEnumerable<CustomerAccount> CustomerAccounts { get; set; }
        public IEnumerable<AccountType> AccountTypes { get; set; }
        public IEnumerable<Branch> Branches { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<LoanDetails> LoanDetails { get; set; }
    }
}