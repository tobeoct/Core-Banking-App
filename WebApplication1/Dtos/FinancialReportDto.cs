using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class FinancialReportDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Debit Account")]
        public string DebitAccount { get; set; }

        [Display(Name = "Debit Amount")]
        public float DebitAmount { get; set; }

        [Display(Name = "Credit Account")]
        public string CreditAccount { get; set; }

        [Display(Name = "Credit Amount")]
        public float CreditAmount { get; set; }

        [Display(Name = "Posting Type")]
        public string PostingType { get; set; }

       

        [Required]
        [Display(Name = "Report Date")]
        public DateTime? ReportDate { get; set; }
    }
}