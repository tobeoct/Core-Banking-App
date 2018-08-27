using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;
using System.Data.Entity;
using WebApplication1.Dtos;
using AutoMapper;

namespace WebApplication1.Controllers.Api
{
    public class CustomersController : ApiController
    {
        private ApplicationDbContext _context;
        private string errorMessage = " ";
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
        [Route("api/Customers/CustomerAccounts")]
        public IHttpActionResult GetCustomerAccounts()
        {
           var customerAccounts = _context.CustomerAccounts.Include(g => g.Branch).Include(g => g.Customer).Include(g => g.AccountType).Include(g => g.LoanDetails).ToList();
         
            
            return Ok(customerAccounts);
        }

        // GET api/Customers/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/Customers/CreateCustomerAccount
        [AcceptVerbs("GET", "POST")]
        [Route("api/Customers/CreateCustomerAccount")]
        public HttpResponseMessage CreateCustomerAccount(CustomerAccountDto customerAccountDto)
        {
//            customerAccountDto.AccountNumber = customerAccountDto.AccountTypeId.ToString() + customerAccountDto.CustomerId.ToString();
            customerAccountDto.AccountBalance = 0;
            if (ValidateEntry(customerAccountDto)==true)
            {
               return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            var customerAccount = new CustomerAccount();
            customerAccount = Mapper.Map<CustomerAccountDto, CustomerAccount>(customerAccountDto);
            
            
            _context.CustomerAccounts.Add(customerAccount);
            _context.SaveChanges();
            List<string> message =new List<string>();
            message.Add("Account Created Successfully");
            message.Add(customerAccount.Id.ToString());
            return Request.CreateResponse(HttpStatusCode.OK,message ); 
        }

        // POST api/Customers/CheckEligibility
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Customers/CheckEligibility")]
        public HttpResponseMessage CheckIfEligibleToCollectLoan(LoanDetailsDto loanDetailsDto)
        {
            var customerLinkedAccount = _context.CustomerAccounts.Single(c => c.Id == loanDetailsDto.LinkedCustomerAccountId);
            if(customerLinkedAccount.LoanDetailsId!=null)
            {
                var linkedLoanDetail = _context.LoanDetails.SingleOrDefault(c => c.Id == customerLinkedAccount.LoanDetailsId);
                if (linkedLoanDetail.LoanAmount != 0)
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
            var customerAccounts = _context.CustomerAccounts.Where(c=>c.AccountTypeId!=loanAccountType.Id && c.CustomerId == customerAccountDto.CustomerId).ToList();
            if (customerAccounts != null)
            {
                foreach (var customerAccount in customerAccounts)
                {
                    if (customerAccount.IsClosed == true)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer Account has account but it is Closed");
                    }

                    if (customerAccount.AccountTypeId == savingsAccountType.Id && customerAccountDto.AccountTypeId == savingsAccountType.Id)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer already has a Savings Account");
                    }
                    if (customerAccount.AccountTypeId == currentAccountType.Id && customerAccountDto.AccountTypeId == currentAccountType.Id)
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
            loanDetailsDto.CustomerLoan = loanDetailsDto.LoanAmount;
            loanDetailsDto.InterestRate = interestRate;
            var loanDetails = new LoanDetails();

            loanDetails = Mapper.Map<LoanDetailsDto, LoanDetails>(loanDetailsDto);
            _context.LoanDetails.Add(loanDetails);
            
            var customerAccount = _context.CustomerAccounts.Single(c => c.Id == loanDetailsDto.LinkedCustomerAccountId);
            customerAccount.LoanDetailsId = loanDetails.Id;
            customerAccount.AccountBalance =customerAccount.AccountBalance+ loanDetails.LoanAmount;

            //Loan Account
            var accountType = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Loan Account"));
            //Customers Linked (Onyema George Loan)
                      
            _context.SaveChanges();
            
            //            if (customerLinkedAccount.AccountTypeId == accountType.Id)//if linked Account is a Loan Account
            //            {
            //                var initialLinkedLoanDetail = _context.LoanDetails.SingleOrDefault(c => c.LinkedCustomerAccountId == loanDetailsDto.LinkedCustomerAccountId);
            //                var customerAcc =
            //                    _context.CustomerAccounts.SingleOrDefault(c => c.LoanDetailsId == customerAccount.LoanDetailsId);
            //                customerAcc.AccountBalance = customerAcc.AccountBalance + loanDetailsDto.LoanAmount;
            //                initialLinkedLoanDetail.LoanAmount = loanDetailsDto.LoanAmount + initialLinkedLoanDetail.LoanAmount;
            //                _context.SaveChanges();
            //
            //            }

            List<string> message = new List<string>();
            message.Add("Loan Disbursed Successfully");
            message.Add(loanDetails.Id.ToString());
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [Route("api/Customers/Terms")]
        public HttpResponseMessage Terms(TermsDto termsDto)
        {
            var terms = new Terms();
            terms = Mapper.Map<TermsDto, Terms>(termsDto);


            _context.Terms.Add(terms);
            _context.SaveChanges();
            List<string> message = new List<string>();
            message.Add("Terms Added Successfully");
            message.Add(terms.Id.ToString());
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
            //var customers 
        }
        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}