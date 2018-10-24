using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trx.Messaging.Iso8583;
using WebApplication1.Models;

namespace WebApplication1.Processor
{
    public class TransactionLogger
    {
        public TransactionLogger()
        {
            
        }
        public void LogTransaction(Iso8583Message message, string type)
        {
            TransactionLog transactionLog = new TransactionLog();
            transactionLog.CardPan = message.Fields[2].Value.ToString();
            if (message.IsRequest())
            {
                transactionLog.ResponseCode = "";
            }
            else
            {
                transactionLog.ResponseCode = message.Fields[39].Value.ToString();

            }
            transactionLog.Amount =((float) message.Fields[4].Value )/ 100;
            transactionLog.Account1 = message.Fields[102].Value.ToString();
            if (message.Fields[103] != null)
            {
                transactionLog.Account2 = message.Fields[103].Value.ToString();
            }

            transactionLog.STAN = message.Fields[11].Value.ToString();
            transactionLog.TransactionDate = DateTime.Now;
            transactionLog.MTI = message.MessageTypeIdentifier.ToString();
            transactionLog.TypeOfEntry = type;
            //transactionLog.ResponseDescription = message.Fields[39].Value.ToString();

            var _context = new ApplicationDbContext();
            _context.TransactionLogs.Add(transactionLog);
            _context.SaveChanges();

        }
    }
}