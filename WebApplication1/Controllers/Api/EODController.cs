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
using WebApplication1.Migrations;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers.Api
{

    public class EODController : ApiController
    {
        private ApplicationDbContext _context;
        private string returnMsg = "<b>Business</b> ";
        private string errorMsg = "";
        private CBA CBA;
        private bool isApplied;
        private GLAccount interestIncomeAcc;
        private GLAccount interestReceivableAcc;
        private GLAccount interestInSuspenseAcc;
        private GLAccount interestOverdueAcc;
        private GLAccount principalOverdueAcc;
        private GLAccount customerLoanAcc;
        public GLAccount cotIncomeReceivableAcc { get; set; }
        private string userId = "";
        private DateTime lastFinancialDate;
        private List<FinancialDates> financialDates;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        //protected SignInManager<ApplicationSignInManager> SignInManager { get; set; }
        public EODController()
        {
            _context = new ApplicationDbContext();
            isApplied = false;

            financialDates = _context.FinancialDates.ToList();
            var EOD = DateTime.Now;
            if (financialDates.Count == 0)
            {
                lastFinancialDate = EOD;

            }
            else
            {
                lastFinancialDate = financialDates.OrderByDescending(p => p.EOD).FirstOrDefault().EOD;
            }


            userId = HttpContext.Current.User.Identity.GetUserId();


            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            ApplicationUser user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);
            if (userId == null || userId.Equals(""))
            {
                var userAcc = _context.Users.SingleOrDefault(c => c.Email.Equals(RoleName.EMAIL));
                if (userAcc != null)
                {
                    userId = userAcc.Id;
                }
                else
                {
                    userId = user.Id;
                }

            }
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
            customerLoanAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.CUSTOMER_LOAN_ACCOUNT));
            cotIncomeReceivableAcc = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals(CBA.COT_INCOME_RECEIVABLE_GL_ACC_NAME));



        }
        // GET api/<controller>
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/EOD/SaveConfig")]
        public HttpResponseMessage SaveConfig(EodConfigDto eodConfigDto)
        {
            var startTime = ConvertToTime(eodConfigDto.StartTimeHour, eodConfigDto.StartTimeMins, eodConfigDto.StartTimePeriod);
            var endTime = ConvertToTime(eodConfigDto.EndTimeHour, eodConfigDto.EndTimeMins, eodConfigDto.EndTimePeriod);
            var fixedTime = ConvertToTime(eodConfigDto.FixedTimeHour, eodConfigDto.FixedTimeMins, eodConfigDto.FixedTimePeriod);

            var eodConfig = _context.EODConfig.FirstOrDefault();
            eodConfig.EODTime = fixedTime;
            eodConfig.EndTime = endTime;
            eodConfig.StartTime = startTime;
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "Updated Successfully");
        }

        // GET api/<controller>
        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/EOD/EODConfig")]
        public HttpResponseMessage EODConfig()
        {
            var startTimeHour = "06";
            var startTimeMins = "00";
            var startTimePeriod = "PM";
            var endTimeHour = "07";
            var endTimeMins = "00";
            var endTimePeriod = "AM";
            var fixedTimeHour = "12";
            var fixedTimeMins = "00";
            var fixedTimePeriod = "AM";
            var list = new List<string>();
            var eodConfig = _context.EODConfig.FirstOrDefault();
            if (eodConfig == null)
            {
                var eodCfg = new EODConfig()
                {
                    EndTime = "07:00",
                    EODTime = "00:00",
                    StartTime = "18:00",
                    IsRunning = false

                };
                _context.EODConfig.Add(eodCfg);
                _context.SaveChanges();
                list.Add(startTimeHour);
                list.Add(startTimeMins);
                list.Add(startTimePeriod);
                list.Add(endTimeHour);
                list.Add(endTimeMins);
                list.Add(endTimePeriod);
                list.Add(fixedTimeHour);
                list.Add(fixedTimeMins);
                list.Add(fixedTimePeriod);
            }

            var endTime = eodConfig.EndTime;
            var startTime = eodConfig.StartTime;
            var fixedTime = eodConfig.EODTime;
            startTimeHour = GetHours(startTime);
            startTimeMins = GetMins(startTime);
            startTimePeriod = GetPeriod(startTime);
            endTimeHour = GetHours(endTime);
            endTimeMins = GetMins(endTime);
            endTimePeriod = GetPeriod(endTime);
            fixedTimeHour = GetHours(fixedTime);
            fixedTimeMins = GetMins(fixedTime);
            fixedTimePeriod = GetPeriod(fixedTime);
            list.Add(startTimeHour);
            list.Add(startTimeMins);
            list.Add(startTimePeriod);
            list.Add(endTimeHour);
            list.Add(endTimeMins);
            list.Add(endTimePeriod);
            list.Add(fixedTimeHour);
            list.Add(fixedTimeMins);
            list.Add(fixedTimePeriod);
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        // GET api/<controller>
        [AcceptVerbs("GET", "POST")]
        [Route("api/EOD/GetStatus")]
        public HttpResponseMessage GetStatus(BusinessStatusDto businessStatusDto)
        {
            var business = _context.BusinessStatus.FirstOrDefault();

            if (business == null)
            {
                var businessStat = new BusinessStatus()
                {
                    Status = false
                };
                _context.BusinessStatus.Add(businessStat);
                _context.SaveChanges();
            }

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

        public class Entries
        {
            public string Narration { get; set; }
            public float Amount { get; set; }
            public DateTime Date { get; set; }
        }
        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/EOD/ViewTransaction")]
        public HttpResponseMessage ViewTransaction([FromUri]int id)
        {
            var entries = new List<Entries>();
            var customerAccount = _context.CustomerAccounts.SingleOrDefault(c => c.Id == id);
            var financialReports = _context.FinancialReports.Where(c => c.CreditAccount.Equals(customerAccount.Name) || c.DebitAccount.Equals(customerAccount.Name)).ToList();
            Entries entry = null; 
            foreach (var report in financialReports)
            {
                if (report.CreditAmount != 0)
                {
                    if (report.PostingType.Equals("Teller Posting"))
                    {
                        if (report.CreditAccount.Equals(customerAccount.Name))
                        {
                            entry = new Entries()
                            {
                                Narration = "Deposit",
                                Amount = report.CreditAmount,
                                Date = (DateTime) report.ReportDate

                            };
                        }
                        else
                        {
                            entry = new Entries()
                            {
                                Narration = "Withdrawal",
                                Amount = report.DebitAmount,
                                Date = (DateTime) report.ReportDate

                            };
                        }

                    }
                    else
                    {
                        entry = new Entries()
                        {
                            Narration = report.PostingType,
                            Amount = report.DebitAmount,
                            Date = (DateTime) report.ReportDate

                        };
                    }
                }

                entries.Add(entry);
            }

            return Request.CreateResponse(HttpStatusCode.OK,entries);
        }
        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/EOD/ViewGLTransaction")]
        public HttpResponseMessage ViewGLTransaction([FromUri]int id)
        {
            
            var glAccount = _context.GlAccounts.SingleOrDefault(c => c.Id == id);
            var financialReports = _context.FinancialReports.Where(c => c.CreditAccount.Equals(glAccount.Name) || c.DebitAccount.Equals(glAccount.Name)).ToList();

            
            return Request.CreateResponse(HttpStatusCode.OK, financialReports);
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/EOD/Start")]
        //        [Authorize(Roles = RoleName.ADMIN_ROLE)]
        public HttpResponseMessage Start(BusinessStatusDto businessStatusDto)
        {
            var nowHour = DateTime.Now.Hour * 100;
            var nowMin = DateTime.Now.Minute;
            var eodConfig = _context.EODConfig.FirstOrDefault();
            var startTime = eodConfig.StartTime;
            var endTime = eodConfig.EndTime;
            var startHour = int.Parse(startTime.Substring(0, 2)) * 100;
            var startMin = int.Parse(GetMins(startTime));
            var endHour = int.Parse(endTime.Substring(0, 2)) * 100;
            var endMin = int.Parse(GetMins(endTime));
            isApplied = true;
            var status = false;
            bool isEOMDone = false;
            var businessStatus = _context.BusinessStatus.FirstOrDefault();
            ValidateGLAccounts();
            // : If Business is being Closed 
            if (businessStatusDto.IntendedAction.Equals("Close"))
            {

                if (((nowHour + nowMin) >= (endHour + endMin) && (nowHour + nowMin) <= (startHour + startMin)))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry you cannot run EOD at this Time, Go To <a href='/EODConfig'><b>EODConfig</b></a> to change Time settings ");
                }
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
                //                if (((nowHour + nowMin) < (endHour + endMin) && (nowHour + nowMin) > (startHour + startMin)))
                //                {
                //                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry you cannot Open EOD at this Time, Go To <a href='/EODConfig'><b>EODConfig</b></a> to change Time settings ");
                //                }
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
        public bool CheckIfConfigComplete()
        {
            var isConfigComplete = false;
            var savingsAccountType = _context.AccountTypes.Where(c => c.Name.Equals("Savings Account")).FirstOrDefault();
            var currentAccountType = _context.AccountTypes.Where(c => c.Name.Equals("Current Account")).FirstOrDefault();
            var loanAccountType = _context.AccountTypes.Where(c => c.Name.Equals("Loan Account")).FirstOrDefault();

            if (savingsAccountType?.CreditInterestRate == null || savingsAccountType.InterestExpenseGLAccountId == null || savingsAccountType.MinimumBalance == null)
            {
                errorMsg = errorMsg + "Savings Account Type Configurations Incomplete";

            }
            if (currentAccountType?.CreditInterestRate == null || currentAccountType.InterestExpenseGLAccountId == null || currentAccountType.MinimumBalance == null || currentAccountType.COT == null || currentAccountType.COTIncomeGLAccountId == null)
            {
                errorMsg = errorMsg + "Current Account Type Configurations Incomplete";

            }
            if (loanAccountType?.DebitInterestRate == null || loanAccountType.InterestIncomeGLAccountId == null)
            {
                errorMsg = errorMsg + "Current Account Type Configurations Incomplete";

            }

            if (errorMsg.Equals(""))
            {
                isConfigComplete = true;
            }

            return isConfigComplete;

        }

        [NonAction]
        public EODConfig GetEODConfig()
        {
            var eodConfig = _context.EODConfig.FirstOrDefault();
            return eodConfig;
        }

        [NonAction]
        public void CloseBusiness(bool isEOMDone)
        {
            var financialDates = _context.FinancialDates.Count();
            // Apply Settlement between Till Accounts and Capital Account
            SettleTill();
            if (errorMsg.Equals(""))
            {
                // : Apply COT on Current Account Withdrawals;
                // COTAccrual();
                COTApplied();
                if (errorMsg.Equals(""))
                {
                    //InterestAccrual();
                    InterestApplied();
                    //InterestAccrual();
                    if (errorMsg.Equals(""))
                    {
                        // InterestExpenseAccrual();
                        InterestExpenseApplied();
                        if (errorMsg.Equals(""))
                        {
                            RunAppliedEOM();
                            if (errorMsg.Equals(""))
                            {

                                // : If END OF FINANCIAL MONTH
                                if (financialDates % 30 == 1 && financialDates >= 29)
                                {

                                    ValidateGLAccounts();

                                    if (errorMsg == "")
                                    {
                                        if (isApplied == false)
                                        {
                                            RunEOM();
                                            isEOMDone = true;
                                        }


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
                    }
                }
            }
        }

        [NonAction]
        public void COTAccrual()
        {
            var currentAccountType = _context.AccountTypes.FirstOrDefault(c => c.Name.Equals("Current Account"));
            var currentAccountTypeId = currentAccountType.Id;
            var cot = (currentAccountType.COT) / 100;
            var customerAccounts =
                _context.CustomerAccounts.Where(c => c.AccountTypeId == currentAccountTypeId).ToList();
            var COTIncomeGLAccount = _context.GlAccounts.FirstOrDefault(c => c.Id == currentAccountType.COTIncomeGLAccountId);
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            var todayDate = DateTime.Now;

            if (COTIncomeGLAccount != null)
            {
                foreach (var account in customerAccounts)
                {

                    float todayWithdrawal = 0;
                    var customerAccountBalance = account.AccountBalance;

                    var tellerPostings = _context.TellerPostings.Where(c =>
                        c.PostingType.Equals("Withdrawal") && c.CustomerAccountId == account.Id &&
                        c.TransactionDate >= lastFinancialDate && c.TransactionDate <= todayDate).ToList();
                    // Total withdrawals made by customer today;
                    foreach (var posting in tellerPostings)
                    {

                        todayWithdrawal = todayWithdrawal + posting.Amount;

                    }

                    if (tellerPostings.Count != 0)
                    {
                        if (currentAccountType != null)
                        {
                            if (cot != null)
                            {
                                var cotAmount = (float)cot * todayWithdrawal;
                                if (capitalAccount != null && capitalAccount.AccountBalance >= cotAmount)
                                {

                                    cotIncomeReceivableAcc.AccountBalance =
                                        cotIncomeReceivableAcc.AccountBalance + cotAmount; // DEBIT
                                    COTIncomeGLAccount.AccountBalance =
                                        COTIncomeGLAccount.AccountBalance + cotAmount; // CREDIT
                                    account.COTIncome = account.COTIncome + cotAmount;


                                    //ADD TO REPORT TABLE IN DB (DOUBLE ENTRY)

                                    AddToReport("COT", cotIncomeReceivableAcc.Name, CBA.COT_INCOME_GL_ACCOUNT, cotAmount);
                                    AddGLPosting(cotAmount, COTIncomeGLAccount, cotIncomeReceivableAcc);
                                    // AddToReport("COT", CBA.COT_INCOME_GL_ACCOUNT, "Capital Account", cotAmount);
                                }
                                else
                                {
                                    errorMsg = errorMsg + "<br/> Insufficient Capital Account Balance";
                                }
                            }
                        }

                        returnMsg = returnMsg + "<br/> : <b>COT</b> Calculated Successfully Successful";
                    }
                    else
                    {
                        returnMsg = returnMsg + "<br/> : <b>COT</b> Calculated Successfully : No Withdrawals";
                    }



                }

            }

            else
            {
                errorMsg = errorMsg + " No <b> " + CBA.COT_INCOME_GL_ACCOUNT + "Created </b>";
            }

        }

        [NonAction]
        public void COTApplied()
        {
            var currentAccountType = _context.AccountTypes.FirstOrDefault(c => c.Name.Equals("Current Account"));
            var currentAccountTypeId = currentAccountType.Id;
            var cot = (currentAccountType.COT) / 100;
            var customerAccounts = _context.CustomerAccounts.Where(c => c.AccountTypeId == currentAccountTypeId).ToList();
            var COTIncomeGLAccount = _context.GlAccounts.FirstOrDefault(c => c.Name.Equals(CBA.COT_INCOME_GL_ACCOUNT));
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            var todayDate = DateTime.Now;

            if (COTIncomeGLAccount != null)
            {
                foreach (var account in customerAccounts)
                {

                    float todayWithdrawal = 0;
                    var customerAccountBalance = account.AccountBalance;

                    var tellerPostings = _context.TellerPostings.Where(c => c.PostingType.Equals("Withdrawal") && c.CustomerAccountId == account.Id && c.TransactionDate >= lastFinancialDate && c.TransactionDate <= todayDate).ToList();
                    // Total withdrawals made by customer today;
                    foreach (var posting in tellerPostings)
                    {

                        todayWithdrawal = todayWithdrawal + posting.Amount;

                    }

                    if (tellerPostings.Count != 0)
                    {
                        if (currentAccountType != null)
                        {
                            if (cot != null)
                            {
                                var cotAmount = (float)cot * todayWithdrawal;
                                if (capitalAccount != null && capitalAccount.AccountBalance >= cotAmount)
                                {

                                    cotIncomeReceivableAcc.AccountBalance = cotIncomeReceivableAcc.AccountBalance + cotAmount; // DEBIT
                                    COTIncomeGLAccount.AccountBalance = COTIncomeGLAccount.AccountBalance + cotAmount; // CREDIT
                                    account.AccountBalance = account.AccountBalance - cotAmount; // DEBIT
                                    cotIncomeReceivableAcc.AccountBalance = cotIncomeReceivableAcc.AccountBalance - cotAmount; // CREDIT
                                                                                                                               //                                    COTIncomeGLAccount.AccountBalance = COTIncomeGLAccount.AccountBalance - cotAmount; // DEBIT
                                                                                                                               //                                    capitalAccount.AccountBalance = capitalAccount.AccountBalance + cotAmount; // CREDIT

                                    //ADD TO REPORT TABLE IN DB (DOUBLE ENTRY)

                                    AddToReport("COT", cotIncomeReceivableAcc.Name, COTIncomeGLAccount.Name, cotAmount);
                                    AddToReport("COT", account.Name, cotIncomeReceivableAcc.Name, cotAmount);
                                    var glPosting = new GLPostings()
                                    {
                                        GlDebitAccountId = cotIncomeReceivableAcc.Id,
                                        DebitNarration = "Debit from " + cotIncomeReceivableAcc.Name,
                                        DebitAmount = cotAmount,
                                        GlCreditAccountId = COTIncomeGLAccount.Id,
                                        CreditNarration = "Credit to " + COTIncomeGLAccount.Name,
                                        CreditAmount = cotAmount,
                                        TransactionDate = DateTime.Now,
                                        UserAccountId = userId

                                    };
                                    _context.GlPostings.Add(glPosting);
                                    _context.SaveChanges();
                                    AddGLPosting(cotAmount, COTIncomeGLAccount, cotIncomeReceivableAcc);
                                }
                                else
                                {
                                    errorMsg = errorMsg + "<br/> Insufficient Capital Account Balance";
                                }
                            }
                        }

                        returnMsg = returnMsg + "<br/> : <b>COT</b> Calculated Successfully Successful";
                    }
                    else
                    {
                        returnMsg = returnMsg + "<br/> : <b>COT</b> Calculated Successfully : No Withdrawals";
                    }



                }

            }

            else
            {
                errorMsg = errorMsg + " No <b> " + CBA.COT_INCOME_GL_ACCOUNT + "Created </b>";
            }

        }

        [NonAction]
        public void COTRepayment()
        {
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            var currentAccountType = _context.AccountTypes.FirstOrDefault(c => c.Name.Equals("Current Account"));
            if (currentAccountType != null)
            {
                var currentAccountTypeId = currentAccountType.Id;
            }

            var COTIncomeGLAccount = _context.GlAccounts.FirstOrDefault(c => c.Id == currentAccountType.COTIncomeGLAccountId);
            var customerAccounts = _context.CustomerAccounts.Where(c => c.AccountTypeId == CBA.CURRENT_ACCOUNT_TYPE_ID).ToList();
            foreach (var customerAccount in customerAccounts)
            {
                customerAccount.AccountBalance = customerAccount.AccountBalance - customerAccount.COTIncome; // DEBIT
                cotIncomeReceivableAcc.AccountBalance = cotIncomeReceivableAcc.AccountBalance - customerAccount.COTIncome; // CREDIT
                COTIncomeGLAccount.AccountBalance = COTIncomeGLAccount.AccountBalance - customerAccount.COTIncome; // DEBIT
                capitalAccount.AccountBalance = capitalAccount.AccountBalance + customerAccount.COTIncome; // CREDIT

                AddToReport("COT", customerAccount.Name, cotIncomeReceivableAcc.Name, customerAccount.COTIncome);
                AddToReport("COT", COTIncomeGLAccount.Name, capitalAccount.Name, customerAccount.COTIncome);
                AddGLPosting(customerAccount.COTIncome, capitalAccount, COTIncomeGLAccount);
            }
        }

        // Till Double Entry
        [NonAction]
        public void SettleTill()
        {
            var tillAccounts = _context.Tellers.ToList();
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            foreach (var tillAccount in tillAccounts)
            {
                var glAccount = _context.GlAccounts.SingleOrDefault(c => c.Id == tillAccount.TillAccountId);

                if (capitalAccount != null)
                {
                    if (glAccount != null)
                    {
                        var balance = glAccount.AccountBalance;
                        capitalAccount.AccountBalance = capitalAccount.AccountBalance + balance; // CREDIT
                        glAccount.AccountBalance = glAccount.AccountBalance - balance; // DEBIT
                        tillAccount.TillAccountBalance = 0;

                        AddToReport("GL Posting", glAccount.Name, capitalAccount.Name, balance);
                        AddGLPosting(balance, capitalAccount, glAccount);
                    }
                }
                else
                {
                    errorMsg = errorMsg + "<br/> Insufficient Capital Account Balance";
                }


            }
        }

        // Interest Applied *Daily*
        [NonAction]
        public void InterestApplied()
        {
            float currentAccountMinimumBalance = 0;
            float savingsAccountMinimumBalance = 0;
            float debitInterestRate = 0;
            var savingsAccount = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Savings Account"));
            var currentAccount = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Current Account"));
            var loanAccount = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Loan Account"));
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            var loanDetails = _context.LoanDetails.ToList();
            var incomeGlAccount =
                _context.GlAccounts.SingleOrDefault(c => c.Id == loanAccount.InterestIncomeGLAccountId);// Income GL Account to be credited

            if (currentAccount?.MinimumBalance != null)
            {
                currentAccountMinimumBalance =
                   (float)currentAccount.MinimumBalance;
            }

            if (savingsAccount?.MinimumBalance != null)
            {
                savingsAccountMinimumBalance =
                   (float)savingsAccount.MinimumBalance;
            }

            if (loanAccount?.DebitInterestRate != null)
            {
                debitInterestRate =
                    ((float)loanAccount.DebitInterestRate) / 100;
            }

            foreach (var loanDetail in loanDetails)
            {
                var linkedAccount =
                    _context.CustomerAccounts.SingleOrDefault(c => c.Id == loanDetail.LinkedCustomerAccountId);
                if (linkedAccount != null && linkedAccount.IsClosed != true)
                {
                    var interest = (debitInterestRate * loanDetail.LoanAmount) / (12 * 30); // Interest to be paid daily;
                    var currentAccountType = _context.AccountTypes.FirstOrDefault(c => c.Name.Equals("Current Account"));
                    if (currentAccountType != null)
                    {
                        var currentAccountTypeId = currentAccountType.Id;
                        float availableBalance = 0; // Available Balance for this customer's account type
                        if (linkedAccount.AccountTypeId == currentAccountTypeId) // If account type is CURRENT
                        {
                            availableBalance = linkedAccount.AccountBalance - currentAccountMinimumBalance;

                        }
                        else // If account type is SAVINGS
                        {
                            availableBalance = linkedAccount.AccountBalance - savingsAccountMinimumBalance;

                        }

                        if (availableBalance > interest)
                        {

                            if (incomeGlAccount != null && capitalAccount != null && capitalAccount.AccountBalance >= interest)
                            {

                                interestReceivableAcc.AccountBalance = interestReceivableAcc.AccountBalance + interest;
                                incomeGlAccount.AccountBalance = incomeGlAccount.AccountBalance + interest; // CREDIT
                                linkedAccount.AccountBalance = linkedAccount.AccountBalance - interest; // DEBIT
                                interestReceivableAcc.AccountBalance = interestReceivableAcc.AccountBalance - interest; // CREDIT
                                // Income from this customer
                                loanDetail.InterestIncome = loanDetail.InterestIncome + interest;

                                // Financial Report Entry
                                AddToReport("Interest Accrual", interestReceivableAcc.Name, incomeGlAccount.Name, interest);
                                AddToReport("Interest Accrual", linkedAccount.Name, interestReceivableAcc.Name, interest);
                                AddGLPosting(interest, incomeGlAccount, interestReceivableAcc);
                            }
                            else
                            {
                                errorMsg = errorMsg + "<br/> Insufficient Capital Account Balance";
                            }
                        }
                    }

                }
            }

        }

        //  Perform Daily Interest Accrual
        [NonAction]
        public void InterestAccrual()
        {
            var loanDetails = _context.LoanDetails.ToList();
            var savingsAccountConfig = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Savings Account"));
            var currentAccountConfig = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Current Account"));
            var loanAccountConfig = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Loan Account"));

            var glAccounts = _context.GlAccounts.ToList();
            GLAccount loanIncomeGlAccount = null;
            if (savingsAccountConfig != null && currentAccountConfig != null && loanAccountConfig != null)
            {

                loanIncomeGlAccount =
                    glAccounts.SingleOrDefault(c => c.Id == loanAccountConfig.InterestIncomeGLAccountId);
            }

            if (loanDetails != null)
            {
                foreach (var detail in loanDetails)
                {
                    var interestRate = (detail.InterestRate / 100);
                    var principal = detail.LoanAmount;
                    var fullInterest = detail.LoanAmount * interestRate;
                    var accruedInterest = (interestRate * principal) / (12 * 30);
                    if (detail.InterestIncome < fullInterest && loanIncomeGlAccount != null)
                    {
                        detail.InterestReceivable = detail.InterestReceivable + accruedInterest;    // DEBIT
                        detail.InterestIncome = detail.InterestIncome + accruedInterest; // CREDIT

                        // : Reflecting on GL Account Balance                       
                        interestReceivableAcc.AccountBalance = interestReceivableAcc.AccountBalance + accruedInterest; // DEBIT *Accounts*
                        loanIncomeGlAccount.AccountBalance = loanIncomeGlAccount.AccountBalance + accruedInterest; // CREDIT *Accounts*


                    }

                    _context.SaveChanges();

                    //ADD TO REPORT TABLE IN DB (DOUBLE ENTRY)
                    AddToReport("Interest Accrual", CBA.INTEREST_RECEIVABLE_ACC_NAME, CBA.INTEREST_INCOME_ACC_NAME, accruedInterest);
                    AddGLPosting(accruedInterest, interestIncomeAcc, interestReceivableAcc);
                    // returnMsg = returnMsg + "<br/> : <b>Interest Accrual </b> Process Successful";

                }
                returnMsg = returnMsg + "<br/> : <b>Interest Accrual </b> Process Successful";
            }


        }

        [NonAction]
        public void InterestExpenseApplied()
        {
            var accounts = _context.CustomerAccounts.Where(c => c.AccountTypeId == CBA.SAVINGS_ACCOUNT_TYPE_ID || c.AccountTypeId == CBA.CURRENT_ACCOUNT_TYPE_ID).ToList();
            var accountTypes = _context.AccountTypes.ToList();
            var savingsAccountType = accountTypes.SingleOrDefault(c => c.Id == CBA.SAVINGS_ACCOUNT_TYPE_ID);
            var currentAccountType = accountTypes.SingleOrDefault(c => c.Id == CBA.CURRENT_ACCOUNT_TYPE_ID);
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            GLAccount interestExpenseGLAccount = null;
            var savingsInterestExpenseGLAccount = _context.GlAccounts.FirstOrDefault(c => c.Id == savingsAccountType.InterestExpenseGLAccountId);
            var currentInterestExpenseGLAccount = _context.GlAccounts.FirstOrDefault(c => c.Id == currentAccountType.InterestExpenseGLAccountId);

            if (savingsAccountType != null && currentAccountType != null)
            {
                var savingsInterestRate = savingsAccountType.CreditInterestRate; // Savings Account Type Interest Rate
                var currentInterestRate = currentAccountType.CreditInterestRate; // Current Account Type Interest Rate
                float interestRate = 0;
                foreach (var acc in accounts)
                {
                    if (acc.AccountTypeId == CBA.SAVINGS_ACCOUNT_TYPE_ID) // If Account is a SAVINGS ACCOUNT
                    {
                        savingsInterestRate = savingsInterestRate / 100;
                        if (savingsInterestRate != null) interestRate = (float)savingsInterestRate;
                        interestExpenseGLAccount = savingsInterestExpenseGLAccount;

                    }
                    else // If Account is a CURRENT ACCOUNT
                    {
                        currentInterestRate = currentInterestRate / 100;
                        if (currentInterestRate != null) interestRate = (float)currentInterestRate;
                        interestExpenseGLAccount = currentInterestExpenseGLAccount;
                    }

                    var interest = acc.AccountBalance * (float)interestRate;

                    if (interestExpenseGLAccount != null && capitalAccount != null && capitalAccount.AccountBalance > interest)
                    {
                        interestExpenseGLAccount.AccountBalance = interestExpenseGLAccount.AccountBalance + interest; // DEBIT
                        acc.AccountBalance = acc.AccountBalance + interest; // CREDIT
                        capitalAccount.AccountBalance = capitalAccount.AccountBalance - interest; // DEBIT
                        interestExpenseGLAccount.AccountBalance = interestExpenseGLAccount.AccountBalance - interest; // CREDIT

                        if (Math.Abs(interest) > 0) // If interest is greater than 0
                        {

                            AddToReport("Interest Expense", interestExpenseGLAccount.Name, acc.Name, interest);
                            AddToReport("GL Posting", capitalAccount.Name, interestExpenseGLAccount.Name, interest);
                            AddGLPosting(interest, interestExpenseGLAccount, capitalAccount);
                        }
                    }
                    else
                    {
                        errorMsg = errorMsg + "<br/> Insufficient Balance in Capital Account ";
                    }

                }
            }

        }

        [NonAction]
        public void InterestExpenseAccrual()
        {
            var accounts = _context.CustomerAccounts.Where(c => c.AccountTypeId == CBA.SAVINGS_ACCOUNT_TYPE_ID || c.AccountTypeId == CBA.CURRENT_ACCOUNT_TYPE_ID).ToList();
            var accountTypes = _context.AccountTypes.ToList();
            var savingsAccountType = accountTypes.SingleOrDefault(c => c.Id == CBA.SAVINGS_ACCOUNT_TYPE_ID);
            var currentAccountType = accountTypes.SingleOrDefault(c => c.Id == CBA.CURRENT_ACCOUNT_TYPE_ID);
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            GLAccount interestExpenseGLAccount = null;
            var savingsInterestExpenseGLAccount = _context.GlAccounts.FirstOrDefault(c => c.Id == savingsAccountType.InterestExpenseGLAccountId);
            var currentInterestExpenseGLAccount = _context.GlAccounts.FirstOrDefault(c => c.Id == currentAccountType.InterestExpenseGLAccountId);
            var interestPayable = _context.GlAccounts.FirstOrDefault(c => c.Name.Equals(CBA.INTEREST_PAYABLE_GL_ACCOUNT));
            if (savingsAccountType != null && currentAccountType != null)
            {
                var savingsInterestRate = savingsAccountType.CreditInterestRate; // Savings Account Type Interest Rate
                var currentInterestRate = currentAccountType.CreditInterestRate; // Current Account Type Interest Rate
                float interestRate = 0;
                foreach (var acc in accounts)
                {
                    if (acc.AccountTypeId == CBA.SAVINGS_ACCOUNT_TYPE_ID) // If Account is a SAVINGS ACCOUNT
                    {
                        savingsInterestRate = savingsInterestRate / 100;
                        if (savingsInterestRate != null) interestRate = (float)savingsInterestRate;
                        interestExpenseGLAccount = savingsInterestExpenseGLAccount;

                    }
                    else // If Account is a CURRENT ACCOUNT
                    {
                        currentInterestRate = currentInterestRate / 100;
                        if (currentInterestRate != null) interestRate = (float)currentInterestRate;
                        interestExpenseGLAccount = currentInterestExpenseGLAccount;
                    }

                    var interest = acc.AccountBalance * (float)interestRate;

                    if (interestExpenseGLAccount != null && interestPayable != null)
                    {


                        interestExpenseGLAccount.AccountBalance = interestExpenseGLAccount.AccountBalance + interest; // DEBIT
                        interestPayable.AccountBalance = interestPayable.AccountBalance + interest;// CREDIT

                        acc.Interest = acc.Interest + interest;


                        if (Math.Abs(interest) > 0) // If interest is greater than 0
                        {

                            AddToReport("GL Posting", interestExpenseGLAccount.Name, interestPayable.Name, interest);
                            _context.SaveChanges();
                            AddGLPosting(interest, interestPayable, interestExpenseGLAccount);
                        }
                    }
                    else
                    {
                        errorMsg = errorMsg + "<br/>  No Interest Payable GL Account or No Interest Expense GL Account";
                    }

                }
            }

        }

        [NonAction]
        public void InterestExpenseRepayment()
        {
            var accountTypes = _context.AccountTypes.ToList();
            var savingsAccountType = accountTypes.SingleOrDefault(c => c.Id == CBA.SAVINGS_ACCOUNT_TYPE_ID);
            var currentAccountType = accountTypes.SingleOrDefault(c => c.Id == CBA.CURRENT_ACCOUNT_TYPE_ID);
            var customerAccounts = _context.CustomerAccounts.Where(c => c.AccountTypeId == savingsAccountType.Id || c.AccountTypeId == currentAccountType.Id).ToList();
            var interestPayable = _context.GlAccounts.FirstOrDefault(c => c.Name.Equals(CBA.INTEREST_PAYABLE_GL_ACCOUNT));
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            foreach (var account in customerAccounts)
            {

                account.AccountBalance = account.AccountBalance + account.Interest; // CREDIT

                GLAccount expenseAccount = null;
                if (savingsAccountType != null && account.AccountTypeId == savingsAccountType.Id)
                {
                    expenseAccount =
                        _context.GlAccounts.SingleOrDefault(c => c.Id == savingsAccountType.InterestExpenseGLAccountId);
                }
                else
                {
                    expenseAccount =
                        _context.GlAccounts.SingleOrDefault(c => c.Id == currentAccountType.InterestExpenseGLAccountId);
                }


                if (interestPayable != null && capitalAccount != null && expenseAccount != null)
                {
                    interestPayable.AccountBalance = interestPayable.AccountBalance - account.Interest; // DEBIT
                    capitalAccount.AccountBalance = capitalAccount.AccountBalance - account.Interest; // DEBIT
                    expenseAccount.AccountBalance = expenseAccount.AccountBalance - account.Interest; // CREDIT
                    account.Interest = 0;
                    AddToReport("Interest Expense", interestPayable.Name, account.Name, account.Interest);
                    AddToReport("GL Posting", capitalAccount.Name, expenseAccount.Name, account.Interest);
                    AddGLPosting(account.Interest, expenseAccount, capitalAccount);
                }

            }

            _context.SaveChanges();

        }

        // Check if the appriopriate GL Accounts are available
        [NonAction]
        public void ValidateGLAccounts()
        {
            var interestPayable = _context.GlAccounts.FirstOrDefault(c => c.Name.Equals(CBA.INTEREST_PAYABLE_GL_ACCOUNT));
            if (interestIncomeAcc == null)
            {
                CreateGlAccount(CBA.INTEREST_INCOME_ACC_NAME);
                // errorMsg = errorMsg + "<br/> : Please create an <b>" + CBA.INTEREST_INCOME_ACC_NAME + "</b>";
            }
            if (interestInSuspenseAcc == null)
            {
                CreateGlAccount(CBA.INTEREST_IN_SUSPENSE_ACC_NAME);
                // errorMsg = errorMsg + "<br/> : Please create an <b>" + CBA.INTEREST_IN_SUSPENSE_ACC_NAME + "</b>";
            }
            if (interestOverdueAcc == null)
            {
                CreateGlAccount(CBA.INTEREST_OVERDUE_ACC_NAME);
                //  errorMsg = errorMsg + "<br/> : Please create an <b>" + CBA.INTEREST_OVERDUE_ACC_NAME + "</b>";
            }
            if (interestReceivableAcc == null)
            {
                CreateGlAccount(CBA.INTEREST_RECEIVABLE_ACC_NAME);
                //  errorMsg = errorMsg + "<br/> : Please create an <b>" + CBA.INTEREST_RECEIVABLE_ACC_NAME + "</b>";
            }
            if (principalOverdueAcc == null)
            {
                CreateGlAccount(CBA.PRINCIPAL_OVERDUE_ACC_NAME);
                //  errorMsg = errorMsg + "<br/> : Please create a <b>" + CBA.PRINCIPAL_OVERDUE_ACC_NAME + "</b>";
            }
            if (cotIncomeReceivableAcc == null)
            {
                CreateGlAccount(CBA.COT_INCOME_RECEIVABLE_GL_ACC_NAME);
                //  errorMsg = errorMsg + "<br/> : Please create a <b>" + CBA.COT_INCOME_RECEIVABLE_GL_ACC_NAME + "</b>";
            }
            if (interestPayable == null)
            {
                CreateGlAccount(CBA.INTEREST_PAYABLE_GL_ACCOUNT);
                //  errorMsg = errorMsg + "<br/> : Please create an <b>" + CBA.INTEREST_PAYABLE_GL_ACCOUNT + "</b>";
            }
        }

        // Create GL Accounts if they dont exist
        [NonAction]
        public void CreateGlAccount(string name)
        {
            var branch = _context.Branches.FirstOrDefault();
            var glCategoriesId = 0;
            var glCategories = _context.GlCategories.Include(c => c.Categories);
            if (name.Equals(CBA.INTEREST_INCOME_ACC_NAME) || name.Equals(CBA.COT_INCOME_GL_ACCOUNT))
            {
                glCategoriesId = glCategories.FirstOrDefault(c => c.Categories.Name.Equals("Income")).Id;
            }
            else if (name.Equals(CBA.COT_INCOME_RECEIVABLE_GL_ACC_NAME) || name.Equals(CBA.INTEREST_RECEIVABLE_ACC_NAME) || name.Equals(CBA.INTEREST_OVERDUE_ACC_NAME) || name.Equals(CBA.PRINCIPAL_OVERDUE_ACC_NAME))
            {
                glCategoriesId = glCategories.FirstOrDefault(c => c.Categories.Name.Equals("Asset")).Id;
            }
            else if (name.Equals(CBA.INTEREST_PAYABLE_GL_ACCOUNT))
            {
                glCategoriesId = glCategories.FirstOrDefault(c => c.Categories.Name.Equals("Liability")).Id;
            }
            else if (name.Equals(CBA.INTEREST_IN_SUSPENSE_ACC_NAME) || name.Equals(CBA.INTEREST_EXPENSE_GL_ACCOUNT))
            {
                glCategoriesId = glCategories.FirstOrDefault(c => c.Categories.Name.Equals("Expense")).Id;
            }

            if (branch != null)
            {
                var glAccount = new GLAccount()
                {
                    AccountBalance = 0,
                    BranchId = branch.Id,
                    Code = getCode(glCategoriesId),
                    Name = name,
                    GlCategoriesId = glCategoriesId

                };
                _context.GlAccounts.Add(glAccount);
                _context.SaveChanges();
            }


        }

        // : EOM Process is carried out at the end of 30 Financal Days - 1 Financial Month
        [NonAction]
        public void RunEOM()
        {
            var customerAccounts = _context.CustomerAccounts.ToList();
            var loanDetails = _context.LoanDetails.Include(c => c.Terms).ToList(); // List of all LOAN Details
            var loanAccountTypeConfig = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Loan Account"));
            float debitInterestRate = 0;
            if (loanAccountTypeConfig != null)
            {
                debitInterestRate = (float)loanAccountTypeConfig.DebitInterestRate;
            }

            foreach (var loanDetail in loanDetails)
            {
                var customerAccount = customerAccounts.Where(c => c.AccountTypeId != CBA.LOAN_ACCOUNT_TYPE_ID).SingleOrDefault(c => c.LoanDetailsId == loanDetail.Id);
                if (customerAccount != null)
                {
                    var customerAccountBalance = customerAccount.AccountBalance;
                    if (_context.AccountTypes != null)
                    {
                        var minimumBalance = _context.AccountTypes.SingleOrDefault(c => c.Id == customerAccount.AccountTypeId).MinimumBalance; // Minimum amount this account type can have

                        var loanAmount = loanDetail.LoanAmount; // Total amount 
                        var monthlyInterest = (debitInterestRate * loanAmount) / 12; // Monthly interest to be repaid
                        var monthlyPrincipal = (loanDetail.Terms.PaymentRate / 100) * loanAmount; // Monthly principal to be repaid
                        var customerLoanAccount = _context.CustomerAccounts.Where(c => c.AccountTypeId == CBA.LOAN_ACCOUNT_TYPE_ID)
                            .SingleOrDefault(c => c.LoanDetailsId == loanDetail.Id);
                        if (customerAccount.IsClosed)
                        {
                            InterestExpenseRepayment();
                            COTRepayment();
                            PerformDoubleEntry(loanDetail, customerAccount, customerLoanAccount, minimumBalance, customerAccountBalance, monthlyPrincipal, monthlyInterest, loanAmount);
                        }
                    }
                }
            }


        }

        [NonAction]
        public void RunAppliedEOM()
        {
            var customerAccounts = _context.CustomerAccounts.ToList();
            var loanDetails = _context.LoanDetails.Include(c => c.Terms).ToList(); // List of all LOAN Details
            var loanAccountTypeConfig = _context.AccountTypes.SingleOrDefault(c => c.Name.Equals("Loan Account"));
            float debitInterestRate = 0;
            if (loanAccountTypeConfig != null)
            {
                debitInterestRate = (float)loanAccountTypeConfig.DebitInterestRate;
            }

            foreach (var loanDetail in loanDetails)
            {
                var customerAccount = customerAccounts.Where(c => c.AccountTypeId != CBA.LOAN_ACCOUNT_TYPE_ID).SingleOrDefault(c => c.LoanDetailsId == loanDetail.Id);
                if (customerAccount != null)
                {
                    var customerAccountBalance = customerAccount.AccountBalance;
                    if (_context.AccountTypes != null)
                    {
                        var minimumBalance = _context.AccountTypes.SingleOrDefault(c => c.Id == customerAccount.AccountTypeId).MinimumBalance; // Minimum amount this account type can have

                        var loanAmount = loanDetail.LoanAmount; // Total amount 
                        var monthlyInterest = (debitInterestRate * loanAmount) / 12; // Monthly interest to be repaid
                        var monthlyPrincipal = (loanDetail.Terms.PaymentRate / 100) * loanAmount; // Monthly principal to be repaid
                        var customerLoanAccount = _context.CustomerAccounts.Where(c => c.AccountTypeId == CBA.LOAN_ACCOUNT_TYPE_ID)
                            .SingleOrDefault(c => c.LoanDetailsId == loanDetail.Id);
                        if (customerAccount.IsClosed == false)
                        {

                            PerformLoanAppliedDoubleEntry(loanDetail, customerAccount, customerLoanAccount, minimumBalance, customerAccountBalance, monthlyPrincipal, monthlyInterest, loanAmount);
                        }
                    }
                }
            }


        }

        // : Perform corresponding DEBIT and CREDIT operations on account balance
        [NonAction]
        public void LoanApplied(LoanDetails loanDetail, CustomerAccount customerAccount, CustomerAccount customerLoanAccount, float minimumBalance, float customerAccountBalance, float monthlyPrincipal, float monthlyInterest)
        {
            // Interest Repayment
            var capitalAccount = _context.GlAccounts.SingleOrDefault(c => c.Name.Equals("Capital Account"));
            var dailyPrincipal = (monthlyPrincipal*12) / 30;
            if (interestReceivableAcc != null)
            {
                // Principal Repayment
                customerAccount.AccountBalance = customerAccount.AccountBalance - dailyPrincipal; // DEBIT
                loanDetail.CustomerLoan = loanDetail.CustomerLoan - dailyPrincipal;
                customerLoanAccount.AccountBalance = customerLoanAccount.AccountBalance - dailyPrincipal; // CREDIT

              

                if (dailyPrincipal != 0)
                {
                    AddToReport("Loan Repayment", customerAccount.Name, customerLoanAccount.Name, dailyPrincipal);
                    //                    AddToReport("GL Posting", customerLoanAccount.Name, capitalAccount.Name, dailyPrincipal);

                }
            }

            _context.SaveChanges();
        }

        [NonAction]
        public void PerformLoanAppliedDoubleEntry(LoanDetails loanDetail, CustomerAccount customerAccount, CustomerAccount customerLoanAccount, float? newMinimumBalance, float customerAccountBalance, float monthlyPrincipal, float monthlyInterest, float loanAmount)
        {
            var minimumBalance = (float)newMinimumBalance; // Minimum Balance this Account Type must have;
            var availableBalance = customerAccountBalance - minimumBalance; // Amount that can be taken out of customers account - DEBIT
            var principalOverdue = loanDetail.PrincipalOverdue; // Principal left unpaid
            var interestOverdue = loanDetail.InterestOverdue; // Interest Left unpaid
            var amountPayable = monthlyPrincipal + monthlyInterest; // Total amount to be paid(settled) at EOM
            var amountOverdue = principalOverdue + interestOverdue; // Total amount left unpaid
            var totalAmountPayable = amountPayable + amountOverdue; // Total amount to be paid(settled)
            LoanApplied(loanDetail, customerAccount, customerLoanAccount, minimumBalance, customerAccountBalance, monthlyPrincipal, monthlyInterest);

        }

        [NonAction]
        public void PerformDoubleEntry(LoanDetails loanDetail, CustomerAccount customerAccount, CustomerAccount customerLoanAccount, float? newMinimumBalance, float customerAccountBalance, float monthlyPrincipal, float monthlyInterest, float loanAmount)
        {
            var minimumBalance = (float)newMinimumBalance; // Minimum Balance this Account Type must have;
            var availableBalance = customerAccountBalance - minimumBalance; // Amount that can be taken out of customers account - DEBIT
            var principalOverdue = loanDetail.PrincipalOverdue; // Principal left unpaid
            var interestOverdue = loanDetail.InterestOverdue; // Interest Left unpaid
            var amountPayable = monthlyPrincipal + monthlyInterest; // Total amount to be paid(settled) at EOM
            var amountOverdue = principalOverdue + interestOverdue; // Total amount left unpaid
            var totalAmountPayable = amountPayable + amountOverdue; // Total amount to be paid(settled)

            if (availableBalance > amountPayable)
            {
                FullLoanRepayment(loanDetail, customerAccount, customerLoanAccount, minimumBalance, customerAccountBalance, monthlyPrincipal, monthlyInterest);
                FullyFunded(loanDetail, customerAccount, customerLoanAccount, minimumBalance, customerAccountBalance, monthlyPrincipal, monthlyInterest);
            }
            else
            {
                PartialLoanRepayment(loanDetail, customerAccount, customerLoanAccount, minimumBalance, customerAccountBalance, monthlyPrincipal, monthlyInterest);
            }

        }

        [NonAction]
        public void FullLoanRepayment(LoanDetails loanDetail, CustomerAccount customerAccount, CustomerAccount customerLoanAccount, float minimumBalance, float customerAccountBalance, float monthlyPrincipal, float monthlyInterest)
        {

            // Interest Repayment

            if (interestReceivableAcc != null)
            {
                customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyInterest; // DEBIT
                interestReceivableAcc.AccountBalance = interestReceivableAcc.AccountBalance - monthlyInterest; // CREDIT

                // Principal Repayment
                customerAccount.AccountBalance = customerAccount.AccountBalance - monthlyPrincipal; // DEBIT
                loanDetail.CustomerLoan = loanDetail.CustomerLoan - monthlyPrincipal;
                customerLoanAccount.AccountBalance = customerLoanAccount.AccountBalance - monthlyPrincipal; // CREDIT

                if (monthlyInterest != 0)
                {

                    AddToReport("Loan Repayment", customerAccount.Name, interestReceivableAcc.Name, monthlyInterest);
                }

                if (monthlyPrincipal != 0)
                {
                    AddToReport("Loan Repayment", customerAccount.Name, customerLoanAccount.Name, monthlyPrincipal);
                }
            }

            _context.SaveChanges();


        }

        [NonAction]
        public void PartialLoanRepayment(LoanDetails loanDetail, CustomerAccount customerAccount, CustomerAccount customerLoanAccount, float minimumBalance, float customerAccountBalance, float monthlyPrincipal, float monthlyInterest)
        {
            var totalAmountToBePaid = monthlyInterest + monthlyPrincipal;
            var availableBalance = customerAccountBalance - minimumBalance;
            var interestToBePaid = availableBalance;
            var interestUnpaid = monthlyInterest - availableBalance;
            var principalToBePaid = (availableBalance - monthlyInterest);
            var principalUnpaid = monthlyPrincipal - principalToBePaid;
            FullLoanRepayment(loanDetail, customerAccount, customerLoanAccount, minimumBalance, customerAccountBalance, principalToBePaid, interestToBePaid);
            if (availableBalance < monthlyInterest)
            {


                interestOverdueAcc.AccountBalance = interestOverdueAcc.AccountBalance + interestUnpaid;
                interestInSuspenseAcc.AccountBalance = interestInSuspenseAcc.AccountBalance + interestUnpaid;
                AddToReport("Loan Repayment", interestOverdueAcc.Name, interestInSuspenseAcc.Name, interestUnpaid);
                AddGLPosting(interestUnpaid, interestInSuspenseAcc, interestOverdueAcc);
            }
            else
            {
                principalOverdueAcc.AccountBalance = principalOverdueAcc.AccountBalance + principalUnpaid;
                customerLoanAccount.AccountBalance = customerLoanAccount.AccountBalance - principalUnpaid;
                loanDetail.CustomerLoan = loanDetail.CustomerLoan - principalUnpaid;
                AddToReport("Loan Repayment", principalOverdueAcc.Name, customerLoanAccount.Name, principalUnpaid);

            }

            _context.SaveChanges();
        }

        [NonAction]
        public void FullyFunded(LoanDetails loanDetail, CustomerAccount customerAccount, CustomerAccount customerLoanAccount, float minimumBalance, float customerAccountBalance, float monthlyPrincipal, float monthlyInterest)
        {
            var availableBalance = customerAccountBalance - minimumBalance;
            var totalAmountToBePaid = monthlyInterest + monthlyPrincipal;
            var remainingBalance = availableBalance - totalAmountToBePaid;
            var interestOverdue = loanDetail.InterestOverdue;
            var principalOverdue = loanDetail.PrincipalOverdue;
            var interestOverdueToBePaid = remainingBalance;
            float principalOverdueToBePaid = 0;
            var totalOverdueToBePaid = interestOverdue + principalOverdue;
            if (remainingBalance < totalOverdueToBePaid)
            {
                principalOverdueToBePaid = remainingBalance - interestOverdue;
            }
            else
            {
                principalOverdueToBePaid = principalOverdue;
            }

            // : If available balance > total amount to be paid
            if (remainingBalance <= interestOverdue)
            {
                customerAccount.AccountBalance = customerAccount.AccountBalance - interestOverdueToBePaid; // DEBIT
                interestIncomeAcc.AccountBalance = interestIncomeAcc.AccountBalance - interestOverdueToBePaid; // CREDIT
                interestInSuspenseAcc.AccountBalance = interestInSuspenseAcc.AccountBalance - interestOverdueToBePaid; // DEBIT
                interestOverdueAcc.AccountBalance = interestOverdueAcc.AccountBalance - interestOverdueToBePaid; // CREDIT
                AddToReport("Loan Repayment", customerAccount.Name, interestIncomeAcc.Name, interestOverdueToBePaid);
                AddToReport("Loan Repayment", interestInSuspenseAcc.Name, interestOverdueAcc.Name, interestOverdueToBePaid);
                AddGLPosting(interestOverdueToBePaid, interestOverdueAcc, interestInSuspenseAcc);
            }
            else
            {
                customerAccount.AccountBalance = customerAccount.AccountBalance - interestOverdueToBePaid; // DEBIT
                interestIncomeAcc.AccountBalance = interestIncomeAcc.AccountBalance - interestOverdueToBePaid; // CREDIT
                interestInSuspenseAcc.AccountBalance = interestInSuspenseAcc.AccountBalance - interestOverdueToBePaid; // DEBIT
                interestOverdueAcc.AccountBalance = interestOverdueAcc.AccountBalance - interestOverdueToBePaid; // CREDIT
                customerAccount.AccountBalance = customerAccount.AccountBalance - principalOverdueToBePaid; // DEBIT
                principalOverdueAcc.AccountBalance = principalOverdueAcc.AccountBalance - principalOverdueToBePaid; // CREDIT
                AddToReport("Loan Repayment", customerAccount.Name, interestIncomeAcc.Name, interestOverdueToBePaid);
                AddToReport("Loan Repayment", interestInSuspenseAcc.Name, interestOverdueAcc.Name, interestOverdueToBePaid);
                AddToReport("Loan Repayment", customerAccount.Name, principalOverdueAcc.Name, principalOverdueToBePaid);
                AddGLPosting(interestOverdueToBePaid, interestOverdueAcc, interestInSuspenseAcc);
            }

            _context.SaveChanges();
        }

        // Create Financial Report Entry
        [NonAction]
        public void AddToReport(string postingType, string debitAccountName, string creditAccountName, float amount)
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

        [NonAction]
        public string GetHours(string time)
        {
            var hours = "";
            hours = time.Substring(0, 2);
            if (int.Parse(hours) > 12)
            {
                var temp = int.Parse(hours) - 12;
                hours = "0" + temp.ToString();
            }
            else if (int.Parse(hours) == 0)
            {
                hours = "12";
            }
            return hours;
        }

        [NonAction]
        public string GetMins(string time)
        {
            var mins = "";
            mins = time.Substring(3, 2);
            return mins;
        }

        [NonAction]
        public string GetPeriod(string time)
        {
            var period = "AM";
            var hours = time.Substring(0, 2);

            if (int.Parse(hours) >= 12)
            {
                period = "PM";
            }

            return period;
        }

        [NonAction]
        public string ConvertToTime(int hour, int mins, string period)
        {
            var tempHr = hour;
            var hr = tempHr.ToString();
            if (hour < 10)
            {
                hr = "0" + tempHr.ToString();
            }
            var realMin = mins.ToString();

            if (mins == 0)
            {
                realMin = "00";
            }
            else if (mins > 0 && mins < 10)
            {
                realMin = "0" + mins.ToString();
            }
            var time = hr + ":" + realMin;
            if (period.Equals("PM"))
            {
                var addedTime = 0;
                if (hour != 12)
                {
                    addedTime = 12;
                }
                else
                {
                    time = "00" + ":" + realMin;
                    addedTime = 0;
                }

                var temp = hour + addedTime;
                //realMin = mins.ToString();


                time = temp.ToString() + ":" + realMin;
            }
            else
            {
                if (hour == 12)
                {
                    time = "00" + ":" + realMin;
                    return time;
                }

                time = hr + ":" + realMin;
            }

            return time;
        }

        [NonAction]
        public string getCode(int id)
        {
            var GLRanCode = RandomString(5);
            var GLId = id;
            var GLAccountCode = Convert.ToString(GLId) + Convert.ToString(GLRanCode);
            Console.WriteLine(GLAccountCode);
            return GLAccountCode;

        }

        [NonAction]
        private static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [NonAction]
        public void AddGLPosting(float amount, GLAccount creditAccount, GLAccount debitAccount)
        {
            var creditAccountId = creditAccount.Id;
            var creditAccountName = creditAccount.Name;
            var debitAccountId = debitAccount.Id;
            var debitAccountName = debitAccount.Name;
            var glPosting = new GLPostings()
            {
                CreditAmount = amount,
                CreditNarration = "Debit from " + debitAccountName,
                DebitAmount = amount,
                DebitNarration = "Debit from " + debitAccountName,
                GlCreditAccountId = creditAccountId,
                GlDebitAccountId = debitAccountId,
                TransactionDate = DateTime.Now,
                UserAccountId = userId

            };
            _context.GlPostings.Add(glPosting);
            _context.SaveChanges();
        }


    }


}