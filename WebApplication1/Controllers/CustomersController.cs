﻿using System;
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

            var branches = _context.Branches.ToList();
            var customers = _context.Customers.ToList();
            var loanDetails = _context.LoanDetails.ToList();
            var AccountTypes = _context.AccountTypes.ToList();

            var viewModel = new CustomerAccountViewModel
            {
                Branches = branches,
                Customers = customers,
                LoanDetails = loanDetails,
                AccountTypes =AccountTypes,
                CustomerAccount = new CustomerAccount()

            };
            return View("CustomerAccount", viewModel);
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
            customerAccount.AccountNumber = Int32.Parse( customerAccountViewModel.CustomerAccount.AccountTypeId.ToString()+customerId.ToString());
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

            return View("CustomerAccount", customerAccountViewModel);
        }

        [NonAction]
        public string getAutoGeneratedPassword()
        {
            var GLRanCode = RandomString(9);

            return GLRanCode;

        }
        [NonAction]
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "01234567890123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}