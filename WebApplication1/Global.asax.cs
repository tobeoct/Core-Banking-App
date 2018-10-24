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
            listener.StartListener("127.0.0.1", 87,"2");


        }
       


    }
}
