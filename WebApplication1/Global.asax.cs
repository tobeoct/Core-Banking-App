using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Services.Description;
using System.Xml.Serialization;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using Trx.Messaging;
using Trx.Messaging.Channels;
using Trx.Messaging.FlowControl;
using Trx.Messaging.Iso8583;
using WebApplication1.Services;
using WebApplication4.App_Start;
using TcpListener = System.Net.Sockets.TcpListener;

namespace WebApplication1
{
    [EnableCors("*", "*", "*")]
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {

          

            Mapper.Initialize(c => c.AddProfile<MappingProfile>());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //            CreateServer("127.0.0.1", 87);
//           
//            AsyncService service = new AsyncService("127.0.0.1", 87);
//            service.Run();
            TRXPeerSetup("127.0.0.1", 87);


        }
        public static void TRXPeerSetup(string address, int port)
        {
            Trx.Messaging.FlowControl.TcpListener tcpListener = new Trx.Messaging.FlowControl.TcpListener(port);
            tcpListener.LocalInterface = address;
            ListenerPeer listener = new ListenerPeer("1",
                new TwoBytesNboHeaderChannel(new Iso8583Ascii1987BinaryBitmapMessageFormatter()),
                new BasicMessagesIdentifier(11, 41),
                tcpListener);

            listener.Receive += (sender, e) => ReceiveListener(sender, e);
            listener.Disconnected += (sender, e) => DisconnectListener(sender,listener);
            listener.Connect();
            Debug.WriteLine($"Now listening to IP {address} port {port}");
        }
        private static void DisconnectListener(object sender, ListenerPeer listener)
        {
            //            throw new NotImplementedException();
            Debug.WriteLine("Disconnected");
            
            (sender as ListenerPeer).Connect();
        }

        private static void ReceiveListener(object sender, ReceiveEventArgs e)
        {
            Debug.WriteLine(e.Message);
            //            throw new NotImplementedException();
        }
        public static bool CreateServer( string address, int port)
        {
            TcpListener server = new TcpListener(IPAddress.Parse(address), port);
            TcpClient client = default(TcpClient);
            try
            {
                server.Start();
               
                Console.WriteLine("Server Started ...");
                while (true)
                {
                    client = server.AcceptTcpClient();
                    byte[] receivedBuffer = new byte[2000000];
                    NetworkStream stream = client.GetStream();
                    stream.Read(receivedBuffer, 0, receivedBuffer.Length);
                    StringBuilder msg = new StringBuilder();
                    //                    string msg = Encoding.ASCII.GetString(receivedBuffer, 0, receivedBuffer.Length);
                    foreach (byte b in receivedBuffer)
                    {
                        if (b.Equals(59))
                        {
                            break;
                        }
                        else
                        {
                            msg.Append(Convert.ToChar(b).ToString());
                        }
                    }
                    Console.WriteLine("THE MESSAGE \n" + msg.ToString());
                    Console.WriteLine("THE LENGTH \n" + msg.Length);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;

            }


        }


    }
}
