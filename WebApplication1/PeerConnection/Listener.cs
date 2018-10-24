using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Trx.Messaging;
using Trx.Messaging.Channels;
using Trx.Messaging.FlowControl;
using Trx.Messaging.Iso8583;

namespace WebApplication1.PeerConnection
{
    public class Listener
    {
        private static Processor.Processor processor;

        public Listener()
        {
            processor = new Processor.Processor();
        }

        public void StartListener(string ipAddress,int port, string id)
        {
            TcpListener tcpListener = new TcpListener(port);
            tcpListener.LocalInterface = ipAddress;
            ListenerPeer listener = new ListenerPeer(id,
                new TwoBytesNboHeaderChannel(new Iso8583Ascii1987BinaryBitmapMessageFormatter()),
                new BasicMessagesIdentifier(11, 41),
                tcpListener);
            //            ListenerPeer
            listener.Error += (sender, e) => ErrorListener(sender, e);
            listener.Receive += (sender, e) => ReceiveListener(sender, e);
            listener.Disconnected += (sender, e) => DisconnectListener(sender);
            listener.Connect();
            Debug.WriteLine($"Now listening on IP {ipAddress} port {port}");
        }

        private static void ErrorListener(object sender, Trx.Utilities.ErrorEventArgs e)
        {
            var i = 0;
        }

        private static void ReceiveListener(object sender, ReceiveEventArgs e
            )
        {
            
            Debug.WriteLine("Message : " + e.Message);
            Debug.WriteLine("Fee : " + e.Message.Fields[28].Value.ToString());
             processor.ProcessTransaction(e.Message as Iso8583Message,sender as ListenerPeer);
        }

        private static void DisconnectListener(object sender)
        {
           
            Console.WriteLine("Reconnecting...");
            (sender as ListenerPeer).Connect();
        }
    }
}