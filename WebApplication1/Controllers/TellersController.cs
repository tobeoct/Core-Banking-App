using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class TellersController : Controller
    {

        private ApplicationDbContext _context;
        private string userId = "";
        protected UserManager<ApplicationUser> UserManager { get; set; }
        //protected SignInManager<ApplicationSignInManager> SignInManager { get; set; }
        public TellersController()
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
    

       
        // GET: Tellers
        public ActionResult Index()
        {
            var users = _context.Users.ToList();
            var accounts = _context.GlAccounts.Include(c=>c.GlCategories).ToList();
            var glCategories = _context.GlCategories.ToList();
            var categories = _context.Categories.ToList();
            var count = _context.Tellers.Count();
            List<GLAccount> tillAccounts = new List<GLAccount>();
            foreach (var category in categories)
            {
                if (category.Name.Equals("Asset"))
                {
                    foreach (var glCategory in glCategories)
                    {
                        if (glCategory.Name.Equals("Cash") && glCategory.CategoriesId == category.Id)

                            foreach (var account in accounts)
                            {
                                //                                if (account.GlCategoriesId == glCategory.Id && account.IsAssigned==false)
                                if (account.GlCategoriesId == glCategory.Id && account.IsAssigned == false)
                                {
                                    tillAccounts.Add(account);
                                }

                            }

                    }
                }

            }
            
            var viewModel = new TellerViewModel()
            {
                Users = users,
                Teller = new Teller(),
                TillAccounts = tillAccounts,
                count = count


            };
            
                
            
            return View("Index", viewModel);
            
        }
        public ActionResult Postings()
        {
            var customerAccounts = _context.CustomerAccounts.Include(c=>c.Customer).Include(c=>c.Branch).Include(c=>c.AccountType).ToList();
            var count = _context.TellerPostings.Count();
            var postingType1 = new PostingTypes();
            postingType1.Id = 1;
            postingType1.Name = "Deposit";
            var postingType2 = new PostingTypes();
            postingType2.Id = 2;
            postingType2.Name = "Withdrawal";

            var postingTypes = new List<string>
            {
                "Deposit",
                "Withdrawal"

            };

            var viewModel = new TellerPostingViewModel()
            {
                CustomerAccounts = customerAccounts,
                PostingType = postingTypes.ToList(),
                TellerPosting = new TellerPosting(),
                count = count
            };
            
                return View("TellerPosting", viewModel);
            
            
        }


        [HttpPost]
        public ActionResult AddTeller(TellerViewModel tellerViewModel)

        {
            tellerViewModel.Teller.TillAccountBalance = 0;
            var tillAccountId = tellerViewModel.Teller.TillAccountId;
            
            var users = _context.Users.ToList();
            var accounts = _context.GlAccounts.ToList();
            var glCategories = _context.GlCategories.ToList();
            var categories = _context.Categories.ToList();
            List<GLAccount> tillAccounts = new List<GLAccount>();
            foreach (var category in categories)
            {
                if (category.Name.Equals("Asset"))
                {
                    foreach (var glCategory in glCategories)
                    {
                        if (glCategory.Name.Equals("Cash") && glCategory.CategoriesId == category.Id)

                            foreach (var account in accounts)
                            {
                                //                                if (account.GlCategoriesId == glCategory.Id && account.IsAssigned == false)
                                if (account.GlCategoriesId == glCategory.Id && account.IsAssigned == false)
                                {
                                    tillAccounts.Add(account);
                                }

                            }

                    }
                }

            }
           
            if (CheckIfUserHasBeenAssignedTeller(tellerViewModel.Teller.UserTellerId))
            {
                var viewModel = new TellerViewModel()
                {
                    Users = users,
                    Teller = new Teller(),
                    TillAccounts = tillAccounts


                };
                return View("Index", viewModel);
            }

            tellerViewModel.Teller.IsAssigned = true;

           
            if (!ModelState.IsValid)
            {


                var viewModel = new TellerViewModel()
                {
                    Users = users,
                    Teller    = new Teller(),
                    TillAccounts = tillAccounts
                    

                };
                return View("Index", viewModel);
            }
            
            
            var teller = new Teller
            {
                IsAssigned = tellerViewModel.Teller.IsAssigned,
                TillAccountId = tellerViewModel.Teller.TillAccountId,
                UserTellerId = tellerViewModel.Teller.UserTellerId
            };
            _context.Tellers.Add(teller);
            _context.SaveChanges();

            MarkAsAssigned(tillAccountId);
            tellerViewModel.Users = users;
            tellerViewModel.TillAccounts = tillAccounts;
            tellerViewModel.Teller = new Teller();

            return RedirectToAction("Index", "Tellers"); 
        }

//        public ActionResult AddTellerPosting(TellerPostingViewModel tellerPostingViewModel)
//        {
//            if (!CheckIfTillAccountBalanceSufficient(tellerPostingViewModel.TellerPosting.Amount))
//            {
//
//            }
//            return View();
//        }

        [NonAction]
        public bool CheckIfTillAccountBalanceSufficient(long amount)
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
            var id = user.Id;
            var tillAccount = _context.Tellers
                .OrderByDescending(p => p.Id)
                .FirstOrDefault(c => c.UserTellerId == id);


            if (tillAccount.TillAccountBalance < amount)
            {
                return false;
            }
            return true;
        }

        [NonAction]
        public void MarkAsAssigned(int TillAccountId)
        {
            var glAccounts = _context.GlAccounts.SingleOrDefault(c => c.Id == TillAccountId);
            glAccounts.IsAssigned = true;
            _context.SaveChanges();
        }

        [NonAction]
        public bool CheckIfUserHasBeenAssignedTeller(string userId)
        {
            var tellers = _context.Tellers.Single(c => c.UserTellerId.Equals(userId));
            if (tellers != null)
            {
                return true;
            }
            
            return false;
        }

    }

}