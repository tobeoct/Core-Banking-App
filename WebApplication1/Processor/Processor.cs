using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public void ProcessTransaction(Iso8583Message message, ListenerPeer listener)
        {
            if (message != null)
            {
                var fee = ExtractFee(message);
                var trxType = ExtractTransactionType(message);
                var accountNumber = ExtractAccountNumber(message);
                var amount = ExtractAmount(message);
                var STAN = ExtractSTAN(message);
                var RRN = ExtractRRN(message);
                if (trxType.ToString().Equals(Codes.WITHDRAWAL))
                {
                    if (accountNumber != null && amount != null)
                    {
                       
                        var responseCode = CBA.PerformDoubleEntry("Debit", accountNumber, (amount + fee/100));
                        SetResponseMessage(message, responseCode);
                       message = SendResponseMessage(listener, message);
                        if (!message.Fields[39].Value.ToString().Equals(Codes.APPROVED))
                        {
                            new TransactionLogger().LogTransaction(message,"Debit");
                            Debug.WriteLine("Transaction Incomplete ...");
                        }
                        else
                        {
                            Debug.WriteLine("Transaction Complete ...");
                        }

                    }
                }
            }
        }

        private static double ExtractFee(Iso8583Message message)
        {
            double fee = 0;
            fee = ConvertFeeFromISOFormat(message.Fields[28].Value.ToString());
            Debug.WriteLine("Fee : " + fee);
            return fee;
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
            message.SetResponseMessageTypeIdentifier();
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

                    listenerPeer.Close();
                    //request.MarkAsExpired();   //uncomment to test timeout
                    return response as Iso8583Message;

                }
                else
                {
                    //logger.Log("Could not connect to Sink Node");
                    //clientPeer.Close();
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
    }
}