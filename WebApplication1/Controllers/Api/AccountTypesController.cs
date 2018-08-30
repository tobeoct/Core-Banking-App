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
    public class AccountTypesController : ApiController
    {
        private ApplicationDbContext _context;
        public AccountTypesController()
        {
            
            _context = new ApplicationDbContext();
        }

        [Route("api/Accounttypes/SavingsAccountType")]
        public IHttpActionResult GetSavingsAccountType()
        {
            var savingsAccType = _context.AccountTypes.Where(c => c.Name.Equals("Savings Account")).Include(c=>c.InterestExpenseGLAccount).ToList();

            return Ok(savingsAccType);
        }

        [Route("api/Accounttypes/LoanAccountType")]
        public IHttpActionResult GetLoanAccountType()
        {
            var loanAccType = _context.AccountTypes.Where(c => c.Name.Equals("Loan Account")).Include(c => c.InterestIncomeGLAccount).ToList();

            return Ok(loanAccType);
        }
        [Route("api/Accounttypes/CurrentAccountType")]
        public IHttpActionResult GetCurrentAccountType()
        {
            var currentAccType = _context.AccountTypes.Where(c => c.Name.Equals("Current Account")).Include(c => c.InterestExpenseGLAccount).Include(c => c.COTIncomeGLAccount).ToList();

            return Ok(currentAccType);
        }
    }
}