using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using Trx.Messaging.FlowControl;
using Trx.Messaging.Iso8583;
using WebApplication1.Models;
using Message = Trx.Messaging.Message;

namespace WebApplication1.Processor
{
    public class Processor
    {
        private static ApplicationDbContext _context;
        public Processor()
        {
            _context = new ApplicationDbContext();
        }
        public Iso8583Message ProcessTransaction(Iso8583Message message, ListenerPeer listener)
        {
            if (message != null)
            {
                if (!message.IsReversalOrChargeBack())
                {

                    message = CheckIfFeeApplies(message);
                    var fee = ExtractFee(message);
                   
                    var trxType = ExtractTransactionType(message);
                    var accountNumber = ExtractAccountNumber(message);
                    var amount = ExtractAmount(message);
                    var STAN = ExtractSTAN(message);
                    var RRN = ExtractRRN(message);
                    if (trxType.ToString().Equals(Codes.WITHDRAWAL) || trxType.ToString().Equals(Codes.PAYMENT))
                    {
                        if (accountNumber != null && amount != null)
                        {           
                            if (trxType.ToString().Equals(Codes.PAYMENT))
                            {

                            }
                            var responseCode = CBA.PerformDoubleEntry("Debit", accountNumber, (amount + fee), CheckIfRemote(message));
                            message = SetResponseMessage(message, responseCode);
                            message = SendResponseMessage(listener, message);
                            if (!message.Fields[39].Value.ToString().Equals(Codes.APPROVED))
                            {

                                Debug.WriteLine("Transaction Incomplete ...");

                            }
                            else
                            {
                                new TransactionLogger().LogTransaction(message, "Debit", "Request");
                                Debug.WriteLine("Transaction Complete ...");

                            }

                        }
                    }
                    else if (trxType.ToString().Equals(Codes.BALANCE_ENQUIRY))
                    {
                        var amountEnquired = CBA.BalanceEnquiry(accountNumber);
                        if (amountEnquired != null)
                        {
                            message.Fields.Add(54, amountEnquired);
                            message.Fields.Add(4, "0000000000");
                            SetResponseMessage(message, Codes.APPROVED);
                            message = SendResponseMessage(listener, message);
                            new TransactionLogger().LogTransaction(message, null, "Balance Enquiry");
                        }


                    }
                    return message;
                }

                if (message.IsReversalOrChargeBack())
                {
                    message = PerformReversal(message, listener);
                }

            }
            return message;
        }

