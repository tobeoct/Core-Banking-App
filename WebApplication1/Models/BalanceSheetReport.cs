using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class BalanceSheetReport
    {
        public string AccountName { get; set; }
        public float DebitAmount { get; set; }
        public float CreditAmount { get; set; }
        public string PostingType { get; set; }
    }


}