using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Dtos;
using WebApplication1.ViewModels;

namespace WebApplication1.Models
{
    public class CBA
    {
        private ApplicationDbContext _context;

        public CBA()
        {
           _context = new ApplicationDbContext();
        }
        public static void AddReport(FinancialReportDto financialReportDto)
        {
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
    }
}