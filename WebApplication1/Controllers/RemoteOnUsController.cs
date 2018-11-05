using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;
namespace WebApplication1.Controllers
{
    public class RemoteOnUsController : Controller
    {
        private ApplicationDbContext _context;

       
        private string userId = "";
        protected UserManager<ApplicationUser> UserManager { get; set; }
       
        public RemoteOnUsController()
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

            ViewBag.Message = RoleName.USER_NAME;
        }
        // GET: RemoteOnUs
        public ActionResult Index()
        {
            var glAccounts = _context.GlAccounts.ToList();
            var account = _context.RemoteOnUsConfig.FirstOrDefault();
            var viewModel = new RemoteOnUsViewModel()
            {
                GLAccounts = glAccounts,
                GlAccountId= account!=null?account.GLAccountId:0
            };
            
            return View(viewModel);
        }
    }
}