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

        public CBA()
        {
           _context = new ApplicationDbContext();
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
            var financialReport = new FinancialReport
            {
                ReportDate = financialReportDto.ReportDate,
                CreditAccount = financialReportDto.CreditAccount,
                CreditAmount = financialReportDto.CreditAmount,
                DebitAccount = financialReportDto.DebitAccount,
                DebitAmount = financialReportDto.DebitAmount,
                PostingType = financialReportDto.PostingType
                
            };
            _context.FinancialReports.Add(financialReport);
            _context.SaveChanges();
            
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