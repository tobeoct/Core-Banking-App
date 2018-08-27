using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    
    public class EODController : ApiController
    {
        private ApplicationDbContext _context;
        private string returnMsg = "Business";
        private string errorMsg = "";
        public EODController()
        {
            _context = new ApplicationDbContext();
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/EOD/Start")]
        public HttpResponseMessage Start(BusinessStatusDto businessStatusDto)
        {

            var status = false;
            
            
            if(businessStatusDto.IntendedAction.Equals("Open"))
            {
                status = true;
            }

            var businessStatus = _context.BusinessStatus.FirstOrDefault();
            if (businessStatus == null)
            {
                var business = new BusinessStatus();
                business.Status = status;
                _context.BusinessStatus.Add(business);

            }
            {
                businessStatus.Status = status;
            }
            
            
            if(businessStatusDto.IntendedAction.Equals("Open"))
            {
                returnMsg = returnMsg + " Opened";
            }
            else
            {
                returnMsg = returnMsg + " Closed";
            }
            if(businessStatusDto.IntendedAction.Equals("Close"))
            {
                CalculateCOT();
                if (!errorMsg.Equals(""))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
                }
                InterestAccrual();
                var financialDates = _context.FinancialDates.Count();
                if(financialDates%30==0)
                {
                    RunEOM();
                }
                
               
            }

            // return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer already has a Savings Account");
            _context.SaveChanges();
            if (businessStatusDto.IntendedAction.Equals("Close"))
            {

                var financialDate = new FinancialDates
                {
                    EOD = DateTime.Now
                };
                _context.FinancialDates.Add(financialDate);
                _context.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK, returnMsg);

        }
        [NonAction]
        public void RunEOM()
        {

        }
        [NonAction]
        public void CalculateCOT()
        {
            var customerAccounts = _context.CustomerAccounts.ToList();
            var COTIncomeGLAccount = _context.GlAccounts.Where(c => c.Name.Equals("COT Income GL Account")).FirstOrDefault();
            var currentAccountId = _context.AccountTypes.Where(c => c.Name.Equals("Current Account")).FirstOrDefault().Id;
            if (COTIncomeGLAccount != null)
            {
                foreach (var account in customerAccounts)
                {
                    if (account.AccountTypeId == currentAccountId)
                    {
                        float todayWithdrawal = 0, cotAmount;
                        float customerAccountBalance = account.AccountBalance;
                        var tellerPostings = _context.TellerPostings.Where(c=> c.PostingType.Equals("Withdrawal") && c.CustomerAccountId == account.Id).ToList();
                        foreach (var posting in tellerPostings)
                        {
                            if(posting.TransactionDate.Day==DateTime.Today.Day)
                            {
                                todayWithdrawal = todayWithdrawal + posting.Amount;
                            }
                            
                        }

                        cotAmount = (5 * customerAccountBalance) / 1000;
                        account.AccountBalance = customerAccountBalance - cotAmount;
                        COTIncomeGLAccount.AccountBalance = COTIncomeGLAccount.AccountBalance + cotAmount;
                        //ADD TO REPORT TABLE IN DB (DOUBLE ENTRY)
                        var financialReportDto = new FinancialReportDto
                        {
                            PostingType = "COT",
                            DebitAccount = account.Name,
                            DebitAmount = cotAmount,
                            CreditAccount = "COT Income GL Account",
                            CreditAmount = cotAmount,
                            ReportDate = DateTime.Now
                        };

                        CBA.AddReport(financialReportDto);
                        returnMsg = returnMsg + ": COT Calculated Successfully Successful";

                    }                       
                }
            }
        
            else
            {
                errorMsg = errorMsg + " No <b> COT Income GL Account Created </b>";
            }
            
        }
          
            

            
            

        
        [NonAction]
        public void InterestAccrual()
        {
            var loanDetails = _context.LoanDetails.ToList();
            float accruedInterest =0;
           
            if (loanDetails!=null)
            {
                foreach (var detail in loanDetails)
                {
                    float interestRate = (detail.InterestRate / 100);
                    float principal = detail.LoanAmount;
                    accruedInterest = ( interestRate*principal ) / (12 * 30);
                    detail.InterestReceivable = detail.InterestReceivable + accruedInterest;
                    detail.InterestIncome = detail.InterestReceivable + accruedInterest;
                    _context.SaveChanges();

                    //ADD TO REPORT TABLE IN DB (DOUBLE ENTRY)
                    var financialReportDto = new FinancialReportDto
                    {
                        PostingType = "Interest Accrual",
                        DebitAccount = "Interest Receivable Account",
                        DebitAmount = accruedInterest,
                        CreditAccount = "Interest Income Account",
                        CreditAmount = accruedInterest,
                        ReportDate = DateTime.Now
                    };

                    CBA.AddReport(financialReportDto);
                    returnMsg = returnMsg + ", Interest Accrual Process Successful";

                }
            }
            
           
        }
        // GET api/<controller>
        [AcceptVerbs("GET", "POST")]
        [Route("api/EOD/GetStatus")]
        public HttpResponseMessage GetStatus(BusinessStatusDto businessStatusDto)
        {
            var business = _context.BusinessStatus.FirstOrDefault();
            var businessStatus = business.Status;
            var status="";
            if (businessStatus==true)
            {
                status = "Opened";
            }
            else
            {
                status = "Closed";
            }
            
            return Request.CreateResponse(HttpStatusCode.OK, status);
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