using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Dtos;
using AutoMapper;
using System.Data.Entity;

namespace WebApplication1.Controllers.Api
{
    public class GeneralLedgersController : ApiController
    {
        private ApplicationDbContext _context;
        private string errorMessage = "";

        public GeneralLedgersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: /api/generalledgers
        public IHttpActionResult GetCategories()
        {

            var categoriesDto = _context.GlCategories.Include(c => c.Categories).ToList();

            return Ok(categoriesDto);
            
        }

        [Route("api/GeneralLedgers/GetAccount")]
        public IHttpActionResult GetAccount()
        {
            var accountDto = _context.GlAccounts.Include(c => c.GlCategories).Include(b => b.Branch).ToList();

            return Ok(accountDto);
        }
        [Route("api/GeneralLedgers/GetPostings")]
        public IHttpActionResult GetPostings()
        {
            var glPostingsDto = _context.GlPostings.Include(c => c.GlCreditAccount).Include(b => b.GlDebitAccount).ToList();


            return Ok(glPostingsDto);
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/GeneralLedgers/AddGLPosting")]
        public HttpResponseMessage AddGLPosting(GLPostingDto glPostingDto)
        {

            glPostingDto.TransactionDate = DateTime.Now;
//            if (!checkAccountBalance(glPostingDto.GlCreditAccountId,glPostingDto.CreditAmount))
//            {
//                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "GL Credit Account Balance Insufficient");
//
//            }
//            if (!checkAccountBalance(glPostingDto.GlDebitAccountId, glPostingDto.DebitAmount))
//            {
//                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "GL Debit Account Balance Insufficient");
//
//            }

            var glPosting = new GLPostings()
            {
                GlCreditAccountId = glPostingDto.GlCreditAccountId,
                CreditAmount = glPostingDto.CreditAmount,
                CreditNarration = glPostingDto.CreditNarration,
                GlDebitAccountId = glPostingDto.GlDebitAccountId,
                DebitAmount = glPostingDto.DebitAmount,
                DebitNarration = glPostingDto.DebitNarration,
                TransactionDate = glPostingDto.TransactionDate

            };
            _context.GlPostings.Add(glPosting);
            _context.SaveChanges();
            CBA.CreditAndDebitAccounts(glPostingDto);
            var financialReportDto = new FinancialReportDto();
            financialReportDto.PostingType = "GL Posting";
            
            financialReportDto.CreditAccount = GetCreditAccountName(glPostingDto.GlCreditAccountId);
            financialReportDto.CreditAmount = glPostingDto.CreditAmount;
            financialReportDto.DebitAccount = GetDebitAccountName(glPostingDto.GlDebitAccountId);
            financialReportDto.DebitAmount = glPostingDto.DebitAmount;
            

            financialReportDto.ReportDate = DateTime.Now;

            CBA.AddReport(financialReportDto);
            return Request.CreateResponse(HttpStatusCode.OK, "GL Posted Successfully");


        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/GeneralLedgers/ValidationChecks")]
        public HttpResponseMessage ValidationChecks(GLPostingDto glPostingDto)
        {
           
            if (!checkAccountBalance(glPostingDto.GlCreditAccountId, glPostingDto.CreditAmount))
            {
                if (errorMessage.Equals(""))
                {
                    errorMessage = errorMessage + "GL Credit Account Balance Insufficient";
                }
                errorMessage = errorMessage + ", GL Credit Account Balance Insufficient";
                

            }
            if (!checkAccountBalance(glPostingDto.GlDebitAccountId, glPostingDto.DebitAmount))
            {
                if (errorMessage.Equals(""))
                {
                    errorMessage = errorMessage + "GL Debit Account Balance Insufficient";
                }
                errorMessage = errorMessage + ", GL Debit Account Balance Insufficient";
            }

            if (!errorMessage.Equals(""))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);

            }
            return Request.CreateResponse(HttpStatusCode.OK, "Proceed");
        }
        public bool checkAccountBalance(int accountId, long amount)
        {
            var account = _context.GlAccounts.SingleOrDefault(c => c.Id == accountId);
            var accountBalance = account.AccountBalance;
            if (accountBalance <= 0)
            { 
                errorMessage = "Account Balance is empty";
                if (accountBalance < amount)
                {
                    return false;
                }
            }
            
            return true;
        }
        public string GetCreditAccountName(int creditAccountId)
        {
            var creditAccount = _context.GlAccounts.SingleOrDefault(c => c.Id == creditAccountId);
            var creditAccountName = creditAccount.Name;
            return creditAccountName;
        }
        public string GetDebitAccountName(int debitAccountId)
        {
            var debitAccount = _context.GlAccounts.SingleOrDefault(c => c.Id == debitAccountId);
            var debitAccountName = debitAccount.Name;
            return debitAccountName;
        }
    }
}