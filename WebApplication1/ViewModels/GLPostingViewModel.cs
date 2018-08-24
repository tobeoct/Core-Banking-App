using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class GLPostingViewModel
    {
        public GLPostings GlPostings { get; set; }

        public IEnumerable<GLAccount> GlAccounts { get; set; }
        public int count { get; set; }
    }
}