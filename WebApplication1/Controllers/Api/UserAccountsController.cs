using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Dtos;
using System.Data.Entity;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace WebApplication1.Controllers.Api
{
    public class UserAccountsController : ApiController
    {
        private ApplicationDbContext _context;
        private static Random random;
        private string userId = "";
        protected UserManager<ApplicationUser> UserManager { get; set; }
        //protected SignInManager<ApplicationSignInManager> SignInManager { get; set; }
        public UserAccountsController()
        {
            _context = new ApplicationDbContext();

            userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);

            if (user != null)
            {
                RoleName.USER_NAME = user.FullName;
                RoleName.EMAIL = user.Email;
            }
        }

        // GET: /api/branches
        public IHttpActionResult GetUserAccounts()
        {
            return Ok(_context.Users.Include(c => c.Branch).ToList());
        }


        // POST api/<controller>
        [HttpPost]
        public IHttpActionResult CreateUserAccount(UserAccountDto userAccountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userAccount = Mapper.Map<UserAccountDto, UserAccount>(userAccountDto);
            _context.UserAccounts.Add(userAccount);
            _context.SaveChanges();
            userAccountDto.Id = userAccount.Id;
            return Created(new Uri(Request.RequestUri + "/" + userAccount.Id), userAccountDto);
        }

        
    }
}