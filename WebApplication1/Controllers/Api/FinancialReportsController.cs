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
   
    /// <summary>
    /// 
    /// </summary>
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

     

        /// <summary>
        /// API CALL TO GET THE LIST OF BALANCE SHEET ENTRIES 
        /// </summary>
        /// <returns>
        /// LIST OF BALANCE SHEET ENTRIES 
        /// </returns>

        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/FinancialReports/BalanceSheet")]
        public HttpResponseMessage BalanceSheet()
        {

            var list = GetBalanceSheetReports();
            return Request.CreateResponse(HttpStatusCode.OK, list);

        }
        
        /// <summary>
        /// API CALL TO COMPUTE THE PROFIT AND LOSS ENTRIES FOR THE PROFIT & LOSS REPORT
        /// </summary>
        /// <returns>
        ///  A LIST OF ENTRIES FOR THE INCOME AND EXPENSES ACCRUED PER QUARTER
        /// </returns>
        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/FinancialReports/ProfitAndLoss")]
        public HttpResponseMessage ProfitAndLoss()
        {
            
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

        /// <summary>
        /// API CALL TO QUERY FOR THE PROFIT AND LOSS ENTRIES FOR THE PROFIT & LOSS REPORT FOR A SPECIFIC QUARTER OR PERIOD OF TIME
        /// </summary>
        /// <returns>
        ///  A LIST OF ENTRIES FOR THE INCOME AND EXPENSES ACCRUED PER QUARTER FOR THE SPECIFIED QUARTER OR PERIOD OF TIME
        /// </returns>
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/FinancialReports/ProfitAndLossQuery")]
        public HttpResponseMessage ProfitAndLossQuery(ReportsDto reportsDto)
        {

            
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

        /// <summary>
        /// API CALL TO COMPUTE THE TRIAL BALANCE ENTRIES
        /// </summary>
        /// <returns>
        /// LIST OF TRIAL BALANCE ENTRIES FOR THE TRIAL BALANCE REPORT
        /// </returns>
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/FinancialReports/TrialBalance")]
        public HttpResponseMessage TrialBalance()
        {
            var financialReports = _context.FinancialReports.ToList();
            var list = new List<BalanceSheetReport>();
            var customerSCAccount = new BalanceSheetReport()
            {
                AccountName = "Customer Savings/Current Account",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Teller Posting"
            };
            var customerLoanAccount = new BalanceSheetReport()
            {
                AccountName = "Customer Loan Account",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Loan Disbursement"
            };

            var accountPayable = new BalanceSheetReport()
            {
                AccountName = "Account Payable",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Interest Expense"
            };
            var accountReceivable = new BalanceSheetReport()
            {
                AccountName = "Account Receivable",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Loan Repayment"
            };
            var income = new BalanceSheetReport()
            {
                AccountName = "Income",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Loan Repayment"
            };
            var expense = new BalanceSheetReport()
            {
                AccountName = "Expense",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Loan Repayment"
            };
            var capitalAccount = new BalanceSheetReport()
            {
                AccountName = "Capital Account",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "GL Posting"
            };
            var tillAccount = new BalanceSheetReport()
            {
                AccountName = "Cash",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "GL Posting"
            };

            foreach (var report in financialReports)
            {
                var creditAccount = report.CreditAccount;
                var creditAmount = report.CreditAmount;
                var creditAccountCategory = report.CreditAccountCategory;
                var debitAccount = report.DebitAccount;
                var debitAmount = report.DebitAmount;
                var debitAccountCategory = report.DebitAccountCategory;
                if (creditAccountCategory.Equals("Liability")) // LIABILITY
                {
                    var glAccount =
                        _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(creditAccount));
                    if (glAccount != null && glAccount.Name.Equals(CBA.INTEREST_PAYABLE_GL_ACCOUNT))
                    {
                        accountPayable.CreditAmount = accountPayable.CreditAmount + creditAmount;
                    }
                    else
                    {
                        customerSCAccount.CreditAmount = customerSCAccount.CreditAmount + creditAmount;
                    }

                }

                else if (creditAccountCategory.Equals("Capital")) // CAPITAL
                {
                    capitalAccount.CreditAmount = capitalAccount.CreditAmount + creditAmount;
                }
                else if (creditAccountCategory.Equals("Income"))
                {
                    income.CreditAmount = income.CreditAmount + creditAmount;
                }

                if (debitAccountCategory.Equals("Asset")) // ASSET
                {
                    var glAccount =
                        _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(debitAccount));
                    if (glAccount == null)
                    {
                        customerLoanAccount.DebitAmount = customerLoanAccount.DebitAmount + debitAmount;

                    }
                    else if (glAccount.Name.Equals(CBA.INTEREST_RECEIVABLE_ACC_NAME) || glAccount.Name.Equals(CBA.COT_INCOME_RECEIVABLE_GL_ACC_NAME))
                    {
                        accountReceivable.DebitAmount = accountReceivable.DebitAmount + debitAmount;
                    }

                    else
                    {

                        tillAccount.DebitAmount = tillAccount.DebitAmount + debitAmount;

                    }
                }
                else if (debitAccountCategory.Equals("Expense")) // EXPENSE
                {
                    expense.DebitAmount = expense.DebitAmount + debitAmount;
                }


            }

            list.Add(tillAccount);
            list.Add(accountReceivable);
            list.Add(customerLoanAccount);
            list.Add(customerSCAccount);
            list.Add(accountPayable);
            list.Add(capitalAccount);
            list.Add(income);
            list.Add(expense);

            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [NonAction]
        public List<BalanceSheetReport> GetBalanceSheetReports()
        {
            var financialReports = _context.FinancialReports.ToList();
            var list = new List<BalanceSheetReport>();
            var customerSCAccount = new BalanceSheetReport()
            {
                AccountName = "Customer Savings/Current Account",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Teller Posting"
            };
            var customerLoanAccount = new BalanceSheetReport()
            {
                AccountName = "Customer Loan Account",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Loan Disbursement"
            };

            var accountPayable = new BalanceSheetReport()
            {
                AccountName = "Account Payable",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Interest Expense"
            };
            var accountReceivable = new BalanceSheetReport()
            {
                AccountName = "Account Receivable",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "Loan Repayment"
            };
            var capitalAccount = new BalanceSheetReport()
            {
                AccountName = "Capital Account",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "GL Posting"
            };
            var tillAccount = new BalanceSheetReport()
            {
                AccountName = "Till Account",
                CreditAmount = 0,
                DebitAmount = 0,
                PostingType = "GL Posting"
            };

            foreach (var report in financialReports)
            {
                var creditAccount = report.CreditAccount;
                var creditAmount = report.CreditAmount;
                var creditAccountCategory = report.CreditAccountCategory;
                var debitAccount = report.DebitAccount;
                var debitAmount = report.DebitAmount;
                var debitAccountCategory = report.DebitAccountCategory;
                if (creditAccountCategory.Equals("Liability"))
                {
                    var glAccount =
                        _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(creditAccount));

                    if (glAccount == null)
                    {
                        var customerAcc = _context.CustomerAccounts.SingleOrDefault(c => c.Name.Equals(creditAccount));
                        if (customerAcc.AccountTypeId == CBA.LOAN_ACCOUNT_TYPE_ID)
                        {
                            customerLoanAccount.DebitAmount = customerLoanAccount.DebitAmount + creditAmount;
                        }
                        customerSCAccount.CreditAmount = customerSCAccount.CreditAmount + creditAmount;

                    }
                    else
                    {
                        if (glAccount.Name.Equals(CBA.INTEREST_PAYABLE_GL_ACCOUNT))
                        {
                            accountPayable.CreditAmount = accountPayable.CreditAmount + creditAmount;
                        }


                    }

                }
                else if (creditAccountCategory.Equals("Expense"))
                {
                    accountPayable.CreditAmount = accountPayable.CreditAmount + creditAmount;
                }
                else if (creditAccountCategory.Equals("Capital"))// CAPITAL
                {
                    capitalAccount.CreditAmount = capitalAccount.CreditAmount + creditAmount;
                }

                if (debitAccountCategory.Equals("Asset"))
                {
                    var glAccount =
                        _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(debitAccount));
                    if (glAccount == null)
                    {
                        customerLoanAccount.DebitAmount = customerLoanAccount.DebitAmount + debitAmount;

                    }
                    else
                    {
                        if (glAccount.Name.Equals(CBA.INTEREST_RECEIVABLE_ACC_NAME))
                        {
                            accountReceivable.DebitAmount = accountReceivable.DebitAmount + debitAmount;
                        }
                        else
                        {
                            tillAccount.DebitAmount = tillAccount.DebitAmount + debitAmount;
                        }

                    }
                }
                else if (debitAccountCategory.Equals("Income"))
                {
                    accountReceivable.DebitAmount = accountReceivable.DebitAmount + debitAmount;
                }

            }

            list.Add(tillAccount);
            list.Add(customerLoanAccount);
            list.Add(accountReceivable);
            list.Add(customerSCAccount);
            list.Add(accountPayable);
            list.Add(capitalAccount);
            return list;
        }

        [NonAction]
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
                var accounts = glAccounts.Where(c => c.GlCategories.Name.Equals(type)).ToList();
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
    
    }
}
