using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace WebApplication1.Controllers
{
    public class BranchesController : Controller
    {
          
        private ApplicationDbContext _context;
        private string userId = "";
        protected UserManager<ApplicationUser> UserManager { get; set; }
        //protected SignInManager<ApplicationSignInManager> SignInManager { get; set; }
        public BranchesController()
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
        
        
        // GET: Branches
        
        public ActionResult Index()
        {
            ViewBag.Message = RoleName.USER_NAME;
            if (User.IsInRole(RoleName.USER_ROLE))
            {
                return View("ReadOnlyList",new Branch());
            }
           

            return View(new Branch());
        }

        [HttpPost]
        public ActionResult Create(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return View("Index",branch);
            }
            //            var branchInDb = _context.Branches.LastOrDefault();
            //            var id = branchInDb.Id + 1;
            //            branch.Id = id;
            // branch.Id = 0;
            _context.Branches.Add(branch);
            _context.SaveChanges();
            return RedirectToAction("Index", "Branches");

        }
    
}
}