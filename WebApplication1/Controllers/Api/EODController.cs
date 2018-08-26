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
            
            _context.SaveChanges();
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
                InterestAccrual();
            }
           
            // return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer already has a Savings Account");

            return Request.CreateResponse(HttpStatusCode.OK, returnMsg);

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

                    var financialReportDto = new FinancialReportDto();
                    financialReportDto.PostingType = "Interest Accrual";

                    financialReportDto.DebitAccount = "Interest Receivable Account";
                    financialReportDto.DebitAmount = accruedInterest;
                    financialReportDto.CreditAccount = "Interest Income Account";
                    financialReportDto.CreditAmount = accruedInterest;


                    financialReportDto.ReportDate = DateTime.Now;

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