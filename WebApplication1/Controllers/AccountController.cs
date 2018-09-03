using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RestSharp;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private string errorMsg="";
        public AccountController()
        {
            _context = new ApplicationDbContext();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            ApplicationUser signedUser = UserManager.FindByEmail(model.Email);
            //            var result = await SignInManager.PasswordSignInAsync(signedUser.UserName, model.Password, model.RememberMe, shouldLockout: false);
            //            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            var result = await SignInManager.PasswordSignInAsync(signedUser.UserName, model.Password, true, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    RoleName.EMAIL = model.Email;
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");

                    return View("Login", model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        //        [Authorize(Roles = "Admin")]
        //        public ActionResult Register()
        //        {
        //            
        //            var branches = new List<Branch>(_context.Branches.ToList());
        //            
        //            return View(new RegisterViewModel{Branch = branches});
        //        }
        //
        //        //
        //        // POST: /Account/Register
        //        [HttpPost]
        //        [Authorize(Roles = "Admin")]
        //        [ValidateAntiForgeryToken]
        //        public async Task<ActionResult> Register(RegisterViewModel model)
        //        {
        //            bool statusRegistration = false;
        //            Guid activationCode = Guid.NewGuid();
        //            string passWord= getAutoGeneratedPassword();
        //            string messageRegistration = string.Empty;
        //            var branches = new List<Branch>(_context.Branches.ToList());
        //            if (ModelState.IsValid)
        //            {
        //                var user = new ApplicationUser
        //                {
        //                    UserName = model.Username,
        //                    Email = model.Email,
        //                    FullName = model.FullName,
        //                    PhoneNumber = model.PhoneNumber,
        //                    BranchId = model.BranchId,
        //                    ActivationCode = activationCode
        //
        //                };
        //                var result = await UserManager.CreateAsync(user, passWord);
        //                if (result.Succeeded)
        //                {
        //                     await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
        //                    //                    var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
        //                    //                    var roleManager = new RoleManager<IdentityRole>(roleStore);
        //                    //                    await roleManager.CreateAsync(new IdentityRole("CanManagePostings"));
        //                    //                    await UserManager.AddToRoleAsync(user.Id,"CanManagePostings");
        //                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //                    // Send an email with this link
        //
        //                    //                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //                    //                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //                    //                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //
        //                    //Verification Email  
        //                    VerificationEmail(model.Email, activationCode.ToString(), passWord);
        //                    
        //                    messageRegistration = "Your account has been created successfully. Check your mail to Activate Your Account";
        //                    statusRegistration = true;
        //                    ViewBag.Message = messageRegistration;
        //                    model.BranchId = 0;
        //                    model.Email = "";
        //                    model.Username = "";
        //                    model.FullName = null;
        //                    model.PhoneNumber = "";
        //
        //
        //
        //                    //                    return View(new RegisterViewModel { Branch = branches });
        //                    //                    RedirectToAction("Index", "UserAccounts", new RegisterViewModel { Branch = branches });
        //                    return View("Index", "UserAccounts", new RegisterViewModel { Branch = branches });
        //                }
        //                AddErrors(result);
        //            }
        //
        //            // If we got this far, something failed, redisplay form
        //            //            RedirectToAction("Index", "UserAccounts", new RegisterViewModel { Branch = branches });
        //            return View("Index", "UserAccounts", new RegisterViewModel {Branch = branches});
        //
        //        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        //        [AllowAnonymous]
        //        public ActionResult ResetPassword(string code)
        //        {
        //            code = "f7c85003-773e-4b1e-9bb7-2e9da261c90b";
        //            return code == null ? View("Error") : View();
        //        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View("ChangePassword");
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(WebApplication1.ViewModels.ChangePasswordViewModel usermodel)
        {
            ApplicationUser user = await UserManager.FindByEmailAsync(RoleName.EMAIL);
            if (user == null)
            {
                ViewBag.Message = "Please Log in As " + RoleName.USER_NAME;

                return View();
            }
            else
            {

                user.PasswordHash = UserManager.PasswordHasher.HashPassword(usermodel.NewPassword);
                var result = await UserManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    //throw exception......
                    return View("Error");
                }
                ViewBag.Message = "Password Changed";

            }


            return View();
        }
        // POST: /Account/ResetPassword
        //        [HttpPost]
        //        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        //        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //        {
        //            string resetToken = await UserManager.GeneratePasswordResetTokenAsync(model.Id);
        //            IdentityResult passwordChangeResult = await UserManager.ResetPasswordAsync(model.Id, resetToken, model.NewPassword);
        //            
        //            if (!ModelState.IsValid)
        //            {
        //                return View(model);
        //            }
        //            var user = await UserManager.FindByEmailAsync(model.Email);
        //            if (user == null)
        //            {
        //                // Don't reveal that the user does not exist
        //                ViewBag.SuccessMessage = "No Such User";
        //                return RedirectToAction("Index", "UserAccounts");
        //            }
        //            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //            if (result.Succeeded)
        //            {
        //                ViewBag.SuccessMessage = "Password has been reset.";
        //                return RedirectToAction("Index", "UserAccounts");
        //            }
        //            AddErrors(result);
        //            return View();
        //        }
        // GET : /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ResetPassword(EditProfileViewModel model)
//        {
//            var email = model.ApplicationUser.Email;
//            ApplicationUser user = await UserManager.FindByEmailAsync(email);
//            string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
//            var password = getAutoGeneratedPassword();
//             VerificationEmail(email, user.Id, resetToken, password, "RESET");
//            if (errorMsg != "")
//            {
//                ViewBag.Message = errorMsg;
//                return View();
//            }
//            IdentityResult passwordChangeResult = await UserManager.ResetPasswordAsync(user.Id, resetToken, password);
//            ViewBag.Message = "Reset Email has been sent";
//
//            return View();
//        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {

            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        public ActionResult GetResetCode()
        {
            var code = "234567";
            return RedirectToAction("ResetPassword");
        }
        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
        [NonAction]
        public string getAutoGeneratedPassword()
        {
            var GLRanCode = RandomString(8);

            return GLRanCode;

        }
        [NonAction]
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+}{|:>?<,./";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [HttpGet]
        public ActionResult ActivationAccount(string id)
        {
            bool statusAccount = false;
            var user = new ApplicationUser();
            using (_context)
            {
                ApplicationUser userAccount = _context.Users.FirstOrDefault(x => x.ActivationCode.ToString() == id);

                if (userAccount != null)
                {

                    userAccount.IsActive = true;
                    statusAccount = true;
                    Console.WriteLine(userAccount.IsActive);
                }
                else
                {
                    ViewBag.Message = "Something Wrong !!";
                }

            }
            ViewBag.Status = statusAccount;
            return View("ActivationAccount");
        }



        [NonAction]
        public void VerificationEmail(string email, string activationCode, string id, string password, string type)
        {

            var url = $"/Account/FinalResetPassword/{id}/{activationCode}/{password}";
            if (Request.Url != null)
            {
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);
                var apiRequest = new RestRequest($"/Account/FinalResetPassword/{HttpUtility.UrlPathEncode(string.Join("+", id))}/{HttpUtility.UrlPathEncode(string.Join("+", activationCode))}/{HttpUtility.UrlPathEncode(string.Join("+", password))}");
                var fromEmail = new MailAddress("tobe.onyema@gmail.com", "Activation Account - FINTECH");
                var toEmail = new MailAddress(email);

                var fromEmailPassword = "0801tobe";
                string subject = "Activation Account ";
                string body = "";
                if (type.Equals("RESET"))
                {
                    body = "<h1>PASSWORD RESET</h1>" +
                                            "<p><b>Username : </b>" + email + "</p>" +
                                            "<p><b>Password : </b>" + password + "</p>";

                    //body = "<br/> Please click on the following link in order to activate your account" + "<a href='" + apiRequest + "' style='font-family:sans-serif;'> Activate your Account </a>" + credentialsBody;
                }
                else
                {

                    body = "<br/> Please click on the following link in order to reset your password" + "<a href='" +
                           link + "' style='font-family:sans-serif;'>Reset Password<a/>";
                }

                try
                {
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                    };

                    using (var message = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true

                    })

                        smtp.Send(message);
                }
                catch (SmtpException ex)
                {


//                     errorMsg = "Mail cannot be sent because of the server problem:";
//                    errorMsg += ex.Message;
                    errorMsg = "There seems to be a network problem. Please try again later";
                    if (errorMsg != "")
                    {
                        return;
                    }

                    Response.Write(errorMsg);
                    //throw new Exception(msg);

                }
                if (errorMsg != "")
                {
                    return;
                }
            }
        }
    }
}