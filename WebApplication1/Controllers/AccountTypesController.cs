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
    public class AccountTypesController : Controller
    {
       
        private ApplicationDbContext _context;
        private string userId = "";
        protected UserManager<ApplicationUser> UserManager { get; set; }
        //protected SignInManager<ApplicationSignInManager> SignInManager { get; set; }
        public AccountTypesController()
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
        // GET: AccountTypes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SavingsAccConfig(string id)
        {
            var GLAccounts = _context.GlAccounts.ToList();
            var savingsAcc = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals( "Savings Account"));
            var viewModel = new SavingsViewModel()
            {
                GlAccounts = GLAccounts,
                SavingsAccountType = savingsAcc
            };
            return View("EditSavingsAccConfig", viewModel);
        }
        public ActionResult CurrentAccConfig(string id)
        {
            var GLAccounts = _context.GlAccounts.ToList();
            var currentAcc = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Current Account"));
            var viewModel = new CurrentAccViewModel()
            {
                GlAccounts = GLAccounts,
                CurrentAccountType = currentAcc
            };
            return View("EditCurrentAccConfig", viewModel);
        }
        public ActionResult LoanAccConfig(string id)
        {
            var GLAccounts = _context.GlAccounts.ToList();
            var loanAcc = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Loan Account"));
            var viewModel = new LoanAccViewModel()
            {
                GlAccounts = GLAccounts,
                LoanAccountType = loanAcc
            };
            return View("EditLoanAccConfig", viewModel);
        }

        [HttpPost]
        public ActionResult EditSavings(SavingsViewModel savingsViewModel)
        {
            if (!ModelState.IsValid)
            {
                var GLAccounts = _context.GlAccounts.ToList();
                var viewModel = new SavingsViewModel()
                {
                    GlAccounts = GLAccounts,
                    SavingsAccountType = new AccountType()
                };
                return View("Index", viewModel);
            }

            var savingsAccountType =
                _context.AccountTypes.SingleOrDefault(c => c.Name.Equals(savingsViewModel.SavingsAccountType.Name));
            //Mapper.Map(generalLedgerCategoryViewModel, generalLedgerCategory);
            savingsAccountType.Id = savingsViewModel.SavingsAccountType.Id;
            
            savingsAccountType.CreditInterestRate = savingsViewModel.SavingsAccountType.CreditInterestRate;
            savingsAccountType.MinimumBalance = savingsViewModel.SavingsAccountType.MinimumBalance;
            savingsAccountType.InterestExpenseGLAccountId =
                savingsViewModel.SavingsAccountType.InterestExpenseGLAccountId;
            _context.SaveChanges();
            return RedirectToAction("Index", "AccountTypes");
           
        }

        [HttpPost]
        public ActionResult EditLoan(LoanAccViewModel loanAccViewModel)
        {
            if (!ModelState.IsValid)
            {
                var GLAccounts = _context.GlAccounts.ToList();
                var viewModel = new LoanAccViewModel()
                {
                    GlAccounts = GLAccounts,
                    LoanAccountType = new AccountType()
                };
                return View("Index", viewModel);
            }

            var loanAccountType = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals( loanAccViewModel.LoanAccountType.Name));
            //Mapper.Map(generalLedgerCategoryViewModel, generalLedgerCategory);
            loanAccountType.Id = loanAccViewModel.LoanAccountType.Id;
            
            loanAccountType.DebitInterestRate = loanAccViewModel.LoanAccountType.DebitInterestRate;
            loanAccountType.InterestIncomeGLAccountId =
                loanAccViewModel.LoanAccountType.InterestIncomeGLAccountId;
            _context.SaveChanges();

            return RedirectToAction("Index", "AccountTypes");

        }

        [HttpPost]
        public ActionResult EditCurrent(CurrentAccViewModel currentAccViewModel)
        {
            if (!ModelState.IsValid)
            {
                var GLAccounts = _context.GlAccounts.ToList();
                var viewModel = new CurrentAccViewModel()
                {
                    GlAccounts = GLAccounts,
                    CurrentAccountType = new AccountType()
                };
                return View("Index", viewModel);
            }

            var currentAccountType = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals( currentAccViewModel.CurrentAccountType.Name));
            //Mapper.Map(generalLedgerCategoryViewModel, generalLedgerCategory);
            currentAccountType.Id = currentAccViewModel.CurrentAccountType.Id;
            
            currentAccountType.CreditInterestRate = currentAccViewModel.CurrentAccountType.CreditInterestRate;
            currentAccountType.MinimumBalance = currentAccViewModel.CurrentAccountType.MinimumBalance;
            currentAccountType.COTIncomeGLAccountId = currentAccViewModel.CurrentAccountType.COTIncomeGLAccountId;
            currentAccountType.InterestExpenseGLAccountId =
                currentAccViewModel.CurrentAccountType.InterestExpenseGLAccountId;
            _context.SaveChanges();

            return RedirectToAction("Index", "AccountTypes");

        }
    }
}