using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class UserAccountViewModel
    {
        public UserAccount UserAccount { get; set; }
        public IEnumerable<Branch> Branch { get; set; }
    }
}