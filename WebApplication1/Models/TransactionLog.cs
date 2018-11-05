using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class TransactionLog
    {
        [Key]
        public long Id { get; set; }
        public string CardPan { get; set; }
        public string MTI { get; set; }
        public float Amount { get; set; }
        public string STAN { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Account1 { get; set; }
        public string Account2 { get; set; }
        public string ResponseCode { get; set; }
        public string TypeOfEntry { get; set; }
        public string Narration { get; set; }
        public bool? RemoteOnUs { get; set; }
    }
}