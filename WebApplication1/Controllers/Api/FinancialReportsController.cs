using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WebApplication1.Dtos;
using WebApplication1.Models;
using System.Data.Entity;
namespace WebApplication1.Controllers.Api
{
    public class FinancialReportsController : ApiController
    {
        private ApplicationDbContext _context;
        private List<BalanceSheetReport> _incomeList;
        private List<BalanceSheetReport> _expenseList;
        private List<BalanceSheetReport> _capitalList;
        private List<BalanceSheetReport> _liabilityList;
        private List<BalanceSheetReport> _assetList;
        private List<List<BalanceSheetReport>> _balanceSheetList;

        public FinancialReportsController()
        {
            _context = new ApplicationDbContext();
            _incomeList = new List<BalanceSheetReport>();
            _expenseList = new List<BalanceSheetReport>();
            _capitalList = new List<BalanceSheetReport>();
            _liabilityList = new List<BalanceSheetReport>();
            _assetList = new List<BalanceSheetReport>();
            _balanceSheetList = new List<List<BalanceSheetReport>>();
        }

        public class BalanceSheetReport
        {
            public string AccountName { get; set; }
            public float DebitAmount { get; set; }
            public float CreditAmount { get; set; }
            public string PostingType { get; set; }

        }
//        [AcceptVerbs("GET", "POST")]
//        [HttpGet]
//        [Route("api/FinancialReports/BalanceSheet")]
//        public HttpResponseMessage BalanceSheet()
//        {
//
//            var glAccounts = _context.GlAccounts.Include(c => c.GlCategories).ToList();
//            var customerAccounts = _context.CustomerAccounts.ToList();
//            _balanceSheetList.Add(GetBalance(glAccounts, "Cash"));
//            _balanceSheetList.Add(GetBalance(glAccounts, "Income"));
//            _balanceSheetList.Add(GetBalance(glAccounts, "Expense"));
//            _balanceSheetList.Add(GetBalance(glAccounts, "Liability"));
//            _balanceSheetList.Add(GetBalance(glAccounts, "Equity"));
//
//            return Request.CreateResponse(HttpStatusCode.OK, _balanceSheetList.ToList());
//
//        }

        public List<BalanceSheetReport> GetBalance(List<GLAccount> glAccounts, string type)
        {
            var list = new List<BalanceSheetReport>();
            float balance = 0;
            float debitAmount = 0;
            float creditAmount = 0;
            var accountName = "";

            if (type.Equals("Liability"))
            {
               
                var accounts = _context.CustomerAccounts.ToList();
                accountName = "Liability";
                foreach (var account in accounts)
                {
                    balance = balance + account.AccountBalance;
                }

                creditAmount = balance;
            }
            else
            {
                 var accounts = glAccounts.Where(c => c.GlCategories.MainAccountCategory.Equals(type)).ToList();
                foreach (var account in accounts)
                {
                    balance = balance + account.AccountBalance;
                }

            }

            if (type == "Cash")
            {
                accountName = "Asset";
                debitAmount = balance;

            }
            else if (type == "Income")
            {
                accountName = "Account Receivable";
                debitAmount = balance;
            }
            else if (type == "Expense")
            {
                accountName = "Account Payable";
                creditAmount = balance;
            }
            else if (type == "Equity")
            {
                accountName = "Capital";
                creditAmount = balance;
            }
            var balanceSheet = new BalanceSheetReport()
            {
                AccountName = accountName,
                CreditAmount = creditAmount,
                DebitAmount = debitAmount,
                PostingType = "None"
            };
            list.Add(balanceSheet);
            return list;
        }
        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/FinancialReports/BalanceSheet")]
        public HttpResponseMessage BalanceSheet()
        {
        
        
            _balanceSheetList.Add(GetAssetList());
            _balanceSheetList.Add(GetIncomeList());
            _balanceSheetList.Add(GetExpenseList());
            _balanceSheetList.Add(GetLiabilityList());
            _balanceSheetList.Add(GetCapitalList());
        
            return Request.CreateResponse(HttpStatusCode.OK, _balanceSheetList.ToList());
        
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
                .Where(c => c.CreditAccount.Equals(CBA.INTEREST_INCOME_ACC_NAME)).ToList();
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

            var incomeAndExpense = new List<string>
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
            var fromDate = DateTime.ParseExact(reportsDto.FromDate, "yyyy-MM-dd", null);
            var toDate = DateTime.ParseExact(reportsDto.ToDate, "yyyy-MM-dd", null);
            float totalInterestIncome = 0;
            float totalCOTIncome = 0;
            float totalInterestExpense = 0;
            float receivable = 0;
            var loanDetails = _context.LoanDetails.ToList();
            var interestIncomeAccounts = _context.FinancialReports
                .Where(c => c.CreditAccount.Equals(CBA.INTEREST_INCOME_ACC_NAME)
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
                    if (interestReceivableAccount.ReportDate.Value.Date > fromDate.Date &&
                        interestReceivableAccount.ReportDate.Value.Date < toDate)
                    {
                        receivable = receivable + interestReceivableAccount.DebitAmount;

                    }

                }
            }

