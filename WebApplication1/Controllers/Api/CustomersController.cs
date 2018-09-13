using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Web;
using WebApplication1.Dtos;
using AutoMapper;

namespace WebApplication1.Controllers.Api
{
    //    [Authorize]
    public class CustomersController : ApiController
    {
        private ApplicationDbContext _context;
        private string errorMessage = "";

        public CustomersController()
        {

            _context = new ApplicationDbContext();
        }
        // GET api/Customers
        public IHttpActionResult GetCustomers()
        {
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }

        // GET api/Customers/CustomerAccounts
        [Route("api/Customers/CustomerAccounts")]
        public IHttpActionResult GetCustomerAccounts()
        {
            var customerAccounts = _context.CustomerAccounts.Include(g => g.Branch).Include(g => g.Customer).Include(g => g.AccountType).Include(g => g.LoanDetails).ToList();


            return Ok(customerAccounts);
        }



        /**
         * CUSTOMER FUNCTIONALITY
         */


        public class Index
        {
            public int Id { get; set; }
        }
        // POST api/Customers/AddCustomer
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Customers/AddCustomer")]
        public HttpResponseMessage AddCustomer(CustomerDto customerDto)
        {
            customerDto.Id = CBA.RandomString(9);
            ValidateCustomer(customerDto);
            if (!errorMessage.Equals(""))
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }
            var customer = new Customer()
            {
                Id = customerDto.Id,
                Address = customerDto.Address.ToString(),
                Email = customerDto.Email.ToString(),
                Gender = customerDto.Gender.ToString(),
                Name = customerDto.Name.ToString(),
                PhoneNumber = customerDto.PhoneNumber.ToString()
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "Customer has been added successfully");
        }

        // POST api/Customers/EditCustomer
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Customers/EditCustomer")]
        public HttpResponseMessage EditCustomer(Index index)
        {
            var id = index.Id.ToString().PadLeft(9, '0');
            var customerInDb = _context.Customers.SingleOrDefault(c => c.Id.Equals(id));
            var customerDto = new CustomerDto();
            if (customerInDb != null)
            {
                customerDto.Address = customerInDb.Address;
                customerDto.Email = customerInDb.Email;
                customerDto.Gender = customerInDb.Gender;
                customerDto.Name = customerInDb.Name;
                customerDto.PhoneNumber = customerInDb.PhoneNumber;

            }
            else
            {
                const string errorMsg = "No such Customer Exists";
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
            }

            // _context.SaveChanges();


            return Request.CreateResponse(HttpStatusCode.OK, customerDto);
        }

        // POST api/Customers/UpdateCustomer
        [AcceptVerbs("GET", "POST", "PUT")]
        [HttpPut]
        [Route("api/Customers/UpdateCustomer")]
        public HttpResponseMessage UpdateCustomer(CustomerDto customerDto)
        {
            var id = customerDto.Id.ToString().PadLeft(9, '0');
            var customerInDb = _context.Customers.SingleOrDefault(c => c.Id.Equals(id));
            if (customerInDb != null)
            {
                customerInDb.Address = customerDto.Address;
                customerInDb.Email = customerDto.Email;
                customerInDb.Gender = customerDto.Gender;
                customerInDb.Name = customerDto.Name;
                customerInDb.PhoneNumber = customerDto.PhoneNumber;
            }
            else
            {
                string errorMsg = "No such Customer Exists : " + id;
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
            }

            _context.SaveChanges();

            const string message = "Customer Info Updated Successfully";
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }


        /**
         * CUSTOMER ACCOUNT FUNCTIONALITY
         */

