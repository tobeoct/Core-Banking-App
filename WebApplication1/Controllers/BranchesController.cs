using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize]
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
            var count = _context.Branches.Count();
            var viewModel = new BranchViewModel()
            {
                Branch = new Branch(),
                count = count
            };

            if (User.IsInRole(RoleName.USER_ROLE) || User.IsInRole(RoleName.TELLER_ROLE))
            {
                return View("ReadOnlyList", viewModel);
            }
            else
            {
                return View(viewModel);
            }
//            return User.IsInRole(RoleName.USER_ROLE) ? View("ReadOnlyList", viewModel) : View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = RoleName.ADMIN_ROLE)]
        public ActionResult Create(BranchViewModel branchViewModel)
        {
            branchViewModel.Branch.DateCreated = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return View("Index", branchViewModel);
            }
            //            var branchInDb = _context.Branches.LastOrDefault();
            //            var id = branchInDb.Id + 1;
            //            branch.Id = id;
            // branch.Id = 0;
            var branch = new Branch()
            {
                Address = branchViewModel.Branch.Address,
                DateCreated = branchViewModel.Branch.DateCreated,
                Name = branchViewModel.Branch.Name
            };
            _context.Branches.Add(branch);
            _context.SaveChanges();
            return RedirectToAction("Index", "Branches");

        }
    
}
}