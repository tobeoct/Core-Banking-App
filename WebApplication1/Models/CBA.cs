using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Dtos;
using WebApplication1.ViewModels;
using System.Data.Entity;
using WebApplication1.Processor;

namespace WebApplication1.Models
{
    public class CBA
    {
        private ApplicationDbContext _context;
        public static string INTEREST_IN_SUSPENSE_ACC_NAME = "Interest-In-Suspense GL Account";
        public static string INTEREST_OVERDUE_ACC_NAME = "Interest Overdue GL Account";
        public static string INTEREST_RECEIVABLE_ACC_NAME = "Interest Receivable GL Account";
        public static string INTEREST_PAYABLE_GL_ACCOUNT = "Interest Payable GL Account";
        public static string INTEREST_INCOME_ACC_NAME = "Interest Income GL Account";
        public static string PRINCIPAL_OVERDUE_ACC_NAME = "Principal Overdue GL Account";
        public static string COT_INCOME_GL_ACCOUNT = "COT Income GL Account";
        public static string INTEREST_EXPENSE_GL_ACCOUNT = "Interest Expense GL Account";
        public static string CAPITAL_ACCOUNT = "Capital Account";
        public static string CUSTOMER_LOAN_ACCOUNT = "Customer Loan Account";
        public static string CUSTOMER_SAVINGS_CURRENT_ACCOUNT = "Customer Savings/Current Account";

        public static string COT_INCOME_RECEIVABLE_GL_ACC_NAME = "COT Income Receivable GL Account";
        public static string VAULT_ACCOUNT = "Vault Account";
        public static int LOAN_ACCOUNT_TYPE_ID = 3;
        public static int CURRENT_ACCOUNT_TYPE_ID = 2;
        public static int SAVINGS_ACCOUNT_TYPE_ID = 1;
        public static string BUSINESS_CLOSED_REFRESH_MSG = "You cannot perform postings, Business is Closed : <b onclick='window.location.reload();' style='cursor:pointer;'>Refresh Page</b>";
        public CBA()
        {
            _context = new ApplicationDbContext();
            var loanAccountType = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Loan Account"));
            var savingsAccountType = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Savings Account"));
            var currentAccountType = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Current Account"));
            if (loanAccountType != null)
            {
                LOAN_ACCOUNT_TYPE_ID = loanAccountType.Id;
            }
            if (savingsAccountType != null)
            {
                SAVINGS_ACCOUNT_TYPE_ID = savingsAccountType.Id;
            }
            if (currentAccountType != null)
            {
                CURRENT_ACCOUNT_TYPE_ID = currentAccountType.Id;
            }

        }

        public static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "01234567890123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void AddReport(FinancialReportDto financialReportDto)
        {
            financialReportDto.ReportDate = DateTime.Now;
            var _context = new ApplicationDbContext();
            var creditAccountCategory = GetCategory(financialReportDto.CreditAccount.ToString());
            var debitAccountCategory = GetCategory(financialReportDto.DebitAccount.ToString());
            var creditAmount = (float)System.Math.Round(financialReportDto.CreditAmount, 2);
            var debitAmount = (float)System.Math.Round(financialReportDto.DebitAmount, 2);
            var financialReport = new FinancialReport
            {
                ReportDate = financialReportDto.ReportDate,
                CreditAccount = financialReportDto.CreditAccount,
                CreditAmount = creditAmount,
                CreditAccountCategory = creditAccountCategory,
                DebitAccount = financialReportDto.DebitAccount,
                DebitAmount = debitAmount,
                DebitAccountCategory = debitAccountCategory,
                PostingType = financialReportDto.PostingType

            };
            _context.FinancialReports.Add(financialReport);
            _context.SaveChanges();

        }

        public static string GetCategory(string accountName)
        {
            var category = "";
            accountName = accountName.ToString().Trim();
            var _context = new ApplicationDbContext();
            var glAccount = _context.GlAccounts.Where(c => c.Name.Equals(accountName)).Include(c => c.GlCategories).Include(c => c.GlCategories.Categories).SingleOrDefault();
            if (glAccount == null)
            {
                var customerAccount = _context.CustomerAccounts.SingleOrDefault(c => c.Name.Equals(accountName));
                if (customerAccount == null || customerAccount.AccountTypeId == LOAN_ACCOUNT_TYPE_ID)
                {
                    category = "Asset";

                }
                else
                {
                    category = "Liability";
                }

                return category;
            }
            if (glAccount != null) category = glAccount.GlCategories.Categories.Name.ToString();
            //            return category;
            //            if (accountName.Equals(COT_INCOME_GL_ACCOUNT) 
            //                || accountName.Equals(INTEREST_RECEIVABLE_ACC_NAME) 
            //                || accountName.Equals(INTEREST_EXPENSE_GL_ACCOUNT) 
            //                || accountName.Equals(INTEREST_INCOME_ACC_NAME) 
            //                || accountName.Equals(INTEREST_OVERDUE_ACC_NAME) 
            //                || accountName.Equals(INTEREST_IN_SUSPENSE_ACC_NAME) 
            //                || accountName.Equals(PRINCIPAL_OVERDUE_ACC_NAME))
            //            {
            //               
            //                if (glAccount != null) category = glAccount.GlCategories.Categories.Name.ToString();
            //                return category;
            //            }
            //            category = "Liability";
            return category;


        }

