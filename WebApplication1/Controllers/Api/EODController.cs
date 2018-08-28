using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Dtos;
using WebApplication1.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Web;

namespace WebApplication1.Controllers.Api
{

    public class EODController : ApiController
    {
        private ApplicationDbContext _context;
        private string returnMsg = "Business : ";
        private string errorMsg = "";
        private CBA CBA;
        private GLAccount interestIncomeAcc;
        private GLAccount interestReceivableAcc;
        private GLAccount interestInSuspenseAcc;
        private GLAccount interestOverdueAcc;
        private GLAccount principalOverdueAcc;
        private float interestIncomeBalance;
        private float interestReceivableBalance;
        private float interestInSuspenseBalance;
        private float interestOverdueBalance;
        private float principalOverdueBalance;
        private string userId="";
        private DateTime lastFinancialDate;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        //protected SignInManager<ApplicationSignInManager> SignInManager { get; set; }
        public EODController()
        {
            _context = new ApplicationDbContext();
            lastFinancialDate = _context.FinancialDates
                                .OrderByDescending(p => p.EOD)
                                .FirstOrDefault()
                                .EOD;
            userId = HttpContext.Current.User.Identity.GetUserId();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            ApplicationUser user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);                            

            if (user != null)
            {
                RoleName.USER_NAME = user.FullName;
                RoleName.EMAIL = user.Email;
            }
        


