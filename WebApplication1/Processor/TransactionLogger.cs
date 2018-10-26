using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public void LogTransaction(Iso8583Message message, string type, string narration)
        {
            TransactionLog transactionLog = new TransactionLog();
            double fee = ExtractFee(message);

            transactionLog.CardPan = message.Fields[2].Value.ToString();
            if (message.IsRequest())
            {
                transactionLog.ResponseCode = "";
            }
            else
            {
                transactionLog.ResponseCode = message.Fields[39].Value.ToString();

            }
            transactionLog.Amount =(float)(ExtractAmount(message)+ fee);
            transactionLog.Account1 = message.Fields[102].Value.ToString();
            if (message.Fields[103] != null)
            {
                transactionLog.Account2 = message.Fields[103].Value.ToString();
            }

            transactionLog.STAN = message.Fields[11].Value.ToString();
            transactionLog.TransactionDate = DateTime.Now;
            transactionLog.MTI = message.MessageTypeIdentifier.ToString();
            transactionLog.TypeOfEntry = type;
            transactionLog.Narration = narration;
            //transactionLog.ResponseDescription = message.Fields[39].Value.ToString();

            var _context = new ApplicationDbContext();
            _context.TransactionLogs.Add(transactionLog);
            _context.SaveChanges();

        }
        private static double ConvertFeeFromISOFormat(string ISOFee)
        {
            string[] feeArr = ISOFee.Split('C');
            return Double.Parse(feeArr[1]);
        }
        private static double ExtractFee(Iso8583Message message)
        {
            double fee = 0;
            fee = ConvertFeeFromISOFormat(message.Fields[28].Value.ToString());
            
            return fee/100;
        }
        private static double ExtractAmount(Iso8583Message message)
        {
            double amount = 0;
            amount = ConvertIsoAmountToDouble(message.Fields[4].Value.ToString());
            Debug.WriteLine("Amount : " + amount);
            return amount;
        }
        private static double ConvertIsoAmountToDouble(string amountIsoFormat)
        {
            double amount = Convert.ToDouble(amountIsoFormat) / 100;
            return amount;
        }

    }
}