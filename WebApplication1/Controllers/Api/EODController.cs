using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Dtos;
using WebApplication1.Models;
using System.Data.Entity;

namespace WebApplication1.Controllers.Api
{

    public class EODController : ApiController
    {
        private ApplicationDbContext _context;
        private string returnMsg = "Business : ";
        private string errorMsg = "";
        private CBA CBA;
        GLAccount interestIncomeAcc;
        GLAccount interestReceivableAcc;
        GLAccount interestInSuspenseAcc;
        GLAccount interestOverdueAcc;
        GLAccount principalOverdueAcc;
        public EODController()
        {
            _context = new ApplicationDbContext();
            CBA = new CBA();
            interestIncomeAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.INTEREST_INCOME_ACC_NAME));
            interestReceivableAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.INTEREST_RECEIVABLE_ACC_NAME));
            interestInSuspenseAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.INTEREST_IN_SUSPENSE_ACC_NAME));
            interestOverdueAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.INTEREST_OVERDUE_ACC_NAME));
            principalOverdueAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.PRINCIPAL_OVERDUE_ACC_NAME));
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/EOD/Start")]
        public HttpResponseMessage Start(BusinessStatusDto businessStatusDto)
        {

            var status = false;
            bool isEOMDone = false;
            var businessStatus = _context.BusinessStatus.FirstOrDefault();

            // : If Business is being Closed 
            if (businessStatusDto.IntendedAction.Equals("Close"))
            {
                var financialDates = _context.FinancialDates.Count();

                CalculateCOT();

                if (!errorMsg.Equals(""))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
                }

                InterestAccrual();

                // : If END OF FINANCIAL MONTH
                if (financialDates % 2 == 1)
                {

                    ValidateGLAccounts();

                    if (errorMsg == "")
                    {
                        RunEOM();
                        isEOMDone = true;

                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "<b>EOM Failed</b> :" + errorMsg);
                    }

                }
                _context.SaveChanges();

                if (isEOMDone == false)
                {
                    var financialDate = new FinancialDates
                    {
                        EOD = DateTime.Now
                    };
                    _context.FinancialDates.Add(financialDate);
                    _context.SaveChanges();
                }
                else
                {
                    var financialDate = new FinancialDates
                    {
                        EOD = DateTime.Now,
                        EOM = DateTime.Now
                    };
                    _context.FinancialDates.Add(financialDate);
                    _context.SaveChanges();
                    returnMsg = returnMsg + "<br/> : <b>EOM</b> Completed";
                }
                returnMsg = returnMsg + "<br/> : Business <b>Closed</b>";

            }
            else
            {
                status = true;
                returnMsg = returnMsg + "<br/> : Business <b>Opened</b>";
            }


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
            return Request.CreateResponse(HttpStatusCode.OK, returnMsg);

        }

        [NonAction]
        public void CloseBusiness()
        {

        }

        [NonAction]
        public void ValidateGLAccounts()
        {

            if (interestIncomeAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create an <b>Interest Income GL Account</b>";
            }
            if (interestInSuspenseAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create an <b>Interest-In-Suspense GL Account</b>";
            }
            if (interestOverdueAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create an <b>Interest Overdue GL Account</b>";
            }
            if (interestReceivableAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create an <b>Interest Receivable GL Account</b>";
            }
            if (principalOverdueAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create a <b>Principal Overdue GL Account</b>";
            }
        }

        // GET api/<controller>
        [AcceptVerbs("GET", "POST")]
        [Route("api/EOD/GetStatus")]
        public HttpResponseMessage GetStatus(BusinessStatusDto businessStatusDto)
        {
            var business = _context.BusinessStatus.FirstOrDefault();
            var businessStatus = business.Status;
            var status = "";
            if (businessStatus == true)
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
                    AddToReport("Interest Accrual", "Interest Receivable GL Account", "Interest Income GL Account", accruedInterest);
                    returnMsg = returnMsg + "<br/> : <b>Interest Accrual </b> Process Successful";

                }
            }


        }

        [NonAction]
        public void CalculateCOT()
        {
            var currentAccountId = _context.AccountTypes.Where(c => c.Name.Equals("Current Account")).FirstOrDefault().Id;
            var customerAccounts = _context.CustomerAccounts.Where(c => c.AccountTypeId == currentAccountId).ToList();
            var COTIncomeGLAccount = _context.GlAccounts.Where(c => c.Name.Equals("COT Income GL Account")).FirstOrDefault();


            if (COTIncomeGLAccount != null)
            {
                foreach (var account in customerAccounts)
                {

                    float todayWithdrawal = 0;
                    float cotAmount;
                    float customerAccountBalance = account.AccountBalance;
                    var tellerPostings = _context.TellerPostings.Where(c => c.PostingType.Equals("Withdrawal") && c.CustomerAccountId == account.Id).ToList();
                    // Total withdrawals made by customer today;
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
                    AddToReport("COT", account.Name + "Account", "COT Income GL Account", cotAmount);

                    returnMsg = returnMsg + "<br/> : <b>COT</b> Calculated Successfully Successful";

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
            var loanDetails = _context.LoanDetails.Include(c => c.Terms).ToList();

            foreach (var loanDetail in loanDetails)
            {
                var customerAccount = customerAccounts.Where(c => c.AccountTypeId != CBA.LOAN_ACCOUNT_TYPE_ID).SingleOrDefault(c => c.LoanDetailsId == loanDetail.Id);
                var customerAccountBalance = customerAccount.AccountBalance;
                var minimumBalance = _context.AccountTypes.SingleOrDefault(c => c.Id == customerAccount.AccountTypeId).MinimumBalance;
                var loanAmount = loanDetail.LoanAmount; // Total amount 
                var monthlyInterest = (loanDetail.InterestRate * loanAmount) / 12; // Monthly interest to be repaid
                var monthlyPrincipal = (loanDetail.Terms.PaymentRate/100) * loanAmount; // Monthly principal to be repaid
                var amountPayable = monthlyPrincipal + monthlyInterest; // Total amount to be repaid monthly
                if (customerAccount.IsClosed == false)
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

                Console.WriteLine(availableBalance);
                GreaterThanOrEqualsToAmountPayable(customerAccount, loanDetail, monthlyPrincipal, monthlyInterest);
            }

            // If customer account balance sufficient to pay back FULL monthly principal and interest ALONE
            if (availableBalance > amountPayable)
            {

                GreaterThanAmountPayable(customerAccount, loanDetail, availableBalance, amountPayable, principalOverdue, interestOverdue);

            }
            // : If customer account balance insufficient to pay back FULL monthly principal and interest
            else
            {
                LessThanOrEqualsToAmountPayable(customerAccount, loanDetail, monthlyPrincipal, monthlyInterest, availableBalance, amountPayable, minimumBalance);
            }
        }

        [NonAction]
        public void LessThanOrEqualsToAmountPayable(CustomerAccount customerAccount, LoanDetails loanDetail, float monthlyPrincipal, float monthlyInterest, float availableBalance, float amountPayable, float minimumBalance)
        {

            var customerDebitAccountName = customerAccount.Name;
            var customerLoanAccount = _context.CustomerAccounts.Where(c => c.AccountTypeId == CBA.LOAN_ACCOUNT_TYPE_ID && c.CustomerId == customerAccount.CustomerId).ToList();
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

                // If PARTLY sufficient to pay back monthly principal
                if (monthlyPrincipal > availableBalance && customerAccount.AccountBalance > minimumBalance)
                {
                    customerAccount.AccountBalance = customerAccount.AccountBalance - availableBalance;   // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan - availableBalance; // CREDIT
                    loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue + (monthlyPrincipal - availableBalance); // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan + (monthlyPrincipal - availableBalance); // CREDIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense + monthlyInterest; // DEBIT
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue + monthlyInterest; // CREDIT

                    AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", availableBalance);
                    AddToReport("EOM", CBA.PRINCIPAL_OVERDUE_ACC_NAME, customerLoanAccountName + " Account", (monthlyPrincipal - availableBalance));
                    AddToReport("EOM", CBA.INTEREST_IN_SUSPENSE_ACC_NAME, CBA.INTEREST_OVERDUE_ACC_NAME, monthlyInterest);
                }
                // : If sufficient to pay back the full monthly principal alone i.e minus monthly interest
                else if (availableBalance == monthlyPrincipal)
                {
                    customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyPrincipal;   // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan - monthlyPrincipal; // CREDIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense + monthlyInterest; // DEBIT
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue + monthlyInterest; // CREDIT

                    // Financial Report Entry
                    AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", monthlyPrincipal);
                    AddToReport("EOM", CBA.INTEREST_IN_SUSPENSE_ACC_NAME, CBA.INTEREST_OVERDUE_ACC_NAME, monthlyInterest);
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
                    AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", monthlyPrincipal);
                    AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_RECEIVABLE_ACC_NAME, amountDeductible);
                    AddToReport("EOM", CBA.INTEREST_IN_SUSPENSE_ACC_NAME, CBA.INTEREST_OVERDUE_ACC_NAME, amountUnpaid);

                }
            }

        }

        [NonAction]
        public void GreaterThanOrEqualsToAmountPayable(CustomerAccount customerAccount, LoanDetails loanDetail, float monthlyPrincipal, float monthlyInterest)
        {


            var customerDebitAccountName = customerAccount.Name;
            var customerLoanAccount = _context.CustomerAccounts.Where(c => c.AccountTypeId == CBA.LOAN_ACCOUNT_TYPE_ID && c.CustomerId == customerAccount.CustomerId).ToList();
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
                AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_RECEIVABLE_ACC_NAME, monthlyInterest);
            }

        }

        [NonAction]
        public void GreaterThanAmountPayable(CustomerAccount customerAccount, LoanDetails loanDetail, float availableBalance, float amountPayable, float principalOverdue, float interestOverdue)
        {
            var customerDebitAccountName = customerAccount.Name;
            var customerLoanAccount = _context.CustomerAccounts.Where(c => c.AccountTypeId == CBA.LOAN_ACCOUNT_TYPE_ID && c.CustomerId == customerAccount.CustomerId).ToList();
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
                    AddToReport("EOM", customerLoanAccountName + " Account", CBA.PRINCIPAL_OVERDUE_ACC_NAME, principalOverdue);

                    // If sufficient to pay back FULL monthly principal and interest and the FULL principal overdue but PART of interest overdue
                    if (availableBalance > (amountPayable + principalOverdue))
                    {
                        customerAccount.AccountBalance = customerAccount.AccountBalance - interestPayable; // DEBIT
                        loanDetail.InterestIncome = loanDetail.InterestIncome - interestPayable; // CREDIT 
                        loanDetail.InterestOverdue = loanDetail.InterestOverdue - interestPayable; // DEBIT
                        loanDetail.InterestInSuspense = loanDetail.InterestInSuspense - interestPayable; // CREDIT
                        AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_INCOME_ACC_NAME, interestPayable);
                        AddToReport("EOM", CBA.INTEREST_OVERDUE_ACC_NAME, CBA.INTEREST_IN_SUSPENSE_ACC_NAME, interestPayable);
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
                    AddToReport("EOM", customerLoanAccountName + " Account", CBA.PRINCIPAL_OVERDUE_ACC_NAME, principalPayable);

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
                    AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_INCOME_ACC_NAME, interestOverdue);
                    AddToReport("EOM", CBA.INTEREST_OVERDUE_ACC_NAME, CBA.INTEREST_IN_SUSPENSE_ACC_NAME, interestOverdue);
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