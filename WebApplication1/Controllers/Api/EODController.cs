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
        GLAccount interestIncomeAcc;
        GLAccount interestReceivableAcc;
        GLAccount interestInSuspenseAcc;
        GLAccount interestOverdueAcc;
        GLAccount principalOverdueAcc;
        public EODController()
        {
            _context = new ApplicationDbContext();
            interestIncomeAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Interest Income Account"));
            interestReceivableAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Interest Receivable Account"));
            interestInSuspenseAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Interest-In-Suspense Account"));
            interestOverdueAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Interest Overdue Account"));
            principalOverdueAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Principal Overdue Account"));
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

            // : If Business is being Closed 
            if(businessStatusDto.IntendedAction.Equals("Close"))
            {
                CalculateCOT();
                if (!errorMsg.Equals(""))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
                }
                InterestAccrual();
                var financialDates = _context.FinancialDates.Count();

                // : If END OF FINANCIAL MONTH
                if(financialDates%2==0)
                {

                    ValidateGLAccounts();
                    if(errorMsg=="")
                    {
                        RunEOM();
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg+" EOM Failed");
                    }
                    
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
        public void ValidateGLAccounts()
        {

            if (interestIncomeAcc == null)
            {
                errorMsg = ": Please create an " + interestIncomeAcc.Name;
            }
            if (interestInSuspenseAcc == null)
            {
                errorMsg = ": Please create an " + interestInSuspenseAcc.Name;
            }
            if (interestOverdueAcc == null)
            {
                errorMsg = ": Please create an " + interestOverdueAcc.Name;
            }
            if (interestReceivableAcc == null)
            {
                errorMsg = ": Please create an " + interestReceivableAcc.Name;
            }
            if (principalOverdueAcc == null)
            {
                errorMsg = ": Please create an " + principalOverdueAcc.Name;
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

    
        [NonAction]
        public void InterestAccrual()
        {
            var loanDetails = _context.LoanDetails.ToList();
            float accruedInterest = 0;

            if (loanDetails != null)
            {
                foreach (var detail in loanDetails)
                {
                    float interestRate = (detail.InterestRate / 100);
                    float principal = detail.LoanAmount;
                    accruedInterest = (interestRate * principal) / (12 * 30);
                    detail.InterestIncome = detail.InterestReceivable + accruedInterest; //DEBIT
                    detail.InterestReceivable = detail.InterestReceivable + accruedInterest; // CREDIT  

                    _context.SaveChanges();

                    //ADD TO REPORT TABLE IN DB (DOUBLE ENTRY)
                    AddToReport("Interest Accrual", "Interest Receivable Account", "Interest Income Account", accruedInterest);


                    returnMsg = returnMsg + ", Interest Accrual Process Successful";

                }
            }


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
                        var tellerPostings = _context.TellerPostings.Where(c => c.PostingType.Equals("Withdrawal") && c.CustomerAccountId == account.Id).ToList();
                        foreach (var posting in tellerPostings)
                        {
                            if (posting.TransactionDate.Day == DateTime.Today.Day)
                            {
                                todayWithdrawal = todayWithdrawal + posting.Amount;
                            }

                        }

                        cotAmount = (5 * customerAccountBalance) / 1000;
                        account.AccountBalance = customerAccountBalance - cotAmount;
                        COTIncomeGLAccount.AccountBalance = COTIncomeGLAccount.AccountBalance + cotAmount;
                        //ADD TO REPORT TABLE IN DB (DOUBLE ENTRY)
                        AddToReport("COT", account.Name+"Account", "COT Income GL Account", cotAmount);

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
        public void RunEOM()
        {
            var customerAccounts = _context.CustomerAccounts.ToList();
            var loanDetails = _context.LoanDetails.ToList();
            
            foreach(var loanDetail in loanDetails)
            {
                var customerAccount = customerAccounts.SingleOrDefault(c => c.LoanDetailsId == loanDetail.Id);
                var customerAccountBalance = customerAccount.AccountBalance;               
                var minimumBalance = _context.AccountTypes.SingleOrDefault(c => c.Id == customerAccount.AccountTypeId).MinimumBalance;
                var loanAmount = loanDetail.LoanAmount; // Total amount 
                var monthlyInterest = (loanDetail.InterestRate * loanAmount) / 12; // Monthly interest to be repaid
                var monthlyPrincipal = loanDetail.Terms.PaymentRate * loanAmount; // Monthly principal to be repaid
                var amountPayable = monthlyPrincipal + monthlyInterest; // Total amount to be repaid monthly
                if(customerAccount.IsClosed==false)
                {
                    PerformDoubleEntry(loanDetail, customerAccount, minimumBalance, customerAccountBalance, monthlyPrincipal, monthlyInterest, loanAmount);
                }
               
            }
            

        }

        // : Perform corresponding DEBIT and CREDIT operations on account balance
        [NonAction]
        public void PerformDoubleEntry(LoanDetails loanDetail, CustomerAccount customerAccount, float? newMinimumBalance, float customerAccountBalance, float monthlyPrincipal, float monthlyInterest, float loanAmount)
        {
            float minimumBalance = (float)newMinimumBalance; // Minimum Balance this Account Type must have;
            float availableBalance = customerAccountBalance - minimumBalance; // Amount that can be taken out of customers account - DEBIT
            float principalOverdue = loanDetail.PrincipalOverdue; // Principal left unpaid
            float interestOverdue = loanDetail.InterestOverdue; // Interest Left unpaid
            float amountPayable = monthlyPrincipal + monthlyInterest; // Total amount to be paid(settled) at EOM
            float amountOverdue = principalOverdue + interestOverdue; // Total amount left unpaid
            float totalAmountPayable = amountPayable + amountOverdue; // Total amount to be paid(settled)

            // If customer account balance sufficient to pay back FULL monthly principal and interest ATLEAST
            if (availableBalance >= amountPayable)
            {

                GreaterThanOrEqualsToAmountPayable(customerAccount,loanDetail,monthlyPrincipal,monthlyInterest);
            }

            // If customer account balance sufficient to pay back FULL monthly principal and interest ALONE
            if (availableBalance > amountPayable)
            {

                GreaterThanAmountPayable(customerAccount, loanDetail, availableBalance, amountPayable, principalOverdue, interestOverdue);
                
            }       
            // : If customer account balance insufficient to pay back FULL monthly principal and interest
            else
            {
                LessThanOrEqualsToAmountPayable(customerAccount, loanDetail, monthlyPrincipal,monthlyInterest, availableBalance, amountPayable, minimumBalance);
            }
        }

        [NonAction]
        public void LessThanOrEqualsToAmountPayable(CustomerAccount customerAccount, LoanDetails loanDetail, float monthlyPrincipal, float monthlyInterest, float availableBalance, float amountPayable, float minimumBalance)
        {
            
           
            var customerDebitAccountName = customerAccount.Name;
            var customerLoanAccount = _context.CustomerAccounts.Where(c => c.AccountTypeId == 3 && c.CustomerId==customerAccount.CustomerId).ToList();
            var customerLoanAccountName="";
            bool isClosed = false;
            foreach (var acc in customerLoanAccount )
            {
                if(acc.IsClosed==true)
                {
                    isClosed = true;
                }
                isClosed = false;
                customerLoanAccountName = acc.Name;
            }
            if(isClosed==false)
            {
               
                // If PARTLY sufficient to pay back monthly principal
                if (monthlyPrincipal > availableBalance && availableBalance > minimumBalance)
                {
                    customerAccount.AccountBalance = customerAccount.AccountBalance - availableBalance;   // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan - availableBalance; // CREDIT
                    loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue + (monthlyPrincipal - availableBalance); // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan + (monthlyPrincipal - availableBalance); // CREDIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense + monthlyInterest; // DEBIT
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue + monthlyInterest; // CREDIT

                    AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", availableBalance);
                    AddToReport("EOM", "Principal Overdue Account", customerLoanAccountName + " Account", (monthlyPrincipal - availableBalance));
                    AddToReport("EOM", "Interest-In-Suspense Account", "Interest Overdue Account", monthlyInterest);
                }
                // : If sufficient to pay back the full monthly principal alone i.e minus monthly interest
                else if (availableBalance == monthlyPrincipal)
                {
                    customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyPrincipal;   // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan - monthlyPrincipal; // CREDIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense + monthlyInterest; // DEBIT
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue + monthlyInterest; // CREDIT

                    // Financial Report Entry
                    AddToReport("EOM", customerDebitAccountName+" Account", customerLoanAccountName+" Account", monthlyPrincipal);
                    AddToReport("EOM", "Interest-In-Suspense Account", "Interest Overdue Account", monthlyInterest);
                }
                // : If sufficient to pay back the FULL monthly principal and PART of monthly interest
                else if (availableBalance > monthlyPrincipal && availableBalance < amountPayable)
                {
                    var amountDeductible = availableBalance - monthlyPrincipal;
                    var amountUnpaid = amountPayable - (availableBalance - monthlyPrincipal);

                    customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyPrincipal;   // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan - monthlyPrincipal; // CREDIT
                    customerAccount.AccountBalance = customerAccount.AccountBalance - amountDeductible; // DEBIT
                    loanDetail.InterestReceivable = loanDetail.InterestReceivable - amountDeductible; // CREDIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense + amountUnpaid; // DEBIT
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue + amountUnpaid; // CREDIT

                    // Financial Report Entry
                    AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", monthlyPrincipal );
                    AddToReport("EOM", customerDebitAccountName + " Account", "Interest Receivable Account", amountDeductible);
                    AddToReport("EOM", "Interest-In-Suspense Account", "Interest Overdue Account", amountUnpaid);

                }
            }
            
        }

        [NonAction]
        public void GreaterThanOrEqualsToAmountPayable( CustomerAccount customerAccount, LoanDetails loanDetail, float monthlyPrincipal, float monthlyInterest)
        {


            var customerDebitAccountName = customerAccount.Name;
            var customerLoanAccount = _context.CustomerAccounts.Where(c => c.AccountTypeId == 3 && c.CustomerId == customerAccount.CustomerId).ToList();
            var customerLoanAccountName = "";
            bool isClosed = false;
            foreach (var acc in customerLoanAccount)
            {
                if (acc.IsClosed == true)
                {
                    isClosed = true;
                }
                isClosed = false;
                customerLoanAccountName = acc.Name;
            }
            if (isClosed == false)
            {
                customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyPrincipal;   // DEBIT
                loanDetail.CustomerLoan = loanDetail.CustomerLoan - monthlyPrincipal; // CREDIT
                customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyInterest; // DEBIT
                loanDetail.InterestReceivable = loanDetail.InterestReceivable - monthlyInterest; // CREDIT

                AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", monthlyPrincipal);
                AddToReport("EOM", customerDebitAccountName + " Account", "Interest Receivable Account", monthlyInterest);
            }

        }

        [NonAction]
        public void GreaterThanAmountPayable(CustomerAccount customerAccount, LoanDetails loanDetail, float availableBalance, float amountPayable, float principalOverdue, float interestOverdue )
        {
            var customerDebitAccountName = customerAccount.Name;
            var customerLoanAccount = _context.CustomerAccounts.Where(c => c.AccountTypeId == 3 && c.CustomerId == customerAccount.CustomerId).ToList();
            var customerLoanAccountName = "";
            bool isClosed = false;
            foreach (var acc in customerLoanAccount)
            {
                if (acc.IsClosed == true)
                {
                    isClosed = true;
                }
                isClosed = false;
                customerLoanAccountName = acc.Name;
            }
            if (isClosed == false)
            {
                float amountOverdue = principalOverdue + interestOverdue; // Total amount left unpaid
                float totalAmountPayable = amountPayable + amountOverdue; // Total amount to be paid(settled)

                // : If customer account balance sufficient to pay back FULL monthly principal and interest and the FULL principal overdue
                if (availableBalance >= (amountPayable + principalOverdue) && availableBalance < totalAmountPayable)
                {
                    var interestUnpaid = totalAmountPayable - availableBalance;
                    var amountPaid = (amountPayable + principalOverdue);
                    var interestPayable = availableBalance - amountPaid;

                    // Overdues
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan - principalOverdue; // DEBIT
                    loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue - principalOverdue; // CREDIT 
                    AddToReport("EOM", customerLoanAccountName + " Account", "Principal Overdue Account", principalOverdue);
                    
                    // If sufficient to pay back FULL monthly principal and interest and the FULL principal overdue but PART of interest overdue
                    if (availableBalance > (amountPayable + principalOverdue))
                    {
                        customerAccount.AccountBalance = customerAccount.AccountBalance - interestPayable; // DEBIT
                        loanDetail.InterestIncome = loanDetail.InterestIncome - interestPayable; // CREDIT 
                        loanDetail.InterestOverdue = loanDetail.InterestOverdue - interestPayable; // DEBIT
                        loanDetail.InterestInSuspense = loanDetail.InterestInSuspense - interestPayable; // CREDIT
                        AddToReport("EOM", customerDebitAccountName + " Account", "Interest Income Account", interestPayable);
                        AddToReport("EOM", "Interest Overdue Account", "Interest-In-Suspense Account", interestPayable);
                    }

                }
                // : If customer account balance sufficient to pay back FULL monthly principal and interest and PART of the principal overdue but NOT interest overdue
                else if (availableBalance >= amountPayable && availableBalance < (amountPayable + principalOverdue))
                {
                    var principalUnpaid = (amountPayable + principalOverdue) - availableBalance;
                    var amountPaid = amountPayable;
                    var principalPayable = availableBalance - amountPaid;


                    // Overdues
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan - principalPayable; // DEBIT
                    loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue - principalPayable; // CREDIT 
                    AddToReport("EOM", customerLoanAccountName + " Account", "Principal Overdue Account", principalPayable);

                }
                // : If customer account balance sufficient to settle TOTAL amount payable i.e plus interest overdue and principal overdue
                else if (availableBalance >= totalAmountPayable)
                {
                    // Overdues
                    customerAccount.AccountBalance = customerAccount.AccountBalance - principalOverdue; // DEBIT
                    loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue - principalOverdue; // CREDIT 
                    customerAccount.AccountBalance = customerAccount.AccountBalance - interestOverdue; // DEBIT
                    loanDetail.InterestIncome = loanDetail.InterestIncome - interestOverdue; // CREDIT 
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue - interestOverdue; // DEBIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense - interestOverdue; // CREDIT

                    AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", principalOverdue);
                    AddToReport("EOM", customerDebitAccountName + " Account", "Interest Income Account", interestOverdue);
                    AddToReport("EOM", "Interest Overdue Account", "Interest-In-Suspense Account", interestOverdue);
                }
            }
        }

        // Create Financial Report Entry
        [NonAction]
        public void AddToReport(string postingType, string debitAccountName, string creditAccountName, float amount)
        {
            var creditAmount = amount;
            var debitAmount = amount;
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


    }
}