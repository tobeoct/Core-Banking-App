using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using Microsoft.AspNet.Identity.Owin;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class FinancialReportsController : Controller
    {
        private string userId = "";
        private ApplicationDbContext _context;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        //protected SignInManager<ApplicationSignInManager> SignInManager { get; set; }
        public FinancialReportsController()
        {
            _context = new ApplicationDbContext();

            userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);

            if (user != null)
            {
                RoleName.USER_NAME = user.FullName;
                RoleName.EMAIL = user.Email;
            }
        }

        // GET: FinancialReports
            public ActionResult Index()
        {
            return View();
        }
    }
}