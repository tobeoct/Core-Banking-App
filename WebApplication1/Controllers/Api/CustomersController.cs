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
        // GET api/<controller>
        public IHttpActionResult GetCustomers()
        {
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }
        [Route("api/Customers/CustomerAccounts")]
        public IHttpActionResult GetCustomerAccounts()
        {
           var customerAccount = _context.CustomerAccounts.Include(g => g.Branch).Include(g => g.Customer).Include(g => g.AccountType).Include(g => g.LoanDetails).ToList();

            return Ok(customerAccount);
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
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

        [Route("api/Customers/LoanDisbursement")]
        public HttpResponseMessage LoanDisbursement(LoanDetailsDto loanDetailsDto)
        {
            
            var loanDetails = new LoanDetails();
            loanDetails = Mapper.Map<LoanDetailsDto, LoanDetails>(loanDetailsDto);
            _context.LoanDetails.Add(loanDetails);
            
            var customerAccount = _context.CustomerAccounts.Single(c => c.Id == loanDetailsDto.LinkedCustomerAccountId);
            customerAccount.LoanDetailsId = loanDetails.Id;
            customerAccount.AccountBalance =customerAccount.AccountBalance+ loanDetails.LoanAmount;
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "Loan Disbursed Successfully");
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