        private static Iso8583Message PerformReversal(Iso8583Message message, ListenerPeer listener)
        {
            var STAN = ExtractSTAN(message);
            message = SetFee(message, 0);
            var transactionsToReverse = _context.TransactionLogs.Where(c => c.STAN.Equals(STAN) && !c.Narration.Equals("Reversal")).ToList();
            try
            {
                string responseCode = null;

                if (transactionsToReverse != null || transactionsToReverse.Count > 0)
                {
                    foreach (var transaction in transactionsToReverse)
                    {
                        if (!transaction.Narration.Equals("Balance Enquiry"))
                        {
                            if (transaction.TypeOfEntry.Equals("Debit"))
                            {
                                if (transaction.Account2 == null)
                                {
                                    bool remote1 =false;
                                    if(transaction.RemoteOnUs==true)
                                    {
                                        remote1 = true;
                                    }
                                    
                                    responseCode = CBA.PerformDoubleEntry("Credit", transaction.Account1, transaction.Amount, remote1);
                                    var isoAmt = Convert.ToDouble(FormatTo2Dp(Convert.ToDecimal(transaction.Amount))) * 100;
                                    message.Fields.Add(4, isoAmt.ToString().PadLeft(10, '0'));
                                    new TransactionLogger().LogTransaction(message, "Credit", "Reversal");
                                    transaction.STAN = transaction.STAN + "R";

                                    continue;
                                }

                            }
                            bool remote = false;
                            if (transaction.RemoteOnUs == true)
                            {
                                remote = true;
                            }
                            responseCode = CBA.PerformDoubleEntry("Debit", transaction.Account1, transaction.Amount, remote);
                            var isoAmount = Convert.ToDouble(FormatTo2Dp(Convert.ToDecimal(transaction.Amount))) * 100;
                            message.Fields.Add(4, isoAmount.ToString().PadLeft(10, '0'));
                            new TransactionLogger().LogTransaction(message, "Debit", "Reversal");
                            transaction.STAN = transaction.STAN + "R";
                        }

                    }
                    _context.SaveChanges();
                    message = SetResponseMessage(message, responseCode);
                    message = SendResponseMessage(listener, message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;

            }
            return message;
        }

        private static Iso8583Message SetFee(Iso8583Message message, double? fee)
        {
            string feeAmount = ConvertFeeToISOFormat(fee);
            message.Fields.Add(28, feeAmount);
            return message;
        }

        private static string ConvertFeeToISOFormat(double? fee)
        {
            //takes in 40naira C0000004000
            double? feeInMinorDenomination = fee * 100; //in Kobo

            StringBuilder feeStringBuilder = new StringBuilder(Convert.ToInt32(feeInMinorDenomination).ToString());
            string padded = feeStringBuilder.ToString().PadLeft(8, '0');
            feeStringBuilder.Replace(feeStringBuilder.ToString(), padded);
            feeStringBuilder.Insert(0, 'C');

            return feeStringBuilder.ToString();
        }

        public static string FormatTo2Dp(decimal myNumber)
        {
            // Use schoolboy rounding, not bankers.
            myNumber = Math.Round(myNumber, 2, MidpointRounding.AwayFromZero);

            return string.Format("{0:0.00}", myNumber);
        }

        private static double ExtractFee(Iso8583Message message)
        {
            double fee = 0;
            fee = ConvertFeeFromISOFormat(message.Fields[28].Value.ToString());
            Debug.WriteLine("Fee : " + fee);
            return fee / 100;
        }
        private static string ExtractChannel(Iso8583Message message)
        {
            string channelCode = message.Fields[41].Value.ToString().Substring(0, 1);
            return channelCode;
        }
        private static string ExtractTransactionType(Iso8583Message message)
        {
            string trxType = message.Fields[3].Value.ToString().Substring(0, 2);
            Debug.WriteLine("Trx Type : " + trxType);
            return trxType;
        }

        private static string ExtractSTAN(Iso8583Message message)
        {
            string STAN = message.Fields[11].Value.ToString();
            Debug.WriteLine("STAN : " + STAN);
            return STAN;
        }

        private static string ExtractRRN(Iso8583Message message)
        {
            string RRN = message.Fields[37].Value.ToString();
            Debug.WriteLine("RRN : " + RRN);
            return RRN;
        }

        private static double ExtractAmount(Iso8583Message message)
        {
            double amount = 0;
            amount = ConvertIsoAmountToDouble(message.Fields[4].Value.ToString());
            Debug.WriteLine("Amount : " + amount);
            return amount;
        }

        private static string ExtractAccountNumber(Iso8583Message message)
        {
            string accountNumber = "";
            accountNumber = message.Fields[102].Value.ToString();
            Debug.WriteLine("Account Number : " + accountNumber);
            return accountNumber;
        }

        private static double ConvertFeeFromISOFormat(string ISOFee)
        {
            string[] feeArr = ISOFee.Split('C');
            return Double.Parse(feeArr[1]);
        }

        private static double ConvertIsoAmountToDouble(string amountIsoFormat)
        {
            double amount = Convert.ToDouble(amountIsoFormat) / 100;
            return amount;
        }

        private static Iso8583Message SetResponseMessage(Iso8583Message message, string responseCode)
        {
            if (message.IsRequest())
            {
                message.SetResponseMessageTypeIdentifier();
            }

            message.Fields.Add(39, responseCode);
            return message;
        }

        private static Iso8583Message SendResponseMessage(ListenerPeer listenerPeer, Iso8583Message message)
        {
            bool needReversal = false;
            int maxNumberOfEntries = 3;
            Message response = null;
            int serverTimeOut = 60000;

            PeerRequest request = null;
            try
            {
                if (listenerPeer.IsConnected)
                {
                    request = new PeerRequest(listenerPeer, message);
                    request.Send();
                    request.WaitResponse(serverTimeOut);
                    if (request.Expired)
                    {
                        //logger.Log("Connection timeout.");
                        needReversal = true;
                        return SetResponseMessage(message, "68"); //Response received too late

                    }

                    if (request != null)
                    {
                        response = request.ResponseMessage;
                        //logger.Log("Message Recieved From FEP");

                    }

                    //                    
                    //request.MarkAsExpired();   //uncomment to test timeout
                    return response as Iso8583Message;

                }
                else
                {
                    //logger.Log("Could not connect to Sink Node");
                    //clientPeer.Close();
                    listenerPeer.Close();
                    Console.WriteLine("Client Peer is not Connected");
                    return SetResponseMessage(message, "91");
                }

                //clientPeer.Close();
            }
            catch (Exception e)
            {
                //logger.Log("An error occured " + e.Message);
                return SetResponseMessage(message, "06");
            }
        }

        private static Iso8583Message CheckIfFeeApplies(Iso8583Message message)
        {
            if (ExtractChannel(message).Equals(Codes.ATM))
            {
                var accountNumber = ExtractAccountNumber(message);
                var transactionType = ExtractTransactionType(message);
                var logs = _context.TransactionLogs.Where(c => c.Account1 == accountNumber && c.Narration.Equals("Request") && c.ResponseCode.Equals(Codes.APPROVED)).ToList();
                var count = logs.Count % 3;
                if (count != 0)
                {
                    message.Fields.Add(28, "C00000000");
                }
            }
            return message;
        }
        private static string ExtractTerminalID(Iso8583Message message)
        {
            var field41 = message.Fields[41].Value.ToString();
            var terminalID = field41.Substring(1, field41.Length - 1);
            return terminalID;

        }
        private static bool CheckIfRemote(Iso8583Message message)
        {
            var terminalID = ExtractTerminalID(message);
            var terminal = _context.ATMTerminals.SingleOrDefault(c => c.TerminalID == terminalID);
            if(terminal==null)
            {
                Debug.WriteLine("Not Remote");
                return false;
               
            }
            Debug.WriteLine("Remote");
            return true;
        }
    }
}