            CBA = new CBA();
            interestIncomeAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.INTEREST_INCOME_ACC_NAME));
            interestReceivableAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.INTEREST_RECEIVABLE_ACC_NAME));
            interestInSuspenseAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.INTEREST_IN_SUSPENSE_ACC_NAME));
            interestOverdueAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.INTEREST_OVERDUE_ACC_NAME));
            principalOverdueAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.PRINCIPAL_OVERDUE_ACC_NAME));
            if (interestIncomeAcc != null)
            {
                interestIncomeBalance = interestIncomeAcc.AccountBalance;
            }

            if (interestReceivableAcc != null)
            {
                interestReceivableBalance = interestReceivableAcc.AccountBalance;
            }

            if (interestInSuspenseAcc != null)
            {
                interestInSuspenseBalance = interestInSuspenseAcc.AccountBalance;
            }

            if (interestOverdueAcc != null)
            {
                interestOverdueBalance = interestOverdueAcc.AccountBalance;
            }

            if (principalOverdueAcc != null)
            {
                principalOverdueBalance = principalOverdueAcc.AccountBalance;
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
        public bool CheckIfConfigComplete()
        {
            var isConfigComplete = false;
            var savingsAccountType = _context.AccountTypes.Where(c => c.Name.Equals("Savings Account")).FirstOrDefault();
            var currentAccountType = _context.AccountTypes.Where(c => c.Name.Equals("Current Account")).FirstOrDefault();
            var loanAccountType = _context.AccountTypes.Where(c => c.Name.Equals("Loan Account")).FirstOrDefault();

            if (savingsAccountType == null || savingsAccountType.CreditInterestRate == null ||
                savingsAccountType.InterestExpenseGLAccountId == null || savingsAccountType.MinimumBalance == null)
            {
                errorMsg = errorMsg + "Savings Account Type Configurations Incomplete";

            }
            if (currentAccountType == null || currentAccountType.CreditInterestRate == null ||
                currentAccountType.InterestExpenseGLAccountId == null || currentAccountType.MinimumBalance == null || currentAccountType.COT == null || currentAccountType.COTIncomeGLAccountId == null)
            {
                errorMsg = errorMsg + "Current Account Type Configurations Incomplete";

            }
            if (loanAccountType == null || loanAccountType.DebitInterestRate == null ||
                loanAccountType.InterestIncomeGLAccountId == null)
            {
                errorMsg = errorMsg + "Current Account Type Configurations Incomplete";

            }

            if (errorMsg.Equals(""))
            {
                isConfigComplete = true;
            }

            return isConfigComplete;

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
                if (CheckIfConfigComplete() == false)
                {

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
                }
                CloseBusiness(isEOMDone);
                if (!errorMsg.Equals(""))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
                }
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
        public void CloseBusiness(bool isEOMDone)
        {
            var financialDates = _context.FinancialDates.Count();

            // : Apply COT on Current Account Withdrawals;
            CalculateCOT();

            if (errorMsg.Equals(""))
            {

                InterestAccrual();

                // : If END OF FINANCIAL MONTH
                if (financialDates % 30 == 1)
                {

                    ValidateGLAccounts();

                    if (errorMsg == "")
                    {
                        RunEOM();
                        isEOMDone = true;

                    }
                    else
                    {
                        errorMsg = "<b>EOM Failed</b> :" + errorMsg;
                        return;
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
        }
      
        /**
         *  Perform Daily Interest Accrual
         */
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
                    float fullInterest = detail.LoanAmount * interestRate;
                    accruedInterest = (interestRate * principal) / (12 * 30);
                    if (detail.InterestIncome < fullInterest)
                    {
                        detail.InterestReceivable = detail.InterestReceivable + accruedInterest;    //DEBIT
                        detail.InterestIncome = detail.InterestIncome + accruedInterest; // CREDIT

                        // : Reflecting on GL Account Balance                       
                        interestReceivableBalance = interestReceivableBalance + accruedInterest;
                        interestIncomeBalance = interestIncomeBalance + accruedInterest;

                    }

                    _context.SaveChanges();

                    //ADD TO REPORT TABLE IN DB (DOUBLE ENTRY)
                    AddToReport("Interest Accrual", CBA.INTEREST_RECEIVABLE_ACC_NAME, CBA.INTEREST_INCOME_ACC_NAME, accruedInterest);
                    // returnMsg = returnMsg + "<br/> : <b>Interest Accrual </b> Process Successful";
                    returnMsg = returnMsg + "<br/> : <b>Interest Accrual </b> Process Successful";
                }
                
            }

            var savingsAccounts = _context.CustomerAccounts.Where(c => c.AccountTypeId == CBA.SAVINGS_ACCOUNT_TYPE_ID)
                .ToList();
            foreach (var acc in savingsAccounts)
            {
                var savingsInterestRate = _context.AccountTypes.Where(c => c.Id == CBA.SAVINGS_ACCOUNT_TYPE_ID)
                    .SingleOrDefault().CreditInterestRate;
                savingsInterestRate = savingsInterestRate / 100;
                var interestExpenseGLAccount = _context.GlAccounts
                    .Where(c => c.Name.Equals(CBA.INTEREST_EXPENSE_GL_ACCOUNT)).FirstOrDefault();
                acc.AccountBalance = acc.AccountBalance + (acc.AccountBalance * (float) savingsInterestRate);
                interestExpenseGLAccount.AccountBalance = interestExpenseGLAccount.AccountBalance -
                                                          (acc.AccountBalance * (float) savingsInterestRate);
                AddToReport("Savings Interest",CBA.INTEREST_EXPENSE_GL_ACCOUNT,acc.Name, (acc.AccountBalance * (float)savingsInterestRate));
                //returnMsg = returnMsg + "<br/> : <b>Savings Interest Accrual </b> Process Successful";
            }


        }
        [NonAction]
        public void ValidateGLAccounts()
        {

            if (interestIncomeAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create an <b>" + CBA.INTEREST_INCOME_ACC_NAME + "</b>";
            }
            if (interestInSuspenseAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create an <b>" + CBA.INTEREST_IN_SUSPENSE_ACC_NAME + "</b>";
            }
            if (interestOverdueAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create an <b>" + CBA.INTEREST_OVERDUE_ACC_NAME + "</b>";
            }
            if (interestReceivableAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create an <b>" + CBA.INTEREST_RECEIVABLE_ACC_NAME + "</b>";
            }
            if (principalOverdueAcc == null)
            {
                errorMsg = errorMsg + "<br/> : Please create a <b>" + CBA.PRINCIPAL_OVERDUE_ACC_NAME + "</b>";
            }
        }

       

        [NonAction]
        public void CalculateCOT()
        {
            var currentAccount = _context.AccountTypes.Where(c => c.Name.Equals("Current Account")).FirstOrDefault();
            var currentAccountId = _context.AccountTypes.Where(c => c.Name.Equals("Current Account")).FirstOrDefault().Id;
            var customerAccounts = _context.CustomerAccounts.Where(c => c.AccountTypeId == currentAccountId).ToList();
            var COTIncomeGLAccount = _context.GlAccounts.Where(c => c.Name.Equals(CBA.COT_INCOME_GL_ACCOUNT)).FirstOrDefault();
            var todaysDate = DateTime.Now;
            if (COTIncomeGLAccount != null)
            {
                foreach (var account in customerAccounts)
                {

                    float todayWithdrawal = 0;
                    float cotAmount = 0;
                    float customerAccountBalance = account.AccountBalance;
                   
                    var tellerPostings = _context.TellerPostings.Where(c => c.PostingType.Equals("Withdrawal") && c.CustomerAccountId == account.Id && c.TransactionDate > lastFinancialDate && c.TransactionDate < todaysDate).ToList();
                    // Total withdrawals made by customer today;
                    foreach (var posting in tellerPostings)
                    {
                       
                            todayWithdrawal = todayWithdrawal + posting.Amount;
                       
                    }

                    if (tellerPostings.Count != 0)
                    {
                        cotAmount = ((float)currentAccount.COT * customerAccountBalance) / 1000;
                        account.AccountBalance = customerAccountBalance - cotAmount;
                        COTIncomeGLAccount.AccountBalance = COTIncomeGLAccount.AccountBalance + cotAmount;

                        //ADD TO REPORT TABLE IN DB (DOUBLE ENTRY)
                        AddToReport("COT", account.Name + "Account", CBA.COT_INCOME_GL_ACCOUNT, cotAmount);
                        returnMsg = returnMsg + "<br/> : <b>COT</b> Calculated Successfully Successful";
                    }
                    else
                    {
                        returnMsg = returnMsg + "<br/> : <b>COT</b> Calculated Successfully Successful : No Withdrawals";
                    }

                    

                }

            }

            else
            {
                errorMsg = errorMsg + " No <b> " + CBA.COT_INCOME_GL_ACCOUNT + "Created </b>";
            }

        }

        // : EOM Process is carried out at the end of 30 Financal Days - 1 Financial Month
        [NonAction]
        public void RunEOM()
        {
            var customerAccounts = _context.CustomerAccounts.ToList();
            var loanDetails = _context.LoanDetails.Include(c => c.Terms).ToList();

            foreach (var loanDetail in loanDetails)
            {
                var customerAccount = customerAccounts.Where(c => c.AccountTypeId != CBA.LOAN_ACCOUNT_TYPE_ID).SingleOrDefault(c => c.LoanDetailsId == loanDetail.Id);
                var customerAccountBalance = customerAccount.AccountBalance;
                var minimumBalance = _context.AccountTypes.SingleOrDefault(c => c.Id == customerAccount.AccountTypeId).MinimumBalance; // Minimum amount this account type can have
                var loanAmount = loanDetail.LoanAmount; // Total amount 
                var monthlyInterest = (loanDetail.InterestRate * loanAmount) / 12; // Monthly interest to be repaid
                var monthlyPrincipal = (loanDetail.Terms.PaymentRate / 100) * loanAmount; // Monthly principal to be repaid
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

            // : If customer account balance sufficient to pay back FULL monthly principal and interest ATLEAST
            if (availableBalance >= amountPayable)
            {
                Console.WriteLine(availableBalance);
                GreaterThanOrEqualsToAmountPayable(customerAccount, loanDetail, monthlyPrincipal, monthlyInterest, availableBalance, amountPayable, principalOverdue, interestOverdue);
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
            // Customers Savings/Current Account Name
            var customerDebitAccountName = customerAccount.Name;
            // Customers Loan Account Name
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

                // : If PARTLY sufficient to pay back monthly interest
                if (monthlyInterest > availableBalance && customerAccount.AccountBalance > minimumBalance)
                {
                    customerAccount.AccountBalance = customerAccount.AccountBalance - availableBalance;   // DEBIT
                    loanDetail.InterestReceivable = loanDetail.InterestReceivable - availableBalance; // CREDIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense + (monthlyInterest - availableBalance);  // DEBIT
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue + (monthlyInterest - availableBalance); // CREDIT
                    loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue + monthlyPrincipal; // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan + monthlyPrincipal; // CREDIT

                    // : Reflecting on GL Account Balance                       
                    interestReceivableBalance = interestReceivableBalance - availableBalance;
                    interestInSuspenseBalance = interestInSuspenseBalance + (monthlyInterest - availableBalance);
                    interestOverdueBalance = interestOverdueBalance + (monthlyInterest - availableBalance);
                    principalOverdueBalance = principalOverdueBalance + monthlyPrincipal;

                    AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_RECEIVABLE_ACC_NAME, availableBalance);
                    AddToReport("EOM", CBA.INTEREST_IN_SUSPENSE_ACC_NAME, CBA.INTEREST_OVERDUE_ACC_NAME, (monthlyInterest - availableBalance));
                    AddToReport("EOM", CBA.PRINCIPAL_OVERDUE_ACC_NAME, customerLoanAccountName + " Account", monthlyPrincipal);
                }
                // : If sufficient to pay back the full monthly interest alone i.e minus monthly principal
                else if (availableBalance == monthlyInterest)
                {
                    customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyInterest;   // DEBIT
                    loanDetail.InterestReceivable = loanDetail.InterestReceivable - monthlyInterest; // CREDIT
                    loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue + monthlyPrincipal; // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan + monthlyPrincipal; // CREDIT

                    // : Reflecting on GL Account Balance                       
                    interestReceivableBalance = interestReceivableBalance - monthlyInterest;
                    principalOverdueBalance = principalOverdueBalance + monthlyPrincipal;

                    // Financial Report Entry
                    AddToReport("EOM", customerDebitAccountName, CBA.INTEREST_RECEIVABLE_ACC_NAME, monthlyInterest);
                    AddToReport("EOM", CBA.PRINCIPAL_OVERDUE_ACC_NAME, customerLoanAccountName + " Account", monthlyPrincipal);

                }
                // : If sufficient to pay back the FULL monthly interest and PART of monthly principal
                else if (availableBalance > monthlyInterest && availableBalance < amountPayable)
                {
                    var amountDeductible = availableBalance - monthlyInterest;
                    var amountUnpaid = amountPayable - (availableBalance - monthlyInterest);

                    customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyInterest; // DEBIT
                    loanDetail.InterestReceivable = loanDetail.InterestReceivable - monthlyInterest; // CREDIT
                    customerAccount.AccountBalance = customerAccount.AccountBalance - amountDeductible;   // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan - amountDeductible; // CREDIT
                    loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue + amountUnpaid; // DEBIT
                    loanDetail.CustomerLoan = loanDetail.CustomerLoan + amountUnpaid; // CREDIT

                    // : Reflecting on GL Account Balance                       
                    interestReceivableBalance = interestReceivableBalance - monthlyInterest;
                    principalOverdueBalance = principalOverdueBalance + amountUnpaid;

                    // : Financial Report Entry
                    AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_RECEIVABLE_ACC_NAME, monthlyInterest);
                    AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", amountDeductible);
                    AddToReport("EOM", CBA.PRINCIPAL_OVERDUE_ACC_NAME, customerLoanAccountName + " Account", amountUnpaid);

                }
            }

        }
        [NonAction]
        public void GreaterThanOrEqualsToAmountPayable(CustomerAccount customerAccount, LoanDetails loanDetail, float monthlyPrincipal, float monthlyInterest, float availableBalance, float amountPayable, float principalOverdue, float interestOverdue)
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

                // : Perform Last Interest Accrual before EOM
                PerformInterestAccrual(customerAccount, loanDetail, monthlyPrincipal, monthlyInterest,
                    customerDebitAccountName, customerLoanAccountName);

                // : If customer account balance sufficient to pay back FULL monthly principal and interest and the FULL interest overdue
                if (availableBalance >= (amountPayable + interestOverdue) && availableBalance < totalAmountPayable)
                {
                    var principalUnpaid = totalAmountPayable - availableBalance;
                    var amountPaid = (amountPayable + interestOverdue);
                    var principalPayable = availableBalance - amountPaid;

                    // Overdues
                    customerAccount.AccountBalance = customerAccount.AccountBalance - interestOverdue; // DEBIT
                    loanDetail.InterestIncome = loanDetail.InterestIncome - interestOverdue; // CREDIT 
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue - interestOverdue; // DEBIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense - interestOverdue; // CREDIT

                    // : Reflecting on GL Account Balance                       
                    interestIncomeBalance = interestIncomeBalance - interestOverdue;
                    interestOverdueBalance = interestOverdueBalance - interestOverdue;
                    interestInSuspenseBalance = interestInSuspenseBalance - interestOverdue;

                    // : Financial Report Entry
                    AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_INCOME_ACC_NAME, interestOverdue);
                    AddToReport("EOM", CBA.INTEREST_OVERDUE_ACC_NAME, CBA.INTEREST_IN_SUSPENSE_ACC_NAME, interestOverdue);




                    // : If sufficient to pay back FULL monthly principal and interest and the FULL interest overdue but PART of principal overdue
                    if (availableBalance > (amountPayable + interestOverdue))
                    {

                        loanDetail.CustomerLoan = loanDetail.CustomerLoan - principalPayable; // DEBIT
                        loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue - principalPayable; // CREDIT 

                        // : Reflecting on GL Account Balance                       
                        principalOverdueBalance = principalOverdueBalance - principalPayable;

                        // : Financial Report Entry
                        AddToReport("EOM", customerLoanAccountName + " Account", CBA.PRINCIPAL_OVERDUE_ACC_NAME, principalOverdue);



                    }

                }
                // : If customer account balance sufficient to pay back FULL monthly principal and interest and PART of the interest overdue but NOT principal overdue
                else if (availableBalance >= amountPayable && availableBalance < (amountPayable + interestOverdue))
                {
                    var interestUnpaid = (amountPayable + interestOverdue) - availableBalance;
                    var amountPaid = amountPayable;
                    var interestPayable = availableBalance - amountPaid;

                    // Overdues
                    customerAccount.AccountBalance = customerAccount.AccountBalance - interestPayable; // DEBIT
                    loanDetail.InterestIncome = loanDetail.InterestIncome - interestPayable; // CREDIT 
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue - interestPayable; // DEBIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense - interestPayable; // CREDIT

                    // : Reflecting on GL Account Balance                       
                    interestIncomeBalance = interestIncomeBalance - interestPayable;
                    interestOverdueBalance = interestOverdueBalance - interestPayable;
                    interestInSuspenseBalance = interestInSuspenseBalance - interestPayable;

                    // : Financial Report Entry
                    AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_INCOME_ACC_NAME, interestPayable);
                    AddToReport("EOM", CBA.INTEREST_OVERDUE_ACC_NAME, CBA.INTEREST_IN_SUSPENSE_ACC_NAME, interestPayable);



                }
                // : If customer account balance sufficient to settle TOTAL amount payable i.e plus interest overdue and principal overdue
                else if (availableBalance >= totalAmountPayable)
                {
                    // Overdues
                    customerAccount.AccountBalance = customerAccount.AccountBalance - interestOverdue; // DEBIT
                    loanDetail.InterestIncome = loanDetail.InterestIncome - interestOverdue; // CREDIT 
                    loanDetail.InterestOverdue = loanDetail.InterestOverdue - interestOverdue; // DEBIT
                    loanDetail.InterestInSuspense = loanDetail.InterestInSuspense - interestOverdue; // CREDIT
                    customerAccount.AccountBalance = customerAccount.AccountBalance - principalOverdue; // DEBIT
                    loanDetail.PrincipalOverdue = loanDetail.PrincipalOverdue - principalOverdue; // CREDIT 

                    // : Reflecting on GL Account Balance                       
                    interestIncomeBalance = interestIncomeBalance - interestOverdue;
                    interestOverdueBalance = interestOverdueBalance - interestOverdue;
                    interestInSuspenseBalance = interestInSuspenseBalance - interestOverdue;
                    principalOverdueBalance = principalOverdueBalance - principalOverdue;

                    // : Financial Report Entry
                    AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_INCOME_ACC_NAME, interestOverdue);
                    AddToReport("EOM", CBA.INTEREST_OVERDUE_ACC_NAME, CBA.INTEREST_IN_SUSPENSE_ACC_NAME, interestOverdue);
                    AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", principalOverdue);

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

        [NonAction]
        public void PerformInterestAccrual(CustomerAccount customerAccount, LoanDetails loanDetail, float monthlyPrincipal, float monthlyInterest, string customerDebitAccountName, string customerLoanAccountName)
        {

            customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyPrincipal;   // DEBIT
            loanDetail.CustomerLoan = loanDetail.CustomerLoan - monthlyPrincipal; // CREDIT
            customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyInterest; // DEBIT
            loanDetail.InterestReceivable = loanDetail.InterestReceivable - monthlyInterest; // CREDIT

            // : Reflecting on GL Account Balance                       
            interestReceivableBalance = interestReceivableBalance - monthlyInterest;

            // : Financial Report Entry
            AddToReport("EOM", customerDebitAccountName + " Account", customerLoanAccountName + " Account", monthlyPrincipal);
            AddToReport("EOM", customerDebitAccountName + " Account", CBA.INTEREST_RECEIVABLE_ACC_NAME, monthlyInterest);

        }


    }
}