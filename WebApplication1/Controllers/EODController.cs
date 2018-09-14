using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class EODController : Controller
    {
        // GET: EOD
        public ActionResult Index()
        {
            return View();
        }

        public class Indexes
        {
            public int Id { get; set; }
        }
//        public ActionResult ViewTransactions([FromUri]int id)
//        {
//            var index = new Indexes()
//            {
//                Id = id
//            };
//            return RedirectToAction("ViewTransaction","Customers",index);
//        }
    }
}