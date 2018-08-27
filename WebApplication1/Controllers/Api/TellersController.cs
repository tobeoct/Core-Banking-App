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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using WebApplication1.ViewModels;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace WebApplication1.Controllers.Api
{
    public class TellersController : ApiController
    {
        private ApplicationDbContext _context;
        private string userId;
        private string errorMessage = "";

        public TellersController()
        {
            _context = new ApplicationDbContext();
            
        }
        //GET api/Tellers
        [Route("api/Tellers")]
        public IHttpActionResult GetTellers()
        {
            var tellersDto = _context.Tellers.Include(c=>c.UserTeller).Include(c=>c.TillAccount).ToList();

            return Ok(tellersDto);
        }

        //GET api/Tellers
        [Route("api/Tellers/TellerPostings")]
        public IHttpActionResult GetTellerPostings()
        {
            var tellerPostingsDto = _context.TellerPostings.Include(c => c.CustomerAccount).ToList();

            return Ok(tellerPostingsDto);
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Tellers/AssignTeller")]
        public HttpResponseMessage AssignTeller(TellerDto tellerDto)
        {
            tellerDto.TillAccountBalance = 0;
            var tillAccountId = tellerDto.TillAccountId;

            
            if (CheckIfUserHasBeenAssignedTeller(tellerDto.UserTellerId))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User Already Assigned A Till Account");
            }

            var teller = new Teller();
            //teller = Mapper.Map<TellerDto, Teller>(tellerDto);
            teller.IsAssigned = tellerDto.IsAssigned;
            teller.TillAccountBalance = tellerDto.TillAccountBalance;
            teller.TillAccountId = tellerDto.TillAccountId;
            teller.UserTellerId = tellerDto.UserTellerId;
            _context.Tellers.Add(teller);
            _context.SaveChanges();
            MarkAsAssigned(tillAccountId);
            return Request.CreateResponse(HttpStatusCode.OK, "Teller Assigned Successfully");
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Tellers/AddTellerPosting")]
        public HttpResponseMessage AddTellerPosting(TellerPostingDto tellerPostingDto)
        {     

            tellerPostingDto.TransactionDate = DateTime.Now;  
            
            var tellerPosting = new TellerPosting();
            tellerPosting = Mapper.Map<TellerPostingDto, TellerPosting>(tellerPostingDto);           
            
            _context.TellerPostings.Add(tellerPosting);
            _context.SaveChanges();

            EnforceDoubleEntry(tellerPostingDto.Amount, tellerPosting.CustomerAccountId, tellerPostingDto.PostingType);

            var financialReportDto = new FinancialReportDto();
            financialReportDto.PostingType ="Teller Posting";
            if (tellerPostingDto.PostingType == "Deposit")
            {
                financialReportDto.CreditAccount = GetCustomerAccountName(tellerPostingDto.CustomerAccountId);
                financialReportDto.CreditAmount = tellerPostingDto.Amount;
                financialReportDto.DebitAccount = GetTillAccountName();
                financialReportDto.DebitAmount = tellerPostingDto.Amount;
            }
            else
            {
                financialReportDto.DebitAccount = GetCustomerAccountName(tellerPostingDto.CustomerAccountId);
                financialReportDto.DebitAmount = tellerPostingDto.Amount;
                financialReportDto.CreditAccount = GetTillAccountName();
                financialReportDto.CreditAmount = tellerPostingDto.Amount;
            }
            
            financialReportDto.ReportDate =  DateTime.Now;
            if(!errorMessage.Equals(""))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }
            CBA.AddReport(financialReportDto);

            return Request.CreateResponse(HttpStatusCode.OK, "Teller Posted Successfully");
           

        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Tellers/ValidationChecks")]
        public HttpResponseMessage ValidationChecks(TellerPostingDto tellerPostingDto)
        {

            if (CheckIfCustomerAccountIsClosed(tellerPostingDto.CustomerAccountId) == true)
            {
                errorMessage = errorMessage + "Customer Account is Closed";

            }

            if (tellerPostingDto.PostingType == "Withdrawal")
            {
                if (!CheckCustomerAccountBalance(tellerPostingDto.CustomerAccountId, tellerPostingDto.Amount))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
                }
                if (!CheckTillBalance(tellerPostingDto.Amount))
                {
                    errorMessage = errorMessage + "Insufficient Till Account Balance";

                }
            }

            if (!errorMessage.Equals(""))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Proceed");

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

        [NonAction]
        public bool CheckIfCustomerAccountIsClosed(int customerAccountId)
        {
            var customerAccount = _context.CustomerAccounts.SingleOrDefault(c => c.Id == customerAccountId);
            if (customerAccount.IsClosed != true)
            {
                return false;
            }
            return true;
        }

        [NonAction]
        public string GetCustomerAccountName(int id)
        {
            var customerAccount = _context.CustomerAccounts.SingleOrDefault(c => c.Id == id);
            var customerAccountName = customerAccount.Name;
            return customerAccountName;
        }
        [NonAction]
        public string GetTillAccountName()
        {
            //            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            //            var userManager = new UserManager<ApplicationUser>(store);
            //            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
            //            userId = user.Id;
            //            var id = userId;
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            var user = userManager.FindByEmail(RoleName.EMAIL);
            var id = user.Id;
            var tillAccount = _context.Tellers
                .OrderByDescending(p => p.Id)
                .FirstOrDefault(c => c.UserTellerId == id);
            if(tillAccount!=null)
            {
                var tillAccountId = tillAccount.TillAccountId;
                var GLAccount = _context.GlAccounts.SingleOrDefault(c => c.Id == tillAccountId);
                var tillAccountName = GLAccount.Name;
                return tillAccountName;
            }

            errorMessage = errorMessage + "You have not been assigned a Till";
            return "";
            
        }

        [NonAction]
        public void EnforceDoubleEntry(long amount, int customerAccountId, string type)
        {
            //            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            //            var userManager = new UserManager<ApplicationUser>(store);
            //            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
            //            userId = user.Id;
            //            var id = userId;
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            var user = userManager.FindByEmail(RoleName.EMAIL);
            var id = user.Id;
            var tillAccount = _context.Tellers
                .OrderByDescending(p => p.Id)
                .FirstOrDefault(c => c.UserTellerId == id);
            if(tillAccount!=null)
            {
                var glAccount = _context.GlAccounts.SingleOrDefault(c => c.Id == tillAccount.TillAccountId);
                var customerAccount = _context.CustomerAccounts.SingleOrDefault(c => c.Id == customerAccountId);
                if (type == "Deposit")
                {
                    tillAccount.TillAccountBalance = tillAccount.TillAccountBalance + amount;
                    glAccount.AccountBalance = glAccount.AccountBalance + amount;
                    customerAccount.AccountBalance = customerAccount.AccountBalance + amount;
                }
                else
                {
                    tillAccount.TillAccountBalance = tillAccount.TillAccountBalance - amount;
                    glAccount.AccountBalance = glAccount.AccountBalance - amount;
                    customerAccount.AccountBalance = customerAccount.AccountBalance - amount;
                }

                _context.SaveChanges();
            }
            else
            {
                errorMessage = errorMessage + "You have not been assigned a Till";
            }
            

        }

        [NonAction]
        public bool CheckTillBalance(long amount)
        {
//            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
//            var userManager = new UserManager<ApplicationUser>(store);
//            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
            
            //            var claimsIdentity = User.Identity as ClaimsIdentity;
            //            if (claimsIdentity != null)
            //            {
            //                // the principal identity is a claims identity.
            //                // now we need to find the NameIdentifier claim
            //                var userIdClaim = claimsIdentity.Claims
            //                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            //
            //                if (userIdClaim != null)
            //                {
            //                     userIdValue = userIdClaim.Value;
            //                }
            //            }
            //
            //            var id = userIdValue;
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            var user = userManager.FindByEmail(RoleName.EMAIL);
            var id = user.Id;
            
//            var id = User.Identity.GetUserId();
            var tillAccount = _context.Tellers
                .OrderByDescending(p => p.Id)
                .FirstOrDefault(c => c.UserTellerId.Equals(id));
            if(tillAccount!=null)
            {
                if (tillAccount.TillAccountBalance <= 0)
                {
                    errorMessage = errorMessage + " Till Account Balance is exhausted";
                    if (tillAccount.TillAccountBalance < amount)
                    {
                        return false;
                    }
                }
            }
            else
            {
                errorMessage = errorMessage + "You have not been assigned a Till";
            }
            
            
            return true;
        }

        [NonAction] public bool CheckCustomerAccountBalance(int customerAccountId, long amount)
        {
            var customerAccount = _context.CustomerAccounts.Include(m=>m.AccountType).SingleOrDefault(c => c.Id == customerAccountId);
            var customerAccountBalance = customerAccount.AccountBalance;
            if (customerAccountBalance < amount)
            {
                errorMessage = errorMessage+ "Insufficient Balance for Transaction";
                return false;

            }
            else
            {
                var minimumBalance = customerAccount.AccountType.MinimumBalance;
                if (minimumBalance != null)
                {

                    if ((customerAccountBalance - amount) < minimumBalance)
                    {
                        errorMessage =errorMessage+ "Minimum Balance for "+customerAccount.AccountType.Name+" would be exceeded";
                        return false;
                    }
                }

            }
            
            
 
            return true;
        }
        [NonAction]
        public void MarkAsAssigned(int TillAccountId)
        {
            var glAccounts = _context.GlAccounts.SingleOrDefault(c => c.Id == TillAccountId);
            glAccounts.IsAssigned = true;
            _context.SaveChanges();
        }

        [NonAction]
        public bool CheckIfUserHasBeenAssignedTeller(string userId)
        {
            var tellers = _context.Tellers.SingleOrDefault(c => c.UserTellerId.Equals(userId));
            if (tellers != null)
            {
                return true;
            }

            return false;
        }
    }
}