        // POST api/Customers/CreateCustomerAccount
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Customers/CreateCustomerAccount")]
        public HttpResponseMessage CreateCustomerAccount(CustomerAccountDto customerAccountDto)
        {
            // customerAccountDto.AccountNumber = customerAccountDto.AccountTypeId.ToString() + customerAccountDto.CustomerId.ToString();

            customerAccountDto.AccountBalance = 0;
            if (ValidateEntry(customerAccountDto) == true)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            var customerAccount = new CustomerAccount();
            customerAccount = Mapper.Map<CustomerAccountDto, CustomerAccount>(customerAccountDto);


            _context.CustomerAccounts.Add(customerAccount);
            _context.SaveChanges();

            var message = new List<string>
            {
                "Account Created Successfully",
                customerAccount.Id.ToString()
            };
            var loanAccount = _context.CustomerAccounts.SingleOrDefault(c => c.Id == customerAccount.Id);
            var loanDetail = _context.LoanDetails.SingleOrDefault(c => c.Id == customerAccount.LoanDetailsId);
            if (loanAccount != null && loanAccount.AccountTypeId == CBA.LOAN_ACCOUNT_TYPE_ID)
            {
                loanAccount.AccountBalance = loanAccount.AccountBalance + loanDetail.LoanAmount; // DEBIT
            }

            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        // POST api/Customers/EditCustomerAccount
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Customers/EditCustomerAccount")]
        public HttpResponseMessage EditCustomerAccount(Index index)
        {
            //  var id = index.Id.ToString().PadLeft(9, '0');
            var id = index.Id;
            var customerAccountInDb = _context.CustomerAccounts.SingleOrDefault(c => c.Id.Equals(id));
            var customerAccountDto = new CustomerAccountDto();
            if (customerAccountInDb != null)
            {
                customerAccountDto.Name = customerAccountInDb.Name;
                customerAccountDto.BranchId = customerAccountInDb.BranchId;

            }
            else
            {
                const string errorMsg = "No such Customer Account Exists";
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
            }

            return Request.CreateResponse(HttpStatusCode.OK, customerAccountDto);
        }

        // POST api/Customers/UpdateCustomerAccount
        [AcceptVerbs("GET", "POST", "PUT")]
        [HttpPut]
        [Route("api/Customers/UpdateCustomerAccount")]
        public HttpResponseMessage UpdateCustomerAccount(CustomerAccountDto customerAccountDto)
        {
            //  var id = index.Id.ToString().PadLeft(9, '0');
            var id = customerAccountDto.Id;
            var customerAccountInDb = _context.CustomerAccounts.SingleOrDefault(c => c.Id == id);
            if (customerAccountInDb != null)
            {
                customerAccountInDb.BranchId = customerAccountDto.BranchId;
                customerAccountInDb.Name = customerAccountDto.Name;
            }
            else
            {
                string errorMsg = "No such Customer Account Exists : " + id;
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
            }

            _context.SaveChanges();

            const string message = "Customer Account Info Updated Successfully";
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        // GET api/Customers/GetAccountStatus
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Customers/GetAccountStatus")]
        public HttpResponseMessage AccountStatus(Index index)
        {
            var customerAccount = _context.CustomerAccounts.SingleOrDefault(c => c.Id == index.Id);
            var stat = "";
            if (customerAccount == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No such Customer Account Exists");
            }
            var status = customerAccount.IsClosed;
            var message = "";
            if (status == true)
            {
                message = "<p><b>Account is CLOSED</b><p> Are you sure you want to <b>OPEN</b> <br/> <i>" + customerAccount.Name +
                          "</i> Account";
                stat = "Open Account";

            }
            else
            {
                message = "<p><b>Account is OPEN</b><p>Are you sure you want to <b>CLOSE</b> <br/><i>" + customerAccount.Name +
                          "</i> Account";
                stat = "Close Account";
            }

            var msg = new List<string>();
            msg.Add(message);
            msg.Add(stat);
            return Request.CreateResponse(HttpStatusCode.OK, msg);
        }

        // GET api/Customers/GetAccountStatus
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Customers/UpdateCustomerAccountStatus")]
        public HttpResponseMessage UpdateCustomerAccountStatus(Index index)
        {
            var customerAccount = _context.CustomerAccounts.SingleOrDefault(c => c.Id == index.Id);
            if (customerAccount == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No such Customer Account Exists");
            }
            var status = customerAccount.IsClosed;
            var message = "";
            if (status == true)
            {
                message = "Account Opened";
                customerAccount.IsClosed = false;
                _context.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, message);
            }
            message = "Account Closed";
            customerAccount.IsClosed = true;
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        // POST api/Customers/CheckEligibility
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Customers/CheckEligibility")]
        public HttpResponseMessage CheckIfEligibleToCollectLoan(LoanDetailsDto loanDetailsDto)
        {
            var customerLinkedAccount = _context.CustomerAccounts.Single(c => c.Id == loanDetailsDto.LinkedCustomerAccountId);
            if (customerLinkedAccount.LoanDetailsId != null)
            {
                var linkedLoanDetail = _context.LoanDetails.SingleOrDefault(c => c.Id == customerLinkedAccount.LoanDetailsId);
                if (linkedLoanDetail != null && linkedLoanDetail.LoanAmount != 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer yet to pay off previous loan");
                }



            }
            return Request.CreateResponse(HttpStatusCode.OK, "Ok");

        }

        // POST api/Customers/CheckIfCustomerHasAccount
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Customers/CheckIfCustomerHasAccount")]
        public HttpResponseMessage CheckIfCustomerHasAccount(CustomerAccountDto customerAccountDto)
        {
            var savingsAccountType = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Savings Account"));
            var loanAccountType = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Loan Account"));
            var currentAccountType = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Current Account"));
            var customerAccounts = _context.CustomerAccounts.Where(c => c.AccountTypeId != loanAccountType.Id && c.CustomerId == customerAccountDto.CustomerId).ToList();
            if (customerAccounts != null)
            {
                foreach (var customerAccount in customerAccounts)
                {
                    if (customerAccount.IsClosed == true)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer Account has account but it is Closed");
                    }

                    if (savingsAccountType != null && (customerAccount.AccountTypeId == savingsAccountType.Id && customerAccountDto.AccountTypeId == savingsAccountType.Id))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer already has a Savings Account");
                    }
                    if (currentAccountType != null && (customerAccount.AccountTypeId == currentAccountType.Id && customerAccountDto.AccountTypeId == currentAccountType.Id))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer already has a Current Account");
                    }

                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, "Ok");

        }

