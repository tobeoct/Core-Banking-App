using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class GLPostingDto
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "GLAccount To Debit")]
        public int GlDebitAccountId { get; set; }     

        [Display(Name = "Debit Amount")]
        public float DebitAmount { get; set; }

        [Display(Name = "Narration")]
        public string DebitNarration { get; set; }

        [Display(Name = "GLAccount To Credit")]
        public int GlCreditAccountId { get; set; }

        [Display(Name = "Credit Amount")]
        public float CreditAmount { get; set; }

        [Display(Name = "Narration")]
        public string CreditNarration { get; set; }

        public DateTime? TransactionDate { get; set; }
    }
}