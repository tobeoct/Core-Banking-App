using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace WebApplication1.Controllers
{
    public class GeneralLedgersController : Controller
    {
        private ApplicationDbContext _context;
        private static Random random;

        private string userId = "";
        protected UserManager<ApplicationUser> UserManager { get; set; }
        //protected SignInManager<ApplicationSignInManager> SignInManager { get; set; }
        public GeneralLedgersController()
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
            random = new Random();


        }
        // GET: GeneralLedgers
        public ActionResult Index()
        {
            ViewBag.Message = RoleName.USER_NAME;
            var categories = _context.Categories.ToList();
            var viewModel = new GLCategoryViewModel
            {
                GlCategory = new GLCategory(),
                Categories = categories
            };
            return View("GLCategory", viewModel);
        }

        // GET: GeneralLedgers/Account
        public ActionResult Account()
        {
            ViewBag.Message = RoleName.USER_NAME;
            var GLCategory = _context.GlCategories.ToList();
            var branches = _context.Branches.ToList();
            var viewModel = new GLAccountViewModel()
            {
                GLCategories = GLCategory,
                Branch = branches
            };
            return View("GLAccount", viewModel);
        }

        public ActionResult Postings()
        {
            var _context = new ApplicationDbContext();
            ViewBag.Message = RoleName.USER_NAME;
            var glAccount = _context.GlAccounts.Include(c=>c.GlCategories).Include(c=>c.Branch).ToList();
            var count = _context.GlAccounts.Count();
            ViewBag.Count = count;
            var viewModel = new GLPostingViewModel()
            {
                GlAccounts =  glAccount,
                GlPostings = new GLPostings(),
                count = count
            };
            return View("GLPosting", viewModel);
        }

        //POST: GeneralLedgers/CreateCategory
        [HttpPost]
        public ActionResult CreateCategory(GLCategoryViewModel generalLedgerCategoryViewModel)
        {
            
            if (!ModelState.IsValid)
            {
                var GLCategory = _context.Categories.ToList();
                var viewModel = new GLCategoryViewModel
                {
                    GlCategory = new GLCategory(),
                    Categories = GLCategory
                };
                return View("GLCategory", viewModel);
            }

            var generalLedgerCategory = new GLCategory();
            //Mapper.Map(generalLedgerCategoryViewModel, generalLedgerCategory);
            generalLedgerCategory.CategoriesId = generalLedgerCategoryViewModel.GlCategory.CategoriesId;
            generalLedgerCategory.Description = generalLedgerCategoryViewModel.GlCategory.Description;
            generalLedgerCategory.MainAccountCategory = generalLedgerCategoryViewModel.GlCategory.MainAccountCategory;
            generalLedgerCategory.Id = generalLedgerCategoryViewModel.GlCategory.Id;
            _context.GlCategories.Add(generalLedgerCategory);
            _context.SaveChanges();
            return RedirectToAction("Index", "GeneralLedgers");
        }

        // POST: GeneralLedgers/CreateAccount
        [HttpPost]
        public ActionResult CreateAccount(GLAccountViewModel generalLedgerAccountViewModel)
        {
            if (!ModelState.IsValid)
            {
                var branches = _context.Branches.ToList();
                var GlCategories = _context.GlCategories.ToList();
                var viewModel = new GLAccountViewModel()
                {
                    Branch = branches,
                    GLCategories = GlCategories,
                    GlAccount = new GLAccount()
                };
                return View("GLAccount", viewModel);
            }

            var generalLedgerAccount = new GLAccount();
            //Mapper.Map(generalLedgerCategoryViewModel, generalLedgerCategory);
            generalLedgerAccount.GlCategoriesId = generalLedgerAccountViewModel.GlAccount.GlCategoriesId;
            generalLedgerAccount.BranchId = generalLedgerAccountViewModel.GlAccount.BranchId;
            generalLedgerAccount.Name = generalLedgerAccountViewModel.GlAccount.Name;
            generalLedgerAccount.Id = generalLedgerAccountViewModel.GlAccount.Id;
            generalLedgerAccount.Code = getCode(generalLedgerAccountViewModel.GlAccount.GlCategoriesId);
            _context.GlAccounts.Add(generalLedgerAccount);
            _context.SaveChanges();
            return RedirectToAction("Account", "GeneralLedgers");
        }

        public bool checkIfAlreadyInDb(string code)
        {
            var result = _context.GlAccounts.ToList();

            foreach (var account in result)
            {
                if (account.Code.Equals(code))
                {
                    return false;
                }
            }

            return true;
        }


        public string getCode(int id)
        {
            var GLRanCode = RandomString(15);
            var GLId = id;
            var GLAccountCode = Convert.ToString(GLId) + Convert.ToString(GLRanCode);
            Console.WriteLine(GLAccountCode);
            return GLAccountCode;

        }

        public static string RandomString(int length)
        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