        public static void BasedOnGLCategories(GLAccount glAccount, string type, float amount)
        {
            var _context = new ApplicationDbContext();
            var glCategories = _context.GlCategories.Include(c => c.Categories).SingleOrDefault(c => c.Id == glAccount.GlCategoriesId);
            var category = _context.Categories.SingleOrDefault(c => c.Id == glCategories.CategoriesId);
            if (type == "Credit")
            {
                if (category != null && (category.Name == "Asset" || category.Name == "Expense"))
                {
                    glAccount.AccountBalance = glAccount.AccountBalance - amount;

                }

                else
                {
                    glAccount.AccountBalance = glAccount.AccountBalance + amount;
                }
            }
            else
            {
                if (category != null && (category.Name == "Asset" || category.Name == "Expense"))
                {
                    glAccount.AccountBalance = glAccount.AccountBalance + amount;
                }
                else
                {
                    glAccount.AccountBalance = glAccount.AccountBalance - amount;
                }
            }


        }

        public static void CreditAndDebitAccounts(GLPostingDto glPostingDto)
        {
            var _context = new ApplicationDbContext();
            var glCreditAccount = _context.GlAccounts.Include(c => c.GlCategories).SingleOrDefault(c => c.Id == glPostingDto.GlCreditAccountId);
            var glDebitAccount = _context.GlAccounts.Include(c => c.GlCategories).SingleOrDefault(c => c.Id == glPostingDto.GlDebitAccountId);
            if (glDebitAccount.GlCategories.Name.Equals("Equity") &&
                    (glCreditAccount.GlCategories.Name.Equals("Cash") || glCreditAccount.GlCategories.Name.Equals("Expense")))
            {
                glDebitAccount.AccountBalance = glDebitAccount.AccountBalance - glPostingDto.DebitAmount;
                glCreditAccount.AccountBalance = glCreditAccount.AccountBalance + glPostingDto.CreditAmount;
            }
            else
            {
                BasedOnGLCategories(glCreditAccount, "Credit", glPostingDto.CreditAmount);
                BasedOnGLCategories(glDebitAccount, "Debit", glPostingDto.DebitAmount);

            }

            _context.SaveChanges();


        }

        public static string PerformDoubleEntry(string type, string accountNumber, double amount)
        {
            var _context = new ApplicationDbContext();
            var customerAccount =
                _context.CustomerAccounts.Include(c => c.AccountType).FirstOrDefault(c => c.AccountNumber.Equals(accountNumber.ToString()));
            var accountBalance = customerAccount.AccountBalance;
            var minimumBalance = customerAccount.AccountType.MinimumBalance;


            if (type.Equals("Debit"))
            {
                if ((accountBalance - minimumBalance) > amount)
                {
                    customerAccount.AccountBalance = (float)(accountBalance - amount);
                    AddToReport("Teller Posting", customerAccount.Name, "ATM-Till",(float) amount);
                    _context.SaveChanges();
                    return Codes.APPROVED;
                }
            }
            if (type.Equals("Credit"))
            {
                customerAccount.AccountBalance = (float)(accountBalance + amount);
                AddToReport("Teller Posting", "ATM-Till", customerAccount.Name, (float)amount);
                _context.SaveChanges();
                return Codes.APPROVED;
            }
            return Codes.INVALID_AMOUNT;

        }
        // Create Financial Report Entry
        
        public static void AddToReport(string postingType, string debitAccountName, string creditAccountName, float amount)
        {
            var creditAmount = (float)System.Math.Round(amount, 2);
            var debitAmount = (float)System.Math.Round(amount, 2);
            var financialReportDto = new FinancialReportDto
            {
                PostingType = postingType,
                DebitAccount = debitAccountName,
                DebitAmount = debitAmount,
                CreditAccount = creditAccountName,
                CreditAmount = creditAmount,
                ReportDate = DateTime.Now
            };

            CBA.AddReport(financialReportDto);
        }


        public static string BalanceEnquiry(string accountNumber)
        {
            double amount = 0;
            var _context = new ApplicationDbContext();
            var customerAccount =
                _context.CustomerAccounts.Include(c => c.AccountType).FirstOrDefault(c => c.AccountNumber.Equals(accountNumber.ToString()));
            amount = Convert.ToDouble(FormatTo2Dp(Convert.ToDecimal(customerAccount.AccountBalance))) * 100;
            string amountInIsoFormat = amount.ToString().PadLeft(10, '0');
            return amountInIsoFormat;
        }
        public static string FormatTo2Dp(decimal myNumber)
        {
            // Use schoolboy rounding, not bankers.
            myNumber = Math.Round(myNumber, 2, MidpointRounding.AwayFromZero);

            return string.Format("{0:0.00}", myNumber);
        }
    }
}