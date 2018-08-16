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
        public AccountTypesController()
        {
            ViewBag.Message = RoleName.USER_NAME;
            _context= new ApplicationDbContext();
        }
        // GET: AccountTypes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SavingsAccConfig(string id)
        {
            var GLAccounts = _context.GlAccounts.ToList();
            var savingsAcc = _context.SavingsAccountTypes.SingleOrDefault(c => c.Id == id);
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
            var currentAcc = _context.CurrentAccountTypes.SingleOrDefault(c => c.Id == id);
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
            var loanAcc = _context.LoanAccountTypes.SingleOrDefault(c => c.Id == id);
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
                    SavingsAccountType = new SavingsAccountType()
                };
                return View("Index", viewModel);
            }

            var savingsAccountType =
                _context.SavingsAccountTypes.SingleOrDefault(c => c.Id == savingsViewModel.SavingsAccountType.Id);
            //Mapper.Map(generalLedgerCategoryViewModel, generalLedgerCategory);
            savingsAccountType.Id = savingsViewModel.SavingsAccountType.Id;
            savingsAccountType.GLAccountId = savingsViewModel.SavingsAccountType.GLAccountId;
            savingsAccountType.CreditInterestRate = savingsViewModel.SavingsAccountType.CreditInterestRate;
            savingsAccountType.MinimumBalance = savingsViewModel.SavingsAccountType.MinimumBalance;
            savingsAccountType.InterestExpenseGlAccountId =
                savingsViewModel.SavingsAccountType.GLAccountId;
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
                    LoanAccountType = new LoanAccountType()
                };
                return View("Index", viewModel);
            }

            var loanAccountType = _context.LoanAccountTypes.SingleOrDefault(c => c.Id == loanAccViewModel.LoanAccountType.Id);
            //Mapper.Map(generalLedgerCategoryViewModel, generalLedgerCategory);
            loanAccountType.Id = loanAccViewModel.LoanAccountType.Id;
            loanAccountType.GlAccountId = loanAccViewModel.LoanAccountType.GlAccountId;
            loanAccountType.DebitInterestRate = loanAccViewModel.LoanAccountType.DebitInterestRate;
            loanAccountType.InterestIncomeGLAccountId =
                loanAccViewModel.LoanAccountType.GlAccountId;
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
                    CurrentAccountType = new CurrentAccountType()
                };
                return View("Index", viewModel);
            }

            var currentAccountType = _context.CurrentAccountTypes.SingleOrDefault(c => c.Id == currentAccViewModel.CurrentAccountType.Id);
            //Mapper.Map(generalLedgerCategoryViewModel, generalLedgerCategory);
            currentAccountType.Id = currentAccViewModel.CurrentAccountType.Id;
            currentAccountType.GLAccountId = currentAccViewModel.CurrentAccountType.GLAccountId;
            currentAccountType.CreditInterestRate = currentAccViewModel.CurrentAccountType.CreditInterestRate;
            currentAccountType.MinimumBalance = currentAccViewModel.CurrentAccountType.MinimumBalance;
            currentAccountType.COTIncomeGLAccountId = currentAccViewModel.CurrentAccountType.GLAccountId;
            currentAccountType.InterestExpenseGLAccountId =
                currentAccViewModel.CurrentAccountType.GLAccountId;
            _context.SaveChanges();

            return RedirectToAction("Index", "AccountTypes");

        }
    }
}