            if (interestIncomeAccounts != null)
            {
                foreach (var interestIncomeAccount in interestIncomeAccounts)
                {

                    if (interestIncomeAccount.ReportDate.Value.Date >= fromDate.Date &&
                        interestIncomeAccount.ReportDate.Value.Date <= toDate)
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

            var incomeAndExpense = new List<string>
            {
                CBA.INTEREST_INCOME_ACC_NAME,
                totalInterestIncome.ToString(CultureInfo.InvariantCulture),
                CBA.COT_INCOME_GL_ACCOUNT,
                totalCOTIncome.ToString(CultureInfo.InvariantCulture),
                CBA.INTEREST_EXPENSE_GL_ACCOUNT,
                totalInterestExpense.ToString()
            };
            return Request.CreateResponse(HttpStatusCode.OK, incomeAndExpense);
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/FinancialReports/TrialBalance")]
        public HttpResponseMessage TrialBalance(ReportsDto reportsDto)
        {
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }
        [NonAction]
        public List<BalanceSheetReport> GetIncomeList()
        {
            _incomeList = GetMyList("Income");

            return _incomeList;
        }

        [NonAction]
        public List<BalanceSheetReport> GetExpenseList()
        {
            _expenseList = GetMyList("Expense");
            return _expenseList;
        }

        [NonAction]
        public List<BalanceSheetReport> GetLiabilityList()
        {
            const string type = "Liability";
            _liabilityList = GetMyList(type);
            var _list = new List<BalanceSheetReport>();
            _list = CompressList(_liabilityList, type);
            return _list;
        }

        [NonAction]
        public List<BalanceSheetReport> GetCapitalList()
        {

            _capitalList = GetMyList("Capital");
            return _capitalList;
        }

        [NonAction]
        public List<BalanceSheetReport> GetAssetList()
        {

            const string type = "Asset";
            _assetList = GetMyList(type);
            var _list = new List<BalanceSheetReport>();
            _list = CompressList(_assetList, type);
            return _list;
        }

        [NonAction]
        public List<BalanceSheetReport> CompressList(List<BalanceSheetReport> list, string type)
        {
            float debitAmount = 0;
            float creditAmount = 0;
            bool isIncome = false;
            bool isExpense = false;
            float incomeAmount = 0;
            float expenseAmount = 0;
            var newList = new List<BalanceSheetReport>();

            var sheetReport = new BalanceSheetReport();

            if (type.Equals("Asset"))
            {
                foreach (var item in GetIncomeList())
                {
                    list.Add(item);
                }
            }
            else if (type.Equals("Liability"))
            {
                foreach (var item in GetExpenseList())
                {
                    list.Add(item);
                }
            }

            foreach (var item in list)
            {
                if (item.PostingType.Equals("Teller Posting") || item.PostingType.Equals("GL Posting") && !item.AccountName.Equals(CBA.INTEREST_EXPENSE_GL_ACCOUNT))
                {
                    if (type.Equals("Asset"))
                    {
                        debitAmount = debitAmount + item.DebitAmount;
                    }
                    else if (type.Equals("Liability"))
                    {
                        creditAmount = creditAmount + item.CreditAmount;
                    }
                }
                else if (item.PostingType.Equals("Savings Interest"))
                {
                    if (type.Equals("Asset"))
                    {
                        debitAmount = debitAmount + item.DebitAmount;
                    }
                    else if (type.Equals("Liability"))
                    {
                        if (item.AccountName.Equals(CBA.INTEREST_EXPENSE_GL_ACCOUNT))
                        {
                            isExpense = true;
                            expenseAmount = expenseAmount + item.CreditAmount;
                        }
                        else
                        {
                            creditAmount = creditAmount + item.CreditAmount;
                        }

                    }
                }
                else if (item.AccountName.Equals(CBA.INTEREST_EXPENSE_GL_ACCOUNT))
                {
                    isExpense = true;
                    expenseAmount = expenseAmount + item.CreditAmount;
                }
                else if (item.AccountName.Equals(CBA.COT_INCOME_GL_ACCOUNT) ||
                         item.AccountName.Equals(CBA.INTEREST_INCOME_ACC_NAME) || item.AccountName.Equals(CBA.INTEREST_RECEIVABLE_ACC_NAME))
                {
                    isIncome = true;

                    incomeAmount = incomeAmount + item.CreditAmount;



                }

                else
                {
                    newList.Add(item);
                }
            }


            if (type.Equals("Asset") || type.Equals("Income"))
            {

                if (isIncome == true)
                {
                    var balance1 = new BalanceSheetReport
                    {
                        AccountName = "Account Receivable",
                        DebitAmount = incomeAmount,
                        CreditAmount = 0,
                        PostingType = "Teller Posting"
                    };
                    newList.Add(balance1);
                    sheetReport.AccountName = "Till Account";
                    sheetReport.DebitAmount = debitAmount;
                    sheetReport.CreditAmount = 0;
                    sheetReport.PostingType = "Teller Posting";
                    newList.Add(sheetReport);
                }
                else
                {
                    sheetReport.AccountName = "Till Account";
                    sheetReport.DebitAmount = debitAmount;
                    sheetReport.CreditAmount = 0;
                    sheetReport.PostingType = "Teller Posting";
                    newList.Add(sheetReport);
                }

            }
            else if (type.Equals("Liability") || type.Equals("Expense"))
            {
                //                if (isExpense == true)
                //                {
                var balance1 = new BalanceSheetReport
                {
                    AccountName = "Account Payable",
                    DebitAmount = 0,
                    CreditAmount = expenseAmount,
                    PostingType = "Teller Posting"
                };
                if (expenseAmount != 0)
                {
                    newList.Add(balance1);
                }

                sheetReport.AccountName = "Customer Savings/Current Account";
                sheetReport.CreditAmount = creditAmount;
                sheetReport.DebitAmount = 0;
                sheetReport.PostingType = "Teller Posting";
                newList.Add(sheetReport);
            }




            return newList;
        }
        [NonAction]
        public List<BalanceSheetReport> GetMyList(string category)
        {
            var list = new List<BalanceSheetReport>();
            var financialReports = _context.FinancialReports.Where(c =>
                c.CreditAccountCategory.Equals(category) || c.DebitAccountCategory.Equals(category)).ToList();
            var listOfDuplicates = new List<string>();
            foreach (var report in financialReports)
            {
                var debitAccountName = report.DebitAccount;
                var creditAccountName = report.CreditAccount;
                var debitDuplicates = new List<FinancialReport>();
                var creditDuplicates = new List<FinancialReport>();


                debitDuplicates = _context.FinancialReports.Where(c =>
                   c.DebitAccount.Equals(debitAccountName) || c.CreditAccount.Equals(debitAccountName)).ToList();

                creditDuplicates = _context.FinancialReports.Where(c =>
                   c.CreditAccount.Equals(creditAccountName) || c.DebitAccount.Equals(creditAccountName)).ToList();


                if (report.DebitAccountCategory.Equals(category))
                {

                    if (!listOfDuplicates.Contains(debitAccountName))
                    {

                        list = SettleDuplicates(debitDuplicates, debitAccountName, listOfDuplicates, list, category, report.PostingType);
                        listOfDuplicates.Add(debitAccountName);


                    }

                }

                if (report.CreditAccountCategory.Equals(category))
                {
                    if (!listOfDuplicates.Contains(creditAccountName))
                    {
                        list = SettleDuplicates(creditDuplicates, creditAccountName, listOfDuplicates, list, category, report.PostingType);
                        listOfDuplicates.Add(creditAccountName);
                    }
                }

            }

            return list;
        }

        [NonAction]
        public List<BalanceSheetReport> SettleDuplicates(List<FinancialReport> duplicates, string accountName, List<string> listOfDuplicates,
            List<BalanceSheetReport> list, string category, string postingType)
        {
            float debAmount = 0;
            float credAmount = 0;
            foreach (var duplicate in duplicates)
            {
                if (duplicate.CreditAccount.Equals(accountName) && duplicate.CreditAccountCategory.Equals(category))
                {
                    credAmount = credAmount + duplicate.CreditAmount;
                }
                else if (duplicate.DebitAccount.Equals(accountName) && duplicate.DebitAccountCategory.Equals(category))
                {
                    debAmount = debAmount + duplicate.DebitAmount;
                }
                else
                {
                    continue;
                }


            }

            if (!listOfDuplicates.Contains(accountName))
            {
                list = AddToList(list, accountName, credAmount, debAmount, postingType);
            }

            return list;

        }

        [NonAction]
        public List<BalanceSheetReport> AddToList(List<BalanceSheetReport> list, string accountName, float creditAmount,
            float debitAmount, string postingType
        )
        {
            var sheetReport = new BalanceSheetReport
            {
                AccountName = accountName,
                CreditAmount = creditAmount,
                DebitAmount = debitAmount,
                PostingType = postingType
            };
            list.Add(sheetReport);
            return list;
        }
    }
}
