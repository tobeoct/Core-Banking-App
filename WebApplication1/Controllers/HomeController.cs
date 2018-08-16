using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
      
        public ActionResult Index()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = manager.FindById(User.Identity.GetUserId());
            ApplicationUser EmpUser = user;
            RoleName.USER_NAME = EmpUser.FullName;
            ViewBag.Message = RoleName.USER_NAME;
            if (User.IsInRole(RoleName.USER_ROLE))
            {
                return View("ReadOnly");
            }
            else if(User.IsInRole(RoleName.ADMIN_ROLE))
            {
                return View();
            }

           return RedirectToAction("Login", "Account");

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}