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

        public class Index
        {
            public int Id { get; set; }
        }
        // GET: /api/GeneralLedgers
        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/GeneralLedgers/GetCategories")]
        //        [Authorize]
        public IHttpActionResult GetCategories()
        {

            var categoriesDto = _context.GlCategories.Include(c => c.Categories).ToList();

            return Ok(categoriesDto);

        }
        // POST: /api/GeneralLedgers/EditGLCategory
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/GeneralLedgers/EditGLCategory")]
        //        [Authorize]
        public HttpResponseMessage EditGLCategory(Index index)
        {
            var id = index.Id;

            var glCategory = _context.GlCategories.SingleOrDefault(c => c.Id == id);
            if (glCategory == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No such GL Category Exists");
            }
            var glCategoriesDto = new GLCategoryDto()
            {
                Name = glCategory.Name,
                Description = glCategory.Description
            };

            return Request.CreateResponse(HttpStatusCode.OK, glCategoriesDto);

        }
        // PUT: /api/GeneralLedgers/UpdateGLCategory
        [AcceptVerbs("GET", "PUT")]
        [HttpPut]
        [Route("api/GeneralLedgers/UpdateGLCategory")]
        //        [Authorize]
        public HttpResponseMessage UpdateGLCategory(GLCategoryDto glCategoryDto)
        {
            var id = glCategoryDto.Id;
            var glCategory = _context.GlCategories.SingleOrDefault(c => c.Id == id);
            if (glCategory == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No such GL Category Exists");
            }

            glCategory.Name = glCategoryDto.Name;
            glCategory.Description = glCategoryDto.Description;
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "GL Category Updated Successfully");

        }
        /**
         * GL ACCOUNTS CATEGORY
         */
        // GET api/GeneralLedgers/GetGLAccounts
        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/GeneralLedgers/GetGLAccounts")]
        //        [Authorize]
        public IHttpActionResult GetGLAccounts()
        {
            var accountDto = _context.GlAccounts.Include(c => c.GlCategories).Include(c => c.GlCategories.Categories).Include(b => b.Branch).ToList();

            return Ok(accountDto);
        }
        // POST: /api/GeneralLedgers/EditGLAccount
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/GeneralLedgers/EditGLAccount")]
        //        [Authorize]
        public HttpResponseMessage EditGLAccount(Index index)
        {
            var id = index.Id;

            var glAccount = _context.GlAccounts.SingleOrDefault(c => c.Id == id);
            if (glAccount == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No such GL Account Exists");
            }
            var glAccountDto = new GLAccountDto()
            {
                Name = glAccount.Name,
                BranchId = glAccount.BranchId
            };

            return Request.CreateResponse(HttpStatusCode.OK, glAccountDto);

        }
        // PUT: /api/GeneralLedgers/UpdateGLAccount
        [AcceptVerbs("GET", "PUT")]
        [HttpPut]
        [Route("api/GeneralLedgers/UpdateGLAccount")]
        //        [Authorize]
        public HttpResponseMessage UpdateGLAccount(GLAccountDto glAccountDto)
        {
            var id = glAccountDto.Id;
            var glAccount = _context.GlAccounts.SingleOrDefault(c => c.Id == id);
            if (glAccount == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No such GL Account Exists");
            }

            glAccount.Name = glAccountDto.Name;
            glAccount.BranchId = glAccountDto.BranchId;
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "GL Account Updated Successfully");

        }

        // GET api/GeneralLedgers/GetPostings
        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/GeneralLedgers/GetPostings")]
        //        [Authorize]
        public IHttpActionResult GetPostings()
        {
            var glPostingsDto = _context.GlPostings.Include(c => c.GlCreditAccount).Include(b => b.GlDebitAccount).Include(c=>c.UserAccount).ToList();


            return Ok(glPostingsDto);
        }

        // GET api/GeneralLedgers/AddGLPosting
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/GeneralLedgers/AddGLPosting")]
//        [Authorize(Roles = RoleName.ADMIN_ROLE)]
        public HttpResponseMessage AddGLPosting(GLPostingDto glPostingDto)
        {
            
            var userId = _context.Users.SingleOrDefault(c => c.Email.Equals(RoleName.EMAIL)).Id;


            glPostingDto.UserAccountId = userId;

            var businessStatus = _context.BusinessStatus.SingleOrDefault();

            if (businessStatus.Status == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, CBA.BUSINESS_CLOSED_REFRESH_MSG);
            }


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
                TransactionDate = glPostingDto.TransactionDate,
                UserAccountId = glPostingDto.UserAccountId

            };
            var tillAccount = _context.Tellers.ToList();
            _context.GlPostings.Add(glPosting);
            _context.SaveChanges();

            CBA.CreditAndDebitAccounts(glPostingDto);

            var financialReportDto = new FinancialReportDto
            {
                PostingType = "GL Posting",
                CreditAccount = GetCreditAccountName(glPostingDto.GlCreditAccountId),
                CreditAmount = glPostingDto.CreditAmount,
                DebitAccount = GetDebitAccountName(glPostingDto.GlDebitAccountId),
                DebitAmount = glPostingDto.DebitAmount,
                ReportDate = DateTime.Now
            };




            CBA.AddReport(financialReportDto);
            return Request.CreateResponse(HttpStatusCode.OK, "GL Posted Successfully");


        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/GeneralLedgers/ValidationChecks")]
        //        [Authorize(Roles = RoleName.ADMIN_ROLE)]
        public HttpResponseMessage ValidationChecks(GLPostingDto glPostingDto)
        {


            //            if (!checkAccountBalance(glPostingDto.GlCreditAccountId, glPostingDto.CreditAmount))
            //            {
            //                if (errorMessage.Equals(""))
            //                {
            //                    errorMessage = errorMessage + "GL Credit Account Balance Insufficient";
            //                }
            //                errorMessage = errorMessage + ", GL Credit Account Balance Insufficient";
            //                
            //
            //            }
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
        public bool checkAccountBalance(int accountId, float amount)
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