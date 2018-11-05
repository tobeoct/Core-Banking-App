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
using WebApplication1.PeerConnection;
using WebApplication1.Services;
using WebApplication4.App_Start;
using TcpListener = System.Net.Sockets.TcpListener;
using WebApplication1.Models;

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
            var listener = new Listener();
            var _context = new ApplicationDbContext();
            var atmTerminals = _context.ATMTerminals.ToList();
            var dataMessage = new DataMessages()
            {
                ATMTerminals = atmTerminals,

            };


            var messageToSend = SerializeObject<DataMessages>(dataMessage);
            try
            {
                ConnectToServer("127.0.01", 8080, messageToSend);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            listener.StartListener("127.0.0.1", 87,"2");
           
           
        }


        public static void ConnectToServer(string serverIp, int serverPort, string message)
        {

            TcpClient client = new TcpClient();
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            Int64 bytecount = Encoding.ASCII.GetByteCount(message);
            byte[] sendData = new byte[bytecount + 1];
            sendData = Encoding.ASCII.GetBytes(message + ";");
            Debug.WriteLine("Connecting to Server");

            client.Connect(serverEndPoint);
            byte[] receivedBuffer = new byte[2000000];
            NetworkStream stream = client.GetStream();
            Debug.WriteLine("Connected to Server");
            stream.Write(sendData, 0, sendData.Length);
            //string line = null;
            //var reader = new StreamReader(stream);
            //while ((line = reader.ReadLine()) != null)
            //{
            //    Console.WriteLine(line);
            //}

            // var writer = new StreamWriter(stream); var reader = new StreamReader(stream); writer.WriteLine("GET /"); writer.Flush(); string line = null; while ((line = reader.ReadLine()) != null) Console.WriteLine(line); }
            stream.Close();
            client.Close();
        }
        public static string SerializeObject<T>(T toSerialize)
        {


            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
}