        // POST api/Customers/LoanDisbursement
        [Route("api/Customers/LoanDisbursement")]
        public HttpResponseMessage LoanDisbursement(LoanDetailsDto loanDetailsDto)
        {

            var loanAccountTypeConfig = _context.AccountTypes.Single(c => c.Name.Equals("Loan Account"));
            var interestRate = loanAccountTypeConfig.DebitInterestRate;
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            capitalAccount.AccountBalance = capitalAccount.AccountBalance - loanDetailsDto.LoanAmount; // DEBIT
            loanDetailsDto.CustomerLoan = loanDetailsDto.LoanAmount; // CREDIT

            // : Add financial entry
            AddToReport("Loan Disbursement", capitalAccount.Name, loanDetailsDto.CustomerLoanAccountName, loanDetailsDto.LoanAmount);

            loanDetailsDto.InterestRate = interestRate;
            var loanDetails = new LoanDetails();

            loanDetails = Mapper.Map<LoanDetailsDto, LoanDetails>(loanDetailsDto);
            _context.LoanDetails.Add(loanDetails);
            _context.SaveChanges();

            var customerAccount = _context.CustomerAccounts.ToList();
            var linkedCustomerAccount =
                customerAccount.SingleOrDefault(c => c.Id == loanDetailsDto.LinkedCustomerAccountId);
            //var glLoanAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.CUSTOMER_LOAN_ACCOUNT));
            if (linkedCustomerAccount != null)
            {
                linkedCustomerAccount.LoanDetailsId = loanDetails.Id;


                linkedCustomerAccount.AccountBalance = linkedCustomerAccount.AccountBalance + loanDetails.LoanAmount; // CREDIT

            }

            // : Financial Report Entry
            AddToReport("Loan Disbursement", loanDetailsDto.CustomerLoanAccountName, linkedCustomerAccount.Name, loanDetails.LoanAmount);

            _context.SaveChanges();
            List<string> message = new List<string>
            {
                "Loan Disbursed Successfully",
                loanDetails.Id.ToString()
            };
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [Route("api/Customers/Terms")]
        public HttpResponseMessage Terms(TermsDto termsDto)
        {
            termsDto.PaymentRate = (float)8.33;
            var terms = new Terms();
            terms = Mapper.Map<TermsDto, Terms>(termsDto);


            _context.Terms.Add(terms);
            _context.SaveChanges();
            List<string> message = new List<string>
            {
                "Terms Added Successfully",
                terms.Id.ToString()
            };
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [NonAction]
        public bool ValidateEntry(CustomerAccountDto customerAccountDto)
        {

            bool error = false;

            if (customerAccountDto.Name == null)
            {
                errorMessage = errorMessage + "Please Enter Account Name. ";
                error = true;

            }
            if (customerAccountDto.CustomerId == null)
            {
                errorMessage = errorMessage + "Please Select A Customer. ";
                error = true;
            }
            if (customerAccountDto.AccountTypeId == 0)
            {
                errorMessage = errorMessage + "Please Select an Account Type. ";
                error = true;
            }
            if (customerAccountDto.BranchId == 0)
            {
                errorMessage = errorMessage + "Please Select a Branch. ";
                error = true;
            }
            return error;

        }

        // Create Financial Report Entry
        [NonAction]
        public void AddToReport(string postingType, string debitAccountName, string creditAccountName, float amount)
        {
            var creditAmount = (float)System.Math.Round(amount, 2);
            var debitAmount = (float)System.Math.Round(amount, 2);
            var financialReportDto = new FinancialReportDto
            {
                PostingType = postingType,
                DebitAccount = debitAccountName,
                DebitAmount = debitAmount,
                CreditAccount = creditAccountName,
                CreditAmount = creditAmount,
                ReportDate = DateTime.Now
            };

            CBA.AddReport(financialReportDto);
        }

        [NonAction]
        public void ValidateCustomer(CustomerDto customerDto)
        {
            var phoneNumber = customerDto.PhoneNumber.ToString();
            var email = customerDto.Email.ToString();
            var name = customerDto.Name.ToString();
            string tel = phoneNumber.Substring(phoneNumber.Length - 9);
            //            if (!Regex.IsMatch(name, @"^[a-zA-Z]+$"))
            //            {
            //                errorMsg = errorMsg + "<br/>Enter only text ";
            //            }
            if (!IsValidEmail(email))
            {
                errorMessage = errorMessage + "<br/>Invalid Email Address ";
            }
            string[] names = SplitWhitespace(name);

            var phone = Regex.Match(phoneNumber, @"(.{9})\s*$");
            var userNumbers = _context.Customers
                .Where(c => c.PhoneNumber.Substring(c.PhoneNumber.Length - 9).Equals(tel)).ToList();
            var userEmails = _context.Customers
                .Where(c => c.Email.Equals(email)).ToList();
            var userNames = _context.Customers.Where(c => c.Name.Contains(name) || c.Name.Equals(name)).ToList();

            if (names.Length > 1)
            {
                if (userNames.Count <= 0)
                {
                    var firstName = names[0];
                    var lastName = names[1];
                    userNames = _context.Customers
                        .Where(c => c.Name.Contains(firstName) && c.Name.Contains(lastName)).ToList();
                }


            }

            if (userNames.Count > 0)
            {
                errorMessage = errorMessage + "<br/>Name already exists. ";
            }
            if (userNumbers.Count > 0)
            {
                errorMessage = errorMessage + "<br/>Phone Number already exists";
            }
            if (userEmails.Count > 0)
            {
                errorMessage = errorMessage + "<br/>Email Address already exists";
            }
        }

        [NonAction]
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
                   && Regex.IsMatch(email, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");
        }

        [NonAction]
        public static string[] SplitWhitespace(string input)
        {
            char[] whitespace = new char[] { ' ', '\t' };
            return input.Split(whitespace);
        }


    }
}