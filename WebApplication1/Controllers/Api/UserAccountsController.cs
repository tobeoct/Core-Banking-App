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

namespace WebApplication1.Controllers.Api
{
    public class UserAccountsController : ApiController
    {
        private ApplicationDbContext _context;

        public UserAccountsController()
        {
            _context = new ApplicationDbContext();
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