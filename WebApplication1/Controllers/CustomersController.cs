using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext _context;

        public CustomersController()
        {

            ViewBag.Message = RoleName.USER_NAME;
            _context = new ApplicationDbContext();
        }
        // GET: Customers
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Account()
        {
            ViewBag.Message = RoleName.USER_NAME;
            var branches = _context.Branches.ToList();
            var customers = _context.Customers.ToList();
            var loanDetails = _context.LoanDetails.ToList();
            var accountTypes = _context.AccountTypes.ToList();
            var customerAccounts = _context.CustomerAccounts.ToList();
            var count = customerAccounts.Count();
            var viewModel = new CustomerAccountViewModel
            {
                Branches = branches,
                Customers = customers,
                LoanDetails = loanDetails,
                AccountTypes =accountTypes,
                CustomerAccount = new CustomerAccount(),
                CustomerAccounts = customerAccounts,
                count = count

            };
            return View("CustomerAccount", viewModel);
        }

        public ActionResult Create(Customer customer)
        {
            customer.Id = CBA.RandomString(9);
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid";
                return View("Index");
            }
            
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return RedirectToAction("Index","Customers");
        }
        [HttpPost]
        public ActionResult CreateCustomerAccount(CustomerAccountViewModel customerAccountViewModel)
        {
            var branches = _context.Branches.ToList();
            var customers = _context.Customers.ToList();
            var loanDetails = _context.LoanDetails.ToList();
            var accountTypes = _context.AccountTypes.ToList();
            if (!ModelState.IsValid)
            {
                

                var viewModel = new CustomerAccountViewModel
                {
                    Branches = branches,
                    Customers = customers,
                    LoanDetails = loanDetails,
                    AccountTypes = accountTypes,
                    CustomerAccount = new CustomerAccount()

                };
                return View("CustomerAccount", viewModel);
            }

            var customerAccount = new CustomerAccount();
            var customerId = customerAccountViewModel.CustomerAccount.CustomerId;
            customerAccount.Id = customerAccountViewModel.CustomerAccount.Id;
            customerAccount.AccountNumber = customerAccountViewModel.CustomerAccount.AccountTypeId.ToString()+customerId.ToString();
            customerAccount.Name = customerAccountViewModel.CustomerAccount.Name;
            customerAccount.BranchId = customerAccountViewModel.CustomerAccount.BranchId;
            customerAccount.AccountTypeId = customerAccountViewModel.CustomerAccount.AccountTypeId;
            customerAccount.IsClosed = false;
            customerAccount.CustomerId = customerId;
            _context.CustomerAccounts.Add(customerAccount);
            _context.SaveChanges();

            customerAccountViewModel.Customers = customers;
            customerAccountViewModel.Branches = branches;
            customerAccountViewModel.AccountTypes = accountTypes;
            customerAccountViewModel.LoanDetails = loanDetails;
            customerAccountViewModel.CustomerAccount = new CustomerAccount();

            return RedirectToAction("Account", "Customers"); ;
        }

        
    }
}