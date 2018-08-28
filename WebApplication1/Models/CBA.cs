using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Dtos;
using WebApplication1.ViewModels;
using System.Data.Entity;
namespace WebApplication1.Models
{
    public class CBA
    {
        private ApplicationDbContext _context;
        public static string INTEREST_IN_SUSPENSE_ACC_NAME = "Interest-In-Suspense GL Account";
        public static string INTEREST_OVERDUE_ACC_NAME = "Interest Overdue GL Account";
        public static string INTEREST_RECEIVABLE_ACC_NAME = "Interest Receivable GL Account";
        public static string INTEREST_INCOME_ACC_NAME = "Interest Income GL Account";
        public static string PRINCIPAL_OVERDUE_ACC_NAME = "Principal Overdue GL Account";
        public static string COT_INCOME_GL_ACCOUNT = "COT Income GL Account";
        public static string INTEREST_EXPENSE_GL_ACCOUNT = "Interest Expense GL Account";
        public static int LOAN_ACCOUNT_TYPE_ID = 3;
        public static int CURRENT_ACCOUNT_TYPE_ID = 2;
        public static int SAVINGS_ACCOUNT_TYPE_ID = 1;

        public CBA()
        {
           _context = new ApplicationDbContext();
            var loanAccountType = _context.AccountTypes.Where(c => c.Name.Equals("Loan Account")).SingleOrDefault();
            var savingsAccountType = _context.AccountTypes.Where(c => c.Name.Equals("Savings Account")).SingleOrDefault();
            var currentAccountType = _context.AccountTypes.Where(c => c.Name.Equals("Current Account")).SingleOrDefault();
            if(loanAccountType!=null)
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
            Random random = new Random();
            const string chars = "01234567890123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void AddReport(FinancialReportDto financialReportDto)
        {
            financialReportDto.ReportDate = DateTime.Now;
           ApplicationDbContext _context = new ApplicationDbContext();
            var creditAccountCategory = GetCategory(financialReportDto.CreditAccount.ToString());
            var debitAccountCategory = GetCategory(financialReportDto.DebitAccount.ToString());
            var financialReport = new FinancialReport
            {
                ReportDate = financialReportDto.ReportDate,
                CreditAccount = financialReportDto.CreditAccount,
                CreditAmount = financialReportDto.CreditAmount,
                CreditAccountCategory = creditAccountCategory,
                DebitAccount = financialReportDto.DebitAccount,
                DebitAmount = financialReportDto.DebitAmount,
                DebitAccountCategory = debitAccountCategory,
                PostingType = financialReportDto.PostingType
                
            };
            _context.FinancialReports.Add(financialReport);
            _context.SaveChanges();
            
        }

        public static string GetCategory(string accountName)
        {
            var category = "";
            ApplicationDbContext _context = new ApplicationDbContext();
            var glAccount = _context.GlAccounts.Where(c => c.Name.Equals(accountName)).Include(c => c.GlCategories).SingleOrDefault();
            category = glAccount.GlCategories.MainAccountCategory.ToString();

            return category;
        }

        public static void BasedOnGLCategories(GLAccount glAccount,string type, long amount )
        {
            ApplicationDbContext  _context = new ApplicationDbContext();
            var glCategories = _context.GlCategories.Include(c=>c.Categories).SingleOrDefault(c=>c.Id==glAccount.GlCategoriesId);
            var category = _context.Categories.SingleOrDefault(c => c.Id == glCategories.CategoriesId);
            if (type == "Credit")
            {
                if (category.Name == "Asset" || category.Name == "Expense")
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
                if (category.Name == "Asset" || category.Name == "Expense")
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
            ApplicationDbContext _context = new ApplicationDbContext();
            var glCreditAccount = _context.GlAccounts.Include(c=>c.GlCategories).SingleOrDefault(c => c.Id == glPostingDto.GlCreditAccountId);
            var glDebitAccount = _context.GlAccounts.Include(c => c.GlCategories).SingleOrDefault(c => c.Id == glPostingDto.GlDebitAccountId);
           
            BasedOnGLCategories(glCreditAccount,"Credit",glPostingDto.CreditAmount);
            BasedOnGLCategories(glDebitAccount,"Debit",glPostingDto.DebitAmount);

            _context.SaveChanges();
            
            
        }
        
    }
}