using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class CustomerViewModel
    {
        public Customer Customer { get; set; }
        public int count { get; set; }
    }
}