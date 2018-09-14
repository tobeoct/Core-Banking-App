using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class FinancialReportViewModel
    {
        public FinancialReport FinancialReport { get; set; }
        public string Name { get; set; }
        public int Id{ get; set; }
    }
}