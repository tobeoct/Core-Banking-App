using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;
using System.Data.Entity;
namespace WebApplication1.Controllers.Api
{
    public class CustomersController : ApiController
    {
        private ApplicationDbContext _context;
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
        public void Post([FromBody]string value)
        {
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