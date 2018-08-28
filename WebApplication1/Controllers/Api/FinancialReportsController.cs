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
    public class FinancialReportsController : ApiController
    {
        private ApplicationDbContext _context;
        public FinancialReportsController()
        {
            _context = new ApplicationDbContext();
        }

        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/FinancialReports/ProfitAndLoss")]
        public HttpResponseMessage ProfitAndLoss()
        {
            //&& c.ReportDate.Value.Date > reportsDto.FromDate.Date
            //    && c.ReportDate.Value.Date < reportsDto.ToDate.Date)

            float totalInterestIncome = 0;
            float totalCOTIncome = 0;
            float totalInterestExpense = 0;
            var loanDetails = _context.LoanDetails.ToList();
            var interestIncomeAccount = _context.FinancialReports
                .Where(c => c.CreditAccount.Equals(CBA.INTEREST_INCOME_ACC_NAME) ).ToList();
            var COTIncomeAccount = _context.FinancialReports
                .Where(c => c.CreditAccount.Equals(CBA.COT_INCOME_GL_ACCOUNT)).ToList();
            var interestExpenseAccount = _context.FinancialReports
                .Where(c => c.DebitAccount.Equals(CBA.INTEREST_EXPENSE_GL_ACCOUNT)).ToList();
            var interestReceivableAccount = _context.FinancialReports
                .Where(c => (c.DebitAccount.Equals(CBA.INTEREST_RECEIVABLE_ACC_NAME))).ToList();


            if (loanDetails != null)
            {
                foreach (var loanDetail in loanDetails)
                {
                    var income = loanDetail.InterestIncome - loanDetail.InterestReceivable;
                    totalInterestIncome = totalInterestIncome + income;
                }
            }


            if (COTIncomeAccount != null)
            {
                foreach (var account in COTIncomeAccount)
                {
                    totalCOTIncome = totalCOTIncome + account.CreditAmount;
                }
            }

            if (interestExpenseAccount != null)
            {
                foreach (var account in interestExpenseAccount)
                {
                    totalInterestExpense = totalInterestExpense + account.DebitAmount;
                }
            }
            List<string> incomeAndExpense = new List<string>
            {
                CBA.INTEREST_INCOME_ACC_NAME,
                totalInterestIncome.ToString(),
                CBA.COT_INCOME_GL_ACCOUNT,
                totalCOTIncome.ToString(),
                CBA.INTEREST_EXPENSE_GL_ACCOUNT,
                totalInterestExpense.ToString()
            };
            return Request.CreateResponse(HttpStatusCode.OK, incomeAndExpense);
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/FinancialReports/ProfitAndLossQuery")]
        public HttpResponseMessage ProfitAndLossQuery(ReportsDto reportsDto)
        {

            //&& c.ReportDate.Value.Date > fromDate.Date
            //    && c.ReportDate.Value.Date < toDate.Date
            DateTime fromDate = DateTime.ParseExact(reportsDto.FromDate, "yyyy-MM-dd", null);
            DateTime toDate = DateTime.ParseExact(reportsDto.ToDate, "yyyy-MM-dd", null);
            float totalInterestIncome = 0;
            float totalCOTIncome = 0;
            float totalInterestExpense = 0;
            float receivable = 0;
            var loanDetails = _context.LoanDetails.ToList();
            var interestIncomeAccounts = _context.FinancialReports
                .Where(c =>  c.CreditAccount.Equals(CBA.INTEREST_INCOME_ACC_NAME)
               ).ToList();
            var COTIncomeAccount = _context.FinancialReports
                .Where(c => c.CreditAccount.Equals(CBA.COT_INCOME_GL_ACCOUNT)).ToList();
            var interestExpenseAccount = _context.FinancialReports
                .Where(c => c.DebitAccount.Equals(CBA.INTEREST_EXPENSE_GL_ACCOUNT)).ToList();
            var interestReceivableAccounts = _context.FinancialReports
                .Where(c => c.DebitAccount.Equals(CBA.INTEREST_RECEIVABLE_ACC_NAME)).ToList();


            if (loanDetails != null)
            {
                foreach (var loanDetail in loanDetails)
                {
                    //receivable = loanDetail.InterestReceivable;

                }
            }
            if (interestReceivableAccounts != null)
            {
                foreach (var interestReceivableAccount in interestReceivableAccounts)
                {
                    if (interestReceivableAccount.ReportDate.Value.Date > fromDate.Date && interestReceivableAccount.ReportDate.Value.Date < toDate)
                    {
                        receivable = receivable + interestReceivableAccount.DebitAmount;

                    }

                }
            }

            if (interestIncomeAccounts != null)
            {
                foreach (var interestIncomeAccount in interestIncomeAccounts)
                {
                    if (interestIncomeAccount.ReportDate.Value.Date >= fromDate.Date && interestIncomeAccount.ReportDate.Value.Date <= toDate)
                    {
                        var income = interestIncomeAccount.CreditAmount;
                        totalInterestIncome = totalInterestIncome + income;
                    }

                }
            }

            if (COTIncomeAccount != null)
            {
                foreach (var account in COTIncomeAccount)
                {
                    if (account.ReportDate.Value.Date >= fromDate.Date && account.ReportDate.Value.Date <= toDate)
                    {
                        totalCOTIncome = totalCOTIncome + account.CreditAmount;
                    }
                }
            }

            if (interestExpenseAccount != null)
            {
                foreach (var account in interestExpenseAccount)
                {
                    if (account.ReportDate.Value.Date >= fromDate.Date && account.ReportDate.Value.Date <= toDate)
                    {
                        totalInterestExpense = totalInterestExpense + account.DebitAmount;
                    }
                }
            }
            totalInterestIncome = totalInterestIncome - receivable;
            List<string> incomeAndExpense = new List<string>
            {
                CBA.INTEREST_INCOME_ACC_NAME,
                totalInterestIncome.ToString(),
                CBA.COT_INCOME_GL_ACCOUNT,
                totalCOTIncome.ToString(),
                CBA.INTEREST_EXPENSE_GL_ACCOUNT,
                totalInterestExpense.ToString()
            };
            return Request.CreateResponse(HttpStatusCode.OK, incomeAndExpense);
        }

    }
}